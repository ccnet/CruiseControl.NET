using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.GenericPlugins
{
	[ReflectorType("xslReportBuildPlugin")]
	public class XslReportBuildPlugin : IPlugin
	{
		private readonly IActionInstantiator actionInstantiator;
		private string xslFileName = "";
		private string description = "no description set";
		private string actionName = "NoActionSet";

		public XslReportBuildPlugin(IActionInstantiator actionInstantiator)
		{
			this.actionInstantiator = actionInstantiator;
		}

		[ReflectorProperty("description")]
		public string LinkDescription
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

		public INamedAction[] NamedActions
		{
			get
			{
				XslReportBuildAction action = (XslReportBuildAction) actionInstantiator.InstantiateAction(typeof(XslReportBuildAction));
				action.XslFileName = XslFileName;
				return new INamedAction[] { new ImmutableNamedAction(actionName, action) } ;
			}
		}
	}
}
