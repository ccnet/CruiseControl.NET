using System;
using System.Configuration;
using System.Collections;
using System.Xml;
using System.IO;
using System.Text;

namespace tw.ccnet.core
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
			
			foreach (XmlNode node in section.SelectNodes("/xslFiles/file")) 
			{
				files.Add(node.Attributes["name"].Value);

			}
			return files;
		}

		#endregion
	}
}
