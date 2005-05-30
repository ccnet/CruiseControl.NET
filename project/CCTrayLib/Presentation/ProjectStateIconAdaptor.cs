using System;
using System.Drawing;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class ProjectStateIconAdaptor : IIconProvider
	{
		private IProjectMonitor monitor;
		private IProjectStateIconProvider iconProvider;

		private StatusIcon currentIcon;

		public ProjectStateIconAdaptor( IProjectMonitor monitor, IProjectStateIconProvider iconProvider )
		{
			this.monitor = monitor;
			this.iconProvider = iconProvider;

			UpdateIcon();

			monitor.Polled += new MonitorPolledEventHandler( Monitor_Polled );
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
				StatusIcon oldValue = currentIcon;
				currentIcon = value;

				if (oldValue != currentIcon)
					OnIconChanged(EventArgs.Empty);
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

		private void OnIconChanged( EventArgs args )
		{
			if (IconChanged != null)
				IconChanged(this, args);
		}
	}
}