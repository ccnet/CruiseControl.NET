using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	public class DefaultConfigurationFileLoader : IConfigurationFileLoader
	{
		public const string XsdSchemaResourceName = "ThoughtWorks.CruiseControl.Core.configuration.ccnet.xsd";

		private ValidationEventHandler handler;
		private NetReflectorConfigurationReader reader;

		public DefaultConfigurationFileLoader() : this(new NetReflectorConfigurationReader())
		{}

		public DefaultConfigurationFileLoader(NetReflectorConfigurationReader reader)
		{
			this.reader = reader;
			reader.InvalidNodeEventHandler += new InvalidNodeEventHandler(WarnOnInvalidNode);
			handler = new ValidationEventHandler(HandleSchemaEvent);
		}

		public IConfiguration Load(FileInfo configFile)
		{
			Log.Info(String.Format("Reading configuration file \"{0}\"", configFile.FullName));
			return PopulateProjectsFromXml(LoadConfiguration(configFile));
		}

		// TODO - this should be private - update tests and make it so
		public XmlDocument LoadConfiguration(FileInfo configFile)
		{
			VerifyConfigFileExists(configFile);

			XmlDocument config = AttemptLoadConfiguration(configFile);
			return config;
		}

		private XmlDocument AttemptLoadConfiguration(FileInfo configFile)
		{
			XmlValidatingLoader loader = CreateXmlValidatingLoader(configFile);
			try
			{
				return loader.Load();
			}
			catch (XmlException ex)
			{
				throw new ConfigurationException("The configuration file contains invalid xml: " + configFile.FullName, ex);
			}
			finally
			{
				loader.Dispose();
			}
		}

		private XmlValidatingLoader CreateXmlValidatingLoader(FileInfo configFile)
		{
			XmlValidatingLoader loader = new XmlValidatingLoader(new XmlTextReader(configFile.FullName));
			loader.ValidationEventHandler += handler;
			return loader;
		}

		private void VerifyConfigFileExists(FileInfo configFile)
		{
			if (! configFile.Exists)
			{
				throw new ConfigurationFileMissingException("Specified configuration file does not exist: " + configFile.FullName);
			}
		}

		private IConfiguration PopulateProjectsFromXml(XmlDocument configXml)
		{
			return reader.Read(configXml);
		}

		private void HandleSchemaEvent(object sender, ValidationEventArgs args)
		{
			Log.Info("Loading config schema: " + args.Message);
		}

		private void WarnOnInvalidNode(InvalidNodeEventArgs args)
		{
			throw new ConfigurationException(args.Message);			// collate warnings into a single object
//			Log.Warning(args.Message);		
		}
	}
}