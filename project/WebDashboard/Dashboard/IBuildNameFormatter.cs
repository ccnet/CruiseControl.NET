
namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IBuildNameFormatter
	{
		string GetPrettyBuildName(string originalBuildName);
	}
}
