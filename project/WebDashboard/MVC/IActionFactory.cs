namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public interface IActionFactory
	{
		IAction Create(IRequest request);
		string[] ActionArguments(IRequest request);
	}
}
