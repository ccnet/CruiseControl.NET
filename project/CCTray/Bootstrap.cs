using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTray
{
	public class Bootstrap
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.DoEvents();

			Application.ThreadException += new ThreadExceptionEventHandler(UnhandledWinFormException);
			try
			{
				ICruiseManagerFactory remoteCruiseManagerFactory = new RemoteCruiseManagerFactory();
				ICruiseProjectManagerFactory cruiseProjectManagerFactory = new CruiseProjectManagerFactory(remoteCruiseManagerFactory);
				CCTrayMultiConfiguration configuration = new CCTrayMultiConfiguration(cruiseProjectManagerFactory, GetSettingsFilename());

				MainForm mainForm = new MainForm(configuration);

				Application.Run(mainForm);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to start: " + ex, AppDomain.CurrentDomain.FriendlyName);
			}
		}

		private static void UnhandledWinFormException(object sender, ThreadExceptionEventArgs e)
		{
			MessageBox.Show("Unhandled exception: " + e.Exception);
		}

		private static string GetSettingsFilename()
		{
			string oldFashionedSettingsFilename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "settings.xml");
			string newSettingsFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "cctray-settings.xml");

			if (File.Exists(oldFashionedSettingsFilename) && !File.Exists(newSettingsFilename))
				File.Copy(oldFashionedSettingsFilename, newSettingsFilename);

			return newSettingsFilename;
		}

	}
}