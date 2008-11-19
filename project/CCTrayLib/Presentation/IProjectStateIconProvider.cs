using System;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public interface IProjectStateIconProvider : IDisposable
	{
		StatusIcon GetStatusIconForState( ProjectState state );
	}
}