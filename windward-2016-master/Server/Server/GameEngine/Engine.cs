/*
 * ----------------------------------------------------------------------------
 * "THE BEER-WARE LICENSE" 
 * As long as you retain this notice you can do whatever you want with this 
 * stuff. If you meet an employee from Windward meet some day, and you think
 * this stuff is worth it, you can buy them a beer in return. Windward Studios
 * ----------------------------------------------------------------------------
 */


using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Server.AI;
using Server.Map;
using Server.Units;
using Server.Utilities;

namespace Server.GameEngine
{
	public class Engine
	{


		private readonly Framework framework;

		internal ConcurrentQueue<AiMessage> RemoteMessages;

		internal class AiMessage
		{
			/// <summary>
			/// The Player this message is for.
			/// </summary>
			public Player Player { get; private set; }

			protected AiMessage(Player player)
			{
				Player = player;
			}
		}

		// The game map.
		internal GameMap MainMap { get; private set; }

		/// <summary>
		/// All of the players.
		/// </summary>
		internal List<Player> Players { get; private set; }

		/// <summary>
		/// All of the hotel chains.
		/// </summary>
		internal List<HotelChain> HotelChains { get; private set; }

		internal List<PlayerTile> DrawTiles = new List<PlayerTile>();

		/// <summary>
		/// Which game we are playing (zero based).
		/// </summary>
		public int GameOn { get; set; }

		/// <summary>
		/// This player is the one who's turn it is on the next tick.
		/// </summary>
		public int IndexPlayerOn { get; private set; }

		// anytime we need a random number.
		private static readonly Random rand = new Random();

		private static readonly ILog log = LogManager.GetLogger(typeof(Engine));

		public Engine(Framework framework)
		{
			GameOn = 0;
			IndexPlayerOn = 0;
			RemoteMessages = new ConcurrentQueue<AiMessage>();
			Players = new List<Player>();

			// Set up hotel chains
			HotelChains = HotelChain.CreateAllHotelChains();

			this.framework = framework;

			// create the map
			MainMap = new GameMap();
		}

		#region initialize

		public void Initialize()
		{
			// new map
			if (GameOn != 0)
			{
				MainMap = new GameMap();
				framework.mainWindow.NewMap();
				IndexPlayerOn = GameOn % Players.Count;
				foreach (var hotel in HotelChains)
					hotel.Reset();
			}

			// get all tiles from map to assign to players - randomized
			DrawTiles = PlayerTile.AllTiles();
			DrawTiles.Shuffle();

			// set players to start of new run
			foreach (Player plyrOn in Players)
			{
				plyrOn.Reset();
				DrawTilesUpToLimit(plyrOn);
			}

			// tell each player we're starting
			foreach (Player plyr in Players)
				plyr.Setup(MainMap, plyr, HotelChains, Players);

			framework.mainWindow.TurnNumber(framework.GameEngine.GameOn + 1);
		}

		public void RestartPlayer(Player player)
		{
			player.Setup(MainMap, player, HotelChains, Players);
		}

		#endregion

		#region execute turn/phase

		public void Tick()
		{
			ValidateData(true);

			Player plyrOn = Players[IndexPlayerOn];

			SpecialPowers.CARD card = GetTiles(plyrOn);

			// debugger override
			if (plyrOn.PlayCardBefore != SpecialPowers.CARD.NONE)
			{
				card = plyrOn.PlayCardBefore;
				plyrOn.PlayCardBefore = SpecialPowers.CARD.NONE;
			}

			// a tile at a time. Even if they can't play a tile, they can still buy stock.
			int numTiles = card == SpecialPowers.CARD.PLACE_4_TILES ? 4 : 1;
			for (int turn = 0; turn < numTiles; turn++)
			{
				// a tile at a time, ask them for their move
				PlayerPlayTile placeTile;
				if (turn < numTiles - 1)
					placeTile = plyrOn.QueryTileOnly(MainMap, plyrOn, HotelChains, Players) ?? new PlayerPlayTile();
				else
					placeTile = plyrOn.QueryTileAndPurchase(MainMap, plyrOn, HotelChains, Players) ?? new PlayerTurn();

				// debugger override
				if (plyrOn.PlayCardAfter != SpecialPowers.CARD.NONE && (placeTile is PlayerTurn))
				{
					((PlayerTurn) placeTile).Card = plyrOn.PlayCardAfter;
					plyrOn.PlayCardAfter = SpecialPowers.CARD.NONE;
				}

				// get a valid tile, if one exists
				if (placeTile.Tile == null)
					placeTile.Tile = plyrOn.Tiles.FirstOrDefault(t => MainMap.Tiles[t.X][t.Y].Type == MapTile.TYPE.UNDEVELOPED);

				// place the tile
				if (placeTile.Tile != null)
				{
					PlaceTile(plyrOn, placeTile);
					// get any newly unplayable ones out of the remaining draw tiles
					DrawTiles.RemoveAll(t => MainMap.IsTileUnplayable(t));
				}

				// if last iteration, we also do stock transactions
				PlayerTurn purchases = placeTile as PlayerTurn;
				if (purchases != null)
					StockTransactions(purchases, plyrOn);
			}

			// if < 6 tiles, draw as needed
			DrawTilesUpToLimit(plyrOn);

			// inc to next player
			IndexPlayerOn ++;
			if (IndexPlayerOn >= Players.Count)
				IndexPlayerOn = 0;

			ValidateData(true);
		}

		private SpecialPowers.CARD GetTiles(Player plyrOn)
		{
			// if any tiles are permanently unplayable, replace them (up to 6 in hand)
			plyrOn.Tiles.RemoveAll(tile => MainMap.IsTileUnplayable(tile));
			DrawTilesUpToLimit(plyrOn);

			// if all their tiles are unplayable (will create a new hotel, no available chains), add until they have a playable one
			while (plyrOn.Tiles.All(tile => ! MainMap.IsTileUndeveloped(tile)) && (DrawTiles.Count > 0))
			{
				plyrOn.Tiles.Add(DrawTiles[0]);
				DrawTiles.RemoveAt(0);
			}
			bool haveValidTile = plyrOn.Tiles.Any(tile => MainMap.IsTileUndeveloped(tile));

			// if they can do the take5/place4 - ask if they want it.
			SpecialPowers.CARD card = SpecialPowers.CARD.NONE;
			if (haveValidTile &&
				plyrOn.Powers.Any(
					pwr => pwr.Card == SpecialPowers.CARD.DRAW_5_TILES || pwr.Card == SpecialPowers.CARD.PLACE_4_TILES))
			{
				card = plyrOn.QuerySpecialPowerBeforeTurn(MainMap, plyrOn, HotelChains, Players);
				if (card != SpecialPowers.CARD.NONE)
				{
					// make sure they have this
					if (plyrOn.Powers.Any(pwr => pwr.Card == card))
						plyrOn.Powers.RemoveAll(pwr => pwr.Card == card);
					else
					{
						log.Info(string.Format("Player {0} tried to play card {1} when already played.", plyrOn.Name, card));
						card = SpecialPowers.CARD.NONE;
					}
				}
			}

			// if take 5, grab them
			if (card == SpecialPowers.CARD.DRAW_5_TILES)
				DrawTilesUpToLimit(plyrOn, plyrOn.Tiles.Count + 5);
			return card;
		}

		private void PlaceTile(Player plyrOn, PlayerPlayTile placeTile)
		{

			// is it a valid tile to play
			if (! plyrOn.Tiles.Any(tile => tile.X == placeTile.Tile.X && tile.Y == placeTile.Tile.Y))
			{
				log.Info(string.Format("Player {0} tried to play tile {1} they do not hold.", plyrOn.Name, placeTile.Tile));
				placeTile.Tile = plyrOn.Tiles.FirstOrDefault(t => MainMap.Tiles[t.X][t.Y].Type == MapTile.TYPE.UNDEVELOPED);
				if (placeTile.Tile == null)
					return;
			}
			if (!MainMap.IsTileUndeveloped(placeTile.Tile))
			{
				log.Info(string.Format("Player {0} tried to play tile {1} which cannot be played.", plyrOn.Name, placeTile.Tile));
				placeTile.Tile = plyrOn.Tiles.FirstOrDefault(t => MainMap.Tiles[t.X][t.Y].Type == MapTile.TYPE.UNDEVELOPED);
				if (placeTile.Tile == null)
					return;
			}

			// is it valid hotels
			if ((placeTile.CreatedHotel != null) && placeTile.CreatedHotel.IsActive)
			{
				log.Info(string.Format("Player {0} tried to create hotel chain {1} which is already active.", plyrOn.Name,
					placeTile.CreatedHotel));
				placeTile.CreatedHotel = null;
			}
			if ((placeTile.MergeSurvivor != null) && (!placeTile.MergeSurvivor.IsActive))
			{
				log.Info(string.Format("Player {0} tried to set merge survivor {1} which is not active.", plyrOn.Name,
					placeTile.MergeSurvivor));
				placeTile.MergeSurvivor = null;
			}

			// remove this from their tiles
			plyrOn.Tiles.RemoveAll(tile => tile == placeTile.Tile);

			// get what it does, so we can then work from that
			GameMap.TileImpact impact = MainMap.GetPlacementImpact(placeTile.Tile);

			// handle any merges
			if (impact.MergeHotels.Count > 0)
			{
				int largestNum = impact.MergeHotels.Max(hotel => hotel.NumTiles);
				HotelChain survivor;
				if (placeTile.MergeSurvivor != null && placeTile.MergeSurvivor.NumTiles == largestNum &&
				    impact.MergeHotels.Any(hotel => hotel == placeTile.MergeSurvivor))
					survivor = placeTile.MergeSurvivor;
				else
					survivor = impact.MergeHotels.First(hotel => hotel.NumTiles == largestNum);
				MainMap.PlaceTile(placeTile.Tile, survivor);

				// first pay out the majority bonuses (before stock below changes the number of shares)
				foreach (HotelChain hotelOn in impact.MergeHotels.Where(hotelOn => hotelOn != survivor))
				{
					List<StockOwner> majorityOwners = hotelOn.FirstMajorityOwners;
					foreach (StockOwner owner in majorityOwners)
						owner.Owner.Cash += hotelOn.FirstMajorityBonus / majorityOwners.Count;
					majorityOwners = hotelOn.SecondMajorityOwners;
					foreach (StockOwner owner in majorityOwners)
						owner.Owner.Cash += hotelOn.SecondMajorityBonus / majorityOwners.Count;
				}

				// ask each stock holder, chain by chain, what they want to do with their shares in the chain going away
				for (int num = 0; num < Players.Count; num++)
				{
					int index = (IndexPlayerOn + num) % Players.Count;
					Player plyrStock = Players[index];
					foreach (HotelChain hotelOn in impact.MergeHotels.Where(hotelOn => hotelOn != survivor))
					{
						if (hotelOn.Owners.All(stock => stock.Owner != plyrStock)) 
							continue;

						PlayerMerge mergeOrder = plyrStock.QueryMergeStock(MainMap, plyrStock, HotelChains, Players, survivor, hotelOn);
						int sharesHeld = plyrStock.Stock.Find(s => s.Chain.Name == hotelOn.Name).NumShares;
						if (mergeOrder == null)
							mergeOrder = new PlayerMerge(sharesHeld, 0, 0);
						else
						{
							// make sure not too many/few
							mergeOrder.Trade = Math.Min(mergeOrder.Trade, sharesHeld);
							sharesHeld -= mergeOrder.Trade;
							mergeOrder.Keep = Math.Min(mergeOrder.Keep, sharesHeld);
							sharesHeld -= mergeOrder.Keep;
							mergeOrder.Sell = sharesHeld;
						}

						int tradeGet = Math.Min(mergeOrder.Trade / 2, survivor.NumAvailableShares);
						mergeOrder.Sell += mergeOrder.Trade - 2 * tradeGet;
						hotelOn.Transfer(plyrStock, - (mergeOrder.Sell + tradeGet * 2));
						plyrStock.Cash += mergeOrder.Sell * hotelOn.StockPrice;
						survivor.Transfer(plyrStock, tradeGet);
					}
				}

				// Convert smaller hotels to large chain. set smaller chains to 0 tiles (available for new chain)
				foreach (HotelChain hotelOn in impact.MergeHotels.Where(hotelOn => hotelOn != survivor))
				{
					MainMap.Merge(survivor, hotelOn);
					survivor.NumTiles += hotelOn.NumTiles;

					// And chain now available
					hotelOn.NumTiles = 0;
				}
				// add this tile and any singles it touches
				survivor.NumTiles++;
				foreach (MapTile tileOn in impact.SingleTiles)
				{
					tileOn.Hotel = survivor;
					survivor.NumTiles++;
				}
			}

			else
				// set up the new chain
				if (impact.CreatesHotel)
				{
					HotelChain newHotel = (placeTile.CreatedHotel != null) && (! placeTile.CreatedHotel.IsActive)
									? placeTile.CreatedHotel : HotelChains.FirstOrDefault(hotel => ! hotel.IsActive);
					if (newHotel == null)
						MainMap.PlaceTile(placeTile.Tile, MapTile.TYPE.SINGLE);
					else
					{
						MainMap.PlaceTile(placeTile.Tile, newHotel);
						newHotel.NumTiles = 1;
						foreach (MapTile tileOn in impact.SingleTiles)
						{
							tileOn.Hotel = newHotel;
							newHotel.NumTiles++;
						}
						// they get their founding free share
						if (newHotel.NumAvailableShares > 0)
							newHotel.Transfer(plyrOn, 1);
					}
				}

				else
					// increase the NumTiles - may be additional
					if (impact.JoinsHotel != null)
					{
						MainMap.PlaceTile(placeTile.Tile, impact.JoinsHotel);
						impact.JoinsHotel.NumTiles++;
						foreach (MapTile tileOn in impact.SingleTiles)
						{
							tileOn.Hotel = impact.JoinsHotel;
							impact.JoinsHotel.NumTiles++;
						}
					}
					else
					{
						// it's all by itself
						MainMap.PlaceTile(placeTile.Tile, MapTile.TYPE.SINGLE);
					}

			// update map for unplayable tiles. A merge can turn 2 chains into in safe chain which changes what's safe
			MainMap.CheckForUnplayableTiles(HotelChains.Any(hotel => !hotel.IsActive));

			ValidateData(false);
		}

		private void StockTransactions(PlayerTurn purchases, Player plyrOn)
		{
			// check card, remove from deck if valid.
			if (purchases.Card != SpecialPowers.CARD.NONE)
			{
				// make sure they have this
				if (plyrOn.Powers.Any(pwr => pwr.Card == purchases.Card))
					plyrOn.Powers.RemoveAll(pwr => pwr.Card == purchases.Card);
				else
				{
					log.Info(string.Format("Player {0} tried to play card {1} when already played.", plyrOn.Name, purchases.Card));
					purchases.Card = SpecialPowers.CARD.NONE;
				}
			}

			int remainingAllowed = purchases.Card == SpecialPowers.CARD.BUY_5_STOCK ? 5 : 3;
			if (purchases.Buy != null)
				foreach (HotelStock stock in purchases.Buy)
				{
					if (!stock.Chain.IsActive)
					{
						log.Info(string.Format("Player {0} tried to purchase {1} shares of {2}. This chain is not active.",
							plyrOn.Name, stock.NumShares, stock.Chain.Name));
						continue;
					}

					int numShares = Math.Min(remainingAllowed, stock.NumShares);
					numShares = Math.Min(numShares, stock.Chain.NumAvailableShares);
					if (purchases.Card != SpecialPowers.CARD.FREE_3_STOCK)
						numShares = Math.Min(numShares, plyrOn.Cash / stock.Chain.StockPrice);
					if (numShares != stock.NumShares)
						log.Info(string.Format("Player {0} tried to purchase {1} shares of {2}. Only allowed to purchase {3} shares.",
							plyrOn.Name, stock.NumShares, stock.Chain.Name, numShares));
					if (numShares == 0)
						continue;
					plyrOn.Cash -= purchases.Card == SpecialPowers.CARD.FREE_3_STOCK ? 0 : stock.Chain.StartPrice * numShares;
					stock.Chain.Transfer(plyrOn, numShares);
					remainingAllowed -= numShares;
				}

			// trade in 2:1?
			if (purchases.Card == SpecialPowers.CARD.TRADE_2_STOCK && (purchases.Trade != null))
			{
				remainingAllowed = 3;
				foreach (var trade in purchases.Trade)
				{
					if (trade.TradeIn2 == null || trade.Get1 == null)
					{
						log.Warn(string.Format("Player {0} tried a trade without listing both hotels.", plyrOn.Name));
						continue;
					}

					// check have trade in, trade-for available
					if (!trade.TradeIn2.IsActive)
					{
						log.Warn(string.Format("Player {0} tried to trade 2:1 to trade in 2 shares of {1}. This chain is not active.",
							plyrOn.Name, trade.TradeIn2.Name));
						continue;
					}
					if (!trade.Get1.IsActive)
					{
						log.Warn(string.Format("Player {0} tried to trade 2:1 to get 1 share of {1}. This chain is not active.",
							plyrOn.Name, trade.Get1.Name));
						continue;
					}

					if ((plyrOn.Stock.All(stock => stock.Chain != trade.TradeIn2)) ||
					    (plyrOn.Stock.First(stock => stock.Chain == trade.TradeIn2).NumShares < 2))
					{
						log.Warn(string.Format(
							"Player {0} tried to trade 2 shares of {1} for 1 share of {2}. Disallowed because does not have 2 shares of {1}",
							plyrOn.Name, trade.TradeIn2.Name, trade.Get1.Name));
						continue;
					}
					if (trade.Get1.NumAvailableShares == 0)
					{
						log.Warn(string.Format(
							"Player {0} tried to trade 2 shares of {1} for 1 share of {2}. Disallowed because no shares of {2} are available",
							plyrOn.Name, trade.TradeIn2.Name, trade.Get1.Name));
						continue;
					}

					// remove the 2, grab the 1
					trade.TradeIn2.Transfer(plyrOn, -2);
					trade.Get1.Transfer(plyrOn, 1);

					if (--remainingAllowed == 0)
						break;
				}
			}

			ValidateData(false);
		}

		private void DrawTilesUpToLimit(Player plyrOn, int total = PlayerTile.NUM_TILES_DRAWN)
		{
			int num = Math.Min(DrawTiles.Count, total - plyrOn.Tiles.Count);
			if (num <= 0) 
				return;
			plyrOn.Tiles.AddRange(DrawTiles.Take(num));
			DrawTiles.RemoveRange(0, num);
		}

		#endregion

		#region testing

#if DEBUG

		public void ValidateData(bool checkUnplayableTiles)
		{
			// map good
			MainMap.ValidateMap(HotelChains);

			// tiles remaining are good
			if (checkUnplayableTiles)
				foreach (PlayerTile tile in DrawTiles)
					Assert(! MainMap.IsTileUnplayable(tile));

			foreach (Player playerOn in Players)
			{
				// When a tile becomes unplayable, that is not removed from a player's tiles until it's their turn
				// so we can't assert that no tile is - Assert(!MainMap.IsTileUnplayable(tile));

				// no deficit spending
				Assert(playerOn.Cash >= 0);

				// stock numbers match
				foreach (HotelStock stock in playerOn.Stock)
				{
					Assert(stock.NumShares > 0 && stock.NumShares <= HotelChain.NUM_SHARES);
					StockOwner owner = stock.Chain.Owners.First(o => o.Owner == playerOn);
					Assert(owner.NumShares == stock.NumShares);
				}

				// no dup chain in player's Stock
				Assert(playerOn.Stock.Count <= HotelChain.NUM_HOTEL_CHAINS);
				Dictionary<string, HotelStock> chains = new Dictionary<string, HotelStock>();
				foreach (HotelStock stock in playerOn.Stock)
				{
					if (chains.ContainsKey(stock.Chain.Name))
						Assert(false);
					else
						chains.Add(stock.Chain.Name, stock);
				}
			}

			// all chains have 25 total shares
			foreach (HotelChain hotel in HotelChains)
			{
				int numShares = hotel.NumAvailableShares + hotel.Owners.Sum(owner => owner.NumShares);
				Assert(numShares == 25);

				if (! hotel.IsActive)
				{
					// there can be owners - from when it was merged away
					Assert(hotel.FirstMajorityOwners.Count == 0);
					Assert(hotel.SecondMajorityOwners.Count == 0);
				}
				else
				{
					// no dup owners
					Assert(hotel.Owners.Count <= Player.NUM_PLAYERS);
					Dictionary<string, Player> owners = new Dictionary<string, Player>();
					foreach (Player playerOn in Players)
					{
						if (owners.ContainsKey(playerOn.Name))
							Assert(true);
						else
							owners.Add(playerOn.Name, playerOn);
					}

					List<StockOwner> firstMajorityHolders = hotel.FirstMajorityOwners;
					List<StockOwner> secondMajorityHolders = hotel.SecondMajorityOwners;
					if (firstMajorityHolders.Count == 0)
					{
						Assert(hotel.Owners.Count == 0);
						Assert(secondMajorityHolders.Count == 0);
					}
					else
					{
						int numFirstMajorityShares = firstMajorityHolders[0].NumShares;
						foreach (var owner in hotel.Owners)
						{
							Assert(owner.NumShares <= numFirstMajorityShares);
							if (owner.NumShares == numFirstMajorityShares)
								Assert(firstMajorityHolders.Any(o => o.Owner == owner.Owner));
							else
								Assert(firstMajorityHolders.All(o => o.Owner != owner.Owner));
						}

						if (firstMajorityHolders.Count == 1)
						{
							int numSecondMajorityShares = secondMajorityHolders[0].NumShares;
							foreach (var owner in hotel.Owners)
							{
								Assert(owner.NumShares == numFirstMajorityShares || owner.NumShares <= numSecondMajorityShares);
								if (owner.NumShares == numSecondMajorityShares)
									Assert(secondMajorityHolders.Any(o => o.Owner == owner.Owner));
								else
									Assert(secondMajorityHolders.All(o => o.Owner != owner.Owner));
							}

						}
						else
						{
							Assert(firstMajorityHolders.Count == secondMajorityHolders.Count);
							foreach (StockOwner owner in firstMajorityHolders)
							{
								Assert(secondMajorityHolders.Any(mh => mh.Owner == owner.Owner));
							}
						}
					}

				}
			}
		}

		public static void Assert(bool shouldBeTrue)
		{
			Trap.trap(!shouldBeTrue);
		}

#else
		private void ValidateData()
		{
		}

		private void ValidateData(bool checkUnplayableTiles) 
		{
		}
#endif

		#endregion
	}
}
