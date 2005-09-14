using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public interface IStatusIconProvider
	{
		StatusIcon GetStatusIconForState( ProjectState state );
	}
}