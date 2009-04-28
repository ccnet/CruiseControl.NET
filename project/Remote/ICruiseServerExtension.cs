using System;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// Provides an extension to ICruiseServer basic functionality.
    /// </summary>
    public interface ICruiseServerExtension
    {
        #region Initialise()
        /// <summary>
        /// Initialises the extension.
        /// </summary>
        /// <param name="server">The server that this extension is for.</param>
        void Initialise(ICruiseServer server, ExtensionConfiguration extensionConfig);
        #endregion

        #region Start()
        /// <summary>
        /// Starts the extension.
        /// </summary>
        void Start();
        #endregion

        #region Stop()
        /// <summary>
        /// Stops the extension.
        /// </summary>
        void Stop();
        #endregion

        #region Abort()
        /// <summary>
        /// Terminates the extension immediately.
        /// </summary>
        void Abort();
        #endregion
    }
}
