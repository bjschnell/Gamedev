/*
 * ----------------------------------------------------------------------------
 * "THE BEER-WARE LICENSE" 
 * As long as you retain this notice you can do whatever you want with this 
 * stuff. If you meet an employee from Windward meet some day, and you think
 * this stuff is worth it, you can buy them a beer in return. Windward Studios
 * ----------------------------------------------------------------------------
 */

using System.Collections.Generic;
using Server.Map;

namespace Server.Units
{
	/// <summary>
	/// A tile held by a player or waiting to be drawn by a player. There is one for each tile location on the board.
	/// </summary>
	public class PlayerTile
	{
		public const int NUM_TILES_DRAWN = 6;

		/// <summary>
		/// The X location on the board. (In the board game this has values 1..12 but this property is 0 based.)
		/// </summary>
		public int X { get; private set; }

		/// <summary>
		/// The Y location on the board. (In the board game this has values A..I.)
		/// </summary>
		public int Y { get; private set; }

		public PlayerTile(int x, int y)
		{
			X = x;
			Y = y;
		}

		/// <summary>
		/// All tiles for the board.
		/// </summary>
		/// <returns></returns>
		public static List<PlayerTile> AllTiles()
		{
			List<PlayerTile> tiles = new List<PlayerTile>();
			for (int x = 0; x < GameMap.NUM_X_TILES; x++)
				for (int y = 0; y < GameMap.NUM_Y_TILES; y++)
					tiles.Add(new PlayerTile(x, y));
			return tiles;
		}

		public override string ToString()
		{
			return string.Format("[{0},{1}]", X, Y);
		}
	}
}
