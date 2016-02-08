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
using System.Drawing;
using System.Linq;
using Server.AI;
using Server.Map;


namespace Server.Units
{
	/// <summary>
	///     Adds engine items to the player object.
	/// </summary>
	public class Player : IDisposable
	{
		/// <summary>
		/// The mode of communication with this player's remote AI.
		/// </summary>
		public enum COMM_MODE
		{
			/// <summary>
			///     Waiting for initial start move.
			/// </summary>
			WAITING_FOR_START,

			/// <summary>
			///     Got the start move.
			/// </summary>
			RECEIVED_START,
		}

		/// <summary>
		/// The number of players in the game.
		/// </summary>
		public const int NUM_PLAYERS = 6;

		private readonly IPlayerAI ai;

		#region properties

		/// <summary>
		/// List of the power-ups we need to display for 5 seconds.
		/// </summary>
		private readonly List<SpecialPowersIcon> displayIcons;

		/// <summary>
		/// The tiles this player is holding. You only see your own, this will be empty for other players.
		/// </summary>
		public List<PlayerTile> Tiles { get; private set; }

		/// <summary>
		/// The special powers this player is holding. This is just the unplayed cards
		/// </summary>
		public List<SpecialPowers> Powers { get; private set; }

		/// <summary>
		/// The cash this player has on hand.
		/// </summary>
		public int Cash { get; set; }

		/// <summary>
		///     The unique identifier for this player. This will remain constant for the length of the game (while the Player
		///     objects passed will
		///     change on every call).
		/// </summary>
		public string Guid { get; private set; }

		/// <summary>
		///     The name of the player.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///     The avatar of the player.
		/// </summary>
		public Image Avatar { get; private set; }

		/// <summary>
		///     The school this player is from.
		/// </summary>
		public string School { get; private set; }

		/// <summary>
		///     The computer language this A.I. was written in.
		/// </summary>
		public string Language { get; private set; }

		/// <summary>
		///     We are waiting for a reply from this player.
		/// </summary>
		public COMM_MODE WaitingForReply { get; set; }

		public List<HotelStock> Stock { get; private set; }

		public bool IsWinner { get; set; }

		/// <summary>
		/// Debug mode only - this player will play this card at the start of their next turn. If they already played this card, the command will be ignored.
		/// </summary>
		public SpecialPowers.CARD PlayCardBefore { get; set; }

		/// <summary>
		/// Debug mode only - this player will play this card at the end of their next turn. If they already played this card, the command will be ignored.
		/// </summary>
		public SpecialPowers.CARD PlayCardAfter { get; set; }

		/// <summary>
		///     The score for this player - this game
		/// </summary>
		public int Score
		{
			get
			{
				int score = Cash;
				foreach (HotelStock stock in Stock.Where(stock => stock.Chain.IsActive))
				{
					score += stock.NumShares * stock.Chain.StockPrice;

					var majorityOwners = stock.Chain.FirstMajorityOwners;
					if (majorityOwners.Any(owner => owner.Owner == this))
						score += stock.Chain.FirstMajorityBonus / majorityOwners.Count;
					var secondMajorityOwners = stock.Chain.SecondMajorityOwners;
					if (secondMajorityOwners.Any(owner => owner.Owner == this))
						score += stock.Chain.SecondMajorityBonus / secondMajorityOwners.Count;
				}
				return score;
			}
		}

		/// <summary>
		///     The score for this player - previous games.
		/// </summary>
		public List<int> Scoreboard { get; private set; }

		/// <summary>
		///     The color for this player on the status window.
		/// </summary>
		public Color SpriteColor { get; private set; }

		/// <summary>
		///     The color for this player on the status window.
		/// </summary>
		public Color TransparentSpriteColor { get; private set; }

		#endregion


		/// <summary>
		///     Create a player object. This is used during setup.
		/// </summary>
		/// <param name="guid">The unique identifier for this player.</param>
		/// <param name="name">The name of the player.</param>
		/// <param name="school">The school this player is from.</param>
		/// <param name="language">The computer language this A.I. was written in.</param>
		/// <param name="avatar">The avatar of the player.</param>
		/// <param name="spriteColor">The color of this player's sprite.</param>
		/// <param name="ai">The AI for this player.</param>
		public Player(string guid, string name, string school, string language, Image avatar, Color spriteColor,
			IPlayerAI ai)
		{
			Guid = guid;
			Name = name;
			School = school.Length <= 18 ? school : school.Substring(0, 18);
			Language = language;
			Avatar = avatar;
			SpriteColor = spriteColor;
			TransparentSpriteColor = Color.FromArgb(96, spriteColor.R, spriteColor.G, spriteColor.B);
			this.ai = ai;
			IsConnected = true;
			displayIcons = new List<SpecialPowersIcon>();

			Scoreboard = new List<int>();

			Cash = 6000;
			Tiles = new List<PlayerTile>();
			Powers = new List<SpecialPowers>();
			Stock = new List<HotelStock>();

			PlayCardBefore = PlayCardAfter = SpecialPowers.CARD.NONE;
		}

		/// <summary>
		///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			ai.Dispose();
		}

		public void Reset()
		{
			Cash = 6000;
			Tiles.Clear();
			Stock.Clear();
			Powers = SpecialPowers.PlayerDeck();
			WaitingForReply = COMM_MODE.WAITING_FOR_START;
			displayIcons.Clear();
			IsWinner = false;

			PlayCardBefore = PlayCardAfter = SpecialPowers.CARD.NONE;
		}

		/// <summary>
		///     The GUID for this player's connection. This will change if the connection has to be re-established. It is
		///     null for the local AIs.
		/// </summary>
		public string TcpGuid
		{
			get { return ai.TcpGuid; }
			set
			{
				IsConnected = value != null;
				ai.TcpGuid = value;
			}
		}

		/// <summary>
		///     true if connected.
		/// </summary>
		public bool IsConnected { get; private set; }

		/// <summary>
		/// Called when the game starts, providing all info.
		/// </summary>
		/// <param name="map">The game map.</param>
		/// <param name="me">The player being setup.</param>
		/// <param name="hotelChains">All hotel chains.</param>
		/// <param name="players">All the players.</param>
		public void Setup(GameMap map, Player me, List<HotelChain> hotelChains, List<Player> players)
		{
			ai.Setup(map, me, hotelChains, players);
		}

		/// <summary>
		/// Asks if you want to play the CARD.DRAW_5_TILES or CARD.PLACE_4_TILES special power. This call will not be made 
		/// if you have already played these cards.
		/// </summary>
		/// <param name="map">The game map.</param>
		/// <param name="me">The player being setup.</param>
		/// <param name="hotelChains">All hotel chains.</param>
		/// <param name="players">All the players.</param>
		/// <returns>CARD.NONE or CARD.DRAW_5_TILES.</returns>
		public SpecialPowers.CARD QuerySpecialPowerBeforeTurn(GameMap map, Player me, List<HotelChain> hotelChains, List<Player> players)
		{
			return ai.QuerySpecialPowerBeforeTurn(map, me, hotelChains, players);
		}

		/// <summary>
		/// Return what tile to play when using the CARD.PLACE_4_TILES. This will be called for the first 3 tiles and is for
		/// pacement only. Any merges due to this will be resolved before the next card is played. For the 4th tile, QueryTileAndPurchase
		/// will be called.
		/// </summary>
		/// <param name="map">The game map.</param>
		/// <param name="me">The player being setup.</param>
		/// <param name="hotelChains">All hotel chains.</param>
		/// <param name="players">All the players.</param>
		/// <returns>The tile(s) to play and the stock to purchase (and trade if CARD.TRADE_2_STOCK is played).</returns>
		public PlayerPlayTile QueryTileOnly(GameMap map, Player me, List<HotelChain> hotelChains, List<Player> players)
		{
			return ai.QueryTileOnly(map, me, hotelChains, players);
		}

		/// <summary>
		/// Return what tile(s) to play and what stock(s) to purchase. At this point merges have not yet been processed.
		/// </summary>
		/// <param name="map">The game map.</param>
		/// <param name="me">The player being setup.</param>
		/// <param name="hotelChains">All hotel chains.</param>
		/// <param name="players">All the players.</param>
		/// <returns>The tile(s) to play and the stock to purchase (and trade if CARD.TRADE_2_STOCK is played).</returns>
		public PlayerTurn QueryTileAndPurchase(GameMap map, Player me, List<HotelChain> hotelChains, List<Player> players)
		{
			return ai.QueryTileAndPurchase(map, me, hotelChains, players);
		}

		/// <summary>
		/// Ask the AI what they want to do with their merged stock. If a merge is for 3+ chains, this will get called once per
		/// removed chain.
		/// </summary>
		/// <param name="map">The game map.</param>
		/// <param name="me">The player being setup.</param>
		/// <param name="hotelChains">All hotel chains.</param>
		/// <param name="players">All the players.</param>
		/// <param name="survivor">The hotel that survived the merge.</param>
		/// <param name="defunct">The hotel that is now defunct.</param>
		/// <returns>What you want to do with the stock.</returns>
		public PlayerMerge QueryMergeStock(GameMap map, Player me, List<HotelChain> hotelChains, List<Player> players, HotelChain survivor, HotelChain defunct)
		{
			return ai.QueryMergeStock(map, me, hotelChains, players, survivor, defunct);
		}

		/// <summary>
		///     Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
		/// </summary>
		/// <returns>
		///     A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
		/// </returns>
		public override string ToString()
		{
			return String.Format("{0}, Score: {1}", Name, Score);
		}

		/// <summary>
		/// Add a powerup that is displayed for 5 seconds in this player's status.
		/// </summary>
		/// <param name="pu">The power-up to display.</param>
		/// <param name="ticks">Number of ticks to display this for.</param>
		public void AddToDisplay(SpecialPowers pu, int ticks)
		{
			displayIcons.RemoveAll(p => p.powerup.Card == pu.Card);
			displayIcons.Add(new SpecialPowersIcon(pu, ticks));
		}

		/// <summary>
		/// One tick occured. Use this to count down the power-up's to display.
		/// </summary>
		public void Tick()
		{
			if (displayIcons.Count == 0)
				return;
			foreach (var pui in displayIcons)
				pui.ticks--;
			displayIcons.RemoveAll(p => p.ticks <= 0);
		}

		/// <summary>
		/// The cards that need to be displayed.
		/// </summary>
		public List<SpecialPowers.CARD> DisplayCards
		{
			get
			{
				return displayIcons.Select(diOn => diOn.powerup.Card).ToList();
			}
		}

		private class SpecialPowersIcon
		{
			internal readonly SpecialPowers powerup;
			internal int ticks;

			public SpecialPowersIcon(SpecialPowers powerup, int ticks)
			{
				this.powerup = powerup;
				this.ticks = ticks;
			}
		}
	}
}