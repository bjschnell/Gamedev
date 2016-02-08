/*
 * ----------------------------------------------------------------------------
 * "THE BEER-WARE LICENSE" 
 * As long as you retain this notice you can do whatever you want with this 
 * stuff. If you meet an employee from Windward meet some day, and you think
 * this stuff is worth it, you can buy them a beer in return. Windward Studios
 * ----------------------------------------------------------------------------
 */

using Server.Units;

namespace Server.AI
{
	/// <summary>
	/// A tile play by a player. Base class for PlayerTurn and used for CARD.PLACE_4_TILES for the placement of the first 3 tiles.
	/// </summary>
	public class PlayerPlayTile
	{
		/// <summary>
		/// The tile to play.
		/// </summary>
		public PlayerTile Tile { get; set; }

		/// <summary>
		/// If the tile played creates a chain, this is the chain.
		/// </summary>
		public HotelChain CreatedHotel { get; set; }

		/// <summary>
		/// If the tile played merges two equally sized chains, this is the surviving chain.
		/// </summary>
		public HotelChain MergeSurvivor { get; set; }

		public PlayerPlayTile()
		{
			Tile = null;
			CreatedHotel = null;
			MergeSurvivor = null;
		}
	}
}
