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
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Server.Map;
using Server.Sprites;

namespace Windwardopolis2Library.map
{
	/// <summary>
	///     The map window.
	/// </summary>
	public partial class MapDisplay : UserControl
	{
		private readonly Font fontOfficeName = new Font("Calibri", 12);

		private readonly StringFormat format = new StringFormat
		{
			LineAlignment = StringAlignment.Center,
			Alignment = StringAlignment.Center
		};

		/// <summary>
		///     Display map coordinates on the top and left.
		/// </summary>
		public bool DisplayCoordinates { get; set; }

		/// <summary>
		///     Create the map window.
		/// </summary>
		public MapDisplay()
		{
			InitializeComponent();
		}

		private IMapInfo Engine
		{
			get
			{
				Control ctrl = Parent;
				while (ctrl != null)
				{
					IMapInfo wnd = ctrl as IMapInfo;
					if (wnd != null)
						return wnd;
					ctrl = ctrl.Parent;
				}
				return null;
			}
		}

		private void MapDisplay_Load(object sender, EventArgs e)
		{
			ZoomChanged();
		}

		public void ZoomChanged()
		{
			// not parent in design mode
			IMapInfo engine = Engine;
			if (engine == null)
				return;

			GameMap map = engine.Map;
			if (map == null)
				return;

			AutoScrollMinSize = new Size(GameMap.NUM_X_TILES * Engine.PixelsPerTile, GameMap.NUM_Y_TILES * Engine.PixelsPerTile);
			Invalidate();
		}

		/// <summary>
		/// Invalidate the tile just played
		/// </summary>
		/// <param name="tile">The map coordinates of the tile to invalidate.</param>
		public void InvalidateTile(Point tile)
		{
			Rectangle rect = new Rectangle(tile.X * Engine.PixelsPerTile-1, tile.Y * Engine.PixelsPerTile-1,
							(tile.X + 1) * Engine.PixelsPerTile+3, (tile.Y + 1) * Engine.PixelsPerTile+3);
			Invalidate(rect);
		}

		/// <summary>
		///     Paint the window.
		/// </summary>
		private void MapDisplay_Paint(object sender, PaintEventArgs pea)
		{

			try
			{
				// not parent in design mode
				IMapInfo engine = Engine;
				if (engine == null)
					return;
				GameMap map = engine.Map;
				if (map == null)
					return;

				Pen pen = new Pen(Color.Red, 4);

				pea.Graphics.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);
				pea.Graphics.CompositingQuality = CompositingQuality.HighQuality;
				pea.Graphics.SmoothingMode = SmoothingMode.HighQuality;
				// no - draws lines around each square
				// pea.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

				Rectangle rectTile = new Rectangle(0, 0, Engine.PixelsPerTile, Engine.PixelsPerTile);
				Rectangle rectOutline = new Rectangle(0, 0, Engine.PixelsPerTile - 5, Engine.PixelsPerTile - 5);

				int xStart = (pea.ClipRectangle.X - AutoScrollPosition.X)/Engine.PixelsPerTile;
				int xEnd = (pea.ClipRectangle.X + pea.ClipRectangle.Width - AutoScrollPosition.X + Engine.PixelsPerTile - 1)/
				           Engine.PixelsPerTile;
				xEnd = Math.Min(xEnd, map.Tiles.Length);
				int yStart = (pea.ClipRectangle.Y - AutoScrollPosition.Y)/Engine.PixelsPerTile;
				int yEnd = (pea.ClipRectangle.Y + pea.ClipRectangle.Height - AutoScrollPosition.Y + Engine.PixelsPerTile - 1)/
				           Engine.PixelsPerTile;
				yEnd = Math.Min(yEnd, map.Tiles[0].Length);

				for (int x = xStart; x < xEnd; x++)
				{
					rectTile.X = x * Engine.PixelsPerTile;
					for (int y = yStart; y < yEnd; y++)
					{
						MapTile square = map.Tiles[x][y];
						rectTile.Y = y * Engine.PixelsPerTile;
						pea.Graphics.DrawImage(square.SpriteBitmap, rectTile);
						if (square.LastPlayed)
						{
							rectOutline.X = rectTile.X + 2;
							rectOutline.Y = rectTile.Y + 2;
							pea.Graphics.DrawRectangle(pen, rectOutline);
						}
					}
				}

#if NAMES
	// Office names. On top of terrain but below everything else
			if (Engine.PixelsPerTile >= 24)
			{
				foreach (SignalSquare officeOn in offices)
				{
					int x = officeOn.X*Engine.PixelsPerTile + Engine.PixelsPerTile/2;
					int y = officeOn.Y*Engine.PixelsPerTile + (int) (Engine.PixelsPerTile*1.5f);
					switch (officeOn.Square.Tile.Direction)
					{
						case MapTile.DIRECTION.NORTH_UTURN:
							y -= Engine.PixelsPerTile*2;
							format.Alignment = StringAlignment.Center;
							break;
						case MapTile.DIRECTION.SOUTH_UTURN:
							format.Alignment = StringAlignment.Center;
							break;
						case MapTile.DIRECTION.WEST_UTURN:
							x -= Engine.PixelsPerTile/2;
							y -= Engine.PixelsPerTile;
							format.Alignment = StringAlignment.Far;
							break;
						case MapTile.DIRECTION.EAST_UTURN:
							x += Engine.PixelsPerTile/2;
							y -= Engine.PixelsPerTile;
							format.Alignment = StringAlignment.Near;
							break;
					}
					pea.Graphics.DrawString(officeOn.Square.Company.Name, fontOfficeName, Brushes.DarkOrange, x, y, format);
				}
				format.Alignment = StringAlignment.Center;
			}
#endif

				// write coordinates
				if (DisplayCoordinates)
				{
					int skip = Engine.PixelsPerTile >= 24 ? 1 : (Engine.PixelsPerTile >= 18 ? 2 : 5);
					for (int x = 0; x < map.Tiles.Length; x += skip)
						pea.Graphics.DrawString(Convert.ToString(x), fontOfficeName, Brushes.White,
							x*Engine.PixelsPerTile + Engine.PixelsPerTile/2,
							- AutoScrollPosition.Y + 12, format);
					for (int y = 0; y < map.Tiles[0].Length; y += skip)
						if (y != 0)
							pea.Graphics.DrawString(Convert.ToString(y), fontOfficeName, Brushes.White,
								-AutoScrollPosition.X + 12,
								y*Engine.PixelsPerTile + Engine.PixelsPerTile/2,
								format);
				}
			}
			catch (Exception)
			{
				// nada (this happened once in all our testing, a DrawImage() threw an exception).
			}
		}

		public void NewMap()
		{
			MapDisplay_Load(null, null);
			Invalidate();
		}

		private void MapDisplay_Scroll(object sender, ScrollEventArgs e)
		{
			Invalidate();
		}

		void MapDisplay_MouseWheel(object sender, MouseEventArgs e)
		{
			Invalidate();
		}

		private void MapDisplay_Resize(object sender, EventArgs e)
		{
			Invalidate();
		}
	}
}