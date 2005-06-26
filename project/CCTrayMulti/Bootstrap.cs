using System;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;
using ThoughtWorks.CruiseControl.CCTrayLib.ServerConnection;

namespace ThoughtWorks.CruiseControl.CCTrayMulti
{
	public class Bootstrap
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.EnableVisualStyles();
			Application.DoEvents();

			try
			{
				ICruiseManagerFactory remoteCruiseManagerFactory = new RemoteCruiseManagerFactory();
				ICruiseProjectManagerFactory cruiseProjectManagerFactory = new CruiseProjectManagerFactory( remoteCruiseManagerFactory );
				CCTrayMultiConfiguration configuration = new CCTrayMultiConfiguration( cruiseProjectManagerFactory, "settings.xml" );

				MainFormController controller = new MainFormController(configuration);

				Application.Run(new MainForm(controller));
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to start: " + ex, AppDomain.CurrentDomain.FriendlyName);
			}
		}

	}
}
