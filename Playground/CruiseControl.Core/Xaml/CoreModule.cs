namespace CruiseControl.Core.Xaml
{
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Utilities;
    using Ninject.Modules;
    using NLog;

    /// <summary>
    /// Defines the IoC mappings.
    /// </summary>
    public class CoreModule
        : NinjectModule
    {
        #region Private fields
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Public methods
        #region Load()
        /// <summary>
        /// Loads this instance.
        /// </summary>
        public override void Load()
        {
            logger.Debug("Initialising IoC mappings");
            this.Bind<IActionInvoker>().To<ActionInvoker>().InSingletonScope();
            this.Bind<IClock>().To<SystemClock>().InSingletonScope();
            this.Bind<IFileSystem>().To<FileSystem>().InSingletonScope();
            this.Bind<ITaskExecutionFactory>().To<TaskExecutionFactory>().InSingletonScope();
            this.Bind<IServerConnectionFactory>().To<ServerConnectionFactory>().InSingletonScope();
        }
        #endregion
        #endregion
    }
}
