using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{	
	public class SvnHistoryParser : IHistoryParser 
	{
		public Modification[] Parse(TextReader svnLog, DateTime from, DateTime to)
		{
			String logText = "";
			String nextLine = svnLog.ReadLine();
			while(nextLine != null) 
			{
				logText = logText + nextLine + "\n";
				nextLine = svnLog.ReadLine();
			}
			XmlDocument log = new XmlDocument();
			log.LoadXml(logText);

			ArrayList mods = new ArrayList();

			XmlElement root = log.DocumentElement;
			XmlNodeList logEntries = root.SelectNodes("/log/logentry");
			foreach(XmlNode logEntry in logEntries) {
				mods.AddRange(ParseModificationsFromLogEntry(logEntry, from, to));
			}

			return (Modification[]) mods.ToArray(typeof(Modification));
		}

		internal ArrayList ParseModificationsFromLogEntry(XmlNode logEntry, DateTime from, DateTime to) {
			String revision = GetAttributeFromNode(logEntry, "revision");
			int changeNumber = int.Parse(revision);

			XmlNode dateNode = logEntry.SelectSingleNode("date");
			DateTime changeTime = ParseDate(dateNode.InnerText);
			if(changeTime < from || to < changeTime) {
				// Work around issue 1642 in Subversion
				return new ArrayList();
			}

			String author = "";
			XmlNode authorNode = logEntry.SelectSingleNode("author");
			if(authorNode != null) {
				author = authorNode.InnerText;
			}

			XmlNode msgNode = logEntry.SelectSingleNode("msg");
			String message = msgNode.InnerText;

			ArrayList mods = new ArrayList();
			XmlNodeList paths = logEntry.SelectNodes("paths/path");
			foreach(XmlNode path in paths) {
				Modification mod = new Modification();
				mod.ChangeNumber = changeNumber;
				mod.ModifiedTime = changeTime;
				mod.UserName = author;
				mod.Comment = message;

				String action = GetAttributeFromNode(path, "action");
				switch(action) {
					case "A":
						mod.Type = "Added";
						break;
					case "D":
						mod.Type = "Deleted";
						break;
					case "M":
						mod.Type = "Modified";
						break;
					default:
						mod.Type = "Unknown action: " + action;
						break;
				}
				string fullFileName = path.InnerText;
				mod.FolderName = GetFolderFromPath(fullFileName);
				mod.FileName = GetFileFromPath(fullFileName);

				mods.Add(mod);
			}

			return mods;
		}

		internal string GetFolderFromPath(string fullFileName) {
			int lastSlashIdx = fullFileName.LastIndexOf("/");
			return fullFileName.Substring(0, lastSlashIdx);
		}

		internal string GetFileFromPath(string fullFileName) {
			int lastSlashIdx = fullFileName.LastIndexOf("/");
			return fullFileName.Substring(lastSlashIdx + 1);
		}

		internal string GetAttributeFromNode(XmlNode node, string attributeName) {
			XmlAttributeCollection attributes = node.Attributes;
			XmlAttribute attribute = (XmlAttribute) attributes.GetNamedItem(attributeName);
			return attribute.InnerText;
		}

		internal DateTime ParseDate(string date) {
			return DateTime.Parse(date);
		}
	}
}

