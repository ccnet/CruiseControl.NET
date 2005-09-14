using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	[XmlRoot(Namespace="", IsNullable=false, ElementName="Configuration")]
	public class PersistentConfiguration
	{
		public Project[] Projects = new Project[0];
		public int PollPeriodSeconds = 5;
		public BuildTransitionNotification BuildTransitionNotification = new BuildTransitionNotification();
		public TrayIconDoubleClickAction TrayIconDoubleClickAction = TrayIconDoubleClickAction.ShowStatusWindow;
	}

}