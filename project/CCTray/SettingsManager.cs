using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace tw.ccnet.remote.monitor
{
	public class SettingsManager
	{
		const string SettingsFileName = "cctray-settings.xml";

		private SettingsManager()
		{
		}

		static public string SettingsPathAndFileName
		{
			get
			{
				return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SettingsFileName);
			}
		}

		public static void WriteSettings(Settings settings)
		{
			TextWriter writer = null;
			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(Settings));
				writer = new StreamWriter(SettingsPathAndFileName);
				serializer.Serialize(writer, settings);
			}
			finally
			{
				if (writer!=null)
					writer.Close();
			}
		}

		public static Settings LoadSettings()
		{
			if (!File.Exists(SettingsPathAndFileName))
			{
				Settings defaults = Settings.CreateDefaultSettings();
				WriteSettings(defaults);
				return defaults;
			}

			// file exists, so deserialise it
			TextReader reader = null;
			try 
			{
				XmlSerializer serializer = new XmlSerializer(typeof(Settings));
				reader = new StreamReader(SettingsPathAndFileName);
				return (Settings)serializer.Deserialize(reader);
			}
			finally
			{
				if (reader!=null)
					reader.Close();
			}
		}
	}
}
