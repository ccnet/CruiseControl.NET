using System.Collections;
using System.Configuration;
using System.Xml;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.WebDashboard.Config
{
	public class PluginsSectionHandler : IConfigurationSectionHandler
	{
		public object Create(object parent, object configContext, XmlNode section)
		{
			ArrayList plugins = new ArrayList();
			
			foreach (XmlNode node in section.ChildNodes) 
			{
				if (node.NodeType == XmlNodeType.Element) 
				{
					string typeName = node.Attributes["typeName"].Value;
					if (typeName == null || typeName == string.Empty)
					{
						throw new CruiseControlException(string.Format("Error reading plugin configuration - tag {0} does not have a typeName attribute", node.Name));
					}
					XmlAttribute assemblyFileNameAttribute = node.Attributes["assemblyFileName"];
					if (assemblyFileNameAttribute == null || assemblyFileNameAttribute.Value == string.Empty)
					{
						plugins.Add(new SimplePluginSpecification(typeName));
					}
					else
					{
						plugins.Add(new AssemblyLoadingPluginSpecification(typeName, assemblyFileNameAttribute.Value));	
					}
				}
			}

			return (IPluginSpecification[]) plugins.ToArray (typeof (IPluginSpecification));
		}
	}
}
