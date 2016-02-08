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
using Server.Map;
using Server.Units;

namespace Server.AI
{
	public interface IPlayerAI : IDisposable
	{
		/// <summary>
		/// The GUID for this player's connection. This will change if the connection has to be re-established. It is
		/// null for the local AIs. This is not the player GUID.
		/// </summary>
		string TcpGuid { get; set; }

		/// <summary>
		/// Called when the game starts, providing all info.
		/// </summary>
		/// <param name="map">The game map.</param>
		/// <param name="me">The player being setup.</param>
		/// <param name="hotelChains">All hotel chains.</param>
		/// <param name="players">All the players.</param>
		void Setup(GameMap map, Player me, List<HotelChain> hotelChains, List<Player> players);

		/// <summary>
		/// Asks if you want to play the CARD.DRAW_5_TILES or CARD.PLACE_4_TILES special power. This call will not be made 
		/// if you have already played these cards.
		/// </summary>
		/// <param name="map">The game map.</param>
		/// <param name="you">The player being setup.</param>
		/// <param name="hotelChains">All hotel chains.</param>
		/// <param name="players">All the players.</param>
		/// <returns>CARD.NONE, CARD.PLACE_4_TILES, or CARD.DRAW_5_TILES.</returns>
		SpecialPowers.CARD QuerySpecialPowerBeforeTurn(GameMap map, Player you, List<HotelChain> hotelChains, List<Player> players);

		/// <summary>
		/// Return what tile to play when using the CARD.PLACE_4_TILES. This will be called for the first 3 tiles and is for
		/// placement only. Any merges due to this will be resolved before the next card is played. For the 4th tile, QueryTileAndPurchase
		/// will be called.
		/// </summary>
		/// <param name="map">The game map.</param>
		/// <param name="you">The player being setup.</param>
		/// <param name="hotelChains">All hotel chains.</param>
		/// <param name="players">All the players.</param>
		/// <returns>The tile(s) to play and the stock to purchase (and trade if CARD.TRADE_2_STOCK is played).</returns>
		PlayerPlayTile QueryTileOnly(GameMap map, Player you, List<HotelChain> hotelChains, List<Player> players);

		/// <summary>
		/// Return what tile(s) to play and what stock(s) to purchase. At this point merges have not yet been processed.
		/// </summary>
		/// <param name="map">The game map.</param>
		/// <param name="you">The player being setup.</param>
		/// <param name="hotelChains">All hotel chains.</param>
		/// <param name="players">All the players.</param>
		/// <returns>The tile(s) to play and the stock to purchase (and trade if CARD.TRADE_2_STOCK is played).</returns>
		PlayerTurn QueryTileAndPurchase(GameMap map, Player you, List<HotelChain> hotelChains, List<Player> players);

		/// <summary>
		/// Ask the AI what they want to do with their merged stock. If a merge is for 3+ chains, this will get called once per
		/// removed chain.
		/// </summary>
		/// <param name="map">The game map.</param>
		/// <param name="you">The player being setup.</param>
		/// <param name="hotelChains">All hotel chains.</param>
		/// <param name="players">All the players.</param>
		/// <param name="survivor">The hotel that survived the merge.</param>
		/// <param name="defunct">The hotel that is now defunct.</param>
		/// <returns>What you want to do with the stock.</returns>
		PlayerMerge QueryMergeStock(GameMap map, Player you, List<HotelChain> hotelChains, List<Player> players, 
			HotelChain survivor, HotelChain defunct);
	}
}