using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTray
{
	public class FileIconStore : IIconStore
	{
		private IStatusIconMapper _mapper = new HashStatusIconMapper();
		public FileIconStore(StatusIcons icons)
		{
			_mapper[IntegrationStatus.Failure.ToString()] = StatusIcon.LoadFromFile(icons.BuildFailed);
			_mapper[IntegrationStatus.Success.ToString()] = StatusIcon.LoadFromFile(icons.BuildSuccessful);
			_mapper[IntegrationStatus.Unknown.ToString()] = StatusIcon.LoadFromFile(icons.Unknown);
			_mapper[IntegrationStatus.Exception.ToString()] = StatusIcon.LoadFromFile(icons.Error);
			_mapper[ProjectActivity.Building.ToString()] = StatusIcon.LoadFromFile(icons.NowBuilding);
		}

	    public StatusIcon this[ProjectStatus status]
		{
			get
			{
				return _mapper[status];
			}
		}
	}
}
