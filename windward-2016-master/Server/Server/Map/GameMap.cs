/*
 * ----------------------------------------------------------------------------
 * "THE BEER-WARE LICENSE" 
 * As long as you retain this notice you can do whatever you want with this 
 * stuff. If you meet an employee from Windward meet some day, and you think
 * this stuff is worth it, you can buy them a beer in return. Windward Studios
 * ----------------------------------------------------------------------------
 */

using System.Collections.Generic;
using System.Linq;
using Server.GameEngine;
using Server.Units;

namespace Server.Map
{
	public class GameMap
	{
		/// <summary>
		/// How many tiles across on the board.
		/// </summary>
		public const int NUM_X_TILES = 12;

		/// <summary>
		/// How many tiles down on the board.
		/// </summary>
		public const int NUM_Y_TILES = 9;

		/// <summary>
		///     The map squares. This is in the format [x][y].
		/// </summary>
		public MapTile[][] Tiles { get; protected set; }

		/// <summary>
		/// The most recently played tile
		/// </summary>
		private static MapTile lastPlayedTile = null;

		public int Height {
			get { return NUM_Y_TILES; }
		}

		public int Width
		{
			get { return NUM_X_TILES; }
		}

		private static readonly int[,] offsets = { { -1, 0 }, { 0, -1 }, { 1, 0 }, { 0, 1 } };

		/// <summary>
		/// Create an empty map
		/// </summary>
		public GameMap()
		{
			Tiles = new MapTile[NUM_X_TILES][];
			for (int x = 0; x < NUM_X_TILES; x++)
			{
				Tiles[x] = new MapTile[NUM_Y_TILES];
				for (int y = 0; y < NUM_Y_TILES; y++)
					Tiles[x][y] = new MapTile();
			}
		}

		public bool IsTileUndeveloped(PlayerTile tile)
		{
			return Tiles[tile.X][tile.Y].Type == MapTile.TYPE.UNDEVELOPED;
		}

		/// <summary>
		/// true if this tile cannot be played. Either it is already played, it would merge 
		/// </summary>
		/// <param name="tile"></param>
		/// <returns></returns>
		public bool IsTileUnplayable(PlayerTile tile)
		{
			return Tiles[tile.X][tile.Y].Type != MapTile.TYPE.UNDEVELOPED && Tiles[tile.X][tile.Y].Type != MapTile.TYPE.UNPLAYABLE_NO_AVAIL_CHAINS;
		}

		/// <summary>
		/// Walk all tiles looking for unplayable ones. Also look for tiles that would create a hotel, but not allowed if
		/// all chains are in use.
		/// </summary>
		/// <param name="availableChains">true if there are chains available to create new hotel chains.</param>
		public void CheckForUnplayableTiles(bool availableChains)
		{
			for (int x = 0; x < NUM_X_TILES; x++)
			{
				for (int y = 0; y < NUM_Y_TILES; y++)
				{
					MapTile tile = Tiles[x][y];
					// set unplayable types to undeveloped as we're re-checking
					if (tile.Type == MapTile.TYPE.UNPLAYABLE_NO_AVAIL_CHAINS)
					{
						tile.Type = MapTile.TYPE.UNDEVELOPED;
					}
					if (tile.Type != MapTile.TYPE.UNDEVELOPED)
						continue;

					HotelChain safeHotel = null;
					bool touchHotel = false;
					bool touchSingle = false;
					for (int index = 0; index <= offsets.GetUpperBound(0); index++)
					{
						int xTest = x + offsets[index, 0];
						int yTest = y + offsets[index, 1];
						if ((xTest < 0) || (NUM_X_TILES <= xTest) || (yTest < 0) || (NUM_Y_TILES <= yTest))
						{
							continue;
						}
						var check = Tiles[xTest][yTest];
						if (check.Type != MapTile.TYPE.HOTEL)
						{
							if (check.Type == MapTile.TYPE.SINGLE)
							{
								touchSingle = true;
							}
							continue;
						}
						touchHotel = true;
						if (! check.Hotel.IsSafe)
						{
							continue;
						}
						if (safeHotel == null || safeHotel == check.Hotel)
						{
							safeHotel = check.Hotel;
							continue;
						}
						// ok, 2 different safe hotels touch - now unplayable
						tile.Type = MapTile.TYPE.UNPLAYABLE_MERGE_SAFE;
					}

					// handle the can't create a new hotel chain right now case
					if ((! availableChains) && touchSingle && (! touchHotel))
					{
						tile.Type = MapTile.TYPE.UNPLAYABLE_NO_AVAIL_CHAINS;
					}
				}
			}
		}

		/// <summary>
		/// Returns the impact of playing this tile. Calling this assumes the tile can be played (is not already played and
		/// does not merge two safe hotels).
		/// </summary>
		/// <param name="tile">The tile to play</param>
		/// <returns>The result of playing this tile.</returns>
		public TileImpact GetPlacementImpact(PlayerTile tile)
		{
			TileImpact rtn = new TileImpact();
			for (int index = 0; index <= offsets.GetUpperBound(0); index++)
			{
				int x = tile.X + offsets[index, 0];
				int y = tile.Y + offsets[index, 1];
				if ((x < 0) || (NUM_X_TILES <= x) || (y < 0) || (NUM_Y_TILES <= y))
				{
					continue;
				}
				MapTile neighbor = Tiles[x][y];
				switch (neighbor.Type)
				{
					case MapTile.TYPE.SINGLE:
						rtn.CreatesHotel = true;
						rtn.SingleTiles.Add(neighbor);
						break;
					case MapTile.TYPE.HOTEL:
						if (! rtn.MergeHotels.Contains(neighbor.Hotel))
						{
							rtn.MergeHotels.Add(neighbor.Hotel);
						}
						break;
				}
			}

			// joins single hotel chain
			if (rtn.MergeHotels.Count == 1)
			{
				rtn.CreatesHotel = false;
				rtn.JoinsHotel = rtn.MergeHotels[0];
				rtn.MergeHotels.Clear();
			}
			else if (rtn.MergeHotels.Count > 1)
			{
				rtn.CreatesHotel = false;
			}
			return rtn;
		}

		/// <summary>
		/// The impact of playing a tile.
		/// </summary>
		public class TileImpact
		{
			/// <summary>
			/// The hotel chains this will merge. There will be 2+ hotels in this list. Will include multiple safe hotels
			/// if this is an illegal placement that will merge safe hotels.
			/// </summary>
			public List<HotelChain> MergeHotels { get; private set; }

			/// <summary>
			/// Single tiles touching this tile.
			/// </summary>
			public List<MapTile> SingleTiles { get; private set; }

			/// <summary>
			/// true if this creates a new hotel chain. true even if there are no available chains to create.
			/// </summary>
			public bool CreatesHotel { get; set; }

			/// <summary>
			/// If it will join a chain, thsi is the chain. Otherwise null.
			/// </summary>
			public HotelChain JoinsHotel { get; set; }

			public TileImpact()
			{
				MergeHotels = new List<HotelChain>();
				SingleTiles = new List<MapTile>();
				JoinsHotel = null;
				CreatesHotel = false;
			}
		}

		/// <summary>
		/// We place this time and if it creates a chain, we set all the new hotel tiles to this chain. If it is an addition
		/// to an existing chain (not a merge) it is set to that chain as are any files it brings in to that chain.
		/// This does not set tiles set due to a merge as it does not know who wins.
		/// </summary>
		/// <param name="tile"></param>
		/// <param name="createdHotel"></param>
		public void PlaceTile(PlayerTile tile, HotelChain createdHotel)
		{
			if (lastPlayedTile != null)
				lastPlayedTile.LastPlayed = false;
			MapTile mapTile = Tiles[tile.X][tile.Y];
			mapTile.Hotel = createdHotel;
			mapTile.Type = MapTile.TYPE.HOTEL;
			mapTile.LastPlayed = true;
			lastPlayedTile = mapTile;
		}

		public void PlaceTile(PlayerTile tile, MapTile.TYPE type)
		{
			if (lastPlayedTile != null)
				lastPlayedTile.LastPlayed = false;
			MapTile mapTile = Tiles[tile.X][tile.Y];
			mapTile.Hotel = null;
			mapTile.Type = type;
			mapTile.LastPlayed = true;
			lastPlayedTile = mapTile;
		}

		public void Merge(HotelChain survivor, HotelChain takenOver)
		{
			// set any tiles set to the takenOver one to survivor
			for (int x = 0; x < NUM_X_TILES; x++)
			{
				for (int y = 0; y < NUM_Y_TILES; y++)
				{
					MapTile tile = Tiles[x][y];
					if (tile.Hotel == takenOver)
						tile.Hotel = survivor;
				}
			}
		}

#if DEBUG

		public void ValidateMap(List<HotelChain> chains)
		{

			bool availableChains = chains.Any(hotel => !hotel.IsActive);
			Dictionary<HotelChain, int> mapNumTiles = chains.ToDictionary(hotel => hotel, hotel => 0);

			for (int x = 0; x < NUM_X_TILES; x++)
			{
				for (int y = 0; y < NUM_Y_TILES; y++)
				{
					MapTile tile = Tiles[x][y];
					// make sure type & Hotel match
					Engine.Assert(((tile.Type != MapTile.TYPE.HOTEL) && (tile.Hotel == null)) ||
					              ((tile.Type == MapTile.TYPE.HOTEL) && (tile.Hotel != null)));

					// if available chains, can't be UNPLAYABLE_NO_AVAIL_CHAINS
					if (availableChains)
						Engine.Assert(tile.Type != MapTile.TYPE.UNPLAYABLE_NO_AVAIL_CHAINS);

					// add to count of num tiles for each chain
					if (tile.Hotel != null)
						mapNumTiles[tile.Hotel] ++;

					HotelChain neighbor = null;
					for (int index = 0; index <= offsets.GetUpperBound(0); index++)
					{
						int xTest = x + offsets[index, 0];
						int yTest = y + offsets[index, 1];
						if ((xTest < 0) || (NUM_X_TILES <= xTest) || (yTest < 0) || (NUM_Y_TILES <= yTest))
						{
							continue;
						}
						MapTile check = Tiles[xTest][yTest];

						// if tile is a hotel, check is the same hotel
						if (tile.Type == MapTile.TYPE.HOTEL && check.Type == MapTile.TYPE.HOTEL)
							Engine.Assert(tile.Hotel == check.Hotel);

						if (check.Type == MapTile.TYPE.HOTEL && check.Hotel.IsSafe)
						{
							if (neighbor == null || neighbor == check.Hotel)
								neighbor = check.Hotel;
							else
								Engine.Assert(tile.Type == MapTile.TYPE.UNPLAYABLE_MERGE_SAFE);
						}

						// if single, neighbors must be undeveloped or unavailable
						if (tile.Type == MapTile.TYPE.SINGLE)
							Engine.Assert(check.Type == MapTile.TYPE.UNDEVELOPED || check.Type == MapTile.TYPE.UNPLAYABLE_MERGE_SAFE ||
							              check.Type == MapTile.TYPE.UNPLAYABLE_NO_AVAIL_CHAINS);
					}

				}
			}

			foreach (HotelChain hotel in chains)
				Engine.Assert(hotel.NumTiles == mapNumTiles[hotel]);
		}

#else

		public void ValidateMap(List<HotelChain> chains)
		{
			
		}

#endif

	}
}
