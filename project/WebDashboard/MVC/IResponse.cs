using System.Web;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public interface IResponse
	{
		void Process(HttpResponse response);
	}
}