using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerReport
{
	public class ServerReportServerPlugin : ICruiseAction, IPluginLinkRenderer, IPlugin
	{
		private readonly IProjectGridAction projectGridAction;

		public ServerReportServerPlugin(IProjectGridAction projectGridAction)
		{
			this.projectGridAction = projectGridAction;
		}

		public IView Execute(ICruiseRequest request)
		{
			return projectGridAction.Execute(request.Request.ActionArguments, this.LinkActionName, request.ServerSpecifier);
		}

		public string LinkDescription
		{
			get { return "Server Report"; }
		}

		public string LinkActionName
		{
			get { return "ViewServerReport"; }
		}

		public TypedAction[] Actions
		{
			get {  return new TypedAction[] { new TypedAction(LinkActionName, this.GetType()) }; }
		}
	}
}
