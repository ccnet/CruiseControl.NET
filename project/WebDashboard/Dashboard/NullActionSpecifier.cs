namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class NullActionSpecifier : IActionSpecifier
	{
		public string ActionName
		{
			get { return ""; }
		}

		public string ToPartialQueryString()
		{
			return "";
		}
	}
}
