namespace CruiseControl.Core
{
    /// <summary>
    /// Defines a remote action that is available.
    /// </summary>
    public class RemoteActionDefinition
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteActionDefinition"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        public RemoteActionDefinition(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }
        #endregion

        #region Public properties
        #region Name
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }
        #endregion

        #region Description
        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description { get; private set; }
        #endregion

        #region InputMessage
        /// <summary>
        /// Gets or sets the type of input message.
        /// </summary>
        /// <value>
        /// The input message.
        /// </value>
        public string InputMessage { get; set; }
        #endregion

        #region OutputMessage
        /// <summary>
        /// Gets or sets the type of output message.
        /// </summary>
        /// <value>
        /// The output message.
        /// </value>
        public string OutputMessage { get; set; }
        #endregion
        #endregion
    }
}
