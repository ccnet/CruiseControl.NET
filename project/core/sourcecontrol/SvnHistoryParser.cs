using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// Parser for output from the Subversion "svn log --xml" command.  See the schema
    /// from the Subversion source repository
    /// at http://svn.collab.net/viewvc/svn/trunk/subversion/svn/schema/log.rnc
    /// for the exact details of the input format.
    /// </summary>
	public class SvnHistoryParser : IHistoryParser
	{
        private bool integrationStatusUnknown = false;

        public bool IntegrationStatusUnknown
        {
            set { integrationStatusUnknown = value; }
            get { return integrationStatusUnknown; }
        }

        /// <summary>
        /// Parse the output from a Subversion "svn log --xml" command into a set of <see cref="Modification"/>s.
        /// </summary>
        /// <param name="svnLog">The output of the "svn log --xml" command.</param>
        /// <param name="from">The starting timestamp.</param>
        /// <param name="to">The ending timestamp.</param>
        /// <returns>A list of modifications between the two timestamps, possibly empty.</returns>
		public Modification[] Parse(TextReader svnLog, DateTime from, DateTime to)
		{
            var mods = new List<Modification>();

			XmlNode svnLogRoot = ReadSvnLogIntoXmlNode(svnLog);
			XmlNodeList logEntries = svnLogRoot.SelectNodes("/log/logentry");
            if (logEntries == null || logEntries.Count == 0)
                Log.Debug("No <logentry>s found under <log>.");
            else
            {
                foreach (XmlNode logEntry in logEntries)
                {
                    mods.AddRange(ParseModificationsFromLogEntry(logEntry, from, to));
                }
            }
            return mods.ToArray();
		}

        /// <summary>
        /// Read the output from a Subversion "svn log --xml" command into an XML document.
        /// </summary>
        /// <param name="svnLog">The output of the "svn log --xml" command.</param>
        /// <returns>The root node of the XML document.</returns>
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

        /// <summary>
        /// Parse a single &lt;logentry&gt; element from the output of a Subversion "svn log --xml"
        /// command into a set of <see cref="Modification"/>s.
        /// </summary>
        /// <param name="logEntry">The &lt;logentry&gt; element of the "svn log --xml" command.</param>
        /// <param name="from">The starting timestamp.</param>
        /// <param name="to">The ending timestamp.</param>
        /// <returns>A list of modifications between the two timestamps, possibly empty.</returns>
        private List<Modification> ParseModificationsFromLogEntry(XmlNode logEntry, DateTime from, DateTime to)
		{
            try
            {
                DateTime changeTime = ParseDate(logEntry);

                if (!IntegrationStatusUnknown)
                {
                    if (changeTime == DateTime.MinValue || changeTime < from || to < changeTime)
                    {
                        // Work around issue 1642 in Subversion (http://subversion.tigris.org/issues/show_bug.cgi?id=1642).
                        return new List<Modification>();
                    }
                }
                else
                {
                    IntegrationStatusUnknown = false;
                }

                int changeNumber = ParseChangeNumber(logEntry);
                string author = ParseAuthor(logEntry);
                string message = ParseMessage(logEntry);

                XmlNodeList paths = logEntry.SelectNodes("paths/path");
                if (paths == null)
                    return new List<Modification>();
                var mods = new List<Modification>();
                foreach (XmlNode path in paths)
                {
                    Modification mod = new Modification();
                    mod.ChangeNumber = changeNumber.ToString();
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
            catch (XmlException e)
            {
                throw new CruiseControlException("Invalid XML received from \"svn log --xml\" output: \"" + logEntry + "\".", e);
            }
		}

        /// <summary>
        /// Convert a Subversion "svn log --xml" action attribute value to a modification type name.
        /// </summary>
        /// <param name="path">The &lt;path&gt; element containing the attribute.</param>
        /// <returns>The modification type name.</returns>
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

        /// <summary>
        /// Parse the check-in message (&lt;msg&gt;) element from a Subversion "svn log --xml" &lt;logentry&gt; element.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <returns>The message if found, or "" if not.</returns>
        private static string ParseMessage(XmlNode logEntry)
		{
		    String msg = "";
			XmlNode msgNode = logEntry.SelectSingleNode("msg");
            if (msgNode != null)
                msg = msgNode.InnerText;
		    return msg;
		}

        /// <summary>
        /// Parse the check-in userid (&lt;author&gt;) element from a Subversion "svn log --xml" &lt;logentry&gt; element.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <returns>The userid if found, or "" if not.</returns>
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

        /// <summary>
        /// Parse the timestamp (&lt;date&gt;) element from a Subversion "svn log --xml" &lt;logentry&gt; element.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <returns>The timestamp if found, or DateTime.MinValue if not.</returns>
		private DateTime ParseDate(XmlNode logEntry)
		{
		    DateTime date = DateTime.MinValue;
			XmlNode dateNode = logEntry.SelectSingleNode("date");
            if (dateNode != null)
                date = ParseDate(dateNode.InnerText);
            return date;
		}

        /// <summary>
        /// Parse the revision number (revision) attribute from a Subversion "svn log --xml" &lt;logentry&gt; element.
        /// </summary>
        /// <param name="logEntry"></param>
        /// <returns></returns>
		private int ParseChangeNumber(XmlNode logEntry)
		{
			String revision = GetAttributeFromNode(logEntry, "revision");
			return int.Parse(revision);
		}

        /// <summary>
        /// Extract the folder name from a file path name in a Subversion "svn log --xml" &lt;path&gt; element.
        /// </summary>
        /// <param name="fullFileName">The path name.</param>
        /// <returns>The folder name.</returns>
		private string GetFolderFromPath(string fullFileName)
		{
			int lastSlashIdx = fullFileName.LastIndexOf("/");
			return fullFileName.Substring(0, lastSlashIdx);
		}

        /// <summary>
        /// Extract the file name from a file path name in a Subversion "svn log --xml" &lt;path&gt; element.
        /// </summary>
        /// <param name="fullFileName">The path name.</param>
        /// <returns>The file name.</returns>
        private string GetFileFromPath(string fullFileName)
		{
			int lastSlashIdx = fullFileName.LastIndexOf("/");
			return fullFileName.Substring(lastSlashIdx + 1);
		}

        /// <summary>
        /// Get an attribute from an XML element.
        /// </summary>
        /// <param name="node">The element.</param>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <returns>The attribute's value.</returns>
		private string GetAttributeFromNode(XmlNode node, string attributeName)
		{
			XmlAttributeCollection attributes = node.Attributes;
			XmlAttribute attribute = (XmlAttribute) attributes.GetNamedItem(attributeName);
			return attribute.InnerText;
		}

        /// <summary>
        /// Parse the timestamp (&lt;date&gt;) value from a Subversion "svn log --xml" &lt;logentry&gt;.
        /// </summary>
        /// <param name="date">The timestamp value as a string.</param>
        /// <returns>The timestamp value as a <see cref="DateTime"/>.</returns>
        private DateTime ParseDate(string date)
		{
			return DateTime.Parse(date, CultureInfo.InvariantCulture);
		}
	}
}