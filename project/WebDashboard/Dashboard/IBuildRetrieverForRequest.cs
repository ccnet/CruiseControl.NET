using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IBuildRetrieverForRequest
	{
		Build GetBuild(IRequestWrapper requestWrapper);
	}
}
