using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTray
{
	/// <summary>
	/// Summary description for IStatusIcon.
	/// </summary>
	public interface IIconStore
	{
		StatusIcon this[ProjectStatus status]
		{
			get;
		}
	}
}
