using System.Web.UI;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public class RequestController
	{
		private readonly IActionFactory actionFactory;

		public RequestController(IActionFactory actionFactory)
		{
			this.actionFactory = actionFactory;
		}

		public void Do(Control parentControl, IRequest request)
		{
			parentControl.Controls.Add(actionFactory.Create(request).Execute(request));
		}
	}
}
