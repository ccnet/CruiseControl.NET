using System;
using System.Configuration;
using System.Collections;
using System.Xml;
using System.IO;
using System.Text;

namespace tw.ccnet.webdashboard
{
	public class ProjectURLsSectionHandler : IConfigurationSectionHandler
	{
		public object Create(object parent, object configContext, System.Xml.XmlNode section)
		{
			ArrayList urls = new ArrayList();
			
			foreach (XmlNode node in section.ChildNodes) 
			{
				if (node.NodeType == System.Xml.XmlNodeType.Element) 
				{
					urls.Add(node.Attributes["url"].Value);
				}
			}

			return urls;
		}
	}
}
