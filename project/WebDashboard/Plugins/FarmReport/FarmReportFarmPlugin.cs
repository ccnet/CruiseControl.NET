using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.FarmReport
{
	// ToDo - Test!
	[ReflectorType("farmReportFarmPlugin")]
	public class FarmReportFarmPlugin : ICruiseAction, IPlugin
	{
		public static readonly string ACTION_NAME = "ViewFarmReport";

		private readonly IProjectGridAction projectGridAction;

		public FarmReportFarmPlugin(IProjectGridAction projectGridAction)
		{
			this.projectGridAction = projectGridAction;
		}

		public IView Execute(ICruiseRequest request)
		{
			return projectGridAction.Execute(ACTION_NAME, request.Request);
		}

		public string LinkDescription
		{
			get { return "Farm Report"; }
		}

		public INamedAction[] NamedActions
		{
			get {  return new INamedAction[] { new ImmutableNamedAction(ACTION_NAME, this) }; }
		}
	}
}
