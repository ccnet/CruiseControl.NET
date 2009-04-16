using Exortech.NetReflector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// Handles the passing of security information.
    /// </summary>
    public class ServerSecurityConfigurationInformation
    {
        private ISecurityManager securityManager;
        private List<ProjectSecurityConfigurationInformation> projectsList = new List<ProjectSecurityConfigurationInformation>();

        /// <summary>
        /// The associated security manager.
        /// </summary>
        [ReflectorProperty("manager", InstanceTypeKey = "type")]
        public ISecurityManager Manager
        {
            get { return securityManager; }
            set { securityManager = value; }
        }

        /// <summary>
        /// The security information on the projects.
        /// </summary>
        [ReflectorProperty("projects")]
        public List<ProjectSecurityConfigurationInformation> Projects
        {
            get { return projectsList; }
        }

        /// <summary>
        /// Adds a new project to the list.
        /// </summary>
        /// <param name="project">The project configuration to add.</param>
        public void AddProject(IProject project)
        {
            if (project.Security != null)
            {
                ProjectSecurityConfigurationInformation info = new ProjectSecurityConfigurationInformation();
                info.Name = project.Name;
                info.Security = project.Security;
                projectsList.Add(info);
            }
        }

        /// <summary>
        /// Returns this class as an XML string.
        /// </summary>
        /// <returns>The XML string containing all the security information.</returns>
        public override string ToString()
        {
            StringWriter buffer = new StringWriter();
            new ReflectorTypeAttribute("security").Write(new XmlTextWriter(buffer), this);
            string xmlData = HidePasswords(buffer.ToString());
            return xmlData;
        }

        /// <summary>
        /// Hides all the passwords in the configuration.
        /// </summary>
        /// <param name="xmlData">The XML data to be returned.</param>
        /// <returns>The XML data with the passwords hidden.</returns>
        public string HidePasswords(string xmlData)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlData);
            foreach (XmlElement passwordNode in document.SelectNodes("//password"))
            {
                passwordNode.InnerText = new string('*', 10);
            }
            return document.OuterXml;
        }
    }
}
