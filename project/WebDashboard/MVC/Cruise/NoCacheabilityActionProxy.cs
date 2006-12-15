using System.Web;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class NoCacheabilityActionProxy : IAction
	{
		private readonly IAction proxiedAction;

		public NoCacheabilityActionProxy(IAction proxiedAction)
		{
			this.proxiedAction = proxiedAction;
		}

		public IResponse Execute(IRequest request)
		{
			HttpContext.Current.Response.Cache.SetNoStore();
			return proxiedAction.Execute(request);
		}
	}
}