using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTray
{
	public class ResourceIconStore : IIconStore
	{
		private IStatusIconMapper _iconStore= new HashStatusIconMapper();
		public static readonly StatusIcon NOW_BUILDING = new StatusIcon("ThoughtWorks.CruiseControl.CCTray.Yellow.ico");
		public static readonly  StatusIcon EXCEPTION = new StatusIcon("ThoughtWorks.CruiseControl.CCTray.Gray.ico");
		public static readonly  StatusIcon SUCCESS = new StatusIcon("ThoughtWorks.CruiseControl.CCTray.Green.ico");
		public static readonly StatusIcon FAILURE = new StatusIcon("ThoughtWorks.CruiseControl.CCTray.Red.ico");
		public static readonly StatusIcon UNKNOWN = new StatusIcon("ThoughtWorks.CruiseControl.CCTray.Gray.ico");

		public ResourceIconStore()
		{
			_iconStore[IntegrationStatus.Failure.ToString()] = FAILURE;
			_iconStore[IntegrationStatus.Success.ToString()] = SUCCESS;
			_iconStore[IntegrationStatus.Unknown.ToString()] = UNKNOWN;
			_iconStore[IntegrationStatus.Exception.ToString()] = EXCEPTION;
			_iconStore[ProjectActivity.Building.ToString()] = NOW_BUILDING;

		}

	    public StatusIcon this[ProjectStatus status]
	    {
	        get
	        {
				return _iconStore[status];
	        }
	    }

	}
}
