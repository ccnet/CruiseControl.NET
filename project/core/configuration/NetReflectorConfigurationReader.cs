using System;
using System.IO;
using System.Xml;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	public class NetReflectorConfigurationReader
	{
		private const string ROOT_ELEMENT = "cruisecontrol";
		private const string CONFIG_ASSEMBLY_PATTERN = "ccnet.*.plugin.dll";
		private readonly NetReflectorTypeTable typeTable;
		private NetReflectorReader reader;

		public event InvalidNodeEventHandler InvalidNodeEventHandler;

		public NetReflectorConfigurationReader()
		{
			typeTable = new NetReflectorTypeTable();
			typeTable.Add(AppDomain.CurrentDomain);
			typeTable.Add(Directory.GetCurrentDirectory(), CONFIG_ASSEMBLY_PATTERN);
			typeTable.InvalidNode += new InvalidNodeEventHandler(HandleUnusedNode);
			reader = new NetReflectorReader(typeTable);
		}

		public IConfiguration Read(XmlDocument document)
		{
            string ConflictingXMLNode = string.Empty;

			VerifyDocumentHasValidRootElement(document);
			try
			{
				Configuration configuration = new Configuration();
				foreach (XmlNode node in document.DocumentElement)
				{
                    ConflictingXMLNode = string.Empty;

					if (!(node is XmlComment))
					{
                        ConflictingXMLNode = "Conflicting project data : " + node.OuterXml;

						IProject project = reader.Read(node) as IProject;	// could this be null?  should check
						configuration.AddProject(project);
					}
				}
				return configuration;
			}
			catch (NetReflectorException ex)
			{
				throw new ConfigurationException("\nUnable to instantiate CruiseControl projects from configuration document." +
                    "\nConfiguration document is likely missing Xml nodes required for properly populating CruiseControl configuration.\n" 
                    + ex.Message + 
                    "\n " + ConflictingXMLNode  , ex);
			}
		}

		private static void VerifyDocumentHasValidRootElement(XmlDocument configXml)
		{
			if (configXml.DocumentElement == null || configXml.DocumentElement.Name != ROOT_ELEMENT)
			{
				throw new ConfigurationException("The configuration document has an invalid root element.  Expected <cruisecontrol>.");
			}
		}

		private void HandleUnusedNode(InvalidNodeEventArgs args)
		{
			if (InvalidNodeEventHandler != null)
				InvalidNodeEventHandler(args);
		}
	}
}