using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public class Project
	{
		[XmlAttribute( AttributeName="serverUrl" )]
		public string ServerUrl;

		[XmlAttribute( AttributeName="projectName"  )]
		public string ProjectName;
	}
}