using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTray
{
	public interface IStatusIconLoader
	{
	    StatusIcon LoadIcon(ProjectStatus status);
	}
}
