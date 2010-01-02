
namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// Stores a string that can be access either publicly (and is hidden) or privately (accessed normally).
    /// </summary>
    public sealed class PrivateString
        : IPrivateData
    {
        #region Public properties
        #region PrivateValue
        /// <summary>
        /// Gets or sets the private value.
        /// </summary>
        /// <value>The private (actual) value.</value>
        public string PrivateValue { get; set; }
        #endregion

        #region PublicValue
        /// <summary>
        /// Gets the public value.
        /// </summary>
        /// <value>The public value.</value>
        public string PublicValue
        {
            get { return "********"; }
        }
        #endregion
        #endregion

        #region Public methods
        #region ToString()
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// <remarks>
        /// This will return the data in public (hidden) mode.
        /// </remarks>
        public override string ToString()
        {
            return this.ToString(SecureDataMode.Public);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance in the specified data mode.
        /// </summary>
        /// <param name="dataMode">The data mode to use.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(SecureDataMode dataMode)
        {
            switch (dataMode)
            {
                case SecureDataMode.Private:
                    return this.PrivateValue;

                default:
                    return this.PublicValue;
            }
        }
        #endregion
        #endregion

        #region Operators
        #region implicit
        /// <summary>
        /// Performs an implicit conversion from <see cref="String"/> to <see cref="PrivateString"/>.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator PrivateString(string args)
        {
            return new PrivateString { PrivateValue = args };
        }
        #endregion
        #endregion
    }
}
