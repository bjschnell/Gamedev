/*
 * ----------------------------------------------------------------------------
 * "THE BEER-WARE LICENSE" 
 * As long as you retain this notice you can do whatever you want with this 
 * stuff. If you meet an employee from Windward meet some day, and you think
 * this stuff is worth it, you can buy them a beer in return. Windward Studios
 * ----------------------------------------------------------------------------
 */

using System.Collections.Generic;
using Server.Units;

namespace Server.AI
{
	/// <summary>
	/// A turn by a player.
	/// </summary>
	public class PlayerTurn : PlayerPlayTile
	{
		/// <summary>
		/// Which card to play this turn. This is just the stock purchase cards.
		/// </summary>
		public SpecialPowers.CARD Card { get; set; }

		/// <summary>
		/// Stocks to purchase (may be free).
		/// </summary>
		public List<HotelStock> Buy { get; private set; }

		/// <summary>
		/// Stocks sold or traded in 2:1. You can have up to 3 of these.
		/// </summary>
		public IList<TradeStock> Trade { get; private set; }

		public class TradeStock
		{
			public HotelChain TradeIn2 { get; private set; }
			public HotelChain Get1 { get; private set; }

			public TradeStock(HotelChain tradeIn2, HotelChain get1)
			{
				TradeIn2 = tradeIn2;
				Get1 = get1;
			}
		}

		public PlayerTurn()
		{
			Card = SpecialPowers.CARD.NONE;
			Buy = new List<HotelStock>();
			Trade = new List<TradeStock>();
		}
	}
}
