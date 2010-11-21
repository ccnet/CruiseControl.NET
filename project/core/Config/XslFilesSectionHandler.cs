using System.Collections;
using System.Configuration;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Config
{
    /// <summary>
    /// 	
    /// </summary>
	public class XslFilesSectionHandler : IConfigurationSectionHandler
	{
        /// <summary>
        /// Creates the specified parent.	
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="configContext">The config context.</param>
        /// <param name="section">The section.</param>
        /// <returns></returns>
        /// <remarks></remarks>
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
