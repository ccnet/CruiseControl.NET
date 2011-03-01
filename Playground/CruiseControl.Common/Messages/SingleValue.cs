namespace CruiseControl.Common.Messages
{
    /// <summary>
    /// A message that passes a single value.
    /// </summary>
    public class SingleValue
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleValue"/> class.
        /// </summary>
        public SingleValue()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleValue"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public SingleValue(string value)
        {
            this.Value = value;
        }
        #endregion

        #region Public properties
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
