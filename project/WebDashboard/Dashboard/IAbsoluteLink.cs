using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IAbsoluteLink
	{
		string Description { get; }
		string AbsoluteURL { get; }
	}
}
