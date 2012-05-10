using System.IO;
using System.Xml;
using System.Xml.Schema;
using ThoughtWorks.CruiseControl.Core.Config.Preprocessor;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Config
{
    /// <summary>
    /// 	
    /// </summary>
	public class DefaultConfigurationFileLoader : IConfigurationFileLoader
	{
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const string XsdSchemaResourceName = "ThoughtWorks.CruiseControl.Core.Config.ccnet.xsd";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string PreprocessorXsltResourceName = "ThoughtWorks.CruiseControl.Core.Config.preprocessor.xslt";

		private ValidationEventHandler handler;
        private INetReflectorConfigurationReader reader;
	    private ConfigPreprocessor preprocessor = new ConfigPreprocessor( new PreprocessorSettings
            {
                IgnoreWhitespace = true
            });

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultConfigurationFileLoader" /> class.	
        /// </summary>
        /// <remarks></remarks>
	    public DefaultConfigurationFileLoader() : this(new NetReflectorConfigurationReader())
		{}

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultConfigurationFileLoader" /> class.	
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <remarks></remarks>
        public DefaultConfigurationFileLoader(INetReflectorConfigurationReader reader)
		{
			this.reader = reader;
			handler = new ValidationEventHandler(HandleSchemaEvent);
		}

        /// <summary>
        /// Loads the specified config file.	
        /// </summary>
        /// <param name="configFile">The config file.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public IConfiguration Load(FileInfo configFile)
		{
			Log.Info(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Reading configuration file \"{0}\"", configFile.FullName));
			return PopulateProjectsFromXml(LoadConfiguration(configFile));
		}

        /// <summary>
        /// Adds the subfile loaded handler.	
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <remarks></remarks>
	    public void AddSubfileLoadedHandler (
	        ConfigurationSubfileLoadedHandler handler)
	    {
	        preprocessor.SubfileLoaded += handler;
	    }

	    // TODO - this should be private - update tests and make it so
        /// <summary>
        /// Loads the configuration.	
        /// </summary>
        /// <param name="configFile">The config file.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public XmlDocument LoadConfiguration(FileInfo configFile)
		{
            VerifyConfigFileExists(configFile);

            string previousCurrentDirectory = Directory.GetCurrentDirectory();
            try
            {
                if (Path.IsPathRooted(configFile.DirectoryName))
                    Directory.SetCurrentDirectory(configFile.DirectoryName);

                XmlDocument config = AttemptLoadConfiguration(configFile);
                return config;
            }
            finally
            {
                Directory.SetCurrentDirectory(previousCurrentDirectory);
            }
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
                    preprocessor.PreProcess(reader, writer, new XmlUrlResolver(), null);
                }
            }
            XmlReaderSettings settings = new XmlReaderSettings();
		    settings.ConformanceLevel = ConformanceLevel.Auto;
		    settings.ProhibitDtd = false;
		    settings.IgnoreWhitespace = true;
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
