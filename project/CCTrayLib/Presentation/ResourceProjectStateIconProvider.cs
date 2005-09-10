using System.Collections;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class ResourceProjectStateIconProvider : IProjectStateIconProvider
	{
		public static readonly StatusIcon YELLOW = new StatusIcon("ThoughtWorks.CruiseControl.CCTrayLib.Yellow.ico");
		public static readonly StatusIcon GRAY = new StatusIcon("ThoughtWorks.CruiseControl.CCTrayLib.Gray.ico");
		public static readonly StatusIcon GREEN = new StatusIcon("ThoughtWorks.CruiseControl.CCTrayLib.Green.ico");
		public static readonly StatusIcon RED = new StatusIcon("ThoughtWorks.CruiseControl.CCTrayLib.Red.ico");

		private static readonly Hashtable map = new Hashtable();

		static ResourceProjectStateIconProvider()
		{
			map.Add(ProjectState.Broken, RED);
			map.Add(ProjectState.Building, YELLOW);
			map.Add(ProjectState.Success, GREEN);
			map.Add(ProjectState.NotConnected, GRAY);
		}

		public StatusIcon GetStatusIconForState(ProjectState state)
		{
			return (StatusIcon) map[state];
		}
	}
}