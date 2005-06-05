using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{

	[XmlRoot( Namespace="", IsNullable=false, ElementName="Configuration" )]
	public class PersistentConfiguration
	{
		public Project[] Projects;
		public BuildTransitionNotification BuildTransitionNotification;
	}

	public class BuildTransitionNotification
	{
		[XmlAttribute(AttributeName="showBalloon")]
		public bool ShowBalloon = false;
	}
}