using System;
using System.Diagnostics;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	/// <summary>
	/// Implements the code to support the tray icon
	/// </summary>
	/// <remarks>
	/// Not entirely convinced this should derive from NotifyIconEx,
	/// should probably reuse by containment instead.
	/// </remarks>
	public class TrayIcon : NotifyIconEx
	{
		IIconProvider iconProvider;
		IProjectMonitor monitor;

		public void BindToIconProvider( IIconProvider iconProvider )
		{
			this.iconProvider = iconProvider;
			Icon = iconProvider.Icon;
			iconProvider.IconChanged += new EventHandler(IconProvider_IconChanged);
		}

		private void IconProvider_IconChanged(object sender, EventArgs e)
		{
			Icon = iconProvider.Icon;
		}

		public void ListenToBuildOccurredEvents( IProjectMonitor monitor, bool showBalloonMessages)
		{
			this.monitor = monitor;
			monitor.Polled  += new MonitorPolledEventHandler(Monitor_Polled);
			
			if (showBalloonMessages)
				monitor.BuildOccurred += new MonitorBuildOccurredEventHandler(Monitor_BuildOccurred);
		}

		private void Monitor_BuildOccurred( object sender, MonitorBuildOccurredEventArgs e )
		{
			string projectName = e.ProjectMonitor.ProjectName;

			string caption = string.Format("{0}: {1}",
				projectName, e.BuildTransition.Caption);

			ShowBalloon(caption, e.BuildTransition.Message,
				e.BuildTransition.ErrorLevel.NotifyInfo, 5000);
		}

		private void Monitor_Polled(object sender, MonitorPolledEventArgs args)
		{
			Text = monitor.SummaryStatusString;
			Debug.WriteLine("Tray message: " + Text);
		}
	}
}