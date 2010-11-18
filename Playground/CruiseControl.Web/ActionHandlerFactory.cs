namespace CruiseControl.Web
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;

    /// <summary>
    /// Factory for resolving <see cref="ActionHandler"/> instances.
    /// </summary>
    public class ActionHandlerFactory
    {
        #region Private fields
        private static ActionHandlerFactory sDefaultFactory;
        private static readonly object FactoryLock = new object();
        #endregion

        #region Public properties
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
                            sDefaultFactory = new ActionHandlerFactory();
                            var container = new CompositionContainer();
                            container.ComposeParts(sDefaultFactory);
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
        [ImportMany]
        public IList<Lazy<ActionHandler, IActionHandlerMetadata>> ActionHandlers { get; set; }
        #endregion
        #endregion
    }
}