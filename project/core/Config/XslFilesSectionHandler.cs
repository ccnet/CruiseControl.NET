using System.Collections;
using System.Configuration;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	public class XslFilesSectionHandler : IConfigurationSectionHandler
	{
		public object Create(object parent, object configContext, XmlNode section)
		{
			ArrayList files = new ArrayList();

			foreach (XmlNode node in section.ChildNodes) 
			{
				if (node.NodeType == System.Xml.XmlNodeType.Element) 
				{
					files.Add(node.Attributes["name"].Value);
				}
			}

			return files;
		}
	}
}
