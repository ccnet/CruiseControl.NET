using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.FarmReport
{
	public class FarmReportFarmPlugin : ICruiseAction, IPluginLinkRenderer, IPlugin
	{
		private readonly IProjectGridAction projectGridAction;

		public FarmReportFarmPlugin(IProjectGridAction projectGridAction)
		{
			this.projectGridAction = projectGridAction;
		}

		public IView Execute(ICruiseRequest request)
		{
			return projectGridAction.Execute(request.Request.ActionArguments, this.LinkActionName);
		}

		public string LinkDescription
		{
			get { return "New Farm Report (in development!)"; }
		}

		public string LinkActionName
		{
			get { return "ViewFarmReport"; }
		}

		public TypedAction[] Actions
		{
			get {  return new TypedAction[] { new TypedAction(LinkActionName, this.GetType()) }; }
		}
	}
}
