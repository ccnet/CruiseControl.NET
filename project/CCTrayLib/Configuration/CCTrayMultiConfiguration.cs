using System.IO;
using System.Xml.Serialization;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public class CCTrayMultiConfiguration : ICCTrayMultiConfiguration
	{
		private PersistentConfiguration persistentConfiguration;
		private ICruiseProjectManagerFactory managerFactory;
		private readonly string configFileName;

		public CCTrayMultiConfiguration(ICruiseProjectManagerFactory managerFactory, string configFileName)
		{
			this.managerFactory = managerFactory;
			this.configFileName = configFileName;

			ReadConfigurationFile();
		}

		public IProjectMonitor[] GetProjectStatusMonitors()
		{
			IProjectMonitor[] retVal = new IProjectMonitor[Projects.Length];
			for (int i = 0; i < Projects.Length; i++)
			{
				Project project = Projects[i];
				ICruiseProjectManager projectManager = managerFactory.Create(project.ServerUrl, project.ProjectName);
				retVal[i] = new ProjectMonitor(projectManager);
			}
			return retVal;
		}

		public Project[] Projects
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
			return new CCTrayMultiConfiguration(managerFactory, configFileName);
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
	}
}