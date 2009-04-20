using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions;
using ThoughtWorks.CruiseControl.Remote;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.GenericPlugins
{
	[ReflectorType("xslReportBuildPlugin")]
	public class XslReportBuildPlugin : ProjectConfigurableBuildPlugin
	{
		private readonly IActionInstantiator actionInstantiator;
		private string xslFileName = "";
		private string description = "no description set";
		private string actionName = "NoActionSet";

		public XslReportBuildPlugin(IActionInstantiator actionInstantiator)
		{
			this.actionInstantiator = actionInstantiator;
		}

        /// <summary>
        /// Optional parameters to pass into the XSL-T.
        /// </summary>
        [ReflectorProperty("parameters", Required = false)]
        public List<XsltParameter> Parameters { get; set; }

		// These 2 are separate due to inheritence / property monkey-ness
		[ReflectorProperty("description")]
		public string ConfiguredLinkDescription
		{
			get
			{
				return description;
			}
			set
			{
				description = value;
			}
		}

		// See note on ConfiguredLinkDescription
		public override string LinkDescription
		{
			get
			{
				return description;
			}
		}

		[ReflectorProperty("actionName")]
		public string ActionName
		{
			get
			{
				return actionName;
			}
			set
			{
				actionName = value;
			}
		}

		[ReflectorProperty("xslFileName")]
		public string XslFileName
		{
			get
			{
				return xslFileName;
			}
			set
			{
				xslFileName = value;
			}
		}

		public override INamedAction[] NamedActions
		{
			get
			{
				XslReportBuildAction action = (XslReportBuildAction) actionInstantiator.InstantiateAction(typeof(XslReportBuildAction));
				action.XslFileName = XslFileName;
                action.Parameters = Parameters;
				return new INamedAction[] { new ImmutableNamedAction(actionName, action) } ;
			}
		}
	}
}
