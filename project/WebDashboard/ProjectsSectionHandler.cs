using System.Configuration;
using System.Collections;
using System.Xml;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	public class ProjectsSectionHandler : IConfigurationSectionHandler
	{
		public object Create(object parent, object configContext, System.Xml.XmlNode section)
		{
			ArrayList projects = new ArrayList();
			
			foreach (XmlNode node in section.ChildNodes) 
			{
				if (node.NodeType == System.Xml.XmlNodeType.Element) 
				{
					projects.Add(new ProjectSpecification(node.Attributes["name"].Value, node.Attributes["logDir"].Value, node.Attributes["serverLogFilePath"].Value, int.Parse(node.Attributes["serverLogFileLines"].Value)));
				}
			}

			return projects;
		}
	}
}
