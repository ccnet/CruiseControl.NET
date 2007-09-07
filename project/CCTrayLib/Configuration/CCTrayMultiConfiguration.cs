using System;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public class CCTrayMultiConfiguration : ICCTrayMultiConfiguration
	{
		private PersistentConfiguration persistentConfiguration;
		private ICruiseServerManagerFactory cruiseServerManagerFactory;
		private ICruiseProjectManagerFactory cruiseProjectManagerFactory;
		private readonly string configFileName;

		public CCTrayMultiConfiguration(ICruiseServerManagerFactory cruiseServerManagerFactory, 
			ICruiseProjectManagerFactory cruiseProjectManagerFactory, string configFileName)
		{
			this.cruiseServerManagerFactory = cruiseServerManagerFactory;
			this.cruiseProjectManagerFactory = cruiseProjectManagerFactory;
			this.configFileName = configFileName;

			ReadConfigurationFile();
		}

        public IProjectMonitor[] GetProjectStatusMonitors(ISingleServerMonitor[] serverMonitors)
		{
			int indexRetval = 0;
			ArrayList indexList = new ArrayList();
			
			for(int i = 0; i < Projects.Length; i++)
			{
				if (Projects[i].ShowProject) indexList.Add(i);
			}
			
			IProjectMonitor[] retVal = new IProjectMonitor[indexList.Count];
			
			foreach(int i in indexList)
			{
				if (Projects[i].ShowProject)
				{
					ICruiseProjectManager projectManager = cruiseProjectManagerFactory.Create(Projects[i]);
					IProjectStatusRetriever projectStatusRetriever = GetServerMonitorForProject(Projects[i], serverMonitors);
					retVal[indexRetval++] = new ProjectMonitor(projectManager, projectStatusRetriever);
				}
			}
			
			return retVal;
		}

        private IProjectStatusRetriever GetServerMonitorForProject(CCTrayProject project, ISingleServerMonitor[] serverMonitors)
	    {
            foreach (ISingleServerMonitor serverMonitor in serverMonitors)
            {
                if (serverMonitor.ServerUrl == project.ServerUrl)
                {
                    return serverMonitor;
                }
            }
	        throw new ApplicationException("Server monitor not found for project: " + project.ProjectName);
	    }

	    public ISingleServerMonitor[] GetServerMonitors()
		{
			BuildServer[] buildServers = GetUniqueBuildServerList();
			ISingleServerMonitor[] serverMonitors = new ISingleServerMonitor[buildServers.Length];
			for (int i = 0; i < buildServers.Length; i++)
			{
				BuildServer buildServer = buildServers[i];
				ICruiseServerManager serverManager = cruiseServerManagerFactory.Create(buildServer);
				serverMonitors[i] = new ServerMonitor(serverManager);
			}
			return serverMonitors;
		}

		/// <summary>
		/// Construct a unique list of the build servers based on the projects configured in CCTray.
		/// </summary>
		/// <returns></returns>
		public BuildServer[] GetUniqueBuildServerList()
		{
			ArrayList buildServerList = new ArrayList();
			for (int i = 0; i < Projects.Length; i++)
			{
				BuildServer buildServer = Projects[i].BuildServer;
				if (!buildServerList.Contains(buildServer))
				{
					buildServerList.Add(buildServer);
				}
			}
			return (BuildServer[])buildServerList.ToArray(typeof(BuildServer));
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
			return new CCTrayMultiConfiguration(cruiseServerManagerFactory, cruiseProjectManagerFactory, configFileName);
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

		public ICruiseServerManagerFactory CruiseServerManagerFactory
		{
			get { return cruiseServerManagerFactory; }
		}

		public ICruiseProjectManagerFactory CruiseProjectManagerFactory
		{
			get { return cruiseProjectManagerFactory; }
		}
	}
}
