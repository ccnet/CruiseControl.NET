using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using System.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IBuildRetriever
	{
        Build GetBuild(IBuildSpecifier buildSpecifier, string sessionToken);
        void GetFile(IBuildSpecifier buildSpecifier, string sessionToken, string fileName, Stream outputStream);
	}
}
