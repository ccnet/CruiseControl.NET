using System.Collections;
using System.Configuration;
using System.Xml;

namespace ThoughtWorks.CruiseControl.WebDashboard.Config
{
	public class PluginsSectionHandler : IConfigurationSectionHandler
	{
		public object Create(object parent, object configContext, XmlNode section)
		{
			ArrayList projectPlugins = new ArrayList();
			
			foreach (XmlNode node in section.ChildNodes) 
			{
				if (node.NodeType == XmlNodeType.Element) 
				{
					projectPlugins.Add(new PluginSpecification(node.Attributes["linkText"].Value, node.Attributes["linkUrl"].Value));
				}
			}

			return projectPlugins;
		}
	}
}
