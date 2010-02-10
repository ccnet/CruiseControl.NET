using System.Collections;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IBuildLogTransformer
	{
        string Transform(IBuildSpecifier build, string[] transformerFileNames, Hashtable xsltArgs, string sessionToken, string[] taskTypes);
	}
}