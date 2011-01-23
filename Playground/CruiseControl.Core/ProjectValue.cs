namespace CruiseControl.Core
{
    /// <summary>
    /// A value within a project.
    /// </summary>
    public class ProjectValue
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectValue"/> class.
        /// </summary>
        public ProjectValue()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectValue"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public ProjectValue(string owner, string name, string value)
        {
            this.Owner = owner;
            this.Name = name;
            this.Value = value;
        }
        #endregion

        #region Public properties
        #region Owner
        /// <summary>
        /// Gets or sets the name of the owner.
        /// </summary>
        /// <value>
        /// The owner's name.
        /// </value>
        public string Owner { get; set; }
        #endregion

        #region Name
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        #endregion

        #region Value
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }
        #endregion
        #endregion
    }
}
