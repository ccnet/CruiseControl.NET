namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class SimpleErrorViewBuilder : IErrorViewBuilder
	{
		// ToDo - Something nicer here - probably use a Velocity Template. We shouldn't use the site template since it might have been that that went screwy
		public IView BuildView(string errorMessage)
		{
			return new StringView(string.Format(
				@"<html><head><title>CruiseControl.NET</title></head><body><h1>An error has occurred in CruiseControl.NET</h1><p>{0}</p></body></html>",
				errorMessage));
		}
	}
}
