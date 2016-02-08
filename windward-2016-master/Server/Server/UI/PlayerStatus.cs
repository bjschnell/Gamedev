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
using System.Linq;
using System.Windows.Forms;
using Server.Sprites;
using Server.Units;

namespace Server.UI
{
	/// <summary>
	///     The window that displays a player's status.
	/// </summary>
	internal partial class PlayerStatus : UserControl
	{
		private static readonly Bitmap[] avatars = { AvatarSprites.avatar1, AvatarSprites.avatar2, AvatarSprites.avatar3, AvatarSprites.avatar4 };
		private static int nextAvatar;

		private bool firstTime = true;

		/// <summary>
		///     Create the window.
		/// </summary>
		/// <param name="player">The player this window is for.</param>
		public PlayerStatus(Player player)
		{
			InitializeComponent();
			pictWinner.Visible = false;
			Player = player;
		}

		/// <summary>
		///     The player this is showing status for.
		/// </summary>
		public Player Player { get; private set; }

		/// <summary>
		///     Redraw this window. Call when status has changed.
		/// </summary>
		public void UpdateStats()
		{
			labelScore.Text = Player.Score.ToString("C0");

			pictNoConnection.Visible = ! Player.IsConnected;
			pictWinner.Visible = Player.IsWinner;

			Invalidate(true);
		}

		public Label GetLabelName()
		{
			return labelName;
		}

		private void PlayerStatus_Load(object sender, EventArgs e)
		{
//			BackColor = Player.SpriteColor;
			if ((Player.Avatar != null) && (Player.Avatar.Width == 32) && (Player.Avatar.Height == 32))
				pictureBoxAvatar.Image = Player.Avatar;
			else
			{
				pictureBoxAvatar.Image = avatars[nextAvatar++];
				if (nextAvatar >= avatars.Length)
					nextAvatar = 0;
			}
			labelName.Text = Player.Name;
		}

		private void PlayerStatus_Paint(object sender, PaintEventArgs pe)
		{

			pe.Graphics.CompositingQuality = CompositingQuality.HighQuality;
			pe.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			
			// cards
			int y = pictureBoxAvatar.Location.Y;
			int x = Size.Width / 2;
			foreach (var card in Player.Powers)
			{
				pe.Graphics.DrawImage(card.Logo, new Rectangle(x, y, 24, 24));
				x += 30;
			}

			// get the shares of each type
			List<HotelStock> firstMajority = Player.Stock.Where(s => s.Chain.FirstMajorityOwners.Any(o => o.Owner == Player)).ToList();
			// if multiple first majority holders, they're the second majority too
			List<HotelStock> secondMajority = Player.Stock.Where(s => s.Chain.SecondMajorityOwners.Any
				(o => (o.Owner == Player) && (!firstMajority.Contains(s)))).ToList();
			List<HotelStock> stockHolder = Player.Stock.Where(s => (! firstMajority.Contains(s)) && (! secondMajority.Contains(s))).ToList();

			// draw the shares
			DrawShares(pe, firstMajority, labelFirst.Location.Y);
			DrawShares(pe, secondMajority, labelSecond.Location.Y);
			DrawShares(pe, stockHolder, labelStock.Location.Y);
		}

		private void DrawShares(PaintEventArgs pe, List<HotelStock> stocks, int y)
		{
			int numShares = stocks.Sum(stock => stock.NumShares);
			if (numShares <= 0) 
				return;
			int x = labelFirst.Location.X + labelFirst.Size.Width;
			int add = Math.Min(14, (Size.Width - x) / numShares);
			foreach (var stock in stocks)
				for (int index = 0; index < stock.NumShares; index++)
				{
					pe.Graphics.DrawImage(stock.Chain.Logo, new Rectangle(x, y, 12, 12));
					x += add;
				}
			// may have fewer than before
			pe.Graphics.FillRectangle(Brushes.WhiteSmoke, new Rectangle(x, y, Size.Width - x, 12));
		}
	}
}