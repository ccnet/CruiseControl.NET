using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public interface IDetailStringProvider
	{
		string FormatDetailString(IProjectMonitor monitor);
	}
}