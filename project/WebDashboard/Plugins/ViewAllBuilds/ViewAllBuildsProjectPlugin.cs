using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewAllBuilds
{
	// ToDo - Test!
	[ReflectorType("viewAllBuildsProjectPlugin")]
	public class ViewAllBuildsProjectPlugin : ICruiseAction, IPlugin
	{
		public static readonly string ACTION_NAME = "ViewAllBuilds";

		private readonly IAllBuildsViewBuilder viewBuilder;

		public ViewAllBuildsProjectPlugin (IAllBuildsViewBuilder viewBuilder)
		{
			this.viewBuilder = viewBuilder;
		}

		public IResponse Execute(ICruiseRequest cruiseRequest)
		{
			return viewBuilder.GenerateAllBuildsView(cruiseRequest.ProjectSpecifier, cruiseRequest.RetrieveSessionToken());
		}

		public string LinkDescription
		{
			get { return "View All Builds"; }
		}

		public INamedAction[] NamedActions
		{
			get {  return new INamedAction[] { new ImmutableNamedAction(ACTION_NAME, this) }; }
		}
	}
}
