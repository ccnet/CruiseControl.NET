using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public class BuildTransitionNotification
	{
		[XmlAttribute(AttributeName="showBalloon")]
		public bool ShowBalloon = true;

		[XmlElement(ElementName = "Sound")]
		public AudioFiles AudioFiles = new AudioFiles();

		[XmlElement(ElementName = "BalloonMessages")]
		public BalloonMessages BalloonMessages = new BalloonMessages();

        [XmlAttribute(AttributeName = "minimumNotificationLevel")]
        public NotifyInfoFlags MinimumNotificationLevel = NotifyInfoFlags.Info;

        [XmlElement(ElementName = "Exec")]
		public ExecCommands Exec = new ExecCommands();
	}
}
