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
			ArrayList mods = new ArrayList();

			XmlNode svnLogRoot = ReadSvnLogIntoXmlNode(svnLog);
			XmlNodeList logEntries = svnLogRoot.SelectNodes("/log/logentry");
			foreach (XmlNode logEntry in logEntries)
			{
				mods.AddRange(ParseModificationsFromLogEntry(logEntry, from, to));
			}
			return (Modification[]) mods.ToArray(typeof(Modification));
		}

		private XmlNode ReadSvnLogIntoXmlNode(TextReader svnLog)
		{
			string logText = svnLog.ReadToEnd();

			XmlDocument log = new XmlDocument();
			try
			{
				log.LoadXml(logText);
			}
			catch (XmlException ex)
			{
				throw new CruiseControlException(string.Format("Unable to load the output from svn: {0}", logText), ex);
			}
			return log.DocumentElement;
		}

		private ArrayList ParseModificationsFromLogEntry(XmlNode logEntry, DateTime from, DateTime to)
		{
			DateTime changeTime = ParseDate(logEntry);
			if (changeTime < from || to < changeTime)
			{
				// Work around issue 1642 in Subversion (http://subversion.tigris.org/issues/show_bug.cgi?id=1642).
				return new ArrayList();
			}

			int changeNumber = ParseChangeNumber(logEntry);
			string author = ParseAuthor(logEntry);
			string message = ParseMessage(logEntry);

			ArrayList mods = new ArrayList();
			XmlNodeList paths = logEntry.SelectNodes("paths/path");
			foreach (XmlNode path in paths)
			{
				Modification mod = new Modification();
				mod.ChangeNumber = changeNumber;
				mod.ModifiedTime = changeTime;
				mod.UserName = author;
				mod.Comment = message;
				mod.Type = ModificationType(path);
				string fullFileName = path.InnerText;
				mod.FolderName = GetFolderFromPath(fullFileName);
				mod.FileName = GetFileFromPath(fullFileName);
				mods.Add(mod);
			}
			return mods;
		}

		private string ModificationType(XmlNode path)
		{
			string action = GetAttributeFromNode(path, "action");
			switch (action)
			{
				case "A":
					return "Added";
				case "D":
					return "Deleted";
				case "M":
					return "Modified";
				case "R":
					return "Replaced";
				default:
					return "Unknown action: " + action;
			}
		}

		private static string ParseMessage(XmlNode logEntry)
		{
			XmlNode msgNode = logEntry.SelectSingleNode("msg");
			return msgNode.InnerText;
		}

		private string ParseAuthor(XmlNode logEntry)
		{
			String author = "";
			XmlNode authorNode = logEntry.SelectSingleNode("author");
			if (authorNode != null)
			{
				author = authorNode.InnerText;
			}
			return author;
		}

		private DateTime ParseDate(XmlNode logEntry)
		{
			XmlNode dateNode = logEntry.SelectSingleNode("date");
			return ParseDate(dateNode.InnerText);
		}

		private int ParseChangeNumber(XmlNode logEntry)
		{
			String revision = GetAttributeFromNode(logEntry, "revision");
			return int.Parse(revision);
		}

		private string GetFolderFromPath(string fullFileName)
		{
			int lastSlashIdx = fullFileName.LastIndexOf("/");
			return fullFileName.Substring(0, lastSlashIdx);
		}

		private string GetFileFromPath(string fullFileName)
		{
			int lastSlashIdx = fullFileName.LastIndexOf("/");
			return fullFileName.Substring(lastSlashIdx + 1);
		}

		private string GetAttributeFromNode(XmlNode node, string attributeName)
		{
			XmlAttributeCollection attributes = node.Attributes;
			XmlAttribute attribute = (XmlAttribute) attributes.GetNamedItem(attributeName);
			return attribute.InnerText;
		}

		private DateTime ParseDate(string date)
		{
			return DateTime.Parse(date, CultureInfo.InvariantCulture);
		}
	}
}