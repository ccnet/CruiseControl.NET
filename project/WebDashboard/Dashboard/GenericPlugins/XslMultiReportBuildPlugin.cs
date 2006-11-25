using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.GenericPlugins
{
	/// <summary>
	/// A Generic XSL report template that can accept multiple transforms.
	/// </summary>
	[ReflectorType("xslMultiReportBuildPlugin")]
	public class XslMultiReportBuildPlugin : ProjectConfigurableBuildPlugin
	{
		public XslMultiReportBuildPlugin(IActionInstantiator actionInstantiator)
		{
			this.actionInstantiator = actionInstantiator;
		}

		private readonly IActionInstantiator actionInstantiator;
		private string[] xslFileNames = new string[0];
		private string description = "no description set";
		private string actionName = "NoActionSet";

		// These 2 are separate due to inheritence / property monkey-ness
		[ReflectorProperty("description")]
		public string ConfiguredLinkDescription
		{
			get { return description; }
			set { description = value; }
		}

		// See note on ConfiguredLinkDescription
		public override string LinkDescription
		{
			get { return description; }
		}

		[ReflectorProperty("actionName")]
		public string ActionName
		{
			get { return actionName; }
			set { actionName = value; }
		}

		[ReflectorArray("xslFileNames")]
		public string[] XslFileNames
		{
			get { return xslFileNames; }
			set { xslFileNames = value; }
		}

		public override INamedAction[] NamedActions
		{
			get
			{
				MultipleXslReportBuildAction buildAction = (MultipleXslReportBuildAction) actionInstantiator.InstantiateAction(typeof (MultipleXslReportBuildAction));
				buildAction.XslFileNames = XslFileNames;
				return new INamedAction[] {new ImmutableNamedAction(ActionName, buildAction)};
			}
		}
	}
}