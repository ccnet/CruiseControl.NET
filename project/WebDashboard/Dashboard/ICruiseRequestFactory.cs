using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface ICruiseRequestFactory
	{
		ICruiseRequest CreateCruiseRequest(IRequest request);
	}
}
