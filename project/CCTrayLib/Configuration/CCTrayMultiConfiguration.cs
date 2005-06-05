using System.IO;
using System.Xml.Serialization;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public class CCTrayMultiConfiguration : ICCTrayMultiConfiguration
	{
		private PersistentConfiguration persistentConfiguration;
		private ICruiseProjectManagerFactory managerFactory;

		public CCTrayMultiConfiguration( ICruiseProjectManagerFactory managerFactory, string configFileName )
		{
			this.managerFactory = managerFactory;

			ReadConfigurationFile( configFileName );
		}

		private void ReadConfigurationFile( string configFileName )
		{
			XmlSerializer serializer = new XmlSerializer( typeof (PersistentConfiguration) );

			using (StreamReader configFile = File.OpenText( configFileName ))
				persistentConfiguration = (PersistentConfiguration) serializer.Deserialize( configFile );
		}

		public IProjectMonitor[] GetProjectStatusMonitors()
		{
			IProjectMonitor[] retVal = new IProjectMonitor[Projects.Length];

			for (int i = 0; i < Projects.Length; i++)
			{
				Project project = Projects[ i ];
				ICruiseProjectManager projectManager = managerFactory.Create( project.ServerUrl, project.ProjectName );
				retVal[ i ] = new ProjectMonitor( projectManager );
			}

			return retVal;
		}

		public Project[] Projects
		{
			get { return persistentConfiguration.Projects; }
		}

		public bool ShouldShowBalloonOnBuildTransition
		{
			get
			{
				if (persistentConfiguration.BuildTransitionNotification == null)
					return true;

				return persistentConfiguration.BuildTransitionNotification.ShowBalloon;
			}
		}
	}

}