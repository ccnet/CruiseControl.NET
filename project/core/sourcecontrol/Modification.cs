using System;
using System.Threading;
using System.Collections;
using System.Xml;
using System.IO;
using tw.ccnet.core.util;

namespace tw.ccnet.core
{
	public class Modification : IComparable 
	{
		/** enable logging for this class */
		private string type = "unknown";
		private string fileName;
		private string folderName;
		private DateTime modifiedTime;
		private string userName;
		private string emailAddress;
		private string comment;

		#region properties
		public string Type
		{
			get { return type; }
			set { type = value; }
		}

		public string FileName
		{
			get { return fileName; }
			set { fileName = value; }
		}

		public string FolderName
		{
			get { return folderName; }
			set { folderName = value; }
		}

		public DateTime ModifiedTime
		{
			get { return modifiedTime; }
			set { modifiedTime = value; }
		}

		public string UserName
		{
			get { return userName; }
			set { userName = value; }
		}

		public string EmailAddress
		{
			get { return emailAddress; }
			set { emailAddress = value; }
		}

		public string Comment
		{
			get { return comment; }
			set { comment = value; }
		}
		#endregion

		public XmlDocument ToDocument() 
		{
			XmlDocument doc = new XmlDocument();
			XmlElement modificationElement = doc.CreateElement("modification");
			doc.AppendChild(modificationElement);
			modificationElement.SetAttribute("type", type);
			XmlElement filenameElement = doc.CreateElement("filename");
			filenameElement.InnerText = fileName;
			XmlElement projectElement = doc.CreateElement("project");
			projectElement.InnerText = folderName;
			XmlElement dateElement = doc.CreateElement("date");
			dateElement.InnerText = DateUtil.FormatDate(modifiedTime);
			XmlElement userElement = doc.CreateElement("user");
			userElement.InnerText = userName;
			XmlElement commentElement = doc.CreateElement("comment");
			commentElement.InnerText = comment;
			
			modificationElement.AppendChild(filenameElement);
			modificationElement.AppendChild(projectElement);
			modificationElement.AppendChild(dateElement);
			modificationElement.AppendChild(userElement);
			modificationElement.AppendChild(commentElement);

			// not all sourcecontrols guarantee a non-null email address
			if ( emailAddress != null ) 
			{
				XmlElement emailAddressElement = doc.CreateElement("email");
				emailAddressElement.InnerText = emailAddress;
				modificationElement.AppendChild(emailAddressElement);
			}
			return doc;
		}

		public string ToXml()
		{
			return ToDocument().OuterXml;
		}

		public int CompareTo(Object o) {
			Modification modification = (Modification) o;
			return modifiedTime.CompareTo(modification.modifiedTime);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();			
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