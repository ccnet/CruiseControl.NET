
namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class DefaultCruiseAction : IAction
	{
		public IView Execute(IRequest request)
		{
			return new HtmlView("To Do - Default Cruise Action");
		}
	}
}
