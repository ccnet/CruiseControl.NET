using System.Collections;
using System.Configuration;
using System.Xml;

namespace ThoughtWorks.CruiseControl.WebDashboard.Config
{
	public class ProjectURLsSectionHandler : IConfigurationSectionHandler
	{
		public object Create(object parent, object configContext, XmlNode section)
		{
			ArrayList urls = new ArrayList();
			
			foreach (XmlNode node in section.ChildNodes) 
			{
				if (node.NodeType == XmlNodeType.Element) 
				{
					urls.Add(node.Attributes["url"].Value);
				}
			}

			return urls;
		}
	}
}
