using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
    public class CCTrayMultiConfiguration : ICCTrayMultiConfiguration
    {
        private PersistentConfiguration persistentConfiguration;
        private readonly ICruiseServerManagerFactory cruiseServerManagerFactory;
        private readonly ICruiseProjectManagerFactory cruiseProjectManagerFactory;
        private readonly string configFileName;
        private readonly IDictionary<BuildServer, ICruiseServerManager> serverManagersList;

        public CCTrayMultiConfiguration(ICruiseServerManagerFactory cruiseServerManagerFactory,
            ICruiseProjectManagerFactory cruiseProjectManagerFactory, string configFileName)
        {
            this.cruiseServerManagerFactory = cruiseServerManagerFactory;
            this.cruiseProjectManagerFactory = cruiseProjectManagerFactory;
            this.configFileName = configFileName;
            serverManagersList = new Dictionary<BuildServer, ICruiseServerManager>();

            ReadConfigurationFile(configFileName);
        }

        public IProjectMonitor[] GetProjectStatusMonitors(ISingleServerMonitor[] serverMonitors)
        {
            int indexRetval = 0;
            ArrayList indexList = new ArrayList();

            for (int i = 0; i < Projects.Length; i++)
            {
                if (Projects[i].ShowProject) indexList.Add(i);
            }

            IProjectMonitor[] retVal = new IProjectMonitor[indexList.Count];

            foreach (int i in indexList)
            {
                if (Projects[i].ShowProject)
                {
                    ICruiseProjectManager projectManager = cruiseProjectManagerFactory.Create(Projects[i], serverManagersList);
                    ISingleServerMonitor serverMonitor = GetServerMonitorForProject(Projects[i], serverMonitors);
                    retVal[indexRetval++] = new ProjectMonitor(Projects[i], projectManager, serverMonitor);
                }
            }

            return retVal;
        }

        private static ISingleServerMonitor GetServerMonitorForProject(CCTrayProject project, IEnumerable<ISingleServerMonitor> serverMonitors)
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
                serverManagersList[buildServer] = serverManager;
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

        public NotifyInfoFlags MinimumNotificationLevel
        {
            get { return persistentConfiguration.BuildTransitionNotification.MinimumNotificationLevel; }
            set { persistentConfiguration.BuildTransitionNotification.MinimumNotificationLevel = value; }
        }

        public bool AlwaysOnTop
        {
            get { return persistentConfiguration.AlwaysOnTop; }
            set { persistentConfiguration.AlwaysOnTop = value; }
        }

        public bool ShowInTaskbar
        {
            get { return persistentConfiguration.ShowInTaskbar; }
            set { persistentConfiguration.ShowInTaskbar = value; }
        }
        /// <summary>
        /// Report any changes to the projects.
        /// </summary>
        public bool ReportProjectChanges
        {
            get { return persistentConfiguration.ReportProjectChanges; }
            set { persistentConfiguration.ReportProjectChanges = value; }
        }

        public string FixUserName
        {
            get { return persistentConfiguration.FixUserName; }
            set { persistentConfiguration.FixUserName = value; }
        }

        public int PollPeriodSeconds
        {
            get { return persistentConfiguration.PollPeriodSeconds; }
            set { persistentConfiguration.PollPeriodSeconds = value; }
        }

        public void Persist()
        {
            WriteConfigurationFile(configFileName);
        }
        /// <summary>
        /// Load the settings from a different location.
        /// </summary>
        /// <param name="settingsFile"></param>
        public virtual void Load(string settingsFile)
        {
            ReadConfigurationFile(settingsFile);
        }

        /// <summary>
        /// Save the settings to a different location.
        /// </summary>
        /// <param name="settingsFile"></param>
        public virtual void Save(string settingsFile)
        {
            WriteConfigurationFile(settingsFile);
        }

        private void ReadConfigurationFile(string settingsFile)
        {
            if (!File.Exists(settingsFile))
            {
                persistentConfiguration = new PersistentConfiguration();
                return;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(PersistentConfiguration));

            using (StreamReader configFile = File.OpenText(settingsFile))
                persistentConfiguration = (PersistentConfiguration)serializer.Deserialize(configFile);
        }

        private void WriteConfigurationFile(string settingsFile)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PersistentConfiguration));

            using (StreamWriter configFile = File.CreateText(settingsFile))
                serializer.Serialize(configFile, persistentConfiguration);
        }

        public void Reload()
        {
            ReadConfigurationFile(configFileName);
        }

        public ICCTrayMultiConfiguration Clone()
        {
            return new CCTrayMultiConfiguration(cruiseServerManagerFactory, cruiseProjectManagerFactory, configFileName);
        }

        public AudioFiles Audio
        {
            get { return persistentConfiguration.BuildTransitionNotification.AudioFiles; }
        }


        public ExecCommands Execs
        {
            get { return persistentConfiguration.BuildTransitionNotification.Exec; }
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

        public GrowlConfiguration Growl
        {
            get { return persistentConfiguration.Growl; }
        }

        public ICruiseServerManagerFactory CruiseServerManagerFactory
        {
            get { return cruiseServerManagerFactory; }
        }

        public ICruiseProjectManagerFactory CruiseProjectManagerFactory
        {
            get { return cruiseProjectManagerFactory; }
        }
        public SpeechConfiguration Speech
        {
            get { return persistentConfiguration.Speech; }
        }
    }
}
