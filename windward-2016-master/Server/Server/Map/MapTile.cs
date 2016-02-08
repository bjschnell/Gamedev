/*
 * ----------------------------------------------------------------------------
 * "THE BEER-WARE LICENSE" 
 * As long as you retain this notice you can do whatever you want with this 
 * stuff. If you meet an employee from Windward meet some day, and you think
 * this stuff is worth it, you can buy them a beer in return. Windward Studios
 * ----------------------------------------------------------------------------
 */

using System.Drawing;
using Server.Sprites;
using Server.Units;

namespace Server.Map
{
	/// <summary>
	/// A specific tile on the map. The type will change when the tile is played or becomes unplayable.
	/// </summary>
	public class MapTile
	{
		private HotelChain _hotel;

		public enum TYPE
		{
			/// <summary>
			/// Nothing on tile, ok to place a tile there.
			/// </summary>
			UNDEVELOPED = 1,
			/// <summary>
			/// Hotel on this tile.
			/// </summary>
			HOTEL = 2,
			/// <summary>
			/// Played tile, needs a second tile to become a chain.
			/// </summary>
			SINGLE = 3,
			/// <summary>
			/// Can never play this tile, would merge 2 safe chains.
			/// </summary>
			UNPLAYABLE_MERGE_SAFE = 4,
			/// <summary>
			/// Can not play this tile now as it would create a chain and there are no available chains.
			/// </summary>
			UNPLAYABLE_NO_AVAIL_CHAINS = 5
		}

		/// <summary>
		/// The type of square.
		/// </summary>
		public TYPE Type { get; set; }

		/// <summary>
		/// True if this was the last tile played
		/// </summary>
		public bool LastPlayed { get; set; }

		/// <summary>
		/// The bitmap for this tile.
		/// </summary>
		public Image SpriteBitmap
		{
			get
			{
				switch (Type)
				{
					case TYPE.UNDEVELOPED:
						return MapSprites.park4;
					case TYPE.SINGLE:
						return MapSprites.office;
					case TYPE.UNPLAYABLE_MERGE_SAFE:
						return MapSprites.road;
					case TYPE.UNPLAYABLE_NO_AVAIL_CHAINS:
						// bugbug - different sprite needed
						return MapSprites.road;
				}
				return Hotel.SpriteBitmap;
			}
		}

		/// <summary>
		/// If this is a hotel, the chain it is. Setting this will set Type too.
		/// </summary>
		public HotelChain Hotel
		{
			get { return _hotel; }
			set
			{
				_hotel = value;
				Type = TYPE.HOTEL;
			}
		}

		public MapTile()
		{
			Type = TYPE.UNDEVELOPED;
			_hotel = null;
			LastPlayed = false;
		}
	}
}
