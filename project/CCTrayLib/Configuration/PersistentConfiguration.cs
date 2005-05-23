using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{

	[XmlRoot( Namespace="", IsNullable=false, ElementName="Configuration" )]
	public class PersistentConfiguration
	{
		public Project[] Projects;
	}
}