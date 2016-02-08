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
using System.Windows.Forms;
using Server.Units;

namespace Server.UI
{
	public partial class DebugWindow : Form
	{
		private TreeNode nodePlayers;

		public DebugWindow(List<Player> players)
		{
			InitializeComponent();

			Setup(players);
		}

		public void Setup(List<Player> players)
		{
			// may be a new game
			treeView.Nodes.Clear();

			// set up companies
			nodePlayers = new TreeNode("Players") {Tag = players};
			treeView.Nodes.Add(nodePlayers);
			foreach (Player plyr in players)
			{
				TreeNode node = new TreeNode(plyr.Name) {Tag = plyr};
				node.Nodes.Add("Tiles:");
				nodePlayers.Nodes.Add(node);
			}

			Update(players);
		}

		public void Update(List<Player> players)
		{
			// update companies
			foreach (TreeNode nodePlyr in nodePlayers.Nodes)
			{
				var tileNode = nodePlyr.Nodes[0];
				IOrderedEnumerable<PlayerTile> sortedTiles = ((Player) nodePlyr.Tag).Tiles.OrderBy(a => a.X).ThenBy(b => b.Y);
				string allTiles = string.Join("; ", sortedTiles);
				tileNode.Text = string.Format("Tiles: {0}", allTiles);
			}
		}
	}
}