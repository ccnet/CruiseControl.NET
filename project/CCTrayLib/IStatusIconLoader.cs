using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib
{
	public interface IStatusIconLoader
	{
		StatusIcon LoadIcon (ProjectStatus status);
	}
}