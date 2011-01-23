using System.Collections;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class ResourceProjectStateIconProvider : IProjectStateIconProvider
	{
		public static readonly StatusIcon YELLOW = new StatusIcon(DefaultProjectIcons.Yellow, false);
		public static readonly StatusIcon GRAY = new StatusIcon(DefaultProjectIcons.Gray, false);
		public static readonly StatusIcon GREEN = new StatusIcon(DefaultProjectIcons.Green, false);
		public static readonly StatusIcon RED = new StatusIcon(DefaultProjectIcons.Red, false);
		public static readonly StatusIcon ORANGE = new StatusIcon(DefaultProjectIcons.Orange, false);

		private static readonly Hashtable map = new Hashtable();

		public void Dispose(){}

		static ResourceProjectStateIconProvider()
		{
			map.Add(ProjectState.Broken, RED);
			map.Add(ProjectState.Building, YELLOW);
			map.Add(ProjectState.BrokenAndBuilding, ORANGE);
			map.Add(ProjectState.Success, GREEN);
			map.Add(ProjectState.NotConnected, GRAY);
		}

		public StatusIcon GetStatusIconForState(ProjectState state)
		{
			return (StatusIcon) map[state];
		}
	}
}
