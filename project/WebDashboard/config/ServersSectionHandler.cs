using System.Collections;
using System.Configuration;
using System.Xml;

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
					servers.Add(new ServerLocation(node.Attributes["name"].Value, node.Attributes["url"].Value));
				}
			}

			return (ServerLocation[]) servers.ToArray(typeof (ServerLocation));
		}

		public static readonly string SectionName = "CCNet/servers";
	}
}
