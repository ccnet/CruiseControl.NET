using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.Core.Security.Auditing
{
    /// <summary>
    /// Reads audit events from an XML file.
    /// </summary>
    [ReflectorType("xmlFileAuditReader")]
    public class FileXmlReader
        : IAuditReader
    {
        private string auditFile = "SecurityAudit.xml";

        /// <summary>
        /// The location to log the audit events.
        /// </summary>
        [ReflectorProperty("location", Required = false)]
        public string AuditFileLocation
        {
            get { return this.auditFile; }
            set { this.auditFile = value; }
        }

        /// <summary>
        /// Reads all the specified number of audit events.
        /// </summary>
        /// <param name="startPosition">The starting position.</param>
        /// <param name="numberOfRecords">The number of records to read.</param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details.</returns>
        public virtual List<AuditRecord> Read(int startPosition, int numberOfRecords)
        {
            return Read(startPosition, numberOfRecords, null);
        }

        /// <summary>
        /// Reads all the specified number of filtered audit events.
        /// </summary>
        /// <param name="startPosition">The starting position.</param>
        /// <param name="numberOfRecords">The number of records to read.</param>
        /// <param name="filter">The filter to use.</param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details that match the filter.</returns>
        public virtual List<AuditRecord> Read(int startPosition, int numberOfRecords, IAuditFilter filter)
        {
            List<AuditRecord> records = new List<AuditRecord>();
            string[] lines = LoadAuditLines();
            int count = 0;
            int position = lines.Length - startPosition;

            while ((position-- > 0) && (count < numberOfRecords))
            {
                string currentLine = lines[position];
                if (!string.IsNullOrEmpty(currentLine))
                {
                    AuditRecord record = ReadRecord(currentLine);
                    if ((filter == null) || filter.CheckFilter(record))
                    {
                        records.Add(record);
                        count++;
                    }
                }
            }

            return records;
        }

        private AuditRecord ReadRecord(string dataLine)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(dataLine);
            AuditRecord record = new AuditRecord();
            record.TimeOfEvent = DateTime.Parse(ReadDataValue(document, "dateTime"));
            record.ProjectName = ReadDataValue(document, "project");
            record.UserName = ReadDataValue(document, "user");
            record.EventType = ReadDataValue<SecurityEvent>(document, "type", SecurityEvent.Unknown);
            record.SecurityRight = ReadDataValue<SecurityRight>(document, "outcome", SecurityRight.Inherit);
            record.Message = ReadDataValue(document, "message");

            return record;
        }

        private string ReadDataValue(XmlDocument document, string key)
        {
            XmlElement element = document.SelectSingleNode("//" + key) as XmlElement;
            if (element != null)
            {
                return element.InnerText;
            }
            else
            {
                return null;
            }
        }

        private TEnum ReadDataValue<TEnum>(XmlDocument document, string key, TEnum defaultValue)
        {
            string value = ReadDataValue(document, key);
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            else
            {
                return (TEnum)Enum.Parse(typeof(TEnum), value);
            }
        }

        /// <summary>
        /// Loads the lines from the audit file.
        /// </summary>
        /// <returns></returns>
        private string[] LoadAuditLines()
        {
            string auditLog = Util.PathUtils.EnsurePathIsRooted(this.auditFile);

            Stream inputStream = File.Open(auditFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader reader = new StreamReader(inputStream);
            string fileData;
            try
            {
                fileData = reader.ReadToEnd();
            }
            finally
            {
                try
                {
                    reader.Close();
                }
                finally
                {
                    inputStream.Close();
                }
            }

            return fileData.Split('\r', '\n');
        }
    }
}
