using System;
namespace ThoughtWorks.CruiseControl.Core.Util
{
    interface IFtpLib
    {
        /// <summary>
        /// Logs into the specified server, with the userName and password
        /// If activeConnectionMode is set to true, active connection is used,
        /// otherwise passive connection.  
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="activeConnectionMode"></param>
        void LogIn(string serverName, string userName, string password, bool activeConnectionMode);

        /// <summary>
        /// Disconnects from the server
        /// </summary>
        void DisConnect();

        /// <summary>
        /// returns true if connected
        /// </summary>
        /// <returns></returns>
        bool IsConnected();
        
        /// <summary>
        /// returns the current path of the server
        /// </summary>
        /// <returns></returns>
        string CurrentWorkingFolder();

        /// <summary>
        /// downloads the remoter folder to the local folder, recursive if wanted
        /// </summary>
        /// <param name="localFolder"></param>
        /// <param name="remoteFolder"></param>
        /// <param name="recursive"></param>
        void DownloadFolder(string localFolder, string remoteFolder, bool recursive);

        /// <summary>
        /// Uploads a local folder to the specified remotefolder, recursive if wanted
        /// </summary>
        /// <param name="remoteFolder"></param>
        /// <param name="localFolder"></param>
        /// <param name="recursive"></param>
        void UploadFolder(string remoteFolder, string localFolder, bool recursive);


        /// <summary>
        /// Returns a list of new or updated files at the ftp site, compared to a local folder
        /// </summary>
        /// <param name="localFolder"></param>
        /// <param name="remoteFolder"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        Modification[] ListNewOrUpdatedFilesAtFtpSite(string localFolder, string remoteFolder, bool recursive);
    }
}
