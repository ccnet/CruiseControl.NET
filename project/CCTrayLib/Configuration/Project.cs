using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public class Project
	{
		private BuildServer buildServer;
		private string projectName;

		public Project()
		{
			buildServer = new BuildServer();
		}

		public Project(string serverUrl, string projectName)
		{
			ServerUrl = serverUrl;
			ProjectName = projectName;
		}

		public Project(BuildServer buildServer, string projectName)
		{
			this.buildServer = buildServer;
			this.projectName = projectName;
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

		[XmlIgnore]
		public BuildServer BuildServer
		{
			get { return buildServer; }
			set { buildServer = value; }
		}
	}
}