using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class ActionSpecifierWithName : IActionSpecifier
	{
		private readonly string actionName;

		public string ActionName
		{
			get { return actionName; }
		}

		public ActionSpecifierWithName(string actionName)
		{
			this.actionName = actionName;
		}

		public string ToPartialQueryString()
		{
			return string.Format("{0}{1}=true", CruiseActionFactory.ACTION_PARAMETER_PREFIX, actionName);
		}
	}
}
