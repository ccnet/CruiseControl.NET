using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public interface IProjectStateIconProvider
	{
		StatusIcon GetStatusIconForState( ProjectState state );
	}
}