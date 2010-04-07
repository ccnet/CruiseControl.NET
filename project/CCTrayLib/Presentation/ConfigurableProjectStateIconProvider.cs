using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class ConfigurableProjectStateIconProvider : IProjectStateIconProvider
	{
		private readonly IDictionary<ProjectState, StatusIcon> map = new Dictionary<ProjectState, StatusIcon>();

		public void Dispose()
		{
			foreach (StatusIcon icon in map.Values)
				icon.Dispose();

            map.Clear();
		}

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
		    if (string.IsNullOrEmpty(iconFilename))
		    {
		        map.Add(projectState, defaultIcon);
		        return;
		    }

		    try
		    {
		        StatusIcon icon = StatusIcon.LoadFromFile(iconFilename);
		        map.Add(projectState, icon);
		        return;
		    }
		    catch (Exception ex)
		    {
		        MessageBox.Show("Failed to load icon " + iconFilename + " for state " + projectState + ": " + ex);
		    }
		}

	    public StatusIcon GetStatusIconForState(ProjectState state)
		{
			return map[state];
		}
	}
}
