using System.Web.UI;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IBuildLogTransformer
	{
		Control Transform(IBuildSpecifier build, params string[] transformerFileNames);
	}
}
