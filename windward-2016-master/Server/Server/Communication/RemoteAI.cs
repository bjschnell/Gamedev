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
using System.Linq;
using System.Text;
using System.Xml.Linq;
using log4net;
using Server.AI;
using Server.Map;
using Server.Units;
using Server.Utilities;

namespace Server.Communication
{
	public class RemoteAI : IPlayerAI
	{
		private readonly Framework framework;

		private static readonly Random rand = new Random();

		private static readonly ILog log = LogManager.GetLogger(typeof(RemoteAI));

		public RemoteAI(Framework framework, string guid)
		{
			this.framework = framework;
			TcpGuid = guid;
		}

		public void Dispose()
		{
			// nada
		}

		public string TcpGuid { get; set; }
		public void Setup(GameMap map, Player me, List<HotelChain> hotelChains, List<Player> players)
		{
			XDocument doc = new XDocument();
			XElement elemRoot = new XElement("setup", new XAttribute("game-start", true), new XAttribute("my-guid", me.Guid));
			doc.Add(elemRoot);

			// the map
			XElement elemMap = new XElement("map", new XAttribute("width", map.Width), new XAttribute("height", map.Height));
			elemRoot.Add(elemMap);

			elemRoot.Add(BuildHotelChains(hotelChains));
			elemRoot.Add(BuildPlayers(players, me.Guid));
	
			elemRoot.Add(new XAttribute("msg-id", rand.Next()));
			framework.tcpServer.SendMessage(TcpGuid, doc.ToString());
		}

		public SpecialPowers.CARD QuerySpecialPowerBeforeTurn(GameMap map, Player you, List<HotelChain> hotelChains, List<Player> players)
		{
			XDocument doc = new XDocument();
			XElement elemRoot = new XElement("query-card");
			doc.Add(elemRoot);
			PopulateGameState(elemRoot, map, you, hotelChains, players);

			string msgId = Guid.NewGuid().ToString();
			elemRoot.Add(new XAttribute("msg-id", msgId));
			framework.tcpServer.SendMessage(TcpGuid, doc.ToString());

			XDocument xml = TcpServer.GetReply(msgId);
			if (xml == null)
			{
				if (log.IsWarnEnabled)
					log.Warn(string.Format("player {0} did not reply to QuerySpecialPowerBeforeTurn.", you.Name));
				return SpecialPowers.CARD.NONE;
			}
			if (xml.Root.Attribute("card") != null)
				return (SpecialPowers.CARD) Convert.ToInt32(xml.Root.Attribute("card").Value);
			return SpecialPowers.CARD.NONE;
		}

		public PlayerPlayTile QueryTileOnly(GameMap map, Player you, List<HotelChain> hotelChains, List<Player> players)
		{
			XDocument doc = new XDocument();
			XElement elemRoot = new XElement("query-tile");
			doc.Add(elemRoot);
			PopulateGameState(elemRoot, map, you, hotelChains, players);

			string msgId = Guid.NewGuid().ToString();
			elemRoot.Add(new XAttribute("msg-id", msgId));
			framework.tcpServer.SendMessage(TcpGuid, doc.ToString());

			XDocument xml = TcpServer.GetReply(msgId);
			if (xml == null)
			{
				if (log.IsWarnEnabled)
					log.Warn(string.Format("player {0} did not reply to QueryTileOnly.", you.Name));
				return null;
			}

			PlayerPlayTile rtn = new PlayerPlayTile();
			if (xml.Root.Attribute("tile-x") != null && xml.Root.Attribute("tile-y") != null)
				rtn.Tile = new PlayerTile(Convert.ToInt32(xml.Root.Attribute("tile-x").Value),
					Convert.ToInt32(xml.Root.Attribute("tile-y").Value));

			if (xml.Root.Attribute("created-hotel") != null)
				rtn.CreatedHotel = hotelChains.FirstOrDefault(h => h.Name == xml.Root.Attribute("created-hotel").Value);
			if (xml.Root.Attribute("merge-survivor") != null)
				rtn.MergeSurvivor = hotelChains.FirstOrDefault(h => h.Name == xml.Root.Attribute("merge-survivor").Value);
			return rtn;
		}

		public PlayerTurn QueryTileAndPurchase(GameMap map, Player you, List<HotelChain> hotelChains, List<Player> players)
		{
			XDocument doc = new XDocument();
			XElement elemRoot = new XElement("query-tile-purchase");
			doc.Add(elemRoot);
			PopulateGameState(elemRoot, map, you, hotelChains, players);

			string msgId = Guid.NewGuid().ToString();
			elemRoot.Add(new XAttribute("msg-id", msgId));
			framework.tcpServer.SendMessage(TcpGuid, doc.ToString());

			XDocument xml = TcpServer.GetReply(msgId);
			if (xml == null)
			{
				if (log.IsWarnEnabled)
					log.Warn(string.Format("player {0} did not reply to QueryTileAndPurchase.", you.Name));
				return null;
			}

			PlayerTurn rtn = new PlayerTurn();

			// PlayerPlayTile part
			if (xml.Root.Attribute("tile-x") != null && xml.Root.Attribute("tile-y") != null)
				rtn.Tile = new PlayerTile(Convert.ToInt32(xml.Root.Attribute("tile-x").Value),
					Convert.ToInt32(xml.Root.Attribute("tile-y").Value));

			if (xml.Root.Attribute("created-hotel") != null)
			{
				rtn.CreatedHotel = hotelChains.FirstOrDefault(h => h.Name == xml.Root.Attribute("created-hotel").Value);
				if (rtn.CreatedHotel == null && log.IsInfoEnabled)
					log.InfoFormat("Requested create hotel with illegal name {0}", xml.Root.Attribute("created-hotel").Value);
			}
			if (xml.Root.Attribute("merge-survivor") != null)
			{
				rtn.MergeSurvivor = hotelChains.FirstOrDefault(h => h.Name == xml.Root.Attribute("merge-survivor").Value);
				if (rtn.MergeSurvivor == null && log.IsInfoEnabled)
					log.InfoFormat("Requested survivor hotel with illegal name {0}", xml.Root.Attribute("merge-survivor").Value);
			}

			// PlayerTurn part
			if (xml.Root.Attribute("card") != null)
				rtn.Card = (SpecialPowers.CARD)Convert.ToInt32(xml.Root.Attribute("card").Value);

			if (xml.Root.Attribute("buy") != null)
			{
				string[] items = xml.Root.Attribute("buy").Value.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
				foreach (string buy in items)
				{
					string[] parts = buy.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);
					HotelChain chain = hotelChains.FirstOrDefault(h => h.Name == parts[0]);
					if (chain != null)
						rtn.Buy.Add(new HotelStock(chain, Convert.ToInt32(parts[1])));
					else
						if (log.IsInfoEnabled)
							log.InfoFormat("Requested buy hotel with illegal name {0}", parts[0]);
				}
			}

			if (xml.Root.Attribute("trade") != null)
			{
				string[] items = xml.Root.Attribute("trade").Value.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
				foreach (string trade in items)
				{
					string[] parts = trade.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);
					HotelChain chain = hotelChains.FirstOrDefault(h => h.Name == parts[0]);
					if (chain != null)
						rtn.Trade.Add(new PlayerTurn.TradeStock(chain, hotelChains.FirstOrDefault(h => h.Name == parts[1])));
					else
						if (log.IsInfoEnabled)
							log.InfoFormat("Requested trade for hotel with illegal name {0}", parts[0]);
				}
			}

			return rtn;
		}

		public PlayerMerge QueryMergeStock(GameMap map, Player you, List<HotelChain> hotelChains, List<Player> players, HotelChain survivor, HotelChain defunct)
		{
			XDocument doc = new XDocument();
			XElement elemRoot = new XElement("query-merge", new XAttribute("survivor", survivor.Name), new XAttribute("defunct", defunct.Name));
			doc.Add(elemRoot);
			PopulateGameState(elemRoot, map, you, hotelChains, players);

			string msgId = Guid.NewGuid().ToString();
			elemRoot.Add(new XAttribute("msg-id", msgId));
			framework.tcpServer.SendMessage(TcpGuid, doc.ToString());

			XDocument xml = TcpServer.GetReply(msgId);
			if (xml == null)
			{
				if (log.IsWarnEnabled)
					log.Warn(string.Format("player {0} did not reply to QueryMergeStock.", you.Name));
				return null;
			}

			int sell = xml.Root.Attribute("sell") != null ? Convert.ToInt32(xml.Root.Attribute("sell").Value) : 0;
			int keep = xml.Root.Attribute("keep") != null ? Convert.ToInt32(xml.Root.Attribute("keep").Value) : 0;
			int trade = xml.Root.Attribute("trade") != null ? Convert.ToInt32(xml.Root.Attribute("trade").Value) : 0;
			PlayerMerge rtn = new PlayerMerge(sell, keep, trade);
			return rtn;
		}

		private static void PopulateGameState(XElement elemRoot, GameMap map, Player me, List<HotelChain> hotelChains, List<Player> players)
		{

			// the map
			elemRoot.Add(BuildMap(map));
			elemRoot.Add(BuildHotelChains(hotelChains));
			elemRoot.Add(BuildPlayers(players, me.Guid));
		}

		#region convert to XML

		private static XElement BuildMap(GameMap map)
		{
			XElement elemMap = new XElement("map",
				new XAttribute("width", map.Width),
				new XAttribute("height", map.Height));
			for (int x = 0; x < map.Width; x++)
			{
				XElement elemColumn = new XElement("column", new XAttribute("x", x));
				StringBuilder buf = new StringBuilder();
				for (int y = 0; y < map.Height; y++)
				{
					MapTile tile = map.Tiles[x][y];
					buf.Append(string.Format("{0}:{1};", (int)tile.Type, tile.Hotel == null ? "" : tile.Hotel.Name));
				}
				elemColumn.Value = buf.ToString();
				elemMap.Add(elemColumn);
			}

			return elemMap;
		}

		private static XElement BuildHotelChains(IEnumerable<HotelChain> hotelChains)
		{
			XElement elemChains = new XElement("hotels");
			foreach (HotelChain hotel in hotelChains)
			{
				XElement elemHotel = new XElement("hotel", new XAttribute("name", hotel.Name),
												new XAttribute("start-price", hotel.StartPrice),
												new XAttribute("num-tiles", hotel.NumTiles),
												new XAttribute("is-active", hotel.IsActive),
												new XAttribute("is-safe", hotel.IsSafe),
												new XAttribute("stock-price", hotel.StockPrice),
												new XAttribute("first-majority", hotel.FirstMajorityBonus),
												new XAttribute("second-majority", hotel.SecondMajorityBonus),
												new XAttribute("num-avail-shares", hotel.NumAvailableShares)
												);
				if (hotel.Owners.Count > 0)
				{
					XElement owners = BuildHotelOwners("owners", hotel.Owners);
					elemHotel.Add(owners);
				}
				if (hotel.FirstMajorityOwners.Count > 0)
				{
					XElement owners = BuildHotelOwners("first-majority", hotel.FirstMajorityOwners);
					elemHotel.Add(owners);
				}
				if (hotel.SecondMajorityOwners.Count > 0)
				{
					XElement owners = BuildHotelOwners("second-majority", hotel.SecondMajorityOwners);
					elemHotel.Add(owners);
				}
				elemChains.Add(elemHotel);
			}
			return elemChains;
		}

		private static XElement BuildHotelOwners(string rootName, IEnumerable<StockOwner> owners)
		{
			XElement elemOwners = new XElement(rootName);
			foreach (var ownerOn in owners)
			{
				XElement elemOwnerOn = new XElement("owner", new XAttribute("guid", ownerOn.Owner.Guid),
												new XAttribute("num-shares", ownerOn.NumShares));
				elemOwners.Add(elemOwnerOn);
			}
			return elemOwners;
		}

		private static XElement BuildPlayers(IEnumerable<Player> players, string guidYou)
		{
			XElement elemPlayers = new XElement("players");
			foreach (Player plyrOn in players)
			{
				XElement elemPlyrOn = new XElement("player",
					new XAttribute("cash", plyrOn.Cash),
					new XAttribute("guid", plyrOn.Guid),
					new XAttribute("name", plyrOn.Name),
					new XAttribute("score", plyrOn.Score));

				if (plyrOn.Guid == guidYou && plyrOn.Tiles.Count > 0)
				{
					XElement elemTiles = new XElement("tiles");
					StringBuilder buf = new StringBuilder();
					foreach (PlayerTile tile in plyrOn.Tiles)
						buf.Append(string.Format("{0}:{1};", tile.X, tile.Y));
					elemTiles.Value = buf.ToString();
					elemPlyrOn.Add(elemTiles);
				}

				if (plyrOn.Powers.Count > 0)
				{
					XElement elemPowers = new XElement("powers");
					StringBuilder buf = new StringBuilder();
					foreach (SpecialPowers pwrOn in plyrOn.Powers)
						buf.Append(string.Format("{0};", (int)pwrOn.Card));
					elemPowers.Value = buf.ToString();
					elemPlyrOn.Add(elemPowers);
				}

				if (plyrOn.Stock.Count > 0)
				{
					XElement elemStock = new XElement("stock");
					StringBuilder buf = new StringBuilder();
					foreach (var stockOn in plyrOn.Stock)
						buf.Append(string.Format("{0}:{1};", stockOn.Chain.Name, stockOn.NumShares));
					elemStock.Value = buf.ToString();
					elemPlyrOn.Add(elemStock);
				}

				if (plyrOn.Scoreboard.Count > 0)
				{
					XElement elemScoreboard = new XElement("scoreboard");
					StringBuilder buf = new StringBuilder();
					foreach (int score in plyrOn.Scoreboard)
						buf.Append(string.Format("{0};", score));
					elemScoreboard.Value = buf.ToString();
					elemPlyrOn.Add(elemScoreboard);
				}

				elemPlayers.Add(elemPlyrOn);
			}
			return elemPlayers;
		}

		#endregion
	}
}
