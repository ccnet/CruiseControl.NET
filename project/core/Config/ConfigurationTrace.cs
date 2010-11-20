namespace ThoughtWorks.CruiseControl.Core.Config
{
    /// <summary>
    /// Defines a trace through the configuration settings.
    /// </summary>
    public sealed class ConfigurationTrace
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationTrace"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="parent">The parent.</param>
        public ConfigurationTrace(object value, ConfigurationTrace parent)
        {
            this.Value = value;
            this.Parent = parent;
        }
        #endregion

        #region Public properties
        #region Value
        /// <summary>
        /// Gets the configuration value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; private set; }
        #endregion

        #region Parent
        /// <summary>
        /// Gets the parent trace.
        /// </summary>
        /// <value>The trace.</value>
        public ConfigurationTrace Parent { get; private set; }
        #endregion
        #endregion

        #region Public methods
        #region GetAncestorValue()
        /// <summary>
        /// Gets the value of an ancestor of a type.
        /// </summary>
        /// <typeparam name="TType">The type of the ancestor.</typeparam>
        /// <returns>The value of the ancestor if found, null otherwise.</returns>
        public TType GetAncestorValue<TType>()
            where TType : class
        {
            var ancestor = this.FindAncestor<TType>();
            if (ancestor != null)
            {
                return (TType)ancestor.Value;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region FindAncestor()
        /// <summary>
        /// Finds the first ancestor of a type.
        /// </summary>
        /// <typeparam name="TType">The type of the ancestor.</typeparam>
        /// <returns>The trace for the ancestor if found, null otherwise.</returns>
        public ConfigurationTrace FindAncestor<TType>()
        {
            if ((this.Value != null) && (this.Value.GetType() == typeof(TType)))
            {
                return this;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Wrap()
        /// <summary>
        /// Wraps a configuration value in the trace.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The wrapped value.</returns>
        public ConfigurationTrace Wrap(object value)
        {
            var trace = new ConfigurationTrace(value, this);
            return trace;
        }
        #endregion

        #region Start()
        /// <summary>
        /// Starts a new configuration trace.
        /// </summary>
        /// <param name="value">The value for the root.</param>
        /// <returns>The new <see cref="ConfigurationTrace"/>.</returns>
        public static ConfigurationTrace Start(object value)
        {
            var trace = new ConfigurationTrace(value, null);
            return trace;
        }
        #endregion
        #endregion
    }
}
