using System.Web.UI;

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

		public void Do(Control parentControl)
		{
			parentControl.Controls.Add(actionFactory.Create(request).Execute(request).Control);
		}
	}
}
