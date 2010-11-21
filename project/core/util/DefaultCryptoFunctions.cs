namespace ThoughtWorks.CruiseControl.Core.Util
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// 	
    /// </summary>
    public class DefaultCryptoFunctions
        : ICryptoFunctions
    {
        #region Public methods
        #region GenerateHash()
        /// <summary>
        /// Generates a security hash of a value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The new hash in base64.</returns>
        public string GenerateHash(string value)
        {
            var sha = new SHA512Managed();
            var data = Encoding.UTF8.GetBytes(value);
            var hashData = sha.ComputeHash(data);
            var hash = Convert.ToBase64String(hashData);
            return hash;
        }
        #endregion
        #endregion
    }
}
