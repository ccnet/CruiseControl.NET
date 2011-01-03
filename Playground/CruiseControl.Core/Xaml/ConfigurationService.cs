namespace CruiseControl.Core.Xaml
{
    using System.IO;
    using System.Xaml;
    using CruiseControl.Core.Interfaces;
    using Ninject;
    using NLog;

    /// <summary>
    /// A configuration service that uses XAML.
    /// </summary>
    public class ConfigurationService 
        : IConfigurationService
    {
        #region Private fields
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IKernel kernel;
        private readonly XamlSchemaContext context;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationService"/> class.
        /// </summary>
        public ConfigurationService()
        {
            this.kernel = new StandardKernel(new CoreModule());
            this.context = new CoreXamlSchemaContext(this.kernel);
        }
        #endregion

        #region Public methods
        #region Load()
        /// <summary>
        /// Loads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public Server Load(Stream stream)
        {
            logger.Debug("Loading configuration from stream");
            var reader = new XamlXmlReader(stream, this.context);
            var server = XamlServices.Load(reader) as Server;
            return server;
        }
        #endregion
        #endregion
    }
}
