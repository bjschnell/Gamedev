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
	/// Used to track who owns what shares. This is held by a HotelChain object.
	/// </summary>
	public class StockOwner
	{
		/// <summary>
		/// The owner of these shares. null if they are available for purchased (unowned).
		/// </summary>
		public Player Owner { get; private set; }

		/// <summary>
		/// The number of shares.
		/// </summary>
		public int NumShares { get; set; }

		public StockOwner(Player owner, int numShares)
		{
			Owner = owner;
			NumShares = numShares;
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}", Owner.Name, NumShares);
		}
	}
}
