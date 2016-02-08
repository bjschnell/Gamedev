/*
 * ----------------------------------------------------------------------------
 * "THE BEER-WARE LICENSE" 
 * As long as you retain this notice you can do whatever you want with this 
 * stuff. If you meet an employee from Windward meet some day, and you think
 * this stuff is worth it, you can buy them a beer in return. Windward Studios
 * ----------------------------------------------------------------------------
 */

namespace Server.Units
{
	/// <summary>
	/// A share of stock in a hotel chain.
	/// </summary>
	public class HotelStock
	{

		public HotelChain Chain { get; private set; }

		public int NumShares { get; set; }

		public HotelStock(HotelChain chain, int numShares)
		{
			Chain = chain;
			NumShares = numShares;
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}", Chain.Name, NumShares);
		}
	}
}
