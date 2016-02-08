/*
 * ----------------------------------------------------------------------------
 * "THE BEER-WARE LICENSE" 
 * As long as you retain this notice you can do whatever you want with this 
 * stuff. If you meet an employee from Windward meet some day, and you think
 * this stuff is worth it, you can buy them a beer in return. Windward Studios
 * ----------------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using log4net;
using Server.AI;
using Server.Communication;
using Server.GameEngine;
using Server.Map;
using Server.UI;
using Server.Units;
using Server.Utilities;
using Timer = System.Windows.Forms.Timer;

namespace Server
{
	/// <summary>
	///     This is the layer between the XML messages and the engine. It also handles the basic game timer.
	/// </summary>
	public class Framework : IEngineCallback
	{
		/// <summary>
		///     The state the game is presently in for communications with the players.
		/// </summary>
		public enum COMM_STATE
		{
			/// <summary>
			///     Could not load the map
			/// </summary>
			NO_MAP,

			/// <summary>
			///     Accepting players joining the game.
			/// </summary>
			ACCEPTING_JOINS,

			/// <summary>
			///     Waiting for players to send setup info.
			/// </summary>
			ACCEPTING_SETUP,

			/// <summary>
			///     Players all setup, ready to start the game.
			/// </summary>
			READY_TO_START,

			/// <summary>
			///     The game is paused.
			/// </summary>
			PAUSED,

			/// <summary>
			///     Executing a step.
			/// </summary>
			STEP,

			/// <summary>
			///     The game is running.
			/// </summary>
			RUNNING,

			/// <summary>
			///     Game is over.
			/// </summary>
			GAME_OVER,
		}

		private bool fullSpeed;
		private int _prevMovesPerSecond = 96;

		/// <summary>
		///     If this is true, then we do 2K moves/second and no map update. Player status updated once every 5 seconds
		/// </summary>
		public bool FullSpeed
		{
			get { return fullSpeed; }
			set
			{
				if (value)
				{
					_prevMovesPerSecond = MovesPerSecond;
					fullSpeed = true;
					MovesPerSecond = GameMap.NUM_X_TILES * GameMap.NUM_Y_TILES + 1;
				}
				else
				{
					MovesPerSecond = _prevMovesPerSecond;
					fullSpeed = false;
				}
			}
		}

		// how often we broadcast the game status (does not change for different ticksPerSecond).
		private const int SECONDS_WAIT_READY = 2;

		/// <summary>
		///     The number of game ticks per second. The default is 3. It takes 6 for each player to get a turn to place a tile.
		/// </summary>
		public int MovesPerSecond { get; set; }

		/// <summary>
		///     the number of game ticks from the start of the game. This works off ticksPerSecond and can be changed
		///     to speed up/slow down the game.
		/// </summary>
		public int GameTicks { get; private set; }

		/// <summary>
		///     If true, then start the game as soon as 1 remote AI joins.
		/// </summary>
		public bool DebugStartMode { get; set; }

		/// <summary>
		///     If this user is set, starts, connects to this user, then exits with a return code of 0.
		/// </summary>
		public string TestUser { get; private set; }

		/// <summary>
		///     The number of games to run for the AutoRun. 0 if not an auto-run.
		/// </summary>
		public int AutoRunNumGames { get; private set; }

		/// <summary>
		///     The filename to write the auto run to.
		/// </summary>
		public string AutoRunFilename { get; private set; }

		/// <summary>
		///     If an autorun - the users to allow (and wait for)
		/// </summary>
		public List<string> AutoRunUsers { get; private set; }

		/// <summary>
		///     true if play the traffic noises.
		/// </summary>
		public bool PlaySounds { get; set; }

		// the traffic WAV file.
		public readonly SoundPlayer trafficPlayer;

		public readonly SoundPlayer winPlayer;

		private COMM_STATE commState = COMM_STATE.ACCEPTING_JOINS;

		internal readonly TcpServer tcpServer = new TcpServer();

		// new connections
		private readonly List<string> pendingGuids = new List<string>();

		// the game engine
		private readonly Engine engine;

		// the main window.
		internal readonly IUserDisplay mainWindow;

		// The game timer. Fires on every tick.
		private Timer timerWorker;

		// The communication timer. Fires 1 second after requesting setup.
		private Timer timerClientWait;

		private static readonly Random rand = new Random();

		// true when we'll accept messages
		private bool acceptMessages = true;

		private static readonly ILog log = LogManager.GetLogger(typeof(Framework));

		/// <summary>
		///     Create the engine.
		/// </summary>
		/// <param name="mainWindow">The main window.</param>
		public Framework(IUserDisplay mainWindow)
		{
			PlaySounds = true;
			MovesPerSecond = 3;
			this.mainWindow = mainWindow;

			string[] args = Environment.GetCommandLineArgs();
			if (args.Length >= 3 && args[1] == "/t")
				TestUser = args[2].Trim();

			if (args.Length > 4 && (args[1] == "/a"))
			{
				AutoRunNumGames = int.Parse(args[2]);
				AutoRunFilename = Path.GetFullPath(args[3]);
				AutoRunUsers = new List<string>();
				for (int ind = 4; ind < args.Length; ind++)
					AutoRunUsers.Add(args[ind]);
			}

			// traffic noises
			using (
				Stream wavFile =
					Assembly.GetExecutingAssembly().GetManifestResourceStream("Server.Sounds.Town_Traffic_01.wav"))
			{
				trafficPlayer = new SoundPlayer(wavFile);
				trafficPlayer.Load();
			}
			using (
				Stream wavFile =
					Assembly.GetExecutingAssembly()
						.GetManifestResourceStream("Server.Sounds.Crowding Cheering Charge-SoundBible.com-284606164.wav"))
			{
				winPlayer = new SoundPlayer(wavFile);
				winPlayer.Load();
			}

			engine = new Engine(this);
			mainWindow.NewMap();
			mainWindow.UpdateMap();

			tcpServer.Start(this);
		}

		public Engine GameEngine
		{
			get { return engine; }
		}

		private delegate void StatusMessageDelegate(string text);

		private delegate void IncomingMessageDelegate(string guid, string message);

		public void StatusMessage(string message)
		{
			mainWindow.CtrlForInvoke.BeginInvoke(new StatusMessageDelegate(mainWindow.StatusMessage), new object[] { message });
		}

		public void ConnectionEstablished(string guid)
		{
			pendingGuids.Add(guid);
		}

		public void IncomingMessage(string guid, string message)
		{
			mainWindow.CtrlForInvoke.BeginInvoke(new IncomingMessageDelegate(_IncomingMessage), new object[] { guid, message });
		}

		public void ConnectionLost(string guid)
		{
			mainWindow.CtrlForInvoke.BeginInvoke(new StatusMessageDelegate(_ConnectionLost), new object[] { guid });
		}

		private void _ConnectionLost(string guid)
		{
			Player player = engine.Players.FirstOrDefault(pl => pl.TcpGuid == guid);
			if (player == null)
			{
				log.Warn(string.Format("unknown TCP GUID {0} dropped", guid));
				return;
			}
			player.TcpGuid = null;

			string msg = string.Format("Player {0} lost connection", player.Name);
			log.Info(msg);
			mainWindow.StatusMessage(msg);
		}

		/// <summary>
		///     The game play mode.
		/// </summary>
		public COMM_STATE Mode
		{
			get { return commState; }
		}

		private void _IncomingMessage(string guid, string message)
		{
			if (!acceptMessages)
			{
				Trap.trap();
				return;
			}

			try
			{
				// get the xml
				XDocument xml;
				try
				{
					xml = XDocument.Parse(message);
				}
				catch (Exception ex)
				{
					log.Error(string.Format("Bad message (XML) from connection {0}, exception: {1}", guid, ex));
					// if an existing player we'll just ignore it. Otherwise we close the connection
					if (engine.Players.Any(pl => pl.TcpGuid == guid))
						return;
					Trap.trap();
					tcpServer.CloseConnection(guid);
					return;
				}

				Player player = engine.Players.FirstOrDefault(pl => pl.TcpGuid == guid);
				XElement root = xml.Root;
				if (root == null)
				{
					Trap.trap();
					log.Error(string.Format("Bad message (XML) from connection {0} - no root node", guid));
					return;
				}

				// if not an existing player, it must be <join>
				if ((player == null) && (root.Name.LocalName != "join"))
				{
					Trap.trap();
					log.Error(string.Format("New player from connection {0} - not a join", guid));
					tcpServer.CloseConnection(guid);
					return;
				}

				switch (root.Name.LocalName)
				{
					case "join":
						MsgPlayerJoining(player, guid, root);
						return;
					case "ready":
						MsgPlayerReady(player, root);
						return;
					default:
						Trap.trap();
						log.Error(string.Format("Bad message (XML) from server - root node {0}", root.Name.LocalName));
						break;
				}
			}
			catch (Exception ex)
			{
				mainWindow.StatusMessage(string.Format("Error on incoming message. Exception: {0}", ex));
			}
		}

		private void CommTimerStart()
		{
			lock (this)
			{
				CommTimerClose();
				timerClientWait = new Timer { Interval = SECONDS_WAIT_READY * 1000 };
				timerClientWait.Tick += CommTimeout;
				timerClientWait.Start();
			}
		}

		private void CommTimerClose()
		{
			lock (this)
			{
				if (timerClientWait == null)
					return;
				timerClientWait.Stop();
				timerClientWait.Dispose();
				timerClientWait = null;
			}
		}

		private void CommTimeout(object sender, EventArgs e)
		{
			CommTimerClose();
			commState = COMM_STATE.READY_TO_START;
			mainWindow.CtrlForInvoke.BeginInvoke((MethodInvoker)(() => mainWindow.SetupGame()));
		}

		/// <summary>
		///     Player is (re)joining the game.
		/// </summary>
		/// <param name="player">The player. null if a join, non-null (maybe) if a re-join.</param>
		/// <param name="guid">The tcp guid of the message.</param>
		/// <param name="root">The XML message.</param>
		private void MsgPlayerJoining(Player player, string guid, XElement root)
		{
			// if the player exists do nothing because we're already set up (client sent it twice).
			if (player != null)
			{
				Trap.trap();
				log.Error(string.Format("Join on existing connected player. connection: {0}", guid));
				return;
			}

			// must have a name
			string name = AttributeOrNull(root, "name");
			if (string.IsNullOrEmpty(name))
			{
				Trap.trap();
				log.Error(string.Format("Join with no name refused. Guid: {0}", guid));
				tcpServer.CloseConnection(guid);
				return;
			}

			// if auto-run, must match
			if (AutoRunNumGames > 0)
			{
				if (!AutoRunUsers.Contains(name))
				{
					log.Error(string.Format("Join from non auto-run player {0} refused. Guid: {1}", name, guid));
					tcpServer.CloseConnection(guid);
					return;
				}
			}

			// check for new connection on existing player
			player = engine.Players.FirstOrDefault(pl => pl.Name == name);
			if (player != null && player.TcpGuid == null)
			{
				player.TcpGuid = guid;
				// resend the setup (they may have re-started)
				if (commState != COMM_STATE.ACCEPTING_JOINS && commState != COMM_STATE.GAME_OVER)
				{
					player.WaitingForReply = Player.COMM_MODE.WAITING_FOR_START;
					GameEngine.RestartPlayer(player);
				}
				if (log.IsInfoEnabled)
					log.Info(string.Format("Player {0} reconnected", name));
				mainWindow.StatusMessage(string.Format("Player {0} reconnected", name));
				UpdateAll();
				return;
			}

			// unique name?
			player = engine.Players.FirstOrDefault(pl => pl.Name == name);
			if (player != null)
			{
				log.Error(string.Format("Player {0} name already exists, duplicate refused.", name));
				mainWindow.StatusMessage(string.Format("Player {0} name already exists, duplicate refused.", name));
				tcpServer.CloseConnection(guid);
				return;
			}

			int ind = engine.Players.Count;
			// do we have room?
			if (ind >= Player.NUM_PLAYERS)
			{
				log.Error(string.Format("Can't add a {0}th player. Name: {1}", Player.NUM_PLAYERS + 1, name));
				tcpServer.CloseConnection(guid);
				return;
			}

			// we're in progress - no new players
			if (commState != COMM_STATE.ACCEPTING_JOINS)
			{
				Trap.trap();
				log.Error(string.Format("Game in progress - new players not allowed. Name: {0}", name));
				mainWindow.StatusMessage(string.Format("game in progress - new players not allowed. Name: {0}", name));
				tcpServer.CloseConnection(guid);
				return;
			}

			// ok, we're all good. Create the player
			XElement elemAvatar = root.Element("avatar");
			Image avatar = null;
			if (elemAvatar != null)
			{
				try
				{
					byte[] data = Convert.FromBase64String(elemAvatar.Value);
					avatar = new Bitmap(new MemoryStream(data));
					if ((avatar.Width != 32) || (avatar.Height != 32))
						mainWindow.StatusMessage(string.Format("Avatar for player {0} not 32x32", name));
				}
				catch (Exception ex)
				{
					mainWindow.StatusMessage(string.Format("Avatar for player {0} had error {1}", name, ex.Message));
				}
			}

			string school = AttributeOrNull(root, "school");
			string language = AttributeOrNull(root, "language");
			player = new Player(Guid.NewGuid().ToString(), name, school, language, avatar, playerColors[ind], new RemoteAI(this, guid));
			engine.Players.Add(player);

			// if we have 6, we're ready to go
			int maxPlayers = AutoRunNumGames == 0 ? Player.NUM_PLAYERS : AutoRunUsers.Count;
			if (DebugStartMode || engine.Players.Count >= maxPlayers || (!string.IsNullOrEmpty(TestUser)))
				CloseJoins();

			string msg = string.Format("Player {0} joined from {1}", name, tcpServer.GetIpAddress(guid));
			log.Info(msg);
			mainWindow.StatusMessage(msg);

			mainWindow.NewPlayerAdded();
			mainWindow.UpdateMenu();
		}

		/// <summary>
		///     Initialize the game, ask players for start positions.
		/// </summary>
		public void CloseJoins()
		{
			// add AI players as needed
			int indName = 0;
			for (int ind = engine.Players.Count; ind < Player.NUM_PLAYERS; ind++)
			{
				Player player = new Player(Guid.NewGuid().ToString(), simpleAiNames[indName++], "Windward Studios", "C#", null, playerColors[ind], new PlayerAI());
				engine.Players.Add(player);
			}
			InitializeGame();
		}

		/// <summary>
		///     Player is ready to start.
		/// </summary>
		/// <param name="player">The player. null if a join, non-null (maybe) if a re-join.</param>
		/// <param name="root">The XML message.</param>
		private void MsgPlayerReady(Player player, XElement root)
		{

			player.WaitingForReply = Player.COMM_MODE.RECEIVED_START;
			// if all ready (or all AI), we start
			if (engine.Players.All(pl => pl.WaitingForReply == Player.COMM_MODE.RECEIVED_START || pl.TcpGuid == null))
			{
				if (commState == COMM_STATE.ACCEPTING_JOINS || commState == COMM_STATE.ACCEPTING_SETUP)
					commState = COMM_STATE.READY_TO_START;
				CommTimerClose();
				mainWindow.SetupGame();

				// test mode? If so we're good and so exit with code 0.
				if ((!string.IsNullOrEmpty(TestUser)) && player.Name == TestUser)
				{
					tcpServer.SendMessage(player.TcpGuid, "<exit/>");
					Thread.Sleep(100);
					Environment.Exit(0);
				}

				if ((DebugStartMode || AutoRunNumGames != 0) && (commState == COMM_STATE.READY_TO_START))
					Play();
			}
		}

		private void InitializeGame()
		{
			commState = COMM_STATE.ACCEPTING_SETUP;

			// reset engine & players
			engine.Initialize();

			// if all ready (or all AI), we start
			if (engine.Players.All(pl => pl.WaitingForReply == Player.COMM_MODE.RECEIVED_START || pl.TcpGuid == null))
				commState = COMM_STATE.READY_TO_START;
			else
				CommTimerStart();

			mainWindow.SetupGame();
		}

		public void RestartJoins()
		{
			acceptMessages = false;

			foreach (Player player in engine.Players.Where(player => player.TcpGuid != null))
				tcpServer.SendMessage(player.TcpGuid, "<exit/>");
			Thread.Sleep(100);

			foreach (Player plyr in engine.Players)
				tcpServer.CloseConnection(plyr.TcpGuid);
			foreach (Player plyr in engine.Players)
				plyr.Dispose();
			engine.Players.Clear();

			commState = COMM_STATE.ACCEPTING_JOINS;
			acceptMessages = true;

			const string msg = "Clear players, re-open for joins";
			log.Info(msg);
			mainWindow.StatusMessage(msg);

			mainWindow.ResetPlayers();
			mainWindow.UpdateMenu();
		}

		/// <summary>
		///     Start or continue (from pause) the game.
		/// </summary>
		public void Play()
		{
			if (AutoRunNumGames != 0)
				FullSpeed = true;

			if ((commState == COMM_STATE.PAUSED) && (timerWorker != null))
			{
				commState = COMM_STATE.RUNNING;
				timerWorker.Start();
				if (PlaySounds)
					trafficPlayer.PlayLooping();
			}
			else
				_Play();
		}

		private void _Play()
		{
			// in case a reset
			if (timerWorker == null)
			{
				if (engine.GameOn != 0)
					InitializeGame();
				engine.GameOn++;

				GameTicks = 0;

				timerWorker = new Timer { Interval = 1000 / MovesPerSecond, Tag = rand.Next() };
				timerWorker.Tick += FrameTick;
			}
			else Trap.trap();

			// we're running
			CommTimerClose();
			commState = COMM_STATE.RUNNING;

			UpdateAll();

			timerWorker.Start();

			if (PlaySounds)
				trafficPlayer.PlayLooping();
		}

		private void UpdateAll()
		{
			mainWindow.UpdateMap();
			mainWindow.UpdatePlayers();
			mainWindow.UpdateMenu();
		}

		public void Step()
		{
			// stop the ticker
			if (timerWorker != null)
				timerWorker.Stop();
			else
			{
				// new game - we create the timer as that IDs if we're starting a new game. But we don't start it
				if (engine.GameOn != 0)
					InitializeGame();
				engine.GameOn++;

				GameTicks = 0;

				timerWorker = new Timer { Interval = 1000 / MovesPerSecond, Tag = rand.Next() };
				timerWorker.Tick += FrameTick;

				CommTimerClose();
				commState = COMM_STATE.PAUSED;
			}

			// run one tick
			commState = COMM_STATE.STEP;
			FrameTick(null, null);
			commState = COMM_STATE.PAUSED;

			// update windows
			UpdateAll();
		}

		public void PauseAtEndOfTurn()
		{
			commState = COMM_STATE.PAUSED;
			timerWorker.Stop();
			if (PlaySounds)
				trafficPlayer.Stop();

			// update windows
			UpdateAll();
		}

		/// <summary>
		///     End the game.
		/// </summary>
		public void Stop()
		{
			if (FullSpeed)
				FullSpeed = false;
			commState = COMM_STATE.GAME_OVER;
			CommTimerClose();
			if (timerWorker != null)
			{
				timerWorker.Stop();
				timerWorker.Dispose();
				timerWorker = null;
			}
			if (PlaySounds)
				trafficPlayer.Stop();
			commState = COMM_STATE.GAME_OVER;

			// update windows
			UpdateAll();
		}

		/// <summary>
		///     The main timer tick handler. Calls all game logic from here on each tick. This is called once every
		///     FRAMES_PER_SECOND.
		/// </summary>
		private void FrameTick(object sender, EventArgs e)
		{
			int numTicks = commState == COMM_STATE.STEP ? 1 : MovesPerSecond;
			numTicks = Math.Max(numTicks, 1);
			numTicks = Math.Min(numTicks, FullSpeed ? GameMap.NUM_X_TILES * GameMap.NUM_Y_TILES + 1 : 12);

			while (numTicks-- > 0)
			{
				// see if game is over
				if (engine.HotelChains.Any(h => h.IsActive) && (engine.HotelChains.All(h => (! h.IsActive) || h.IsSafe) || engine.HotelChains.Any(h => h.NumTiles >= 41)))
				{
					Player plyrWin = engine.Players.OrderByDescending(pl => pl.Score).First();
					plyrWin.IsWinner = true;
					mainWindow.StatusMessage(string.Format("Game over, winner {0} with {1} points", plyrWin.Name, plyrWin.Score));
					GameOver();
					return;
				}

				GameTicks++;
				engine.Tick();

				if (!FullSpeed)
				{
					mainWindow.SetActivePlayer(engine.Players[engine.IndexPlayerOn]);
					
					// we always redraw everything - for any create or merge it can be all and that happens a lot
					mainWindow.UpdateMap();

					mainWindow.RenderMapChanges();
					mainWindow.UpdatePlayers();
					mainWindow.UpdateDebug();
				}
			}

			// render the map & player status
			if (FullSpeed && playerUpdateInterval-- < 0)
			{
				mainWindow.UpdatePlayers();
				playerUpdateInterval = 24;
			}
		}

		private int playerUpdateInterval;

		private void GameOver()
		{
			Stop();
			if (PlaySounds)
				winPlayer.Play();

			// add scores to playes
			foreach (Player plyrOn in engine.Players)
				plyrOn.Scoreboard.Add(plyrOn.Score);

			// update all windows
			mainWindow.UpdateDebug();
			mainWindow.UpdatePlayers();
			mainWindow.UpdateMenu();
			mainWindow.UpdateMap();

			if (AutoRunNumGames <= 0)
				return;
			if (engine.GameOn < AutoRunNumGames)
			{
				Play();
				return;
			}

			// write out the results
			XDocument xml = new XDocument();
			XElement root = new XElement("players");
			xml.Add(root);
			foreach (Player player in engine.Players)
			{
				XElement elem = new XElement("player", new XAttribute("name", player.Name))
				{
					Value = string.Join(";", player.Scoreboard)
				};
				root.Add(elem);
			}
			xml.Save(AutoRunFilename);

			foreach (Player player in engine.Players.Where(player => player.TcpGuid != null))
				tcpServer.SendMessage(player.TcpGuid, "<exit/>");

			mainWindow.Exit();
		}

		private static string AttributeOrNull(XElement element, string name)
		{
			XAttribute attr = element.Attribute(name);
			return attr == null ? null : attr.Value;
		}

		// names for SimpleAI
		private static readonly string[] simpleAiNames =
		{
			"Sheldon Cooper", "Leonard Hofstadter", "Rajesh Koothrappali", "Howard Wolowitz", "Penny",
			"Bernadette Rostenkowski", "Amy Farrah Fowler", "Leslie Winkle", "Barry Kripke", "Stuart Bloom"
		};

		private static readonly Color[] playerColors =
		{
			Color.FromArgb(255, 255, 3, 3), Color.FromArgb(255, 0, 66, 255), Color.FromArgb(255, 28, 230, 185),
			Color.FromArgb(255, 83, 0, 127), Color.FromArgb(255, 255, 252, 1),
			Color.FromArgb(255, 254, 138, 14), Color.FromArgb(255, 32, 192, 0), Color.FromArgb(255, 229, 91, 176),
			Color.FromArgb(255, 126, 191, 241), Color.FromArgb(255, 16, 98, 70)
		};
	}
}