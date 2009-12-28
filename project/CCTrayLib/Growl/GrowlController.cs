using System;
using System.Collections.Generic;
using System.Text;
using Growl.Connector;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using Growl.CoreLibrary;
using System.Drawing;
using System.Collections.Specialized;
using System.Collections;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Growl
{
	public class GrowlController
	{
		private const string ApplicationName = "CCTray";
		private GrowlConnector growl;
		private GrowlConfiguration configuration;
		private IProjectMonitor monitor;
		private GrowlMessages growlMessages = new GrowlMessages();

		public GrowlController(IProjectMonitor monitor, GrowlConfiguration configuration)
		{
			this.configuration = configuration;
			this.monitor = monitor;
			this.InitializeLibrary();
			this.InitializeGrowlInstance();
		}

		private void InitializeGrowlInstance()
		{
			if (this.configuration.Enabled)
			{
				if (!string.IsNullOrEmpty(this.configuration.Hostname))
				{
					growl = new GrowlConnector(this.configuration.Password, this.configuration.Hostname, this.configuration.Port);
				}
				else if (!string.IsNullOrEmpty(this.configuration.Password))
				{
					growl = new GrowlConnector(this.configuration.Password);
				}
				else
				{
					growl = new GrowlConnector();
				}

				this.RegisterApplication();

				monitor.Polled += Monitor_Polled;

				monitor.BuildOccurred += Monitor_BuildOccurred;
				monitor.MessageReceived += Monitor_MessageReceived;

			}
			
		}

		private string lastStatus;
		private void Monitor_Polled(object sender, MonitorPolledEventArgs args)
		{
			if (lastStatus != monitor.SummaryStatusString)
			{
				lastStatus = monitor.SummaryStatusString;
				Notification notification = new Notification(ApplicationName, "Message", null, "CCTray", monitor.SummaryStatusString);
				growl.Notify(notification);
			}
		}

		private void Monitor_MessageReceived(string projectName, Message message)
		{
			Notification notification = new Notification(ApplicationName, "Message", null, projectName, message.ToString());
			growl.Notify(notification);
		}

		private void Monitor_BuildOccurred(object sender, MonitorBuildOccurredEventArgs e)
		{
			if (e.BuildTransition.ErrorLevel.NotifyInfo < this.configuration.MinimumNotificationLevel)
			    return;

			string projectName = e.ProjectMonitor.Detail.ProjectName;

			
			CaptionAndMessage captionAndMessage = GetCaptionAndMessageForBuildTransition(e.BuildTransition);
			string caption = string.Format("{0}: {1} ",
			                               projectName, captionAndMessage.Caption);

			Notification notification = new Notification(ApplicationName, captionAndMessage.Caption, null, projectName, captionAndMessage.Message);
			growl.Notify(notification);
		}

		private readonly IDictionary messages = new HybridDictionary();

		private void InitializeLibrary()
		{
			if (growlMessages != null)
			{
				messages.Add(BuildTransition.Broken, growlMessages.BrokenBuildMessage);
				messages.Add(BuildTransition.Fixed, growlMessages.FixedBuildMessage);
				messages.Add(BuildTransition.StillFailing, growlMessages.StillFailingBuildMessage);
				messages.Add(BuildTransition.StillSuccessful, growlMessages.StillSuccessfulBuildMessage);
			}
		}

		

		private CaptionAndMessage GetCaptionAndMessageForBuildTransition(BuildTransition buildTransition)
		{
			return (CaptionAndMessage) messages[buildTransition];
		}

		private void RegisterApplication()
		{
			List<NotificationType> notificationTypes = new List<NotificationType>();

			notificationTypes.Add(new NotificationType(growlMessages.BrokenBuildMessage.Caption, 
				growlMessages.BrokenBuildMessage.Caption, DefaultProjectIcons.Red.ToBitmap(), true));
			notificationTypes.Add(new NotificationType(growlMessages.FixedBuildMessage.Caption, 
				growlMessages.FixedBuildMessage.Caption, DefaultProjectIcons.Green.ToBitmap(), true));
			notificationTypes.Add(new NotificationType(growlMessages.StillFailingBuildMessage.Caption, 
				growlMessages.StillFailingBuildMessage.Caption, DefaultProjectIcons.Red.ToBitmap(), true));
			notificationTypes.Add(new NotificationType(growlMessages.StillSuccessfulBuildMessage.Caption, 
				growlMessages.StillSuccessfulBuildMessage.Caption, DefaultProjectIcons.Green.ToBitmap(), true));
			notificationTypes.Add(new NotificationType("Message", "Message", DefaultProjectIcons.Orange.ToBitmap(), true));

			Application app = new Application(ApplicationName);

			app.Icon = DefaultProjectIcons.Green.ToBitmap();

			growl.Register(app, notificationTypes.ToArray());
		}
	}
}
