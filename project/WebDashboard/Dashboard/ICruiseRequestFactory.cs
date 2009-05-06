using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface ICruiseRequestFactory
	{
        ICruiseRequest CreateCruiseRequest(IRequest request, 
            ICruiseUrlBuilder urlBuilder,
            ISessionRetriever retriever);
	}
}
