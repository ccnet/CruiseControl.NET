using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class NameValueCruiseRequestFactory : ICruiseRequestFactory
	{
		public ICruiseRequest CreateCruiseRequest (IRequest request)
		{
			return new QueryStringRequestWrapper(request.Params);
		}
	}
}
