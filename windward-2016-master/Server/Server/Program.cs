/*
 * ----------------------------------------------------------------------------
 * "THE BEER-WARE LICENSE" 
 * As long as you retain this notice you can do whatever you want with this 
 * stuff. If you meet an employee from Windward meet some day, and you think
 * this stuff is worth it, you can buy them a beer in return. Windward Studios
 * ----------------------------------------------------------------------------
 */

using System;
using System.Windows.Forms;
using log4net;
using log4net.Config;
using Server.UI;

namespace Server
{
	static class Program
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(Program));

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			XmlConfigurator.Configure();
			log.Info("***** Acquire starting *****");

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainWindow());
		}
	}
}
