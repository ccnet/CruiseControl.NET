using System.Collections;
using System.Configuration;
using System.Xml;
using ThoughtWorks.CruiseControl.WebDashboard.Config;

namespace ThoughtWorks.CruiseControl.WebDashboard.Config
{
	public class ProjectsSectionHandler : IConfigurationSectionHandler
	{
		public object Create(object parent, object configContext, XmlNode section)
		{
			ArrayList projects = new ArrayList();
			
			foreach (XmlNode node in section.ChildNodes) 
			{
				if (node.NodeType == XmlNodeType.Element) 
				{
					projects.Add(new ProjectSpecification(node.Attributes["name"].Value, node.Attributes["logDir"].Value, node.Attributes["serverLogFilePath"].Value, int.Parse(node.Attributes["serverLogFileLines"].Value)));
				}
			}

			return (ProjectSpecification[]) projects.ToArray(typeof (ProjectSpecification));
		}
	}
}
