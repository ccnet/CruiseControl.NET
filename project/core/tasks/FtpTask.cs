using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// <para>
    /// The ftp task / publisher allows to download or upload files/ folders, for example, uploading a new version of a web page to ftp site
    /// of an ISP.
    /// </para>
    /// </summary>
    /// <title>FTP Task / Publisher </title>
    /// <version>1.5</version>
    /// <example>
    /// <code>
    /// &lt;ftp&gt;
    /// &lt;serverName&gt;ftp.isp.com&lt;/serverName&gt;
    /// &lt;userName&gt;john&lt;/userName&gt;
    /// &lt;password&gt;doe&lt;/password&gt;
    /// &lt;action&gt;UploadFolder&lt;/action&gt;
    /// &lt;ftpFolderName&gt;site/config&lt;/ftpFolderName&gt;
    /// &lt;localFolderName&gt;d:\website\config&lt;/localFolderName&gt;
    /// &lt;recursiveCopy&gt;true&lt;/recursiveCopy&gt;
    /// &lt;/ftp&gt;
    /// </code>
    /// </example>
    [ReflectorType("ftp")]
    public class FtpTask : TaskBase
    {
        public enum FtpAction
        {
            UploadFolder,
            DownloadFolder
        }
        //todo: limit number of files shown to the last 10 like in build stage : done
        //todo : change color of progress bar in cctray to red if a failure is found

        /// <summary>
        /// The name of the server to connect to.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("serverName", Required = true)]
        public string ServerName = string.Empty;

        /// <summary>
        /// The username to log in with.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("userName", Required = true)]
        public string UserName = string.Empty;

        /// <summary>
        /// The password to use.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("password", Required = true)]
        public string Password = string.Empty;

        /// <summary>
        /// Whether to use active connection mode or not.
        /// </summary>
        /// <version>1.5</version>
        /// <default>true</default>
        [ReflectorProperty("useActiveConnectionMode", Required = false)]
        public bool UseActiveConnectionMode = true;

        /// <summary>
        /// The action to perform.
        /// </summary>
        /// <version>1.5</version>
        /// <default>DownloadFolder</default>
        [ReflectorProperty("action", Required = false)]
        public FtpAction Action = FtpAction.DownloadFolder;

        /// <summary>
        /// The path to the folder to use on the FTP server.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("ftpFolderName", Required = true)]
        public string FtpFolderName = string.Empty;

        /// <summary>
        /// The to the folder to use on the local machine.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("localFolderName", Required = true)]
        public string LocalFolderName = string.Empty;

        /// <summary>
        /// Whether to perform a recursive copy or not.
        /// </summary>
        /// <version>1.5</version>
        /// <default>true</default>
        [ReflectorProperty("recursiveCopy", Required = false)]
        public bool RecursiveCopy = true;

        protected override bool Execute(IIntegrationResult result)
        {
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : GetDescription());

            string remoteFolder = FtpFolderName;
            FtpLib ftp = new FtpLib(this,result.BuildProgressInformation);

            
            try
            {
                ftp.LogIn(ServerName, UserName, Password, UseActiveConnectionMode);

                if (!FtpFolderName.StartsWith("/"))
                {
                    remoteFolder = System.IO.Path.Combine(ftp.CurrentWorkingFolder(),FtpFolderName);
                }

                if (Action == FtpAction.UploadFolder)
                {
                    Log.Debug("Uploading {0} to {1}, recursive : {2}", LocalFolderName, remoteFolder, RecursiveCopy);
                    ftp.UploadFolder(remoteFolder, LocalFolderName, RecursiveCopy);
                }

                if (Action == FtpAction.DownloadFolder)
                {
                    Log.Debug("Downloading {0} to {1}, recursive : {2}", remoteFolder, LocalFolderName, RecursiveCopy);
                    ftp.DownloadFolder(LocalFolderName, remoteFolder, RecursiveCopy);
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex);
                // try to disconnect in a proper way on getting an error
                if (ftp != null)
                {
                    try
                    {  // swallow exception on disconnect to keep the original error
                        if (ftp.IsConnected()) ftp.DisConnect();
                    }
                    catch { }
                }
                Log.Info("throwing");
                throw;
            }

            return true;
        }

        private string GetDescription()
        {
            if (Action == FtpAction.DownloadFolder)
            {
                return string.Concat("Downloading ", FtpFolderName, " to ", LocalFolderName);
            }

            return string.Concat("Uploading ", LocalFolderName, " to ", FtpFolderName);
        }

    }
}
