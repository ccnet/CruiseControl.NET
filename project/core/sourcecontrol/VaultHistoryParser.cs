using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	public class VaultHistoryParser : IHistoryParser
	{
		private CultureInfo culture;

		public VaultHistoryParser() : this(CultureInfo.CurrentCulture)
		{}

		public VaultHistoryParser(CultureInfo culture)
		{
			this.culture = culture;
		}

		public Modification[] Parse(TextReader history, DateTime from, DateTime to)
		{
			ArrayList mods = new ArrayList();
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(ExtractXmlFromHistory(history));
			XmlNode parent = xml.SelectSingleNode("/vault/history");
			foreach (XmlNode node in parent.ChildNodes)
			{
				if (EntryWithinRange(node, from, to))
				{
					Modification modification = GetModification(node);
					mods.Add(modification);
				}
			}
			return (Modification[]) mods.ToArray(typeof (Modification));
		}

		/// <summary
		/// The Vault command line client (vault.exe), at least for
		/// version 2.0.4, is not guaranteed to output valid XML in
		/// that there may be some not XML output surrounding the XML.
		/// This method strips away any non-XML	output surrounding
		/// the <vault>...</vault> elements.
		/// </summary
		/// <param name="history"Output from Vault client is read from this reader.</param>
		/// <returns>string containing only the XML output from the Vault client.</returns>
		/// <exception cref="CruiseControlException">The <vault> start element or </vault> end element cannot be found.</exception>
		private string ExtractXmlFromHistory(TextReader history)
		{
			string output = history.ReadToEnd();
			return Vault3.ExtractXmlFromOutput(output);
		}

		private bool EntryWithinRange(XmlNode node, DateTime from, DateTime to)
		{
			DateTime date = DateTime.Parse(node.Attributes["date"].InnerText, culture);
			return (date > from && date < to);
		}

		private Modification GetModification(XmlNode node)
		{
			string folderName = null;
			string fileName = null;
			StringBuilder nameBuilder = new StringBuilder(255);
			ushort modTypeID = ushort.Parse(node.Attributes["type"].Value);
			int index;
			
			nameBuilder.Append(node.Attributes["name"].InnerText);
			
			switch ( modTypeID )
			{
				case 10: // add or delete, name attribute contains only the folder
				case 80:
                    // BUG?  This looks like it will incorrectly parse "Added filename containing blanks.txt"
                    // as "blanks.txt" instead of "filename containing blanks.txt".
					index = node.Attributes["actionString"].InnerText.LastIndexOf(' ');
					nameBuilder.Append('/');
					nameBuilder.Append(node.Attributes["actionString"].InnerText.Substring(index).Trim());
					break;
			}
			
			string name = nameBuilder.ToString();
			index = name.LastIndexOf('/');
			if (index == -1)
			{
				folderName = name;
			}
			else
			{
				folderName = name.Substring(0, index);
				fileName = name.Substring(index + 1, name.Length - index - 1);
			}
			DateTime date = DateTime.Parse(node.Attributes["date"].InnerText, culture);
			Modification modification = new Modification();
			modification.FileName = fileName;
			modification.FolderName = folderName;
			modification.ModifiedTime = date;
			modification.UserName = node.Attributes["user"].InnerText;
			modification.Type = GetTypeString(node.Attributes["type"].InnerText);
			modification.Comment = GetComment(node);
			modification.ChangeNumber = int.Parse(node.Attributes["txid"].InnerText);
			return modification;
		}

		private string GetComment(XmlNode node)
		{
			string comment = null;
			if (node.Attributes["comment"] != null)
			{
				comment = node.Attributes["comment"].InnerText;
			}
			return comment;
		}

		#region GetTypeString

		private string GetTypeString(string type)
		{
			// got these from http://support.sourcegear.com/viewtopic.php?t=152
			switch (type)
			{
				case "10":
					return "Added";
				case "20":
					return "Branched from";
				case "30":
					return "Branched from item";
				case "40":
					return "Branched from share";
				case "50":
					return "Branched from share item";
				case "60":
					return "Checked in";
				case "70":
					return "Created";
				case "80":
					return "Deleted";
				case "90":
					return "Labeled";
				case "120":
					return "Moved from";
				case "130":
					return "Moved to";
				case "140":
					return "Obliterated";
				case "150":
					return "Pinned";
				case "160":
					return "Property changed";
				case "170":
					return "Renamed";
				case "180":
					return "Renamed item";
				case "190":
					return "Shared to";
				case "200":
					return "Snapshot";
				case "201":
					return "Snapshot from";
				case "202":
					return "Snapshot item";
				case "210":
					return "Undeleted";
				case "220":
					return "Unpinned";
				case "230":
					return "Rolled back";
			}
			return type;
		}

		#endregion
	}
}