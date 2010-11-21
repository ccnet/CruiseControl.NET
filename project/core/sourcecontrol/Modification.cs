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
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public string Type = "unknown";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public string FileName;
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public string FolderName;
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public DateTime ModifiedTime;
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public string UserName;
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public string ChangeNumber;
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public string Version = string.Empty;
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public string Comment;
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public string Url;
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public string IssueUrl;
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public string EmailAddress;

        /// <summary>
        /// Toes the XML.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string ToXml()
        {
            StringWriter writer = new StringWriter();
            ToXml(new XmlTextWriter(writer));
            return writer.ToString();
        }

        /// <summary>
        /// Toes the XML.	
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <remarks></remarks>
        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("modification");
            writer.WriteAttributeString("type", Type);
            writer.WriteElementString("filename", FileName);
            writer.WriteElementString("project", FolderName);
            writer.WriteElementString("date", DateUtil.FormatDate(ModifiedTime));
            writer.WriteElementString("user", UserName);
            writer.WriteElementString("comment", Comment);
            writer.WriteElementString("changeNumber", ChangeNumber);
            if (!string.IsNullOrEmpty(Version)) writer.WriteElementString("version", Version);
            XmlUtil.WriteNonNullElementString(writer, "url", Url);
            XmlUtil.WriteNonNullElementString(writer, "issueUrl", IssueUrl);
            XmlUtil.WriteNonNullElementString(writer, "email", EmailAddress);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Compares to.	
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public int CompareTo(Object o)
        {
            Modification modification = (Modification)o;
            return ModifiedTime.CompareTo(modification.ModifiedTime);
        }

        /// <summary>
        /// Gets the hash code.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Equalses the specified obj.	
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override bool Equals(object obj)
        {
            return ReflectionUtil.ReflectionEquals(this, obj);
        }

        /// <summary>
        /// Toes the string.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string ToString()
        {
            return ReflectionUtil.ReflectionToString(this);
        }

        /// <summary>
        /// Retrieves the change number of the last modification.
        /// </summary>
        /// <param name="modifications">The modifications to check.</param>
        /// <returns>The last change number if there are any changes, null otherwise.</returns>
        /// <remarks>
        /// Since ChangeNumbers are no longer numbers, this will return null if there are no 
        /// modifications.
        /// </remarks>
        public static string GetLastChangeNumber(Modification[] modifications)
        {
            var lastModification = new Modification
            {
                ModifiedTime = DateTime.MinValue,
                ChangeNumber = null
            };
            foreach (Modification modification in modifications)
            {
                if (modification.ModifiedTime > lastModification.ModifiedTime)
                {
                    lastModification = modification;
                }
            }
            return lastModification.ChangeNumber;
        }
    }
}