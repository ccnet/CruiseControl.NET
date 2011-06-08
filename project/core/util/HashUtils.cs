namespace ThoughtWorks.CruiseControl.Core.util
{
    using System.Linq;

    /// <summary>
    /// Utilities for working with <see cref="IHashStore"/> instances.
    /// </summary>
    public static class HashUtils
    {
        #region Public methods
        #region AreSame()
        /// <summary>
        /// Checks if two hashes are the same.
        /// </summary>
        /// <param name="store1">The first store.</param>
        /// <param name="store2">The second store.</param>
        /// <returns>
        /// <c>true</c> if the hashes are the same; <c>false</c> otherwise.
        /// </returns>
        public static bool AreSame(IHashStore store1, IHashStore store2)
        {
            var hash1 = store1 != null && store1.Hash != null
                            ? store1.Hash
                            : new byte[0];
            var hash2 = store2 != null && store2.Hash != null
                            ? store2.Hash
                            : new byte[0];
            if (hash1.Length != hash2.Length)
            {
                return false;
            }

            return !hash1.Where((t, loop) => t != hash2[loop]).Any();
        }
        #endregion
        #endregion
    }
}
