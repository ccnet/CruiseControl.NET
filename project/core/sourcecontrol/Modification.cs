using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core
{
	/// <summary>
	/// Value object representing the data associated with a source control modification.
	/// </summary>
	[XmlRoot("modification")]
	public class Modification : IComparable
	{
		public string Type = "unknown";
		public string FileName;
		public string FolderName;
		public DateTime ModifiedTime;
		public string UserName;
		public int ChangeNumber;
		public string Version = "";
		public string Comment;
		public string Url;
		public string EmailAddress;

		public string ToXml()
		{
			StringWriter writer = new StringWriter();
			ToXml(new XmlTextWriter(writer));
			return writer.ToString();
		}

		public void ToXml(XmlWriter writer)
		{
			writer.WriteStartElement("modification");
			writer.WriteAttributeString("type", Type);
			writer.WriteElementString("filename", FileName);
			writer.WriteElementString("project", FolderName);
			writer.WriteElementString("date", DateUtil.FormatDate(ModifiedTime));
			writer.WriteElementString("user", UserName);
			writer.WriteElementString("comment", Comment);
			writer.WriteElementString("changeNumber", ChangeNumber.ToString());
			if (! StringUtil.IsBlank(Version)) writer.WriteElementString("version", Version);
			XmlUtil.WriteNonNullElementString(writer, "url", Url);
			XmlUtil.WriteNonNullElementString(writer, "email", EmailAddress);
			writer.WriteEndElement();
		}

		public int CompareTo(Object o)
		{
			Modification modification = (Modification) o;
			return ModifiedTime.CompareTo(modification.ModifiedTime);
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return ReflectionUtil.ReflectionEquals(this, obj);
		}

		public override string ToString()
		{
			return ReflectionUtil.ReflectionToString(this);
		}
	}
}