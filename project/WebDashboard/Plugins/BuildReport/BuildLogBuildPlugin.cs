using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport
{
	public class BuildLogBuildPlugin : IPluginLinkRenderer, IPlugin
	{
		public string LinkDescription
		{
			get { return "View Build Log"; }
		}

		public string LinkActionName
		{
			get { return HtmlBuildLogAction.ACTION_NAME; }
		}

		public TypedAction[] Actions
		{
			get {  
				return new TypedAction[]
				{
					new TypedAction(HtmlBuildLogAction.ACTION_NAME, typeof(HtmlBuildLogAction)),
// We don't define this here right now since we need a way to define decorators
// See CruiseObjectGiverInitializer for linked ToDo
//					new TypedAction(XmlBuildLogAction.ACTION_NAME, typeof(XmlBuildLogAction)), 
				}; 
			}
		}
	}
}
