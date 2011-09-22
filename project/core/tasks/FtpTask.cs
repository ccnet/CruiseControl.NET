﻿namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;

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
    /// <remarks>
    /// <includePage>Integration_Properties</includePage>
    /// </remarks>
    [ReflectorType("ftp")]
    public class FtpTask : TaskBase
    {
        /// <summary>
        /// 	
        /// </summary>
        public enum FtpAction
        {
            /// <summary>
            /// Uploads the specified folder to the ftp server
            /// </summary>
            UploadFolder,
            /// <summary>
            /// Downloads the specified folder from the ftp server
            /// </summary>
            DownloadFolder
        }
        //todo: limit number of files shown to the last 10 like in build stage : done
        //todo : change color of progress bar in cctray to red if a failure is found

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FtpTask"/> class.
        /// </summary>
        public FtpTask()
        {
            this.ServerName = string.Empty;
            this.UserName = string.Empty;
            this.Password = string.Empty;
            this.UseActiveConnectionMode = true;
            this.Action = FtpAction.DownloadFolder;
            this.FtpFolderName = string.Empty;
            this.LocalFolderName = string.Empty;
            this.RecursiveCopy = true;
        }
        #endregion

        /// <summary>
        /// The name of the server to connect to.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("serverName", Required = true)]
        public string ServerName { get; set; }

        /// <summary>
        /// The username to log in with.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("userName", Required = true)]
        public string UserName { get; set; }

        /// <summary>
        /// The password to use.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("password", Required = true)]
        public string Password { get; set; }

        /// <summary>
        /// Whether to use active connection mode or not.
        /// </summary>
        /// <version>1.5</version>
        /// <default>true</default>
        [ReflectorProperty("useActiveConnectionMode", Required = false)]
        public bool UseActiveConnectionMode { get; set; }

        /// <summary>
        /// The action to perform.
        /// </summary>
        /// <version>1.5</version>
        /// <default>DownloadFolder</default>
        [ReflectorProperty("action", Required = false)]
        public FtpAction Action { get; set; }

        /// <summary>
        /// The path to the folder to use on the FTP server.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("ftpFolderName", Required = true)]
        public string FtpFolderName { get; set; }

        /// <summary>
        /// The to the folder to use on the local machine.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("localFolderName", Required = true)]
        public string LocalFolderName { get; set; }

        /// <summary>
        /// Whether to perform a recursive copy or not.
        /// </summary>
        /// <version>1.5</version>
        /// <default>true</default>
        [ReflectorProperty("recursiveCopy", Required = false)]
        public bool RecursiveCopy { get; set; }

        /// <summary>
        /// Time difference between server and client (relative to client) in hours.
        /// </summary>
        /// <version>1.6</version>
        /// <default>0</default>
        [ReflectorProperty("timeDifference", Required = false)]
        public int TimeDifference { get; set; }



        /// <summary>
        /// Executes the specified result.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override bool Execute(IIntegrationResult result)
        {
            var relativeLocalFolder = result.BaseFromWorkingDirectory(this.LocalFolderName);
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : GetDescription(relativeLocalFolder));

            string remoteFolder = FtpFolderName;
            FtpLib ftp = new FtpLib(this,result.BuildProgressInformation);

            
            try
            {
                ftp.LogIn(ServerName, UserName, Password, UseActiveConnectionMode);

                ftp.TimeDifference = new TimeSpan(TimeDifference, 0, 0);



                if (!FtpFolderName.StartsWith("/"))
                {
                    remoteFolder = System.IO.Path.Combine(ftp.CurrentWorkingFolder(),FtpFolderName);
                }

                if (Action == FtpAction.UploadFolder)
                {
                    Log.Debug("Uploading {0} to {1}, recursive : {2}", relativeLocalFolder, remoteFolder, RecursiveCopy);
                    ftp.UploadFolder(remoteFolder, relativeLocalFolder, RecursiveCopy);
                }

                if (Action == FtpAction.DownloadFolder)
                {
                    Log.Debug("Downloading {0} to {1}, recursive : {2}", remoteFolder, relativeLocalFolder, RecursiveCopy);
                    ftp.DownloadFolder(relativeLocalFolder, remoteFolder, RecursiveCopy);
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex);
                // try to disconnect in a proper way on getting an error
                    try
                    {  // swallow exception on disconnect to keep the original error
                        if (ftp.IsConnected()) ftp.DisConnect();
                    }
                    catch { }
                Log.Info("throwing");
                throw;
            }

            return true;
        }

        private string GetDescription(string relativeLocalFolder)
        {
            if (Action == FtpAction.DownloadFolder)
            {
                return string.Concat("Downloading ", FtpFolderName, " to ", relativeLocalFolder);
            }

            return string.Concat("Uploading ", relativeLocalFolder, " to ", FtpFolderName);
        }

    }
}
