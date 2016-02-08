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
using Server.Sprites;

namespace Server.Units
{
	/// <summary>
	/// For an optional part of the game. Every player is dealt 5 cards, one of each special power at the start of the
	/// game. They can play one on any turn.
	/// </summary>
	public class SpecialPowers
	{

		/// <summary>
		/// The specific power of this card.
		/// </summary>
		public enum CARD
		{
			/// <summary>
			/// Instead of purchasing stock, you can draw up to 3 shares of stock for free.
			/// </summary>
			FREE_3_STOCK = 1,
			/// <summary>
			/// You can purchase up to 5 shares of stock this turn instead of the normal 3.
			/// </summary>
			BUY_5_STOCK = 2,
			/// <summary>
			/// You can do up to 3 2:1 trades where you trade in 2 shares of stock and in return receive 1 share of stock.
			/// All shares involved must be for active chains. This is in addition to the 3 shares you can purchase.
			/// </summary>
			TRADE_2_STOCK = 3,
			/// <summary>
			/// Draw 5 tiles at the beginning of this turn. You do not draw any additional tiles until you've been reduced
			/// back down to 5 times.
			/// </summary>
			DRAW_5_TILES = 4,
			/// <summary>
			/// Place 4 tiles on the board this turn. Any merges due to a tile placement are resolved before the next tile is
			/// placed. 4 new tiles are drawn after all 4 are played.
			/// </summary>
			PLACE_4_TILES = 5,
			/// <summary>
			/// This is not a special power. It is used to pass that you are playing no card.
			/// </summary>
			NONE = 0
		}

		/// <summary>
		/// The name of the power-up.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// The card.
		/// </summary>
		public CARD Card { get; private set; }

		/// <summary>
		/// The card logo.
		/// </summary>
		public Bitmap Logo { get; private set; }

		private SpecialPowers(string name, CARD card, Bitmap logo)
		{
			Name = name;
			Card = card;
			Logo = logo;
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		public override string ToString()
		{
			return Name;
		}

		/// <summary>
		/// A list of cards to assign to a player. One of each special power.
		/// </summary>
		/// <returns>One of each special power.</returns>
		public static List<SpecialPowers> PlayerDeck()
		{
			List<SpecialPowers> powerups = new List<SpecialPowers>
			{
				new SpecialPowers("Purchase 3 shares of stock for free", CARD.FREE_3_STOCK, CardSprites.money),
				new SpecialPowers("Purchase 5 shares of stock", CARD.BUY_5_STOCK, CardSprites.cashier),
				new SpecialPowers("Trade in 2 shares of stock for 1", CARD.TRADE_2_STOCK, CardSprites.money2),
				new SpecialPowers("Draw 5 tiles", CARD.DRAW_5_TILES, CardSprites.houses),
				new SpecialPowers("Place 4 tiles", CARD.PLACE_4_TILES, CardSprites.office_building)
			};
			return powerups;
		}
	}
}
