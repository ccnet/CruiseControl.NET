using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using System.Diagnostics;
using ThoughtWorks.CruiseControl.Remote;
using System.IO;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
    public partial class PackagesListForm : Form
    {
        private IProjectMonitor project;
        private Exception currentError;
        private Stopwatch loadStopwatch = new Stopwatch();
        private PackageDetails[] packages;
        private string downloadLocation;

        public PackagesListForm(IProjectMonitor project)
        {
            InitializeComponent();
            Project = project;
        }

        public IProjectMonitor Project
        {
            get { return project; }
            set
            {
                // Nice big logic check to make sure nothing is missed out
                bool changeMonitor = ((value != null) && (value.Detail != null) && (value.Detail.Configuration != null));
                if (changeMonitor)
                {
                    if ((project == null) ||
                        (project.Detail == null) ||
                        (project.Detail.Configuration == null))
                    {
                        changeMonitor = true;
                    }
                    else
                    {
                        changeMonitor = (project.Detail.Configuration != value.Detail.Configuration);
                    }
                }

                // Now we can do the actual monitor change
                if (changeMonitor)
                {
                    Text = string.Format("Available Packages for {0} [{1}]",
                        value.Detail.ProjectName,
                        value.Detail.ServerName);
                    project = value;
                    packageList.Items.Clear();
                    RefreshList();
                }
            }
        }

        private void RefreshList()
        {
            // If the background worker is not busy, then start the refresh, otherwise there is already
            // a refresh under way, so there is no need to start another
            if (!listLoader.IsBusy)
            {
                // Update the display and then start the background worker
                statusLabel.Text = "Loading status...";
                currentError = null;
                loadStopwatch.Reset();
                loadStopwatch.Start();
                listLoader.RunWorkerAsync();
            }
        }

        private void listLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                packages = project.RetrievePackageList();
            }
            catch (Exception error)
            {
                currentError = error;
            }
        }

        private void listLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            loadStopwatch.Stop();
            statusLabel.Text = string.Format("Package list loaded ({0:0.00}s)",
                Convert.ToDouble(loadStopwatch.ElapsedMilliseconds) / 1000);
            if (currentError == null)
            {
                // Update the display
                packageList.Items.Clear();
                foreach (PackageDetails package in packages)
                {
                    ListViewItem item = new ListViewItem(new string[] {
                        package.Name,
                        package.BuildLabel,
                        package.DateTime.ToString("F"),
                        package.NumberOfFiles.ToString(),
                        FormatSize(package.Size)
                    });
                    item.Tag = package;
                    packageList.Items.Add(item);
                }
            }
            else
            {
                // Tell the user an error occurred and what sort of error
                MessageBox.Show("Unable to refresh status: " + currentError.Message,
                    "Refresh Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string FormatSize(long size)
        {
            double workingSize = size;
            if (size > 1048576)
            {
                workingSize = workingSize / 1048576;
                return string.Format("{0:0.00}Mb", workingSize);
            }
            else if (size > 1024)
            {
                workingSize = workingSize / 1024;
                return string.Format("{0:0.00}Kb", workingSize);
            }
            else
            {
                return string.Format("{0}b", workingSize);
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void downloadButton_Click(object sender, EventArgs e)
        {
            DownloadPackages();
        }

        private void packageList_DoubleClick(object sender, EventArgs e)
        {
            DownloadPackages();
        }

        private void DownloadPackages()
        {
            if (packageList.SelectedItems.Count == 0)
            {
                MessageBox.Show("You have not selected any packages to download",
                    "Download Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "Select the location to download the packages to:";
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    downloadLocation = dialog.SelectedPath;
                    List<PackageDetails> downloadList = new List<PackageDetails>();
                    foreach (ListViewItem item in packageList.SelectedItems)
                    {
                        downloadList.Add(item.Tag as PackageDetails);
                    }
                    packages = downloadList.ToArray();
                    statusLabel.Text = string.Format("Downloading packages (0 of {0})", downloadList.Count);
                    currentError = null;
                    downloader.RunWorkerAsync();
                }
            }
        }

        private void downloader_DoWork(object sender, DoWorkEventArgs e)
        {
            int count = 0;
            foreach (PackageDetails package in packages)
            {
                Stream outputStream = File.Create(Path.Combine(downloadLocation, package.Name + ".zip"));
                try
                {
                    project.TransferFile(package.FileName, outputStream);
                }
                finally
                {
                    outputStream.Flush();
                    outputStream.Close();
                }

                downloader.ReportProgress(count++);
            }
        }

        private void downloader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            statusLabel.Text = "All packages downloaded";
            if (currentError != null)
            {
                // Tell the user an error occurred and what sort of error
                MessageBox.Show("Unable to download all packages: " + currentError.Message,
                    "Download Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Process folderDisplay = new Process();
                folderDisplay.StartInfo = new ProcessStartInfo(downloadLocation);
                folderDisplay.Start();
            }
        }

        private void downloader_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            statusLabel.Text = string.Format("Downloading packages ({1} of {0})", 
                packages.Length,
                e.ProgressPercentage);
        }
    }
}
