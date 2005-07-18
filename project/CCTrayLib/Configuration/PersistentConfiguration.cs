using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{

	[XmlRoot( Namespace="", IsNullable=false, ElementName="Configuration" )]
	public class PersistentConfiguration
	{
		public Project[] Projects = new Project[0];
		public int PollPeriodSeconds = 5;
		public BuildTransitionNotification BuildTransitionNotification = new BuildTransitionNotification();
	}

	public class BuildTransitionNotification
	{
		[XmlAttribute(AttributeName="showBalloon")]
		public bool ShowBalloon = true;

		[XmlElement(ElementName = "Sound")]
		public AudioFiles AudioFiles = new AudioFiles();
	}

	public class AudioFiles
	{
		public string BrokenBuildSound;
		public string FixedBuildSound;
		public string StillFailingBuildSound;
		public string StillSuccessfulBuildSound;		
	}
}