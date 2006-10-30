using System;
using System.Diagnostics;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	/// <summary>
	/// Implements the code to support the tray icon
	/// </summary>
	public class TrayIcon : NotifyIconEx
	{
		private IIconProvider iconProvider;
		private IProjectMonitor monitor;
		private IBalloonMessageProvider balloonMessageProvider;
		
		public IIconProvider IconProvider
		{
			set
			{
				iconProvider = value;
				iconProvider.IconChanged += new EventHandler(IconProvider_IconChanged);
				IconProvider_IconChanged(null, null);
			}
		}

		public IBalloonMessageProvider BalloonMessageProvider
		{
			set { balloonMessageProvider = value; }
		}

		public void BindToProjectMonitor(IProjectMonitor monitor, bool showBalloonMessages)
		{
			this.monitor = monitor;
			monitor.Polled += new MonitorPolledEventHandler(Monitor_Polled);

			if (showBalloonMessages)
			{
				monitor.BuildOccurred += new MonitorBuildOccurredEventHandler(Monitor_BuildOccurred);
				monitor.MessageReceived += new MessageEventHandler(Monitor_MessageReceived);
			}
		}

		private void Monitor_MessageReceived(Message message)
		{
			ShowBalloon(message.ToString(), message.ToString(), NotifyInfoFlags.Info, 5000);
		}

		private void IconProvider_IconChanged(object sender, EventArgs e)
		{
			Icon = iconProvider.Icon;
		}

		private void Monitor_BuildOccurred(object sender, MonitorBuildOccurredEventArgs e)
		{
			string projectName = e.ProjectMonitor.Detail.ProjectName;

			CaptionAndMessage captionAndMessage = balloonMessageProvider.GetCaptionAndMessageForBuildTransition(e.BuildTransition);
			string caption = string.Format("{0}: {1}", projectName, captionAndMessage.Caption);

			ShowBalloon(caption, captionAndMessage.Message, e.BuildTransition.ErrorLevel.NotifyInfo, 5000);
		}

		private void Monitor_Polled(object sender, MonitorPolledEventArgs args)
		{
			Text = monitor.SummaryStatusString;
			Debug.WriteLine("Tray message: " + Text);
		}
	}
}