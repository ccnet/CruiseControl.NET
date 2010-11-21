
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

            string[] LocalFiles = null;

            LocalFiles = System.IO.Directory.GetFiles(localFolder, "*.*");
            this.ftpServer.ChangeWorkingDirectory(remoteFolder);


            // remove the local folder value, so we can work relative
            for (int i = 0; i <= LocalFiles.Length - 1; i++)
            {
                LocalFiles[i] = LocalFiles[i].Remove(0, localFolder.Length + 1);
            }


            //upload files
            //ftpServer.Exists throws an error, so we must do it ourselves
            EnterpriseDT.Net.Ftp.FTPFile[] ftpServerFileInfo = this.ftpServer.GetFileInfos();


            foreach (var LocalFile in LocalFiles)
            {
                if (!FileExistsAtFtp(ftpServerFileInfo, LocalFile))
                {
                    this.ftpServer.UploadFile(System.IO.Path.Combine(localFolder, LocalFile), LocalFile);
                }
                else
                {
                    if (FileIsDifferentAtFtp(ftpServerFileInfo, LocalFile, localFolder))
                    {
                        this.ftpServer.DeleteFile(LocalFile);
                        this.ftpServer.UploadFile(System.IO.Path.Combine(localFolder, LocalFile), LocalFile);
                    }

                }
            }


            if (!recursive) return;

            //upload folders
            string[] Folders = null;

            string LocalTargetFolder = null;
            string FtpTargetFolder = null;


            Folders = System.IO.Directory.GetDirectories(localFolder);

            // remove the local folder value, so we can work relative
            for (int i = 0; i <= Folders.Length - 1; i++)
            {
                Folders[i] = Folders[i].Remove(0, localFolder.Length + 1);
            }


            foreach (var Folder in Folders)
            {
                //explicit set the folder back, because of recursive calls
                this.ftpServer.ChangeWorkingDirectory(remoteFolder);


                if (!FolderExistsAtFtp(ftpServerFileInfo, Folder))
                {
                    this.ftpServer.CreateDirectory(Folder);
                }

                LocalTargetFolder = System.IO.Path.Combine(localFolder, Folder);
                FtpTargetFolder = string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0}/{1}", remoteFolder, Folder);

                UploadFolder(FtpTargetFolder, LocalTargetFolder, recursive);
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

            EnterpriseDT.Net.Ftp.FTPFile[] FtpServerFileInfo = this.ftpServer.GetFileInfos();

            string LocalTargetFolder = null;
            string FtpTargetFolder = null;
            bool DownloadFile = false;
            string LocalFile = null;
            System.IO.FileInfo fi = default(System.IO.FileInfo);

            if (!System.IO.Directory.Exists(localFolder))
            {
                Log.Trace("creating {0}", localFolder);
                System.IO.Directory.CreateDirectory(localFolder);
            }

            foreach (EnterpriseDT.Net.Ftp.FTPFile CurrentFileOrDirectory in FtpServerFileInfo)
            {
                if (recursive)
                {
                    if (CurrentFileOrDirectory.Dir && CurrentFileOrDirectory.Name != "." && CurrentFileOrDirectory.Name != "..")
                    {

                        LocalTargetFolder = System.IO.Path.Combine(localFolder, CurrentFileOrDirectory.Name);
                        FtpTargetFolder = string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0}/{1}", remoteFolder, CurrentFileOrDirectory.Name);

                        if (!System.IO.Directory.Exists(LocalTargetFolder))
                        {
                            Log.Trace("creating {0}", LocalTargetFolder);
                            System.IO.Directory.CreateDirectory(LocalTargetFolder);
                        }

                        GetTheList(mods, LocalTargetFolder, FtpTargetFolder, recursive);

                        //set the ftp working folder back to the correct value
                        this.ftpServer.ChangeWorkingDirectory(remoteFolder);
                    }
                }

                if (!CurrentFileOrDirectory.Dir)
                {
                    DownloadFile = false;
                    Modification m = new Modification();

                    LocalFile = System.IO.Path.Combine(localFolder, CurrentFileOrDirectory.Name);


                    // check file existence
                    if (!System.IO.File.Exists(LocalFile))
                    {
                        DownloadFile = true;
                        m.Type = "added";
                    }
                    else
                    {
                        //check file size
                        fi = new System.IO.FileInfo(LocalFile);
                        if (CurrentFileOrDirectory.Size != fi.Length)
                        {
                            DownloadFile = true;
                            m.Type = "Updated";
                        }
                        else
                        {
                            //check modification time
                            if (CurrentFileOrDirectory.LastModified != fi.CreationTime)
                            {
                                DownloadFile = true;
                                m.Type = "Updated";
                            }
                        }
                    }

                    if (DownloadFile)
                    {                        
                        m.FileName = CurrentFileOrDirectory.Name;
                        m.FolderName = remoteFolder;
                        m.ModifiedTime = CurrentFileOrDirectory.LastModified;
                        
                        mods.Add(m);
                    }
                }
            }
        }

        private bool FileExistsAtFtp(EnterpriseDT.Net.Ftp.FTPFile[] ftpServerFileInfo, string localFileName)
        {

            bool Found = false;

            foreach (EnterpriseDT.Net.Ftp.FTPFile CurrentFileOrDirectory in ftpServerFileInfo)
            {
                if (!CurrentFileOrDirectory.Dir && CurrentFileOrDirectory.Name.ToLower() == localFileName.ToLower())
                {
                    Found = true;
                }
            }

            return Found;
        }

        private bool FolderExistsAtFtp(EnterpriseDT.Net.Ftp.FTPFile[] ftpServerFileInfo, string localFileName)
        {

            bool Found = false;
            string updatedFolderName = null;

            foreach (EnterpriseDT.Net.Ftp.FTPFile CurrentFileOrDirectory in ftpServerFileInfo)
            {
                if (CurrentFileOrDirectory.Name.EndsWith("/"))
                {
                    updatedFolderName = CurrentFileOrDirectory.Name.Remove(CurrentFileOrDirectory.Name.Length - 1, 1);
                }
                else
                {
                    updatedFolderName = CurrentFileOrDirectory.Name;
                }

                if (CurrentFileOrDirectory.Dir && updatedFolderName.ToLower() == localFileName.ToLower())
                {
                    Found = true;
                }
            }

            return Found;
        }

        private bool FileIsDifferentAtFtp(EnterpriseDT.Net.Ftp.FTPFile[] ftpServerFileInfo, string localFile, string localFolder)
        {
            bool isDifferent = false;
            System.IO.FileInfo fi = default(System.IO.FileInfo);


            foreach (EnterpriseDT.Net.Ftp.FTPFile CurrentFileOrDirectory in ftpServerFileInfo)
            {
                if (!CurrentFileOrDirectory.Dir && CurrentFileOrDirectory.Name.ToLower() == localFile.ToLower())
                {
                    fi = new System.IO.FileInfo(System.IO.Path.Combine(localFolder, localFile));

                    if (fi.Length != CurrentFileOrDirectory.Size || fi.LastWriteTime > CurrentFileOrDirectory.LastModified)
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