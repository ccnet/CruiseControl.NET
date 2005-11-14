namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class DefaultCruiseAction : IAction
	{
		public IResponse Execute(IRequest request)
		{
			return new HtmlFragmentResponse("To Do - Default Cruise Action");
		}
	}
}