using System.Xml.Serialization;
using System.Windows.Forms;

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
        public ToolTipIcon MinimumNotificationLevel = ToolTipIcon.Info;

        [XmlElement(ElementName = "Exec")]
		public ExecCommands Exec = new ExecCommands();
	}
}
