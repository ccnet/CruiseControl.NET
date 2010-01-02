
namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// Wrapper class around the EnterpriseDT Ftp library
    /// </summary>
    public class FtpLib : IFtpLib
    {
        private EnterpriseDT.Net.Ftp.FTPConnection FtpServer;
        private Tasks.TaskBase CallingTask;
        private Util.BuildProgressInformation bpi;


        public FtpLib(Tasks.TaskBase callingTask, Util.BuildProgressInformation buildProgressInformation)
        {
            CallingTask = callingTask;
            bpi = buildProgressInformation;

            this.FtpServer = new EnterpriseDT.Net.Ftp.FTPConnection();

            this.FtpServer.ReplyReceived += HandleMessages;

            this.FtpServer.CommandSent += HandleMessages;

            this.FtpServer.Downloaded += new EnterpriseDT.Net.Ftp.FTPFileTransferEventHandler(FtpServer_Downloaded);

            this.FtpServer.Uploaded += new EnterpriseDT.Net.Ftp.FTPFileTransferEventHandler(FtpServer_Uploaded);

        }


        public FtpLib(Util.BuildProgressInformation buildProgressInformation)
        {
            bpi = buildProgressInformation;

            this.FtpServer = new EnterpriseDT.Net.Ftp.FTPConnection();

            this.FtpServer.ReplyReceived += HandleMessages;

            this.FtpServer.CommandSent += HandleMessages;
        }

        public FtpLib()
        {
            this.FtpServer = new EnterpriseDT.Net.Ftp.FTPConnection();

            this.FtpServer.ReplyReceived += HandleMessages;

            this.FtpServer.CommandSent += HandleMessages;

        }

        public void LogIn(string serverName, string userName, string password, bool activeConnectionMode)
        {

            Log.Info("Connecting to {0} ...", serverName);

            {
                this.FtpServer.ServerAddress = serverName;
                this.FtpServer.UserName = userName;
                this.FtpServer.Password = password;
                this.FtpServer.Connect();

                if (activeConnectionMode)
                {
                    Log.Trace("Active mode enabled");
                    this.FtpServer.ConnectMode = EnterpriseDT.Net.Ftp.FTPConnectMode.ACTIVE;
                }
                else
                {
                    Log.Trace("Passive mode enabled");
                    this.FtpServer.ConnectMode = EnterpriseDT.Net.Ftp.FTPConnectMode.PASV;
                }

                this.FtpServer.TransferType = EnterpriseDT.Net.Ftp.FTPTransferType.BINARY;
            }
        }

        public void DownloadFolder(string localFolder, string remoteFolder, bool recursive)
        {

            this.FtpServer.ChangeWorkingDirectory(remoteFolder);

            EnterpriseDT.Net.Ftp.FTPFile[] FtpServerFileInfo = this.FtpServer.GetFileInfos();

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
                        FtpTargetFolder = string.Format("{0}/{1}", remoteFolder, CurrentFileOrDirectory.Name);

                        if (!System.IO.Directory.Exists(LocalTargetFolder))
                        {
                            Log.Trace("creating {0}", LocalTargetFolder);
                            System.IO.Directory.CreateDirectory(LocalTargetFolder);
                        }

                        DownloadFolder(LocalTargetFolder, FtpTargetFolder, recursive);

                        //set the ftp working folder back to the correct value
                        this.FtpServer.ChangeWorkingDirectory(remoteFolder);
                    }
                }

                if (!CurrentFileOrDirectory.Dir)
                {
                    DownloadFile = false;

                    LocalFile = System.IO.Path.Combine(localFolder, CurrentFileOrDirectory.Name);


                    // check file existence
                    if (!System.IO.File.Exists(LocalFile))
                    {
                        DownloadFile = true;
                    }
                    else
                    {
                        //check file size
                        fi = new System.IO.FileInfo(LocalFile);
                        if (CurrentFileOrDirectory.Size != fi.Length)
                        {
                            DownloadFile = true;
                            System.IO.File.Delete(LocalFile);
                        }
                        else
                        {
                            //check modification time
                            if (CurrentFileOrDirectory.LastModified != fi.CreationTime)
                            {
                                DownloadFile = true;
                                System.IO.File.Delete(LocalFile);

                            }
                        }
                    }


                    if (DownloadFile)
                    {
                        Log.Trace("Downloading {0}", CurrentFileOrDirectory.Name);
                        this.FtpServer.DownloadFile(localFolder, CurrentFileOrDirectory.Name);

                        fi = new System.IO.FileInfo(LocalFile);
                        fi.CreationTime = CurrentFileOrDirectory.LastModified;
                        fi.LastAccessTime = CurrentFileOrDirectory.LastModified;
                        fi.LastWriteTime = CurrentFileOrDirectory.LastModified;
                    }

                }

            }
        }

        public void UploadFolder(string remoteFolder, string localFolder, bool recursive)
        {

            string[] LocalFiles = null;

            LocalFiles = System.IO.Directory.GetFiles(localFolder, "*.*");
            this.FtpServer.ChangeWorkingDirectory(remoteFolder);


            // remove the local folder value, so we can work relative
            for (int i = 0; i <= LocalFiles.Length - 1; i++)
            {
                LocalFiles[i] = LocalFiles[i].Remove(0, localFolder.Length + 1);
            }


            //upload files
            //FtpServer.Exists throws an error, so we must do it ourselves
            EnterpriseDT.Net.Ftp.FTPFile[] FtpServerFileInfo = this.FtpServer.GetFileInfos();


            foreach (var LocalFile in LocalFiles)
            {
                if (!FileExistsAtFtp(FtpServerFileInfo, LocalFile))
                {
                    this.FtpServer.UploadFile(System.IO.Path.Combine(localFolder, LocalFile), LocalFile);
                }
                else
                {
                    if (FileIsDifferentAtFtp(FtpServerFileInfo, LocalFile, localFolder))
                    {
                        this.FtpServer.DeleteFile(LocalFile);
                        this.FtpServer.UploadFile(System.IO.Path.Combine(localFolder, LocalFile), LocalFile);
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
                this.FtpServer.ChangeWorkingDirectory(remoteFolder);


                if (!FolderExistsAtFtp(FtpServerFileInfo, Folder))
                {
                    this.FtpServer.CreateDirectory(Folder);
                }

                LocalTargetFolder = System.IO.Path.Combine(localFolder, Folder);
                FtpTargetFolder = string.Format("{0}/{1}", remoteFolder, Folder);

                UploadFolder(FtpTargetFolder, LocalTargetFolder, recursive);
            }
        }

        public void DisConnect()
        {
            this.FtpServer.Close();
        }

        public bool IsConnected()
        {
            return this.FtpServer.IsConnected;
        }

        public string CurrentWorkingFolder()
        {
            return this.FtpServer.ServerDirectory;
        }

        public Modification[] ListNewOrUpdatedFilesAtFtpSite(string localFolder, string remoteFolder, bool recursive)
        {
            System.Collections.Generic.List<Modification> mods = new System.Collections.Generic.List<Modification>();

            GetTheList(mods, localFolder, remoteFolder, recursive);

            return mods.ToArray();
        }

        private void GetTheList(System.Collections.Generic.List<Modification> mods, string localFolder, string remoteFolder, bool recursive)
        {
            this.FtpServer.ChangeWorkingDirectory(remoteFolder);

            EnterpriseDT.Net.Ftp.FTPFile[] FtpServerFileInfo = this.FtpServer.GetFileInfos();

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
                        FtpTargetFolder = string.Format("{0}/{1}", remoteFolder, CurrentFileOrDirectory.Name);

                        if (!System.IO.Directory.Exists(LocalTargetFolder))
                        {
                            Log.Trace("creating {0}", LocalTargetFolder);
                            System.IO.Directory.CreateDirectory(LocalTargetFolder);
                        }

                        GetTheList(mods, LocalTargetFolder, FtpTargetFolder, recursive);

                        //set the ftp working folder back to the correct value
                        this.FtpServer.ChangeWorkingDirectory(remoteFolder);
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

                    if (fi.Length != CurrentFileOrDirectory.Size || fi.LastWriteTime != CurrentFileOrDirectory.LastModified)
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
            CallingTask.CurrentStatus.AddChild(new ThoughtWorks.CruiseControl.Remote.ItemStatus(information));

            if (CallingTask.CurrentStatus.ChildItems.Count > 10)
            {
                CallingTask.CurrentStatus.ChildItems.RemoveAt(0);
            }

        }
    }
}