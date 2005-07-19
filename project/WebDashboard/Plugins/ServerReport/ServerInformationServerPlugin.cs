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
	/// <summary>
	/// General information about a CCNet server
	/// </summary>
	[ReflectorType("serverInformationServerPlugin")]
	public class ServerInformationServerPlugin : ICruiseAction, IPlugin
	{
		private readonly IFarmService farmService;
		private readonly IVelocityViewGenerator viewGenerator;

		public ServerInformationServerPlugin(IFarmService farmService, IVelocityViewGenerator viewGenerator)
		{
			this.farmService = farmService;
			this.viewGenerator = viewGenerator;
		}

		public IView Execute(ICruiseRequest request)
		{
			Hashtable velocityContext = new Hashtable();
			
			velocityContext["serverversion"] = farmService.GetServerVersion(request.ServerSpecifier);
			velocityContext["servername"] = request.ServerSpecifier.ServerName;
			
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

	}
}
