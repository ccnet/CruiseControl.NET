using System.Web;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.ActionDecorators;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public interface IResponse
	{
		void Process(HttpResponse response);
        // TODO: Getter only needed for testing
        ConditionalGetFingerprint ServerFingerprint { get; set; }
	}
}