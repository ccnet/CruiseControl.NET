using System;
using System.Collections;
using System.Drawing;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTray
{
	class DefaultStatusIconLoader : IStatusIconLoader
	{
		private Hashtable _iconStore;

	    public DefaultStatusIconLoader()
	    {
			_iconStore = new Hashtable();
			_iconStore[IntegrationStatus.Failure.ToString()] = StatusIcon.FAILURE;
			_iconStore[IntegrationStatus.Success.ToString()] = StatusIcon.SUCCESS;
			_iconStore[IntegrationStatus.Unknown.ToString()] = StatusIcon.UNKNOWN;
			_iconStore[IntegrationStatus.Exception.ToString()] = StatusIcon.EXCEPTION;
			_iconStore[ProjectActivity.Building.ToString()] = StatusIcon.NOW_BUILDING;
	    }

	    public StatusIcon LoadIcon(ProjectStatus status)
	    {
	        return GetStatusIcon(status.BuildStatus, status.Activity);
	    }
		private StatusIcon GetStatusIcon(IntegrationStatus status, ProjectActivity activity)
		{
			string statusKey = status.ToString();
			if(ProjectActivity.Building == activity)
			{
				statusKey = activity.ToString();
			}
			if (!_iconStore.ContainsKey(statusKey))
				throw new Exception("Unsupported Status: " + statusKey);
			return (StatusIcon)_iconStore[statusKey];
		}

	}
}
