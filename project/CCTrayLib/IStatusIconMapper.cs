using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib
{
	public interface IStatusIconMapper
	{
		StatusIcon this [string status] { set; }

		StatusIcon this [ProjectStatus status] { get; }

	}
}