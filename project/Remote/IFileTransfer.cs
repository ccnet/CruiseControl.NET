using System.IO;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// Allow files to be transfered.
    /// </summary>
    public interface IFileTransfer
    {
        #region Methods
        #region Download()
        /// <summary>
        /// Download a file.
        /// </summary>
        /// <param name="destination">The destination to download the file to.</param>
        void Download(Stream destination);
        #endregion
        #endregion
    }
}
