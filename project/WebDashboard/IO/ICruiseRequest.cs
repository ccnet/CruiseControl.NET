using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	public interface ICruiseRequest
	{
		string ServerName { get; }
		string ProjectName { get; }
		string BuildName { get; }
		IRequest Request { get; }
	}
}
