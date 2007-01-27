using System.IO;
using System.Xml.Serialization;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public class CCTrayMultiConfiguration : ICCTrayMultiConfiguration
	{
		private PersistentConfiguration persistentConfiguration;
		private ICruiseProjectManagerFactory cruiseProjectManagerFactory;
		private readonly string configFileName;

		public CCTrayMultiConfiguration(ICruiseProjectManagerFactory cruiseProjectManagerFactory, string configFileName)
		{
			this.cruiseProjectManagerFactory = cruiseProjectManagerFactory;
			this.configFileName = configFileName;

			ReadConfigurationFile();
		}

		public IProjectMonitor[] GetProjectStatusMonitors()
		{
			IProjectMonitor[] retVal = new IProjectMonitor[Projects.Length];
			for (int i = 0; i < Projects.Length; i++)
			{
				CCTrayProject project = Projects[i];
				ICruiseProjectManager projectManager = cruiseProjectManagerFactory.Create(project);
				retVal[i] = new ProjectMonitor(projectManager);
			}
			return retVal;
		}

		public CCTrayProject[] Projects
		{
			get { return persistentConfiguration.Projects; }
			set { persistentConfiguration.Projects = value; }
		}

		public bool ShouldShowBalloonOnBuildTransition
		{
			get { return persistentConfiguration.BuildTransitionNotification.ShowBalloon; }
			set { persistentConfiguration.BuildTransitionNotification.ShowBalloon = value; }
		}

		public int PollPeriodSeconds
		{
			get { return persistentConfiguration.PollPeriodSeconds; }
			set { persistentConfiguration.PollPeriodSeconds = value; }
		}

		public void Persist()
		{
			WriteConfigurationFile();
		}

		private void ReadConfigurationFile()
		{
			if (!File.Exists(configFileName))
			{
				persistentConfiguration = new PersistentConfiguration();
				return;
			}

			XmlSerializer serializer = new XmlSerializer(typeof (PersistentConfiguration));

			using (StreamReader configFile = File.OpenText(configFileName))
				persistentConfiguration = (PersistentConfiguration) serializer.Deserialize(configFile);
		}

		private void WriteConfigurationFile()
		{
			XmlSerializer serializer = new XmlSerializer(typeof (PersistentConfiguration));

			using (StreamWriter configFile = File.CreateText(configFileName))
				serializer.Serialize(configFile, persistentConfiguration);
		}

		public void Reload()
		{
			ReadConfigurationFile();
		}

		public ICCTrayMultiConfiguration Clone()
		{
			return new CCTrayMultiConfiguration(cruiseProjectManagerFactory, configFileName);
		}

		public AudioFiles Audio
		{
			get { return persistentConfiguration.BuildTransitionNotification.AudioFiles; }
		}

		public TrayIconDoubleClickAction TrayIconDoubleClickAction
		{
			get { return persistentConfiguration.TrayIconDoubleClickAction; }
			set { persistentConfiguration.TrayIconDoubleClickAction = value; }
		}

		public BalloonMessages BalloonMessages
		{
			get { return persistentConfiguration.BuildTransitionNotification.BalloonMessages; }
		}

		public Icons Icons
		{
			get { return persistentConfiguration.Icons; }
		}

		public X10Configuration X10
		{
			get { return persistentConfiguration.X10; }
		}

		public ICruiseProjectManagerFactory CruiseProjectManagerFactory
		{
			get { return cruiseProjectManagerFactory; }
		}
	}
}