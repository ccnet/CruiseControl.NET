using System;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.CCTray
{
	[Serializable]
	public struct Sound
	{
		[XmlAttribute()]
		public bool Play;
		[XmlAttribute()]
		public string FileName;

		public Sound(string fileName)
		{
			Play = false;
			FileName = fileName;
		}
	}
}
