using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.GenericPlugins
{
	[ReflectorType("configurablePlugin")]
	public class ConfigurablePlugin : IPlugin
	{
		private string description;
		private INamedAction[] namedActions;

		public string LinkActionName
		{
			get
			{
				return NamedActions[0].ActionName;
			}
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

		[ReflectorArray("actions", Required=true)]
		public INamedAction[] NamedActions
		{
			get
			{
				return namedActions;
			}
			set
			{
				namedActions = value;
			}
		}
	}
}
