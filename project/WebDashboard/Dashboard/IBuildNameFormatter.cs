
namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IBuildNameFormatter
	{
		string GetPrettyBuildName(IBuildSpecifier buildSpecifier);
		string GetCssClassForBuildLink(IBuildSpecifier buildSpecifier);
	}
}
