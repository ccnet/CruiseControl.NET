using System;
using System.Collections.Generic;
using System.Web;
using Exortech.NetReflector;
using Exortech.NetReflector.Util;
using System.Xml;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport
{
    public class BuildReportXslFilenameSerialiser
        : XmlMemberSerialiser
    {
        public BuildReportXslFilenameSerialiser(ReflectorMember info, ReflectorPropertyAttribute attribute)
            : base(info, attribute)
        { }

        public override void Write(XmlWriter writer, object target)
        {
        }

        public override object Read(XmlNode node, NetReflectorTypeTable types)
        {
            var files = new List<BuildReportXslFilename>();
            foreach (XmlNode fileNode in node.SelectNodes("xslFile"))
            {
                var newFile = new BuildReportXslFilename();
                foreach (XmlNode childNode in fileNode.ChildNodes)
                {
                    if (childNode.NodeType == XmlNodeType.Text)
                    {
                        newFile.Filename = childNode.InnerText.Trim();
                    }
                    else if (childNode.LocalName == "includedProjects")
                    {
                        foreach (XmlNode projectNode in childNode.ChildNodes)
                        {
                            newFile.IncludedProjects.Add(projectNode.InnerText.Trim());
                        }
                    }
                    else if (childNode.LocalName == "excludedProjects")
                    {
                        foreach (XmlNode projectNode in childNode.ChildNodes)
                        {
                            newFile.ExcludedProjects.Add(projectNode.InnerText.Trim());
                        }
                    }
                }

                files.Add(newFile);
            }

            return files.ToArray();
        }
    }
}
