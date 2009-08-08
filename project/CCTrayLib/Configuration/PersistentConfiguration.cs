using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	[XmlRoot(Namespace="", IsNullable=false, ElementName="Configuration")]
	public class PersistentConfiguration
	{
		public CCTrayProject[] Projects = new CCTrayProject[0];
		public int PollPeriodSeconds = 5;
		public BuildTransitionNotification BuildTransitionNotification = new BuildTransitionNotification();
		public TrayIconDoubleClickAction TrayIconDoubleClickAction = TrayIconDoubleClickAction.ShowStatusWindow;
		public Icons Icons = new Icons();
		public X10Configuration X10 = new X10Configuration();
		public SpeechConfiguration Speech = new SpeechConfiguration();
		public GrowlConfiguration Growl = new GrowlConfiguration();
        public bool AlwaysOnTop = false;
		public bool ShowInTaskbar = false;
        public bool ReportProjectChanges = true;
        public string FixUserName = string.Empty;
	}
}
