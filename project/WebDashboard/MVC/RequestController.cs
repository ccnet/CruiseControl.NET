namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public class RequestController
	{
		private readonly IRequest request;
		private readonly IActionFactory actionFactory;

		public RequestController(IActionFactory actionFactory, IRequest request)
		{
			this.actionFactory = actionFactory;
			this.request = request;
		}

		public IView Do()
		{
			IAction action = actionFactory.Create(request);
			request.ActionArguments = actionFactory.ActionArguments(request);
			return action.Execute(request);
		}
	}
}
