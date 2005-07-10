using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public class Project
	{
		public Project()
		{
		}

		public Project(string serverUrl, string projectName)
		{
			ServerUrl = serverUrl;
			ProjectName = projectName;
		}

		[XmlAttribute( AttributeName="serverUrl" )]
		public string ServerUrl;

		[XmlAttribute( AttributeName="projectName"  )]
		public string ProjectName;
	}
}