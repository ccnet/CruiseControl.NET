using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.GenericPlugins
{
	[ReflectorType("xslReportPlugin")]
	public class XslReportPlugin : IPlugin
	{
		private readonly IActionInstantiator actionInstantiator;
		private string xslFileName = "";
		private string description = "no description set";
		private string actionName = "NoActionSet";

		public XslReportPlugin(IActionInstantiator actionInstantiator)
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
				XslReportAction action = (XslReportAction) actionInstantiator.InstantiateAction(typeof(XslReportAction));
				action.XslFileName = XslFileName;
				return new INamedAction[] { new ImmutableNamedAction(actionName, action) } ;
			}
		}
	}
}
