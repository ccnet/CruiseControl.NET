using System;
using System.Collections;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTray
{
	/// <summary>
	/// Summary description for HashStatusIconMapper.
	/// </summary>
	public class HashStatusIconMapper : IStatusIconMapper
	{
	    Hashtable _store = new Hashtable();
		public HashStatusIconMapper()
		{
		}

		public StatusIcon this[string status]
		{
			set
			{
				_store[status] = value;
			}
		}

		public StatusIcon this[ThoughtWorks.CruiseControl.Remote.ProjectStatus status]
		{
			get
			{
				string statusKey = status.BuildStatus.ToString();
			    ProjectActivity activity = status.Activity;
				if(ProjectActivity.Building == activity)
				{
					statusKey = activity.ToString();
				}
				if (!_store.ContainsKey(statusKey))
					throw new Exception("Unsupported Status: " + statusKey);
				return (StatusIcon)_store[statusKey];

			}
		}

	}
}
