namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public interface IErrorViewBuilder
	{
		IView BuildView(string errorMessage);
	}
}
