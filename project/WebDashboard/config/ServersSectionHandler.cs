using System;
using System.Collections;
using System.Configuration;
using System.Xml;
using ThoughtWorks.CruiseControl.WebDashboard.config;

namespace ThoughtWorks.CruiseControl.WebDashboard.Config
{
	public class ServersSectionHandler : IConfigurationSectionHandler
	{
		public object Create(object parent, object configContext, XmlNode section)
		{
			ArrayList servers = new ArrayList();
			
			foreach (XmlNode node in section.ChildNodes) 
			{
				if (node.NodeType == XmlNodeType.Element) 
				{
					servers.Add(new ServerSpecification(node.Attributes["name"].Value, node.Attributes["url"].Value));
				}
			}

			return (ServerSpecification[]) servers.ToArray(typeof (ServerSpecification));
		}

		public static readonly string SectionName = "servers";
	}
}
