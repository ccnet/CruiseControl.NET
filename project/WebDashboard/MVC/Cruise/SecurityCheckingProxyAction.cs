using System.Web.UI;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.Config;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class SecurityCheckingProxyAction : IAction
	{
		private readonly IConfigurationGetter configurationGetter;
		private readonly IAction proxiedAction;

		public SecurityCheckingProxyAction(IAction proxiedAction, IConfigurationGetter configurationGetter)
		{
			this.proxiedAction = proxiedAction;
			this.configurationGetter = configurationGetter;
		}

		public Control Execute(IRequest request)
		{
			string configValue = configurationGetter.GetSimpleConfigSetting("AllowSecureActions");
			if (configValue != null && configValue == "true")
			{
				return proxiedAction.Execute(request);
			}
			else
			{
				HtmlGenericControl control = new HtmlGenericControl("p");
				control.InnerHtml = "Secure actions are not allowed - set 'AllowSecureActions' to 'true' in configuration to allow them.<br/>Requested action was of type [" + proxiedAction.GetType().ToString() + "]";
				return control;
			}
		}
	}
}
