/*
 * ----------------------------------------------------------------------------
 * "THE BEER-WARE LICENSE" 
 * As long as you retain this notice you can do whatever you want with this 
 * stuff. If you meet an employee from Windward meet some day, and you think
 * this stuff is worth it, you can buy them a beer in return. Windward Studios
 * ----------------------------------------------------------------------------
 */

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Server.Sprites;
using Server.Utilities;

namespace Server.Units
{
	/// <summary>
	/// A hotel chain. Each chain has 0 or 1 instance on the board.
	/// </summary>
	public class HotelChain
	{
		/// <summary>
		/// The number of hotel chains in the game. This should be 1 - 2 more than the number of players. If you set
		/// this above 12, you need to create more chains in the code below.
		/// </summary>
		public const int NUM_HOTEL_CHAINS = 7;

		/// <summary>
		/// The total number of shares issued for each chain.
		/// </summary>
		public const int NUM_SHARES = 25;

		/// <summary>
		///     The name of the hotel chain.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///     The company logo.
		/// </summary>
		public Bitmap Logo { get; private set; }

		/// <summary>
		/// The owners of shares in this chain. Each player (Owner) will occur 0 or 1 time in this list. Shares not in 
		/// this list are available.
		/// </summary>
		public List<StockOwner> Owners { get; private set; }

		/// <summary>
		/// The bitmap for this hotel's tile.
		/// </summary>
		public Image SpriteBitmap { get; private set; }

		/// <summary>
		/// The start price for this chain. Should be 200, 300, or 400.
		/// </summary>
		public int StartPrice { get; private set; }

		/// <summary>
		/// The number of tiles this hotel has.
		/// </summary>
		public int NumTiles { get; set; }

		/// <summary>
		/// True if this chain is active on the board.
		/// </summary>
		public bool IsActive
		{
			get
			{
				return NumTiles >= 2;
			}
		}

		/// <summary>
		/// Returns true if this chain is safe and cannot be merged out of existence.
		/// </summary>
		public bool IsSafe
		{
			get { return NumTiles >= 11; }
		}

		/// <summary>
		/// All stockholders who get the first majority bonus.
		/// </summary>
		public List<StockOwner> FirstMajorityOwners
		{
			get
			{
				if ((! IsActive) || (Owners.Count == 0))
					return new List<StockOwner>();

				int numShares = Owners.Max(shares => shares.NumShares);
				return Owners.FindAll(shares => shares.NumShares == numShares);
			}
		}

		/// <summary>
		/// All stockholders who get the second majority bonus. This is the same list as the FirstMajorityOwners if there is more
		/// than 1 FirstMajorityOwner.
		/// </summary>
		public List<StockOwner> SecondMajorityOwners
		{
			get
			{
				// if just one stockholder, they get both
				if ((!IsActive) || (Owners.Count <= 1))
					return FirstMajorityOwners;

				// get the firstMajority - if > 1 person, this is it
				int numShares = Owners.Max(shares => shares.NumShares);
				var majorityOwners = Owners.FindAll(shares => shares.NumShares == numShares);
				if (majorityOwners.Count > 1)
					return majorityOwners;

				// get those with the 2nd highest count
				numShares = Owners.Max(shares => shares.NumShares == numShares ? 0 : shares.NumShares);
				return Owners.FindAll(shares => shares.NumShares == numShares);
			}
		}

		private int TilesToPriceMult
		{
			get
			{
				if (NumTiles <= 6)
					return NumTiles;
				if (NumTiles <= 10)
					return 6;
				if (NumTiles <= 20)
					return 7;
				if (NumTiles <= 30)
					return 8;
				if (NumTiles <= 40)
					return 9;
				return 10;
			}
		}

		/// <summary>
		/// The price for a share of this stock.
		/// </summary>
		public int StockPrice
		{
			get
			{
				if (NumTiles == 0)
					return StartPrice;
				return (TilesToPriceMult - 2) * 100 + StartPrice;
			}
		}

		/// <summary>
		/// The price for the first majority's bonus.
		/// </summary>
		public int FirstMajorityBonus
		{
			get
			{
				if (NumTiles == 0)
					return 0;
				return StockPrice * 10;
			}
		}

		/// <summary>
		/// The price for the second majority's bonus.
		/// </summary>
		public int SecondMajorityBonus
		{
			get
			{
				if (NumTiles == 0)
					return 0;
				return StockPrice * 5;
			}
		}

		public int NumAvailableShares
		{
			get
			{
				return NUM_SHARES - Owners.Sum(stock => stock.NumShares);
			}
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="name">The name of the company.</param>
		/// <param name="logo">The company logo.</param>
		public HotelChain(string name, Bitmap logo, Bitmap mapSprite)
		{
			Name = name;
			Logo = logo;
			SpriteBitmap = new Bitmap(mapSprite.Width, mapSprite.Height);
			Graphics g = Graphics.FromImage(SpriteBitmap);
			g.DrawImage(mapSprite, 0, 0);
			g.DrawImage(logo, mapSprite.Width - 18, mapSprite.Height - 18, 16, 16);
			Owners = new List<StockOwner> ();
		}

		public void Reset()
		{
			NumTiles = 0;
			Owners.Clear();
			FirstMajorityOwners.Clear();
			SecondMajorityOwners.Clear();
		}

		public void Transfer(Player plyrOn, int numShares)
		{
			if (numShares == 0)
				return;
			StockOwner owner = Owners.FirstOrDefault(stock => stock.Owner == plyrOn);
			if (owner == null)
			{
				owner = new StockOwner(plyrOn, numShares);
				Owners.Add(owner);
			}
			else
			{
				owner.NumShares += numShares;
				if (owner.NumShares <= 0)
					Owners.RemoveAll(stock => stock.Owner == plyrOn);
			}
			HotelStock stockholding = plyrOn.Stock.FirstOrDefault(stock => stock.Chain == this);
			if (stockholding != null)
			{
				stockholding.NumShares += numShares;
				if (stockholding.NumShares <= 0)
					plyrOn.Stock.RemoveAll(stock => stock.Chain == this);
			}
			else
			{
				plyrOn.Stock.Add(new HotelStock(this, numShares));
			}
			Trap.trap(NumAvailableShares < 0);
		}

		/// <summary>
		///     Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
		/// </summary>
		/// <returns>
		///     A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
		/// </returns>
		public override string ToString()
		{
			return Name;
		}

		/// <summary>
		/// All of the hotel chains for the game. Each call creates a new list so the returned list can be changed.
		/// </summary>
		public static List<HotelChain> CreateAllHotelChains()
		{
			List<HotelChain> allCompanies = new List<HotelChain>
			{
				new HotelChain("Windward", CompanySprites.windward, MapSprites.Building_01),
				new HotelChain("JetBrains", CompanySprites.jetbrains, MapSprites.Building_02),
				new HotelChain("Hewlett-Packard", CompanySprites.hp, MapSprites.Building_03),
				new HotelChain("salesforce.com", CompanySprites.sfdc, MapSprites.Building_04),
				new HotelChain("Microsoft", CompanySprites.microsoft, MapSprites.Building_05),
				new HotelChain("Amazon", CompanySprites.amazon, MapSprites.Building_06),
				new HotelChain("Google", CompanySprites.google, MapSprites.Building_07),
				new HotelChain("Facebook", CompanySprites.facebook, MapSprites.Building_01),
				new HotelChain("Apple", CompanySprites.apple, MapSprites.Building_02),
				new HotelChain("LinkedIn", CompanySprites.linkedin, MapSprites.Building_03),
				new HotelChain("Twitter", CompanySprites.twitter, MapSprites.Building_04),
				new HotelChain("Oracle", CompanySprites.oracle, MapSprites.Building_05)
			};

			allCompanies.RemoveRange(NUM_HOTEL_CHAINS, allCompanies.Count-NUM_HOTEL_CHAINS);

			// set the start price
			const int numHighLow = NUM_HOTEL_CHAINS / 3;
			for (int index = 0; index < numHighLow; index++)
				allCompanies[index].StartPrice = 400;
			for (int index = numHighLow; index < allCompanies.Count - numHighLow; index++)
				allCompanies[index].StartPrice = 300;
			for (int index = allCompanies.Count - numHighLow; index < allCompanies.Count; index++)
				allCompanies[index].StartPrice = 200;

			allCompanies.Sort((c1, c2) => System.String.Compare(c1.Name, c2.Name));
			return allCompanies;
		}
	}
}