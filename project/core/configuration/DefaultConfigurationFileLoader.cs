using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Xsl;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Config.Preprocessor;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	public class DefaultConfigurationFileLoader : IConfigurationFileLoader
	{
		public const string XsdSchemaResourceName = "ThoughtWorks.CruiseControl.Core.configuration.ccnet.xsd";
        public const string PreprocessorXsltResourceName = "ThoughtWorks.CruiseControl.Core.configuration.preprocessor.xslt";

		private ValidationEventHandler handler;
        private INetReflectorConfigurationReader reader;
	    private ConfigPreprocessor preprocessor = new ConfigPreprocessor();

	    public DefaultConfigurationFileLoader() : this(new NetReflectorConfigurationReader())
		{}

        public DefaultConfigurationFileLoader(INetReflectorConfigurationReader reader)
		{
			this.reader = reader;
			handler = new ValidationEventHandler(HandleSchemaEvent);
		}

		public IConfiguration Load(FileInfo configFile)
		{
			Log.Info(String.Format("Reading configuration file \"{0}\"", configFile.FullName));
			return PopulateProjectsFromXml(LoadConfiguration(configFile));
		}

	    public void AddSubfileLoadedHandler (
	        ConfigurationSubfileLoadedHandler handler)
	    {
	        preprocessor.SubfileLoaded += handler;
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
			try
			{
				return CreateXmlValidatingLoader(configFile).Load();
			}
			catch (XmlException ex)
			{
				throw new ConfigurationException("The configuration file contains invalid xml: " + configFile.FullName, ex);
			}
		}

		private XmlValidatingLoader CreateXmlValidatingLoader(FileInfo configFile)
		{
            XmlDocument doc = new XmlDocument();
            
            // Run the config file through the preprocessor.
            XmlReaderSettings settings2 = new XmlReaderSettings();
            settings2.ProhibitDtd = false;
            using (XmlReader reader = XmlReader.Create(configFile.FullName, settings2))
            {
                using( XmlWriter writer = doc.CreateNavigator().AppendChild() )
                {                                        
                    preprocessor.PreProcess( reader, writer, null, null );
                }
            }
            XmlReaderSettings settings = new XmlReaderSettings();
		    settings.ConformanceLevel = ConformanceLevel.Auto;
		    settings.ProhibitDtd = false;
            // Wrap the preprocessed output with an XmlValidatingLoader
		    XmlValidatingLoader loader =
		        new XmlValidatingLoader( XmlReader.Create( doc.CreateNavigator().ReadSubtree(), settings ) );
			loader.ValidationEventHandler += handler;
			return loader;
		}

		private static void VerifyConfigFileExists(FileInfo configFile)
		{
			if (! configFile.Exists)
			{
				throw new ConfigurationFileMissingException("Specified configuration file does not exist: " + configFile.FullName);
			}
		}

		private IConfiguration PopulateProjectsFromXml(XmlDocument configXml)
		{
			return reader.Read(configXml, null);
		}

		private static void HandleSchemaEvent(object sender, ValidationEventArgs args)
		{
			Log.Info("Loading config schema: " + args.Message);
		}
	}    
}
