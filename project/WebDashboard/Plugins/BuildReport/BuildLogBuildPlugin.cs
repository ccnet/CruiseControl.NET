using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport
{
	// ToDo - Test!
	[ReflectorType("buildLogBuildPlugin")]
	public class BuildLogBuildPlugin : IPlugin
	{
		private readonly IActionInstantiator actionInstantiator;

		public BuildLogBuildPlugin(IActionInstantiator actionInstantiator) 
		{
			this.actionInstantiator = actionInstantiator;
		}

		public string LinkDescription
		{
			get { return "View Build Log"; }
		}

		public string LinkActionName
		{
			get { return HtmlBuildLogAction.ACTION_NAME; }
		}

		public INamedAction[] NamedActions
		{
			get {  
				return new INamedAction[]
				{
					new ImmutableNamedAction(HtmlBuildLogAction.ACTION_NAME, actionInstantiator.InstantiateAction(typeof(HtmlBuildLogAction)))
// We don't define this here right now since we need a way to define decorators
// See CruiseObjectGiverInitializer for linked ToDo
//					new TypedAction(XmlBuildLogAction.ACTION_NAME, typeof(XmlBuildLogAction)), 
				}; 
			}
		}
	}
}
