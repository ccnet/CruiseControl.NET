using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerReport
{
	// ToDo - Test!
	[ReflectorType("serverReportServerPlugin")]
	public class ServerReportServerPlugin : ICruiseAction, IPlugin
	{
		public static readonly string ACTION_NAME = "ViewServerReport";

		private readonly IProjectGridAction projectGridAction;

		public ServerReportServerPlugin(IProjectGridAction projectGridAction)
		{
			this.projectGridAction = projectGridAction;
		}

		public IView Execute(ICruiseRequest request)
		{
			return projectGridAction.Execute(request.Request.ActionArguments, this.LinkActionName, request.ServerSpecifier, request.Request);
		}

		public string LinkDescription
		{
			get { return "Server Report"; }
		}

		public string LinkActionName
		{
			get { return ACTION_NAME; }
		}

		public INamedAction[] NamedActions
		{
			get {  return new INamedAction[] { new ImmutableNamedAction(LinkActionName, this) }; }
		}
	}
}
