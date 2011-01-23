namespace CruiseControl.Core.Interfaces
{
    using System.IO;

    /// <summary>
    /// Abstracts the service for loading configuration.
    /// </summary>
    public interface IConfigurationService
    {
        #region Public methods
        #region Load()
        /// <summary>
        /// Loads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        Server Load(Stream stream);
        #endregion
        #endregion
    }
}