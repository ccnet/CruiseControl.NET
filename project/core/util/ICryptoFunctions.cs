namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// Defines some common crypto functions.
    /// </summary>
    public interface ICryptoFunctions
    {
        #region Public methods
        #region GenerateHash()
        /// <summary>
        /// Generates a security hash of a value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The new hash in base64.</returns>
        string GenerateHash(string value);
        #endregion
        #endregion
    }
}
