
namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class DefaultCruiseAction : IAction
	{
		public IView Execute(IRequest request)
		{
			return new StringView("To Do - Default Cruise Action");
		}
	}
}
