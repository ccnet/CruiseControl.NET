
namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// Wrapper class around the EnterpriseDT Ftp library
    /// </summary>
    public class FtpLib : IFtpLib
    {
        private EnterpriseDT.Net.Ftp.FTPConnection ftpServer;
        private Tasks.TaskBase callingTask;
        private BuildProgressInformation bpi;


        /// <summary>
        /// Initializes a new instance of the <see cref="FtpLib" /> class.	
        /// </summary>
        /// <param name="callingTask">The calling task.</param>
        /// <param name="buildProgressInformation">The build progress information.</param>
        /// <remarks></remarks>
        public FtpLib(Tasks.TaskBase callingTask, BuildProgressInformation buildProgressInformation)
        {
            this.callingTask = callingTask;
            bpi = buildProgressInformation;

            this.ftpServer = new EnterpriseDT.Net.Ftp.FTPConnection();

            this.ftpServer.ReplyReceived += HandleMessages;

            this.ftpServer.CommandSent += HandleMessages;

            this.ftpServer.Downloaded += new EnterpriseDT.Net.Ftp.FTPFileTransferEventHandler(FtpServer_Downloaded);

            this.ftpServer.Uploaded += new EnterpriseDT.Net.Ftp.FTPFileTransferEventHandler(FtpServer_Uploaded);

        }


        /// <summary>
        /// Initializes a new instance of the <see cref="FtpLib" /> class.	
        /// </summary>
        /// <param name="buildProgressInformation">The build progress information.</param>
        /// <remarks></remarks>
        public FtpLib(BuildProgressInformation buildProgressInformation)
        {
            bpi = buildProgressInformation;

            this.ftpServer = new EnterpriseDT.Net.Ftp.FTPConnection();

            this.ftpServer.ReplyReceived += HandleMessages;

            this.ftpServer.CommandSent += HandleMessages;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpLib" /> class.	
        /// </summary>
        /// <remarks></remarks>
        public FtpLib()
        {
            this.ftpServer = new EnterpriseDT.Net.Ftp.FTPConnection();

            this.ftpServer.ReplyReceived += HandleMessages;

            this.ftpServer.CommandSent += HandleMessages;

        }

        /// <summary>
        /// Gets or sets the time difference.	
        /// </summary>
        /// <value>The time difference.</value>
        /// <remarks></remarks>
        public System.TimeSpan TimeDifference
        {
            get
            {
                return ftpServer.TimeDifference;
            }
 
            set
            {
                ftpServer.TimeDifference = value;
            }
        }



        /// <summary>
        /// Logs the in.	
        /// </summary>
        /// <param name="serverName">Name of the server.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="activeConnectionMode">The active connection mode.</param>
        /// <remarks></remarks>
        public void LogIn(string serverName, string userName, string password, bool activeConnectionMode)
        {

            Log.Info("Connecting to {0} ...", serverName);

            {
                this.ftpServer.ServerAddress = serverName;
                this.ftpServer.UserName = userName;
                this.ftpServer.Password = password;
                this.ftpServer.Connect();

                if (activeConnectionMode)
                {
                    Log.Trace("Active mode enabled");
                    this.ftpServer.ConnectMode = EnterpriseDT.Net.Ftp.FTPConnectMode.ACTIVE;
                }
                else
                {
                    Log.Trace("Passive mode enabled");
                    this.ftpServer.ConnectMode = EnterpriseDT.Net.Ftp.FTPConnectMode.PASV;
                }

                this.ftpServer.TransferType = EnterpriseDT.Net.Ftp.FTPTransferType.BINARY;
            }
        }

        /// <summary>
        /// Downloads the folder.	
        /// </summary>
        /// <param name="localFolder">The local folder.</param>
        /// <param name="remoteFolder">The remote folder.</param>
        /// <param name="recursive">The recursive.</param>
        /// <remarks></remarks>
        public void DownloadFolder(string localFolder, string remoteFolder, bool recursive)
        {

            this.ftpServer.ChangeWorkingDirectory(remoteFolder);

            var ftpServerFileInfo = this.ftpServer.GetFileInfos();

            var fi = default(System.IO.FileInfo);

            if (!System.IO.Directory.Exists(localFolder))
            {
                Log.Trace("creating {0}", localFolder);
                System.IO.Directory.CreateDirectory(localFolder);
            }

            foreach (EnterpriseDT.Net.Ftp.FTPFile currentFileOrDirectory in ftpServerFileInfo)
            {
                if (recursive)
                {
                    if (currentFileOrDirectory.Dir && currentFileOrDirectory.Name != "." && currentFileOrDirectory.Name != "..")
                    {

                        string localTargetFolder = System.IO.Path.Combine(localFolder, currentFileOrDirectory.Name);
                        string ftpTargetFolder = string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0}/{1}", remoteFolder, currentFileOrDirectory.Name);

                        if (!System.IO.Directory.Exists(localTargetFolder))
                        {
                            Log.Trace("creating {0}", localTargetFolder);
                            System.IO.Directory.CreateDirectory(localTargetFolder);
                        }

                        DownloadFolder(localTargetFolder, ftpTargetFolder, recursive);

                        //set the ftp working folder back to the correct value
                        this.ftpServer.ChangeWorkingDirectory(remoteFolder);
                    }
                }

                if (!currentFileOrDirectory.Dir)
                {
                    bool downloadFile = false;

                    string localFile = System.IO.Path.Combine(localFolder, currentFileOrDirectory.Name);


                    // check file existence
                    if (!System.IO.File.Exists(localFile))
                    {
                        downloadFile = true;
                    }
                    else
                    {
                        //check file size
                        fi = new System.IO.FileInfo(localFile);
                        if (currentFileOrDirectory.Size != fi.Length)
                        {
                            downloadFile = true;
                            System.IO.File.Delete(localFile);
                        }
                        else
                        {
                            //check modification time
                            if (currentFileOrDirectory.LastModified != fi.CreationTime)
                            {
                                downloadFile = true;
                                System.IO.File.Delete(localFile);

                            }
                        }
                    }


                    if (downloadFile)
                    {
                        Log.Trace("Downloading {0}", currentFileOrDirectory.Name);
                        this.ftpServer.DownloadFile(localFolder, currentFileOrDirectory.Name);

                        fi = new System.IO.FileInfo(localFile);
                        fi.CreationTime = currentFileOrDirectory.LastModified;
                        fi.LastAccessTime = currentFileOrDirectory.LastModified;
                        fi.LastWriteTime = currentFileOrDirectory.LastModified;
                    }

                }

            }
        }

        /// <summary>
        /// Uploads the folder.	
        /// </summary>
        /// <param name="remoteFolder">The remote folder.</param>
        /// <param name="localFolder">The local folder.</param>
        /// <param name="recursive">The recursive.</param>
        /// <remarks></remarks>
        public void UploadFolder(string remoteFolder, string localFolder, bool recursive)
        {

            string[] localFiles = null;

            localFiles = System.IO.Directory.GetFiles(localFolder, "*.*");
            this.ftpServer.ChangeWorkingDirectory(remoteFolder);


            // remove the local folder value, so we can work relative
            for (int i = 0; i <= localFiles.Length - 1; i++)
            {
                localFiles[i] = localFiles[i].Remove(0, localFolder.Length + 1);
            }


            //upload files
            //ftpServer.Exists throws an error, so we must do it ourselves
            EnterpriseDT.Net.Ftp.FTPFile[] ftpServerFileInfo = this.ftpServer.GetFileInfos();


            foreach (var localFile in localFiles)
            {
                if (!FileExistsAtFtp(ftpServerFileInfo, localFile))
                {
                    this.ftpServer.UploadFile(System.IO.Path.Combine(localFolder, localFile), localFile);
                }
                else
                {
                    if (FileIsDifferentAtFtp(ftpServerFileInfo, localFile, localFolder))
                    {
                        this.ftpServer.DeleteFile(localFile);
                        this.ftpServer.UploadFile(System.IO.Path.Combine(localFolder, localFile), localFile);
                    }

                }
            }


            if (!recursive) return;

            //upload folders
            string[] folders = null;

            string localTargetFolder = null;
            string ftpTargetFolder = null;


            folders = System.IO.Directory.GetDirectories(localFolder);

            // remove the local folder value, so we can work relative
            for (int i = 0; i <= folders.Length - 1; i++)
            {
                folders[i] = folders[i].Remove(0, localFolder.Length + 1);
            }


            foreach (var folder in folders)
            {
                //explicit set the folder back, because of recursive calls
                this.ftpServer.ChangeWorkingDirectory(remoteFolder);


                if (!FolderExistsAtFtp(ftpServerFileInfo, folder))
                {
                    this.ftpServer.CreateDirectory(folder);
                }

                localTargetFolder = System.IO.Path.Combine(localFolder, folder);
                ftpTargetFolder = string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0}/{1}", remoteFolder, folder);

                UploadFolder(ftpTargetFolder, localTargetFolder, recursive);
            }
        }

        /// <summary>
        /// Dises the connect.	
        /// </summary>
        /// <remarks></remarks>
        public void DisConnect()
        {
            this.ftpServer.Close();
        }

        /// <summary>
        /// Determines whether this instance is connected.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsConnected()
        {
            return this.ftpServer.IsConnected;
        }

        /// <summary>
        /// Currents the working folder.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string CurrentWorkingFolder()
        {
            return this.ftpServer.ServerDirectory;
        }

        /// <summary>
        /// Lists the new or updated files at FTP site.	
        /// </summary>
        /// <param name="localFolder">The local folder.</param>
        /// <param name="remoteFolder">The remote folder.</param>
        /// <param name="recursive">The recursive.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Modification[] ListNewOrUpdatedFilesAtFtpSite(string localFolder, string remoteFolder, bool recursive)
        {
            System.Collections.Generic.List<Modification> mods = new System.Collections.Generic.List<Modification>();

            GetTheList(mods, localFolder, remoteFolder, recursive);

            return mods.ToArray();
        }

        private void GetTheList(System.Collections.Generic.List<Modification> mods, string localFolder, string remoteFolder, bool recursive)
        {
            this.ftpServer.ChangeWorkingDirectory(remoteFolder);

            EnterpriseDT.Net.Ftp.FTPFile[] ftpServerFileInfo = this.ftpServer.GetFileInfos();

            string localTargetFolder = null;
            string ftpTargetFolder = null;
            bool downloadFile = false;
            string localFile = null;
            System.IO.FileInfo fi = default(System.IO.FileInfo);

            if (!System.IO.Directory.Exists(localFolder))
            {
                Log.Trace("creating {0}", localFolder);
                System.IO.Directory.CreateDirectory(localFolder);
            }

            foreach (EnterpriseDT.Net.Ftp.FTPFile currentFileOrDirectory in ftpServerFileInfo)
            {
                if (recursive)
                {
                    if (currentFileOrDirectory.Dir && currentFileOrDirectory.Name != "." && currentFileOrDirectory.Name != "..")
                    {

                        localTargetFolder = System.IO.Path.Combine(localFolder, currentFileOrDirectory.Name);
                        ftpTargetFolder = string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0}/{1}", remoteFolder, currentFileOrDirectory.Name);

                        if (!System.IO.Directory.Exists(localTargetFolder))
                        {
                            Log.Trace("creating {0}", localTargetFolder);
                            System.IO.Directory.CreateDirectory(localTargetFolder);
                        }

                        GetTheList(mods, localTargetFolder, ftpTargetFolder, recursive);

                        //set the ftp working folder back to the correct value
                        this.ftpServer.ChangeWorkingDirectory(remoteFolder);
                    }
                }

                if (!currentFileOrDirectory.Dir)
                {
                    downloadFile = false;
                    Modification m = new Modification();

                    localFile = System.IO.Path.Combine(localFolder, currentFileOrDirectory.Name);


                    // check file existence
                    if (!System.IO.File.Exists(localFile))
                    {
                        downloadFile = true;
                        m.Type = "added";
                    }
                    else
                    {
                        //check file size
                        fi = new System.IO.FileInfo(localFile);
                        if (currentFileOrDirectory.Size != fi.Length)
                        {
                            downloadFile = true;
                            m.Type = "Updated";
                        }
                        else
                        {
                            //check modification time
                            if (currentFileOrDirectory.LastModified != fi.CreationTime)
                            {
                                downloadFile = true;
                                m.Type = "Updated";
                            }
                        }
                    }

                    if (downloadFile)
                    {                        
                        m.FileName = currentFileOrDirectory.Name;
                        m.FolderName = remoteFolder;
                        m.ModifiedTime = currentFileOrDirectory.LastModified;
                        
                        mods.Add(m);
                    }
                }
            }
        }

        private bool FileExistsAtFtp(EnterpriseDT.Net.Ftp.FTPFile[] ftpServerFileInfo, string localFileName)
        {

            bool found = false;

            foreach (EnterpriseDT.Net.Ftp.FTPFile currentFileOrDirectory in ftpServerFileInfo)
            {
                if (!currentFileOrDirectory.Dir && currentFileOrDirectory.Name.ToLower() == localFileName.ToLower())
                {
                    found = true;
                }
            }

            return found;
        }

        private bool FolderExistsAtFtp(EnterpriseDT.Net.Ftp.FTPFile[] ftpServerFileInfo, string localFileName)
        {

            bool found = false;
            string updatedFolderName = null;

            foreach (EnterpriseDT.Net.Ftp.FTPFile currentFileOrDirectory in ftpServerFileInfo)
            {
                if (currentFileOrDirectory.Name.EndsWith("/"))
                {
                    updatedFolderName = currentFileOrDirectory.Name.Remove(currentFileOrDirectory.Name.Length - 1, 1);
                }
                else
                {
                    updatedFolderName = currentFileOrDirectory.Name;
                }

                if (currentFileOrDirectory.Dir && updatedFolderName.ToLower() == localFileName.ToLower())
                {
                    found = true;
                }
            }

            return found;
        }

        private bool FileIsDifferentAtFtp(EnterpriseDT.Net.Ftp.FTPFile[] ftpServerFileInfo, string localFile, string localFolder)
        {
            bool isDifferent = false;
            System.IO.FileInfo fi = default(System.IO.FileInfo);


            foreach (EnterpriseDT.Net.Ftp.FTPFile currentFileOrDirectory in ftpServerFileInfo)
            {
                if (!currentFileOrDirectory.Dir && currentFileOrDirectory.Name.ToLower() == localFile.ToLower())
                {
                    fi = new System.IO.FileInfo(System.IO.Path.Combine(localFolder, localFile));

                    if (fi.Length != currentFileOrDirectory.Size || fi.LastWriteTime > currentFileOrDirectory.LastModified)
                    {
                        isDifferent = true;
                    }
                }
            }

            return isDifferent;
        }

        private void HandleMessages(object sender, EnterpriseDT.Net.Ftp.FTPMessageEventArgs e)
        {
            bpi.AddTaskInformation(e.Message);

            Log.Trace(e.Message);
        }

        private void FtpServer_Uploaded(object sender, EnterpriseDT.Net.Ftp.FTPFileTransferEventArgs e)
        {
            string file;
            if (!e.RemoteDirectory.EndsWith("/"))
                file = string.Concat("Uploaded : ", e.RemoteDirectory, "/", e.RemoteFile);
            else
                file = string.Concat("Uploaded : ", e.RemoteDirectory, e.RemoteFile);

            AddTaskStatusItem(file);
        }

        private void FtpServer_Downloaded(object sender, EnterpriseDT.Net.Ftp.FTPFileTransferEventArgs e)
        {
            string file;
            if (!e.RemoteDirectory.EndsWith("/"))
                file = string.Concat("Downloaded : ", e.RemoteDirectory, "/", e.RemoteFile);
            else
                file = string.Concat("Downloaded : ", e.RemoteDirectory, e.RemoteFile);

            AddTaskStatusItem(file);
        }

        private void AddTaskStatusItem(string information)
        {
            callingTask.CurrentStatus.AddChild(new ThoughtWorks.CruiseControl.Remote.ItemStatus(information));

            if (callingTask.CurrentStatus.ChildItems.Count > 10)
            {
                callingTask.CurrentStatus.ChildItems.RemoveAt(0);
            }

        }
    }
}