using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport
{
	// ToDo - Test!
	[ReflectorType("buildReportBuildPlugin")]
	public class BuildReportBuildPlugin : IPlugin
	{
		public static readonly string ACTION_NAME = "ViewBuildReport";

		private readonly IActionInstantiator actionInstantiator;
		private string[] xslFileNames = new string[0];

		public BuildReportBuildPlugin(IActionInstantiator actionInstantiator)
		{
			this.actionInstantiator = actionInstantiator;
		}

		public string LinkDescription
		{
			get { return "Build Report"; }
		}

		[ReflectorArray("xslFileNames")]
		public string[] XslFileNames
		{
			get
			{
				return xslFileNames;
			}
			set
			{
				xslFileNames = value;
			}
		}

		public INamedAction[] NamedActions
		{
			get
			{
				MultipleXslReportBuildAction buildAction = (MultipleXslReportBuildAction) actionInstantiator.InstantiateAction(typeof(MultipleXslReportBuildAction));
				buildAction.XslFileNames = XslFileNames;
				return new INamedAction[] { new ImmutableNamedAction(ACTION_NAME, buildAction) } ;
			}
		}
	}
}
