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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;
using Server.Map;
using Server.Sprites;
using Server.Units;
using Server.Utilities;

namespace Server.UI
{
	/// <summary>
	///     The main window.
	/// </summary>
	public partial class MainWindow : Form, IUserDisplay, IMapInfo
	{
		private PlayerScore dlgScoreboard;
		private StatusMessages dlgStatusList;
		private DebugWindow dlgDebugWindow;

		/// <summary>
		///     Create the window.
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();
		}

		private void MainWindow_Load(object sender, EventArgs e)
		{
			// initialize engine
			GameFramework = new Framework(this);
			PixelsPerTile = 72;

			// restore windows
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Windward Studios\Acquire\windows"))
				if (key != null)
				{
					// only restore if matches
					int cx = (int) key.GetValue("desktop-cx", -1);
					int cy = (int) key.GetValue("desktop-cy", -1);
					if ((cx == Screen.PrimaryScreen.WorkingArea.Width) && (cy == Screen.PrimaryScreen.WorkingArea.Height))
					{
						RestoreWindow(this, "main");
						FormWindowState state;
						Point location;
						Size size;
						RestoreWindow(key, "scoreboard", out state, out location, out size);
						if (state != FormWindowState.Minimized)
							CreateOrActivateScoreboard();
						RestoreWindow(key, "status", out state, out location, out size);
						if (state != FormWindowState.Minimized)
							CreateOrActivateStatus();
					}

					GameFramework.MovesPerSecond = (int) key.GetValue("game-speed", GameFramework.MovesPerSecond);

					toolStripMenuItemCoordinates.Checked = Convert.ToBoolean(key.GetValue("display-coordinates", "false"));

					GameFramework.DebugStartMode = Convert.ToBoolean(key.GetValue("run-debug", "false"));
					GameFramework.PlaySounds = Convert.ToBoolean(key.GetValue("play-sounds", "true"));

					PixelsPerTile = Convert.ToInt32(key.GetValue("zoom", PixelsPerTile));
				}

			DebugCheckEnable();
			MuteBitmap();

			mapDisplay.DisplayCoordinates = toolStripMenuItemCoordinates.Checked;

			switch (PixelsPerTile)
			{
				case 12:
					toolStripMenuItemZoom50.Checked = true;
					break;
				case 18:
					toolStripMenuItemZoom75.Checked = true;
					break;
				case 24:
					toolStripMenuItemZoom100.Checked = true;
					break;
				case 48:
					toolStripMenuItemZoom200.Checked = true;
					break;
			}
			mapDisplay.ZoomChanged();

			UpdateMap();
			UpdatePlayers();
			UpdateMenu();
		}

		public void Exit()
		{
			Close();
		}

		public static void RestoreWindow(Form ctrl, string name)
		{
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Windward Studios\Acquire\windows"))
				if (key != null)
				{
					// only restore if matches
					int cx = (int) key.GetValue("desktop-cx", -1);
					int cy = (int) key.GetValue("desktop-cy", -1);
					if ((cx == Screen.PrimaryScreen.WorkingArea.Width) && (cy == Screen.PrimaryScreen.WorkingArea.Height))
					{
						FormWindowState state;
						Point location;
						Size size;
						RestoreWindow(key, name, out state, out location, out size);
						switch (state)
						{
							case FormWindowState.Maximized:
								ctrl.WindowState = state;
								break;
							case FormWindowState.Normal:
								ctrl.Location = location;
								ctrl.Size = size;
								break;
						}
					}
				}
		}

		/// <summary>
		///     The engine that operates the game.
		/// </summary>
		internal Framework GameFramework { get; private set; }

		/// <summary>
		///     Used for Invoke when we get TCP callbacks.
		/// </summary>
		public Control CtrlForInvoke
		{
			get { return this; }
		}

		private void toolStripButtonJoin_Click(object sender, EventArgs e)
		{
			Trace.WriteLine("menu: Join");
			if (
				MessageBox.Show(this, @"Do you want to end this series and drop all players?", @"Acquire",
					MessageBoxButtons.YesNo) != DialogResult.Yes)
				return;
			GameFramework.RestartJoins();
			UpdateMenu();
		}

		private void toolStripButtonClosed_Click(object sender, EventArgs e)
		{
			Trace.WriteLine("menu: Locked");
			if (GameFramework.GameEngine.Players.Count < Player.NUM_PLAYERS)
				if (
					MessageBox.Show(this, @"Have all players joined for this series?", @"Acquire", MessageBoxButtons.YesNo) !=
					DialogResult.Yes)
					return;
			GameFramework.CloseJoins();
			UpdateMenu();
		}

		/// <summary>
		///     The play button was clicked.
		/// </summary>
		private void Play_Click(object sender, EventArgs e)
		{
			Trace.WriteLine("menu: Play");
			GameFramework.Play();
			UpdateMenu();
		}

		private void toolStripButtonStep_Click(object sender, EventArgs e)
		{
			Trace.WriteLine("menu: Step");
			GameFramework.Step();
			UpdateMenu();
		}

		/// <summary>
		///     The pause button was clicked.
		/// </summary>
		private void Pause_Click(object sender, EventArgs e)
		{
			Trace.WriteLine("menu: Pause");
			if (GameFramework.FullSpeed)
				GameFramework.FullSpeed = false;
			GameFramework.PauseAtEndOfTurn();
			UpdateMenu();
		}

		/// <summary>
		///     The stop button was clicked.
		/// </summary>
		private void Stop_Click(object sender, EventArgs e)
		{
			Trace.WriteLine("menu: Stop");
			if (MessageBox.Show(this, @"Do you want to end this run?", @"Acquire", MessageBoxButtons.YesNo) !=
			    DialogResult.Yes)
				return;
			if (GameFramework.FullSpeed)
				GameFramework.FullSpeed = false;
			GameFramework.Stop();
			UpdateMenu();
		}

		private void toolStripButtonSpeed_Click(object sender, EventArgs e)
		{
			Trace.WriteLine("menu: Speed");
			using (GameSpeed dlg = new GameSpeed(GameFramework.MovesPerSecond))
			{
				if (dlg.ShowDialog(this) == DialogResult.OK)
					GameFramework.MovesPerSecond = dlg.MovesPerSecond;
			}
			UpdateMenu();
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			GameFramework.PlaySounds = ! GameFramework.PlaySounds;
			MuteBitmap();

			if (GameFramework.Mode == Framework.COMM_STATE.RUNNING)
			{
				if (!GameFramework.PlaySounds)
					GameFramework.trafficPlayer.Stop();
				else
					GameFramework.trafficPlayer.PlayLooping();
			}
		}

		private void MuteBitmap()
		{
			toolStripMuteSound.Image = GameFramework.PlaySounds ? StatusSprites.loudspeaker : StatusSprites.loudspeaker_preferences;
		}

		private void toolStripMenuItem2_Click(object sender, EventArgs e)
		{
			PixelsPerTile = 96;
			toolStripMenuItemZoom200.Checked = true;
			toolStripMenuItemZoom100.Checked = toolStripMenuItemZoom75.Checked = toolStripMenuItemZoom50.Checked = false;
			mapDisplay.Invalidate();
			using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Windward Studios\Acquire\windows"))
				if (key != null)
					key.SetValue("zoom", PixelsPerTile);
		}

		private void toolStripMenuItem3_Click(object sender, EventArgs e)
		{
			PixelsPerTile = 72;
			toolStripMenuItemZoom100.Checked = true;
			toolStripMenuItemZoom200.Checked = toolStripMenuItemZoom75.Checked = toolStripMenuItemZoom50.Checked = false;
			mapDisplay.Invalidate();
			using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Windward Studios\Acquire\windows"))
				if (key != null)
					key.SetValue("zoom", PixelsPerTile);
		}

		private void toolStripMenuItem4_Click(object sender, EventArgs e)
		{
			PixelsPerTile = 48;
			toolStripMenuItemZoom75.Checked = true;
			toolStripMenuItemZoom200.Checked = toolStripMenuItemZoom100.Checked = toolStripMenuItemZoom50.Checked = false;
			mapDisplay.Invalidate();
			using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Windward Studios\Acquire\windows"))
				if (key != null)
					key.SetValue("zoom", PixelsPerTile);
		}

		private void toolStripMenuItem5_Click(object sender, EventArgs e)
		{
			PixelsPerTile = 36;
			toolStripMenuItemZoom50.Checked = true;
			toolStripMenuItemZoom200.Checked = toolStripMenuItemZoom100.Checked = toolStripMenuItemZoom75.Checked = false;
			mapDisplay.Invalidate();
			using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Windward Studios\Acquire\windows"))
				if (key != null)
					key.SetValue("zoom", PixelsPerTile);
		}

		private void toolStripMenuItemCoordinates_Click(object sender, EventArgs e)
		{
			toolStripMenuItemCoordinates.Checked = !toolStripMenuItemCoordinates.Checked;
			mapDisplay.DisplayCoordinates = toolStripMenuItemCoordinates.Checked;
			mapDisplay.Invalidate();
		}

		private void toolStripDebugReset_Click(object sender, EventArgs e)
		{
			GameFramework.Stop();
			GameFramework.RestartJoins();
		}

		#region scoreboard

		private void toolStripButtonScoreboard_Click(object sender, EventArgs e)
		{
			CreateOrActivateScoreboard();
		}

		private void CreateOrActivateScoreboard()
		{
			if (dlgScoreboard != null)
			{
				dlgScoreboard.WindowState = FormWindowState.Normal;
				dlgScoreboard.Activate();
				return;
			}

			dlgScoreboard = new PlayerScore(GameFramework);
			dlgScoreboard.SetupGame();
			dlgScoreboard.Closing += dlgScoreboard_Closing;
			dlgScoreboard.Show(this);
		}

		private void dlgScoreboard_Closing(object sender, CancelEventArgs e)
		{
			dlgScoreboard = null;
		}

		#endregion

		#region debug window

		private void toolStripButtonDebugWindow_Click(object sender, EventArgs e)
		{
			CreateOrActivateDebugWindow();
		}

		private void CreateOrActivateDebugWindow()
		{
			if (dlgDebugWindow != null)
			{
				dlgDebugWindow.WindowState = FormWindowState.Normal;
				dlgDebugWindow.Activate();
				return;
			}

			dlgDebugWindow = new DebugWindow(GameFramework.GameEngine.Players);
			dlgDebugWindow.Closing += dlgDebugWindow_Closing;
			dlgDebugWindow.Show();
		}

		private void dlgDebugWindow_Closing(object sender, CancelEventArgs e)
		{
			dlgDebugWindow = null;
		}

		#endregion

		#region status form

		private void toolStripButtonStatusList_Click(object sender, EventArgs e)
		{
			CreateOrActivateStatus();
		}

		private void CreateOrActivateStatus()
		{
			if (dlgStatusList != null)
			{
				dlgStatusList.WindowState = FormWindowState.Normal;
				dlgStatusList.Activate();
				return;
			}

			dlgStatusList = new StatusMessages();
			dlgStatusList.Closing += dlgStatusList_Closing;
			dlgStatusList.Show();
			foreach (string msg in messages)
				dlgStatusList.AddMessage(msg);
		}

		private void dlgStatusList_Closing(object sender, CancelEventArgs e)
		{
			dlgStatusList = null;
		}

		private readonly IList<string> messages = new List<string>();

		/// <summary>
		///     Adds a message to the status window.
		/// </summary>
		/// <param name="message">The message to add.</param>
		public void StatusMessage(string message)
		{
			messages.Add(message);
			if (dlgStatusList != null)
				dlgStatusList.AddMessage(message);
		}

		#endregion

		/// <summary>
		///     Update the main window menu.
		/// </summary>
		public void UpdateMenu()
		{
			toolStripButtonJoinOpened.Enabled = GameFramework.Mode == Framework.COMM_STATE.GAME_OVER;
			toolStripButtonJoinClosed.Enabled = GameFramework.Mode == Framework.COMM_STATE.ACCEPTING_JOINS;
				// bugbug && GameFramework.GameEngine.Players.Count > 0;
			toolStripButtonPlay.Enabled = GameFramework.Mode == Framework.COMM_STATE.READY_TO_START ||
			                              GameFramework.Mode == Framework.COMM_STATE.PAUSED ||
			                              GameFramework.Mode == Framework.COMM_STATE.GAME_OVER;
			toolStripButtonStep.Enabled = GameFramework.Mode == Framework.COMM_STATE.READY_TO_START ||
			                              GameFramework.Mode == Framework.COMM_STATE.PAUSED ||
			                              GameFramework.Mode == Framework.COMM_STATE.GAME_OVER;
			toolStripButtonPause.Enabled = GameFramework.Mode == Framework.COMM_STATE.RUNNING;
			toolStripButtonStop.Enabled = GameFramework.Mode == Framework.COMM_STATE.RUNNING ||
			                              GameFramework.Mode == Framework.COMM_STATE.PAUSED;

			toolStripButtonDebugWindow.Enabled = GameFramework.GameEngine.Players != null &&
			                                     GameFramework.GameEngine.Players.Count > 0 &&
			                                     GameFramework.GameEngine.HotelChains != null &&
												 GameFramework.GameEngine.HotelChains.Count > 0;
			toolStripDebugReset.Enabled = GameFramework.DebugStartMode &&
			                              (GameFramework.Mode == Framework.COMM_STATE.RUNNING ||
			                               GameFramework.Mode == Framework.COMM_STATE.STEP
			                               || GameFramework.Mode == Framework.COMM_STATE.PAUSED ||
			                               GameFramework.Mode == Framework.COMM_STATE.GAME_OVER);
			toolStripFullSpeed.Enabled = GameFramework.Mode == Framework.COMM_STATE.READY_TO_START ||
			                             GameFramework.Mode == Framework.COMM_STATE.PAUSED ||
			                             GameFramework.Mode == Framework.COMM_STATE.GAME_OVER;

			toolStripButtonPlayCard.Enabled = GameFramework.Mode == Framework.COMM_STATE.PAUSED;
		}

		/// <summary>
		///     Got a close window event. Ask for confirmation if a game is in process.
		/// </summary>
		private void GameDisplay_FormClosing(object sender, FormClosingEventArgs fcea)
		{
			if ((GameFramework.Mode != Framework.COMM_STATE.GAME_OVER) &&
			    (MessageBox.Show(this, @"Do you want to close the game?", @"Acquire", MessageBoxButtons.YesNo) !=
			     DialogResult.Yes))
				fcea.Cancel = true;

			if (fcea.Cancel)
				return;

			SaveWindowLocations();

			if (dlgScoreboard != null)
				dlgScoreboard.Close();
			if (dlgStatusList != null)
				dlgStatusList.Close();
			if (dlgDebugWindow != null)
				dlgDebugWindow.Close();
			GameFramework.tcpServer.CloseAllConnections();
		}

		private void SaveWindowLocations()
		{
			using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Windward Studios\Acquire\windows"))
				if (key != null)
				{
					// save desktop size - only restore if matches
					key.SetValue("desktop-cx", Screen.PrimaryScreen.WorkingArea.Width);
					key.SetValue("desktop-cy", Screen.PrimaryScreen.WorkingArea.Height);

					SaveWindow(key, this, "main");
					if (dlgScoreboard != null)
						SaveWindow(key, dlgScoreboard, "scoreboard");
					else
						key.SetValue("scoreboard-mode", FormWindowState.Minimized);
					if (dlgStatusList != null)
						SaveWindow(key, dlgStatusList, "status");
					else
						key.SetValue("status-mode", FormWindowState.Minimized);

					key.SetValue("game-speed", GameFramework.MovesPerSecond);

					key.SetValue("display-coordinates", Convert.ToString(toolStripMenuItemCoordinates.Checked));

					key.SetValue("run-debug", Convert.ToString(GameFramework.DebugStartMode));
					key.SetValue("play-sounds", Convert.ToString(GameFramework.PlaySounds));
				}
		}

		private static void SaveWindow(RegistryKey key, Form ctrl, string name)
		{
			key.SetValue(name + "-mode", ctrl.WindowState);
			key.SetValue(name + "-x", ctrl.Location.X);
			key.SetValue(name + "-y", ctrl.Location.Y);
			key.SetValue(name + "-cx", ctrl.Size.Width);
			key.SetValue(name + "-cy", ctrl.Size.Height);
		}

		private static void RestoreWindow(RegistryKey key, string name, out FormWindowState state, out Point location,
			out Size size)
		{
			string str = (string) key.GetValue(name + "-mode");
			state = string.IsNullOrEmpty(str)
				? FormWindowState.Minimized
				: (FormWindowState) Enum.Parse(typeof (FormWindowState), str);
			int x = (int) key.GetValue(name + "-x", -1);
			int y = (int) key.GetValue(name + "-y", -1);
			location = x == -1 || y == -1 ? Point.Empty : new Point(x, y);
			x = (int) key.GetValue(name + "-cx", -1);
			y = (int) key.GetValue(name + "-cy", -1);
			size = x == -1 || y == -1 ? Size.Empty : new Size(x, y);
		}

		/// <summary>
		///     The app is exiting - close all connections.
		/// </summary>
		private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
		{
			GameFramework.tcpServer.CloseAllConnections();
		}

		/// <summary>
		///     Called when starting a game.
		/// </summary>
		public void SetupGame()
		{
			if (dlgScoreboard != null)
				dlgScoreboard.SetupGame();
			if (dlgDebugWindow != null)
				dlgDebugWindow.Setup(GameFramework.GameEngine.Players);

			UpdateMap();
			UpdatePlayers();
			UpdateMenu();
			UpdateDebug();
		}

		/// <summary>
		///     New player added to the game
		/// </summary>
		public void NewPlayerAdded()
		{
			if (dlgScoreboard != null)
				dlgScoreboard.NewPlayerAdded();
			Trap.trap(dlgDebugWindow != null);
			if (dlgDebugWindow != null)
				dlgDebugWindow.Setup(GameFramework.GameEngine.Players);
		}

		/// <summary>
		///     Update the map window.
		/// </summary>
		public void UpdateMap()
		{
			mapDisplay.Invalidate();
			mapDisplay.Update();
		}

		/// <summary>
		///     Called to re-render all locations with vehicles.
		/// </summary>
		public void RenderMapChanges()
		{
			mapDisplay.Update();
		}

		public void SetActivePlayer(Player plyr)
		{
			if (dlgScoreboard != null)
				dlgScoreboard.SetActivePlayer(plyr);
		}

		/// <summary>
		///     Called to update (re-draw) the player status windows.
		/// </summary>
		public void UpdatePlayers()
		{
			if (dlgScoreboard != null)
				dlgScoreboard.UpdatePlayers();
		}

		public void UpdateDebug()
		{
			if (dlgDebugWindow != null)
				dlgDebugWindow.Update(GameFramework.GameEngine.Players);
		}

		/// <summary>
		///     Called to delete all player status windows the player status windows.
		/// </summary>
		public void ResetPlayers()
		{
			Trap.trap(dlgScoreboard == null);
			if (dlgScoreboard != null)
				dlgScoreboard.ResetPlayers();
			Trap.trap(dlgDebugWindow != null);
			if (dlgDebugWindow != null)
				dlgDebugWindow.Setup(GameFramework.GameEngine.Players);
		}

		/// <summary>
		///     Set the turn or phase number increases.
		/// </summary>
		/// <param name="turn">The turn number.</param>
		public void TurnNumber(int turn)
		{
			Text = string.Format("Acquire - Game: {0}", turn);
		}

		/// <summary>
		///     The game map.
		/// </summary>
		public GameMap Map
		{
			get
			{
				return GameFramework == null ? null : (GameFramework.GameEngine == null ? null : GameFramework.GameEngine.MainMap);
			}
		}

		/// <summary>
		///     The Limos to display on the map.
		/// </summary>
		public List<Player> Players
		{
			get { return GameFramework.GameEngine.Players; }
		}

		private int pixelsPerTile;

		/// <summary>
		///     The pixels per tile (due to zoom). Will be 48, 24, 12, or 6
		/// </summary>
		public int PixelsPerTile
		{
			get { return pixelsPerTile; }
			private set
			{
				pixelsPerTile = value;
				mapDisplay.ZoomChanged();
			}
		}

		public void NewMap()
		{
			mapDisplay.ZoomChanged();
		}

		private void toolStripDebugRun_Click(object sender, EventArgs e)
		{
			GameFramework.DebugStartMode = !toolStripDebugRun.Checked;
			DebugCheckEnable();
		}

		private void DebugCheckEnable()
		{
			toolStripDebugRun.Checked = GameFramework.DebugStartMode;
			toolStripDebugRun.Image = toolStripDebugRun.Checked ? StatusSprites.debug_run : StatusSprites.bug_yellow;
		}

		private void toolStripFullSpeed_Click(object sender, EventArgs e)
		{
			GameFramework.FullSpeed = true;
			GameFramework.Play();
			UpdateMenu();
		}

		private void toolStripButtonPlayCard_Click(object sender, EventArgs e)
		{
			using (PlayCard dlg = new PlayCard())
			{
				Player plyrNext = GameFramework.GameEngine.Players[GameFramework.GameEngine.IndexPlayerOn];
				dlg.labelPlayer.Text = plyrNext.Name;

				// disable ones already played
				dlg.rb3Free.Enabled = plyrNext.Powers.Any(card => card.Card == SpecialPowers.CARD.FREE_3_STOCK);
				dlg.rbBuy5.Enabled = plyrNext.Powers.Any(card => card.Card == SpecialPowers.CARD.BUY_5_STOCK);
				dlg.rbTrade2.Enabled = plyrNext.Powers.Any(card => card.Card == SpecialPowers.CARD.TRADE_2_STOCK);
				dlg.rbTake5Tiles.Enabled = plyrNext.Powers.Any(card => card.Card == SpecialPowers.CARD.DRAW_5_TILES);
				dlg.rbPlace4Tiles.Enabled = plyrNext.Powers.Any(card => card.Card == SpecialPowers.CARD.PLACE_4_TILES);

				if (dlg.ShowDialog(this) != DialogResult.OK)
					return;

				// before place tile
				if (dlg.rbTake5Tiles.Checked)
					plyrNext.PlayCardBefore = SpecialPowers.CARD.DRAW_5_TILES;
				else if (dlg.rbPlace4Tiles.Checked)
					plyrNext.PlayCardBefore = SpecialPowers.CARD.PLACE_4_TILES;
				else
					plyrNext.PlayCardBefore = SpecialPowers.CARD.NONE;

				// after place tile
				if (dlg.rb3Free.Checked)
					plyrNext.PlayCardAfter = SpecialPowers.CARD.FREE_3_STOCK;
				else if (dlg.rbBuy5.Checked)
					plyrNext.PlayCardAfter = SpecialPowers.CARD.BUY_5_STOCK;
				else if (dlg.rbTrade2.Checked)
					plyrNext.PlayCardAfter = SpecialPowers.CARD.TRADE_2_STOCK;
				else
					plyrNext.PlayCardAfter = SpecialPowers.CARD.NONE;
			}
		}
	}
}