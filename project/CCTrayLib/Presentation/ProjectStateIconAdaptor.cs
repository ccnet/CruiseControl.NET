using System;
using System.Drawing;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class ProjectStateIconAdaptor : IIconProvider
	{
		private readonly IProjectMonitor monitor;
		private readonly IProjectStateIconProvider iconProvider;

		private StatusIcon currentIcon;

		public ProjectStateIconAdaptor( IProjectMonitor monitor, IProjectStateIconProvider iconProvider )
		{
			this.monitor = monitor;
			this.iconProvider = iconProvider;

			UpdateIcon();

			monitor.Polled += Monitor_Polled;
		}

		public Icon Icon
		{
			get { return currentIcon.Icon; }
		}

		public StatusIcon StatusIcon
		{
			get { return currentIcon; }
			set
			{
                if (currentIcon != value)
                {
                    currentIcon = value;
                    OnIconChanged();
                }
			}
		}

		private void UpdateIcon()
		{
			StatusIcon = iconProvider.GetStatusIconForState( monitor.ProjectState );
		}

		private void Monitor_Polled( object sender, MonitorPolledEventArgs args )
		{
			UpdateIcon();
		}

		public event EventHandler IconChanged;

		private void OnIconChanged()
		{
			if (IconChanged != null)
                IconChanged(this, EventArgs.Empty);
		}
	}
}
