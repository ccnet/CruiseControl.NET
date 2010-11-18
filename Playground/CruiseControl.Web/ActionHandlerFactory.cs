namespace CruiseControl.Web
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Configuration;
    using System.Reflection;
    using System.ComponentModel.Composition.Primitives;
    using System.IO;
    using NLog;

    /// <summary>
    /// Factory for resolving <see cref="ActionHandler"/> instances.
    /// </summary>
    public class ActionHandlerFactory
    {
        #region Private fields
        private static readonly Logger sLogger = LogManager.GetCurrentClassLogger();
        private static ActionHandlerFactory sDefaultFactory;
        private static readonly object FactoryLock = new object();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the <see cref="ActionHandlerFactory"/> class.
        /// </summary>
        static ActionHandlerFactory()
        {
            PluginFolder = ConfigurationStore.PluginDirectory;
            sLogger.Debug("Plug-ins folder is {0}", PluginFolder);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionHandlerFactory"/> class.
        /// </summary>
        public ActionHandlerFactory()
        {
            this.ActionHandlers = new List<Lazy<ActionHandler, IActionHandlerMetadata>>();
        }
        #endregion

        #region Public properties
        #region PluginDirectory
        /// <summary>
        /// Gets or sets the plug-in directory.
        /// </summary>
        /// <value>The plug-in directory.</value>
        /// <remarks>
        /// This is the location that the factory will search for any plug-ins.
        /// </remarks>
        public static string PluginFolder { get; set; }
        #endregion

        #region Default
        /// <summary>
        /// Gets or sets the default factory.
        /// </summary>
        /// <value>The default factory.</value>
        public static ActionHandlerFactory Default
        {
            get
            {
                if (sDefaultFactory == null)
                {
                    lock (FactoryLock)
                    {
                        if (sDefaultFactory == null)
                        {
                            // Generate the new factory and compose it
                            sLogger.Debug("Initialising ActionHandlerFactory");
                            sDefaultFactory = new ActionHandlerFactory();
                            ComposablePartCatalog catalog = new AssemblyCatalog(                                
                                Assembly.GetExecutingAssembly());
                            if (!string.IsNullOrEmpty(PluginFolder) &&
                                Directory.Exists(PluginFolder))
                            {
                                sLogger.Debug("Loading plug-ins from {0}", PluginFolder);
                                catalog = new AggregateCatalog(
                                    catalog,
                                    new DirectoryCatalog(PluginFolder));
                            }

                            var container = new CompositionContainer(catalog);
                            container.ComposeParts(sDefaultFactory);
                            sLogger.Debug("{0} action handler(s) found", sDefaultFactory.ActionHandlers.Count);
                        }
                    }
                }

                return sDefaultFactory;
            }

            set { sDefaultFactory = value; }
        }
        #endregion

        #region ActionHandlers
        /// <summary>
        /// Gets or sets the action handlers.
        /// </summary>
        /// <value>The action handlers.</value>
        [ImportMany(AllowRecomposition = true)]
        public IList<Lazy<ActionHandler, IActionHandlerMetadata>> ActionHandlers { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Reset()
        /// <summary>
        /// Resets the default factory.
        /// </summary>
        public static void Reset()
        {
            lock (FactoryLock)
            {
                // Set it to null so it will be reloaded automatically
                sLogger.Debug("Resetting ActionHandlerFactory");
                sDefaultFactory = null;
            }
        }
        #endregion
        #endregion
    }
}