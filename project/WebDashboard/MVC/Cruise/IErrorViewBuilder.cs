namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public interface IErrorViewBuilder
	{
		IResponse BuildView(string errorMessage);
	}
}
