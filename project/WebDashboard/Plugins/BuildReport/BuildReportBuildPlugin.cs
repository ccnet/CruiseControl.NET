using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.GenericPlugins;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport
{
	// ToDo - Test!
	[ReflectorType("buildReportBuildPlugin")]
	public class BuildReportBuildPlugin : XslMultiReportBuildPlugin
	{
		public static readonly string ACTION_NAME = "ViewBuildReport";

		public BuildReportBuildPlugin(IActionInstantiator actionInstantiator):base(actionInstantiator)
		{
			ActionName = ACTION_NAME;
		}

		public override string LinkDescription
		{
			get { return "Build Report"; }
		}

		public new string ActionName
		{
			get
			{
				return ACTION_NAME;
			}
			set
			{
			}
		}

		public new string ConfiguredLinkDescription
		{
			get { return "Build Report"; }
			set {}
		}
	}
}