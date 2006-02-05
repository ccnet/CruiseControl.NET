using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.GenericPlugins;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport
{
	// ToDo - Test!
	[ReflectorType("buildLogBuildPlugin")]
	public class BuildLogBuildPlugin : ProjectConfigurableBuildPlugin
	{
		private readonly IActionInstantiator actionInstantiator;

		public BuildLogBuildPlugin(IActionInstantiator actionInstantiator)
		{
			this.actionInstantiator = actionInstantiator;
		}

		public override string LinkDescription
		{
			get { return "View Build Log"; }
		}

		public override INamedAction[] NamedActions
		{
			get
			{
				return new INamedAction[]
					{
						new ImmutableNamedAction(HtmlBuildLogAction.ACTION_NAME, actionInstantiator.InstantiateAction(typeof (HtmlBuildLogAction)))
// We don't define this here right now since we need a way to define decorators
// See CruiseObjectSourceInitializer for linked ToDo
//					new TypedAction(XmlBuildLogAction.ACTION_NAME, typeof(XmlBuildLogAction)), 
					};
			}
		}
	}
}