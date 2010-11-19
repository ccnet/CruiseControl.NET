namespace CruiseControl.Web
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.IO;
    using System.Reflection;
    using Configuration;
    using NLog;

    /// <summary>
    /// Factory for resolving <see cref="ActionHandler"/> instances.
    /// </summary>
    public class ActionHandlerFactory
    {
        #region Private fields
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static ActionHandlerFactory DefaultFactory;
        private static readonly object FactoryLock = new object();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the <see cref="ActionHandlerFactory"/> class.
        /// </summary>
        static ActionHandlerFactory()
        {
            PluginFolder = Folders.PluginDirectory;
            Logger.Debug("Plug-ins folder is {0}", PluginFolder);
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
                if (DefaultFactory == null)
                {
                    lock (FactoryLock)
                    {
                        if (DefaultFactory == null)
                        {
                            // Generate the new factory and compose it
                            Logger.Debug("Initialising ActionHandlerFactory");
                            DefaultFactory = new ActionHandlerFactory();
                            ComposablePartCatalog catalog = new AssemblyCatalog(                                
                                Assembly.GetExecutingAssembly());
                            if (!string.IsNullOrEmpty(PluginFolder) &&
                                Directory.Exists(PluginFolder))
                            {
                                Logger.Debug("Loading plug-ins from {0}", PluginFolder);
                                catalog = new AggregateCatalog(
                                    catalog,
                                    new DirectoryCatalog(PluginFolder));
                            }

                            var container = new CompositionContainer(catalog);
                            container.ComposeParts(DefaultFactory);
                            Logger.Debug("{0} action handler(s) found", DefaultFactory.ActionHandlers.Count);
                        }
                    }
                }

                return DefaultFactory;
            }

            set { DefaultFactory = value; }
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
                Logger.Debug("Resetting ActionHandlerFactory");
                DefaultFactory = null;
            }
        }
        #endregion
        #endregion
    }
}