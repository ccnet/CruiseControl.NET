using System.Web.UI;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IBuildLogTransformer
	{
		string Transform(IBuildSpecifier build, params string[] transformerFileNames);
	}
}
