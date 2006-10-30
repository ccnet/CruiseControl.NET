using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class ConfigurableProjectStateIconProvider : IProjectStateIconProvider
	{
		private readonly IDictionary map = new HybridDictionary();

		public ConfigurableProjectStateIconProvider(Icons icons)
		{
			LoadIcon(ProjectState.Broken, icons.BrokenIcon, ResourceProjectStateIconProvider.RED);
			LoadIcon(ProjectState.Building, icons.BuildingIcon, ResourceProjectStateIconProvider.YELLOW);
			LoadIcon(ProjectState.Success, icons.SuccessIcon, ResourceProjectStateIconProvider.GREEN);
			LoadIcon(ProjectState.NotConnected, icons.NotConnectedIcon, ResourceProjectStateIconProvider.GRAY);
			LoadIcon(ProjectState.BrokenAndBuilding, icons.BrokenAndBuildingIcon, ResourceProjectStateIconProvider.ORANGE);
		}

		private void LoadIcon(ProjectState projectState, string iconFilename, StatusIcon defaultIcon)
		{
			if (iconFilename != null && iconFilename.Length > 0)
			{
				try
				{
					StatusIcon icon = StatusIcon.LoadFromFile(iconFilename);
					
					Debug.WriteLine("Using custom icon " + iconFilename + " for state " + projectState);
					map.Add(projectState, icon);
					return;
				}
				catch (Exception ex)
				{
					MessageBox.Show("Failed to load icon " + iconFilename + " for state " + projectState + ": " + ex);
				}
			}
			
			Debug.WriteLine("Using default icon for state " + projectState);
			map.Add(projectState, defaultIcon);	
		}

		public StatusIcon GetStatusIconForState(ProjectState state)
		{
			return (StatusIcon) map[state];
		}
	}
}