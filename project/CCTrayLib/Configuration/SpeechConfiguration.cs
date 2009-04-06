using System;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public class SpeechConfiguration
	{
		public bool Enabled = false;
		public bool SpeakBuildStarted = true;
		public bool SpeakBuildResults = true;
	}
}
