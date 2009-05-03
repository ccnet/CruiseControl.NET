using Exortech.NetReflector;
using System;
using System.IO;
using System.Text;
using System.Xml;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.Core.Security.Auditing
{
    /// <summary>
    /// Stores audit events in an XML file.
    /// </summary>
    [ReflectorType("xmlFileAudit")]
    public class FileXmlLogger
        : AuditLoggerBase, IAuditLogger
    {
        private string auditFile = "SecurityAudit.xml";

        /// <summary>
        /// The location to log the audit events.
        /// </summary>
        [ReflectorProperty("location", Required=false)]
        public string AuditFileLocation
        {
            get { return this.auditFile; }
            set { this.auditFile = value; }
        }

        /// <summary>
        /// Performs the actual logging of a security event
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <param name="userName">The name of the user.</param>
        /// <param name="eventType">The type of event.</param>
        /// <param name="eventRight">The right of the event.</param>
        /// <param name="message">Any security message.</param>
        protected override void DoLogEvent(string projectName, string userName, SecurityEvent eventType, SecurityRight eventRight, string message)
        {
            // Generate the log entry
            XmlDocument auditXml = new XmlDocument();
            XmlElement xmlRoot = auditXml.CreateElement("event");
            auditXml.AppendChild(xmlRoot);
            AddXmlElement(auditXml, xmlRoot, "dateTime", DateTime.Now.ToString("o"));
            if (!string.IsNullOrEmpty(projectName)) AddXmlElement(auditXml, xmlRoot, "project", projectName);
            if (!string.IsNullOrEmpty(userName)) AddXmlElement(auditXml, xmlRoot, "user", userName);
            AddXmlElement(auditXml, xmlRoot, "type", eventType.ToString());
            if (eventRight != SecurityRight.Inherit) AddXmlElement(auditXml, xmlRoot, "outcome", eventRight.ToString());
            if (!string.IsNullOrEmpty(message)) AddXmlElement(auditXml, xmlRoot, "message", message);

            // Write the entry
            string auditLog = Util.PathUtils.EnsurePathIsRooted(this.auditFile);
            lock (this)
            {
                File.AppendAllText(auditLog, auditXml.OuterXml.Replace(Environment.NewLine, " ") + Environment.NewLine);
            }
        }

        private void AddXmlElement(XmlDocument document, XmlElement parent, string name, string value)
        {
            XmlElement newNode = document.CreateElement(name);
            newNode.InnerText = value;
            parent.AppendChild(newNode);
        }
    }
}
