using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerReport
{
    /// <title>Server Information Server Plugin</title>
    /// <version>1.4.4</version>
    /// <summary>
    /// The Server Information Server Plugin gives you information about a build server, for example the version of CruiseControl.NET the build
    /// server is running.
    /// </summary>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;serverInformationServerPlugin /&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;serverInformationServerPlugin minFreeSpace="524288" /&gt;
    /// </code>
    /// </example>
    [ReflectorType("serverInformationServerPlugin")]
	public class ServerInformationServerPlugin : ICruiseAction, IPlugin
	{
		private readonly IFarmService farmService;
		private readonly IVelocityViewGenerator viewGenerator;
		private long minFreeSpace = 1048576;

		public ServerInformationServerPlugin(IFarmService farmService, IVelocityViewGenerator viewGenerator)
		{
			this.farmService = farmService;
			this.viewGenerator = viewGenerator;
		}

        /// <summary>
        /// The minimum required amount of free disk space in bytes. If the free disk space is less than this a warning will be displayed.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>1048576</default>
        [ReflectorProperty("minFreeSpace", Required = false)]
		public long MinFreeSpace
        {
			get { return minFreeSpace; }
			set { minFreeSpace = value; }
        }

		public IResponse Execute(ICruiseRequest request)
		{
			Hashtable velocityContext = new Hashtable();
			
			velocityContext["serverversion"] = farmService.GetServerVersion(request.ServerSpecifier);
			velocityContext["servername"] = request.ServerSpecifier.ServerName;
            long freeSpace = farmService.GetFreeDiskSpace(request.ServerSpecifier);
            velocityContext["serverSpace"] = FormatSpace(freeSpace);
			velocityContext["spaceMessage"] = minFreeSpace > freeSpace ?
                "WARNING: Disk space is running low!" :
                string.Empty;
			
			return viewGenerator.GenerateView(@"ServerInfo.vm", velocityContext);
		}

		public string LinkDescription
		{
			get { return "View Server Info"; }
		}

		public INamedAction[] NamedActions
		{
			get {  return new INamedAction[] { new ImmutableNamedAction("ViewServerInfo", this) }; }
		}

        private string FormatSpace(long space)
        {
            string formated;
            double value = space;

            if (space > 1073741824)
            {
                value /= 1073741824;
                formated = string.Format("{0:#,##0.00} Gb", value);
            }
            else if (space > 1048576)
            {
                value /= 1048576;
                formated = string.Format("{0:#,##0.00} Mb", value);
            }
            else if (space > 1024)
            {
                value /= 1024;
                formated = string.Format("{0:#,##0.00} Kb", value);
            }
            else
            {
                formated = string.Format("{0:#,##0} b", space);
            }

            return formated;
        }

	}
}
