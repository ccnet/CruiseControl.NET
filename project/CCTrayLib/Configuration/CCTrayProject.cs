using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	[XmlType("Project")]
	public class CCTrayProject
	{
		private BuildServer buildServer;
		private string projectName;
		private bool showProject;

		public CCTrayProject()
		{
			buildServer = new BuildServer();
			showProject = true;
		}

		public CCTrayProject(string serverUrl, string projectName)
		{
			ServerUrl = serverUrl;
			ProjectName = projectName;
			showProject = true;
		}

		public CCTrayProject(BuildServer buildServer, string projectName)
		{
			this.buildServer = buildServer;
			this.projectName = projectName;
			showProject = true;
		}

		[XmlAttribute(AttributeName="serverUrl")]
		public string ServerUrl
		{
			get { return buildServer.Url; }
			set { buildServer = new BuildServer(value); }
		}

		[XmlAttribute(AttributeName="projectName")]
		public string ProjectName
		{
			get { return projectName; }
			set { projectName = value; }
		}
		
		[XmlAttribute(AttributeName = "showProject")]
		public bool ShowProject
		{
			get { return showProject; }
			set { showProject = value; }
		}
		
		[XmlIgnore]
		public BuildServer BuildServer
		{
			get { return buildServer; }
			set { buildServer = value; }
		}
	}
}
