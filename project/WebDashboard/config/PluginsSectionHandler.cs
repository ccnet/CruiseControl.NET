using System.Collections;
using System.Configuration;
using System.Xml;

namespace ThoughtWorks.CruiseControl.WebDashboard.Config
{
	public class PluginsSectionHandler : IConfigurationSectionHandler
	{
		public static readonly string SectionName = "CCNet/plugins";

		public object Create(object parent, object configContext, XmlNode section)
		{
			ArrayList projectPlugins = new ArrayList();
			
			foreach (XmlNode node in section.ChildNodes) 
			{
				if (node.NodeType == XmlNodeType.Element) 
				{
					string typeName = node.Attributes["typeName"].Value;
					XmlAttribute assemblyNameAttribute = node.Attributes["assemblyName"];
					if (assemblyNameAttribute == null || assemblyNameAttribute.Value == string.Empty)
					{
						projectPlugins.Add(new SimplePluginSpecification(typeName));
					}
					else
					{
						projectPlugins.Add(new AssemblyLoadingPluginSpecification(typeName, assemblyNameAttribute.Value));	
					}
				}
			}

			return (IPluginSpecification[]) projectPlugins.ToArray (typeof (IPluginSpecification));
		}
	}
}
