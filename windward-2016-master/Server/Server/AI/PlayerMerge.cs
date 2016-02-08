/*
 * ----------------------------------------------------------------------------
 * "THE BEER-WARE LICENSE" 
 * As long as you retain this notice you can do whatever you want with this 
 * stuff. If you meet an employee from Windward meet some day, and you think
 * this stuff is worth it, you can buy them a beer in return. Windward Studios
 * ----------------------------------------------------------------------------
 */

namespace Server.AI
{
	/// <summary>
	/// Players stock transactions in the result of a merge.
	/// </summary>
	public class PlayerMerge
	{

		/// <summary>
		/// How many shares to sell.
		/// </summary>
		public int Sell { get; set; }

		/// <summary>
		/// How many shares to keep.
		/// </summary>
		public int Keep { get; set; }

		/// <summary>
		/// How many shares to trade in 2:1. For any that can't be traded, they will be sold.
		/// </summary>
		public int Trade { get; set; }

		public PlayerMerge(int sell, int keep, int trade)
		{
			Sell = sell;
			Keep = keep;
			Trade = trade;
		}

		public override string ToString()
		{
			return string.Format("sell:{0}, keep:{1}, trade:{2}", Sell, Keep, Trade);
		}
	}
}
