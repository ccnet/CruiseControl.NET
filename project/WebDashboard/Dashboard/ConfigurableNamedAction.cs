using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	[ReflectorType("namedAction")]
	public class ConfigurableNamedAction : INamedAction
	{
		private string actionName;
		private ICruiseAction action;

		[ReflectorProperty("name")]
		public string ActionName
		{
			get { return actionName; }
			set { actionName = value; }
		}

		[ReflectorProperty("action", InstanceTypeKey="type")]
		public ICruiseAction Action
		{
			get { return action; }
			set { action = value; }
		}
	}
}
