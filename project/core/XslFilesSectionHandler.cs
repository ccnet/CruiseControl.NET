using System;
using System.Configuration;
using System.Collections;
using System.Xml;
using System.IO;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core
{
	/// <summary>
	/// Summary description for XslFilesSectionHandler.
	/// </summary>
	public class XslFilesSectionHandler : IConfigurationSectionHandler
	{
		public XslFilesSectionHandler()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		#region IConfigurationSectionHandler Members

		public object Create(object parent, object configContext, System.Xml.XmlNode section)
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

		#endregion
	}
}
