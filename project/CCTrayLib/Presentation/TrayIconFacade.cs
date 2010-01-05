using System;
using System.Diagnostics;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	/// <summary>
	/// Implements the code to support the tray icon
	/// </summary>
	public class TrayIconFacade 
	{
        private readonly NotifyIcon trayIcon;
		private IIconProvider iconProvider;
		private IProjectMonitor monitor;
		private IBalloonMessageProvider balloonMessageProvider;
        private ToolTipIcon minimumNotificationLevel;

        public TrayIconFacade(NotifyIcon icon)
        {
            this.trayIcon = icon;
        }
		
		public IIconProvider IconProvider
		{
			set
			{
				iconProvider = value;
				iconProvider.IconChanged += IconProvider_IconChanged;
				IconProvider_IconChanged(null, null);
			}
		}

		public IBalloonMessageProvider BalloonMessageProvider
		{
			set { balloonMessageProvider = value; }
		}

        public void BindToProjectMonitor(IProjectMonitor monitor, bool showBalloonMessages, ToolTipIcon minimumNotificationLevel)
		{
			this.monitor = monitor;
			monitor.Polled += Monitor_Polled;
            this.minimumNotificationLevel = minimumNotificationLevel;

			if (showBalloonMessages)
			{
				monitor.BuildOccurred += Monitor_BuildOccurred;
				monitor.MessageReceived += Monitor_MessageReceived;
			}
		}

        private void Monitor_MessageReceived(string projectName, ThoughtWorks.CruiseControl.Remote.Message message)
		{
            trayIcon.ShowBalloonTip(5000, projectName, message.ToString(), ToolTipIcon.Info);
		}

		private void IconProvider_IconChanged(object sender, EventArgs e)
		{
            trayIcon.Icon = iconProvider.Icon;
		}

        private void Monitor_BuildOccurred(object sender, MonitorBuildOccurredEventArgs e)
        {
            if (e.BuildTransition.ErrorLevel.NotifyInfo < minimumNotificationLevel) 
                return;

            string projectName = e.ProjectMonitor.Detail.ProjectName;

            CaptionAndMessage captionAndMessage = balloonMessageProvider.GetCaptionAndMessageForBuildTransition(e.BuildTransition);
            string caption = string.Format("{0}: {1} ",
                                           projectName, captionAndMessage.Caption);

            trayIcon.ShowBalloonTip(
                5000, 
                caption, 
                captionAndMessage.Message,
                e.BuildTransition.ErrorLevel.NotifyInfo);
        }

		private void Monitor_Polled(object sender, MonitorPolledEventArgs args)
		{
			trayIcon.Text = monitor.SummaryStatusString;
            Debug.WriteLine("Tray message: " + trayIcon.Text);
		}
	}
}
