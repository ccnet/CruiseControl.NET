using System;
using System.Collections;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTray
{
	public interface IStatusIconMapper
	{
		StatusIcon this[string status]
		{
			set;
		}

		StatusIcon this[ProjectStatus status]
		{
			get;
		}

	}
}
