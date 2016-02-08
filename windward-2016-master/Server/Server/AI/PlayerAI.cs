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
using System.Linq;
using Server.Map;
using Server.Units;

namespace Server.AI
{
	/// <summary>
	///     A very simplistic implementation of the AI. This AI is used for additional players if we don't have NUM_PLAYERS remote players.
	/// </summary>
	public class PlayerAI : IPlayerAI
	{

		private static readonly Random rand = new Random();

		public void Dispose()
		{
			// nada
		}

		/// <summary>
		/// The GUID for this player's connection. This will change if the connection has to be re-established. It is
		/// null for the local AIs. This is not the player GUID.
		/// </summary>
		public string TcpGuid
		{
			get { return null; }
			set
			{
				/* nada */
			}
		}

		/// <summary>
		/// Called when the game starts, providing all info.
		/// </summary>
		/// <param name="map">The game map.</param>
		/// <param name="me">The player being setup.</param>
		/// <param name="hotelChains">All hotels on the board.</param>
		/// <param name="players">All the players.</param>
		public void Setup(GameMap map, Player me, List<HotelChain> hotelChains, List<Player> players)
		{
			// do nothing
		}

		/// <summary>
		/// Asks if you want to play the CARD.DRAW_5_TILES or CARD.PLACE_4_TILES special power. This call will not be made 
		/// if you have already played these cards.
		/// </summary>
		/// <param name="map">The game map.</param>
		/// <param name="me">The player being setup.</param>
		/// <param name="hotelChains">All hotel chains.</param>
		/// <param name="players">All the players.</param>
		/// <returns>CARD.NONE, CARD.PLACE_4_TILES, or CARD.DRAW_5_TILES.</returns>
		public SpecialPowers.CARD QuerySpecialPowerBeforeTurn(GameMap map, Player me, List<HotelChain> hotelChains, List<Player> players)
		{
			if (rand.Next(30) == 1)
			{
				return SpecialPowers.CARD.DRAW_5_TILES;
			}
			if (rand.Next(30) == 1)
			{
				return SpecialPowers.CARD.PLACE_4_TILES;
			}
			return SpecialPowers.CARD.NONE;
		}

		/// <summary>
		/// Return what tile to play when using the CARD.PLACE_4_TILES. This will be called for the first 3 tiles and is for
		/// placement only. Any merges due to this will be resolved before the next card is played. For the 4th tile, QueryTileAndPurchase
		/// will be called.
		/// </summary>
		/// <param name="map">The game map.</param>
		/// <param name="me">The player being setup.</param>
		/// <param name="hotelChains">All hotel chains.</param>
		/// <param name="players">All the players.</param>
		/// <returns>The tile(s) to play and the stock to purchase (and trade if CARD.TRADE_2_STOCK is played).</returns>
		public PlayerPlayTile QueryTileOnly(GameMap map, Player me, List<HotelChain> hotelChains, List<Player> players)
		{
			PlayerPlayTile playTile = new PlayerPlayTile();
			playTile.Tile = me.Tiles.Count == 0 ? null : me.Tiles[rand.Next(me.Tiles.Count)];
			playTile.CreatedHotel = playTile.MergeSurvivor = hotelChains.FirstOrDefault(hotel => ! hotel.IsActive);
			return playTile;
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
			PlayerTurn turn = new PlayerTurn();
			turn.Tile = me.Tiles.Count == 0 ? null : me.Tiles[rand.Next(me.Tiles.Count)];
			turn.CreatedHotel = turn.MergeSurvivor = hotelChains.FirstOrDefault(hotel => !hotel.IsActive);

			turn.Buy.Add(new HotelStock(hotelChains[rand.Next(hotelChains.Count)], 1 + rand.Next(3)));
			turn.Buy.Add(new HotelStock(hotelChains[rand.Next(hotelChains.Count)], 1 + rand.Next(3)));

			if (rand.Next(20) != 1)
			{
				return turn;
			}

			switch (rand.Next(3))
			{
				case 0:
					turn.Card = SpecialPowers.CARD.BUY_5_STOCK;
					turn.Buy.Add(new HotelStock(hotelChains[rand.Next(hotelChains.Count)], 3));
					return turn;
				case 1:
					turn.Card = SpecialPowers.CARD.FREE_3_STOCK;
					return turn;
				default:
					if (me.Stock.Count > 0)
					{
						turn.Card = SpecialPowers.CARD.TRADE_2_STOCK;
						turn.Trade.Add(new PlayerTurn.TradeStock(me.Stock[rand.Next(me.Stock.Count)].Chain, hotelChains[rand.Next(hotelChains.Count)]));
					}
					return turn;
			}
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
		public PlayerMerge QueryMergeStock(GameMap map, Player me, List<HotelChain> hotelChains, List<Player> players, HotelChain survivor,
			HotelChain defunct)
		{
			HotelStock myStock = me.Stock.First(stock => stock.Chain == defunct);
			return new PlayerMerge(myStock.NumShares / 3, myStock.NumShares / 3, (myStock.NumShares + 2) / 3);
		}
	}
}