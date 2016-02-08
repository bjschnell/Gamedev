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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Server.Units;

namespace Server.UI
{
	public partial class PlayerScore : Form
	{
		private const int INDEX_FIRST_SCORE_COL = 3;

		private readonly List<PlayerStatus> playerStats = new List<PlayerStatus>();
		private DataTable tableScores;
		private DataTable tableStocks;
		private readonly Framework framework;

		public PlayerScore(Framework framework)
		{
			InitializeComponent();
			this.framework = framework;
		}

		/// <summary>
		///     Starting a game.
		/// </summary>
		public void SetupGame()
		{
			// if have stats windows - dispose them
			while (playerStats.Count > 0)
			{
				PlayerStatus statOn = playerStats[0];
				playerStats.RemoveAt(0);
				panelPlayers.Controls.Remove(statOn);
				statOn.Dispose();
			}

			// create player stats windows
			NewPlayerAdded();
		}

		/// <summary>
		///     New player added to the game
		/// </summary>
		public void NewPlayerAdded()
		{
			if (framework.GameEngine == null)
				return;

			// create player stats windows
			foreach (Player playerOn in framework.GameEngine.Players)
			{
				if (playerStats.Any(psOn => psOn.Player == playerOn))
					continue;

				PlayerStatus ps = new PlayerStatus(playerOn) {Width = panelPlayers.Width / 2};

				ps.Top = (playerStats.Count / 2) * ps.Height;
				if ((playerStats.Count & 1) == 0)
					ps.Left = 0;
				else
					ps.Left = panelPlayers.Width / 2;
				playerStats.Add(ps);
				panelPlayers.Controls.Add(ps);
			}

			// order them by name
			int height = -1;
			int winOn = 0;
			panelPlayers.SuspendLayout();
			playerStats.Sort((a, b) => (a.Player.Name.CompareTo(b.Player.Name)));
			foreach (PlayerStatus psOn in playerStats)
			{
				// set up top position
				if (height == -1)
					height = psOn.Height;

				psOn.Top = (winOn / 2) * height;
				if ((winOn & 1) == 0)
					psOn.Left = 0;
				else
					psOn.Left = panelPlayers.Width / 2;
				winOn++;
			}
			if (playerStats.Count > 0)
				panelPlayers.Height = ((playerStats.Count + 1) / 2) * playerStats[0].Height;
			panelPlayers.ResumeLayout();
			UpdatePlayers();
		}

		public void SetActivePlayer(Player plyr)
		{

			foreach (PlayerStatus psOn in playerStats)
			{
				if (psOn.Player.Guid == plyr.Guid)
					psOn.GetLabelName().ForeColor = Color.Red;
				else
					psOn.GetLabelName().ForeColor = Color.Black;
			}
		}

		/// <summary>
		///     Update (re-draw) the player status windows.
		/// </summary>
		public void UpdatePlayers()
		{
			if (playerStats.Count == 0)
				return;

			foreach (PlayerStatus ps in playerStats)
				ps.UpdateStats();
			panelPlayers.Invalidate();
			panelPlayers.Update();

			UpdateScoreboard();
		}

		/// <summary>
		///     Called to delete all player status windows the player status windows.
		/// </summary>
		public void ResetPlayers()
		{
			if (tableScores != null)
			{
				tableScores.Dispose();
				tableScores = null;

				UpdateScoreboard();
			}

			if (playerStats.Count != 0)
			{
				foreach (PlayerStatus ps in playerStats)
					ps.Dispose();
				playerStats.Clear();

				panelPlayers.Invalidate();
				panelPlayers.Update();
			}
		}

		private void UpdateScoreboard()
		{
			if (playerStats.Count == 0)
				return;
			UpdateScores();
			UpdateStocks();
		}

		private void UpdateScores()
		{

			bool incGameOn = framework.Mode != Framework.COMM_STATE.GAME_OVER;

			// create a new table (new game or player added)?
			if ((tableScores == null) || (tableScores.Rows.Count != playerStats.Count) ||
			    (tableScores.Columns.Count !=
			     playerStats[0].Player.Scoreboard.Count + (incGameOn ? 1 : 0) + INDEX_FIRST_SCORE_COL + 1))
			{
				tableScores = new DataTable();
				tableScores.Columns.Add("Player", typeof (string));
				tableScores.Columns.Add("School", typeof (string));
				tableScores.Columns.Add("Lang", typeof (string));
				for (int ind = 0; ind < playerStats[0].Player.Scoreboard.Count + (incGameOn ? 1 : 0); ind++)
					tableScores.Columns.Add("Game " + (ind + 1), typeof (float));
				tableScores.Columns.Add("Total", typeof (float));
				foreach (PlayerStatus plyrOn in playerStats.OrderBy(pl => pl.Player.Name))
				{
					List<object> cells = new List<object> {plyrOn.Player.Name, plyrOn.Player.School, plyrOn.Player.Language};
					float total = 0;
					foreach (float score in plyrOn.Player.Scoreboard)
					{
						cells.Add(score);
						total += score;
					}
					if (incGameOn)
					{
						cells.Add(plyrOn.Player.Score);
						total += plyrOn.Player.Score;
					}
					cells.Add(total);
					tableScores.Rows.Add(cells.ToArray());
				}

				dataGridViewScores.SuspendLayout();
				dataGridViewScores.DataSource = null;
				dataGridViewScores.DataSource = tableScores;
				for (int ind = 3; ind < dataGridViewScores.Columns.Count; ind++)
					dataGridViewScores.Columns[ind].DefaultCellStyle.Format = "C0";
				dataGridViewScores.Sort(dataGridViewScores.Columns[dataGridViewScores.Columns.Count - 1],
					ListSortDirection.Descending);
				dataGridViewScores.ResumeLayout();
				return;
			}

			// update existing table
			dataGridViewScores.SuspendLayout();

			foreach (PlayerStatus plyrOn in playerStats.OrderBy(pl => pl.Player.Name))
			{
				DataRow row = tableScores.Rows.Cast<DataRow>().FirstOrDefault(rowOn => (string) rowOn[0] == plyrOn.Player.Name);
				if (row == null)
					continue;
				float total = 0;
				int colOn = INDEX_FIRST_SCORE_COL;
				foreach (float score in plyrOn.Player.Scoreboard)
				{
					row[colOn++] = score;
					total += score;
				}
				if (incGameOn)
				{
					row[colOn++] = plyrOn.Player.Score;
					total += plyrOn.Player.Score;
				}
				row[colOn] = total;
			}

			dataGridViewScores.ResumeLayout();
		}

		private void UpdateStocks()
		{
			if (framework.GameEngine.HotelChains == null)
				return;

			if (tableStocks == null || (tableStocks.Columns.Count != framework.GameEngine.Players.Count + 4))
			{
				tableStocks = new DataTable();
				tableStocks.Columns.Add("Hotel", typeof (string));
				foreach (Player plyrOn in framework.GameEngine.Players.OrderBy(pl => pl.Name))
					tableStocks.Columns.Add(plyrOn.Name, typeof (int));
				tableStocks.Columns.Add("Available", typeof (int));
				tableStocks.Columns.Add("Num Tiles", typeof(int));
				tableStocks.Columns.Add("Price", typeof(string));
				foreach (HotelChain chain in framework.GameEngine.HotelChains.OrderBy(h => h.Name))
				{
					List<object> cells = new List<object> {chain.Name};
					tableStocks.Rows.Add(cells.ToArray());
				}

				dataGridViewStock.SuspendLayout();
				dataGridViewStock.DataSource = null;
				dataGridViewStock.DataSource = tableStocks;
				dataGridViewStock.ResumeLayout();
				return;
			}

			// update existing table
			dataGridViewStock.SuspendLayout();

			foreach (HotelChain chain in framework.GameEngine.HotelChains.OrderBy(h => h.Name))
			{
				DataRow row = tableStocks.Rows.Cast<DataRow>().FirstOrDefault(rowOn => (string) rowOn[0] == chain.Name);
				if (row == null)
					continue;
				int colOn = 1;
				foreach (Player plyrOn in framework.GameEngine.Players.OrderBy(pl => pl.Name))
				{
					StockOwner stock = chain.Owners.FirstOrDefault(c => c.Owner == plyrOn);
					row[colOn++] = stock == null ? 0 : stock.NumShares;
				}
				row[colOn++] = chain.NumAvailableShares;
				row[colOn++] = chain.NumTiles;
				row[colOn] = string.Format("{0:C}", chain.StockPrice);
			}

			dataGridViewStock.ResumeLayout();
		}

		private void PlayerScore_Load(object sender, EventArgs e)
		{
			MainWindow.RestoreWindow(this, "scoreboard");
		}

		private void panelPlayers_SizeChanged(object sender, EventArgs e)
		{
			int wid = ClientRectangle.Width / 2;
			for (int index = 0; index < playerStats.Count; index++)
			{
				PlayerStatus psOn = playerStats[index];
				psOn.Left = (index & 0x01) == 0x01 ? wid : 0;
				psOn.Width = wid;
			}
		}
	}
}