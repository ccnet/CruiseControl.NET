namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public interface IAction
	{
		IView Execute(IRequest request);
	}
}
