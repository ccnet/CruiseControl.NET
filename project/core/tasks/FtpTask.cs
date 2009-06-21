using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    [ReflectorType("ftp")]
    class FtpTask : TaskBase
    {
        public enum FtpAction
        {
            UploadFolder,
            DownloadFolder
        }

        [ReflectorProperty("serverName", Required = true)]
        public string ServerName = string.Empty;

        [ReflectorProperty("userName", Required = true)]
        public string UserName = string.Empty;

        [ReflectorProperty("password", Required = true)]
        public string Password = string.Empty;

        [ReflectorProperty("useActiveConnectionMode", Required = true)]
        public bool UseActiveConnectionMode = true;

        [ReflectorProperty("action", Required = true)]
        public FtpAction Action = FtpAction.DownloadFolder;

        [ReflectorProperty("ftpFolderName", Required = true)]
        public string FtpFolderName = string.Empty;

        [ReflectorProperty("localFolderName", Required = true)]
        public string LocalFolderName = string.Empty;

        [ReflectorProperty("recursiveCopy", Required = true)]
        public bool RecursiveCopy = true;

        protected override bool Execute(IIntegrationResult result)
        {
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : "Ftp");

            string remoteFolder = FtpFolderName;
            FtpLib ftp = new FtpLib();

            try
            {
                ftp.LogIn(ServerName, UserName, Password, UseActiveConnectionMode);

                if (!FtpFolderName.StartsWith("/"))
                {
                    remoteFolder = ftp.CurrentWorkingFolder() + "/" + FtpFolderName;
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
                // try to disconnect in a proper way on getting an error
                if (ftp != null)
                {
                    try
                    {  // swallow exception on disconnect to keep the original error
                        if (ftp.IsConnected()) ftp.DisConnect();
                    }
                    catch { }
                }
                throw ex;
            }

            return true;
        }

    }
}
