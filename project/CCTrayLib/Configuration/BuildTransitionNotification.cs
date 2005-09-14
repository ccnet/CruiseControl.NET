using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public class BuildTransitionNotification
	{
		[XmlAttribute(AttributeName="showBalloon")] public bool ShowBalloon = true;

		[XmlElement(ElementName = "Sound")] public AudioFiles AudioFiles = new AudioFiles();
	}
}