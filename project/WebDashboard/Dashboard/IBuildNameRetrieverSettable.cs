using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IBuildNameRetrieverSettable
	{
		IBuildNameRetriever BuildNameRetriever { set; }
	}
}
