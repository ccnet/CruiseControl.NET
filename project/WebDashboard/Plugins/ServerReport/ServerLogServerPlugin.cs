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
	[ReflectorType("serverLogServerPlugin")]
	public class ServerLogServerPlugin : ICruiseAction, IPlugin
	{
		private readonly IFarmService farmService;
		private readonly IVelocityViewGenerator viewGenerator;

		public ServerLogServerPlugin(IFarmService farmService, IVelocityViewGenerator viewGenerator)
		{
			this.farmService = farmService;
			this.viewGenerator = viewGenerator;
		}

		public IResponse Execute(ICruiseRequest request)
		{
			Hashtable velocityContext = new Hashtable();
			velocityContext["log"] = farmService.GetServerLog(request.ServerSpecifier);

			return viewGenerator.GenerateView(@"ServerLog.vm", velocityContext);
		}

		public string LinkDescription
		{
			get { return "View Server Log"; }
		}

		public INamedAction[] NamedActions
		{
			get {  return new INamedAction[] { new ImmutableNamedAction("ViewServerLog", this) }; }
		}
	}
}
