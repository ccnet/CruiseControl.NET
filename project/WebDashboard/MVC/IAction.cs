namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public interface IAction
	{
		IResponse Execute(IRequest request);
	}
}