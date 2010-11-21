using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// <para>
    /// The Ftp Soure control block allows to detect new and changed files at an Ftp site.
    /// </para>
    /// <para type="warning">
    /// Deleted files are <b>NOT</b> detected.
    /// </para>
    /// </summary>
    /// <title>FTP Source control</title>
    /// <version>1.4.4</version>
    /// <key name="type">
    /// <description>The type of source control block.</description>
    /// <value>ftpSourceControl</value>
    /// </key>
    /// <example>
    /// <code>
    /// &lt;sourcecontrol type="ftpSourceControl"&gt;
    /// &lt;serverName&gt;ftp.isp.com&lt;/serverName&gt;
    /// &lt;userName&gt;john&lt;/userName&gt;
    /// &lt;password&gt;doe&lt;/password&gt;
    /// &lt;ftpFolderName&gt;config&lt;/ftpFolderName&gt;
    /// &lt;localFolderName&gt;d:\temp\config&lt;/localFolderName&gt;
    /// &lt;recursiveCopy&gt;true&lt;/recursiveCopy&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// </example>
    [ReflectorType("ftpSourceControl")]
    public class FtpSourceControl 
        : SourceControlBase
    {
        private FtpLib ftp;

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpSourceControl"/> class.
        /// </summary>
        public FtpSourceControl()
        {
            this.ServerName = string.Empty;
            this.UserName = string.Empty;
            this.Password = string.Empty;
            this.UseActiveConnectionMode = true;
            this.FtpFolderName = string.Empty;
            this.LocalFolderName = string.Empty;
            this.RecursiveCopy = true;
        }

        /// <summary>
        /// The name of the server to connect to.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>n/a</default>
        [ReflectorProperty("serverName", Required = true)]
        public string ServerName { get; set; }

        /// <summary>
        /// The user name to log in with.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>n/a</default>
        [ReflectorProperty("userName", Required = true)]
        public string UserName { get; set; }

        /// <summary>
        /// The password for the user.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>n/a</default>
        [ReflectorProperty("password", typeof(PrivateStringSerialiserFactory), Required = true)]
        public PrivateString Password { get; set; }

        /// <summary>
        /// Whether to use active connection mode or not.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>true</default>
        [ReflectorProperty("useActiveConnectionMode", Required = false)]
        public bool UseActiveConnectionMode { get; set; }

        /// <summary>
        /// The folder name of on the ftp site.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>n/a</default>
        [ReflectorProperty("ftpFolderName", Required = true)]
        public string FtpFolderName { get; set; }

        /// <summary>
        /// The folder name on the local system.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>n/a</default>
        [ReflectorProperty("localFolderName", Required = true)]
        public string LocalFolderName { get; set; }

        /// <summary>
        /// Whether to recurse into subfolders or not.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>true</default>
        [ReflectorProperty("recursiveCopy", Required = true)]
        public bool RecursiveCopy { get; set; }

        /// <summary>
        /// Gets the modifications.	
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
        {
            ftp = new FtpLib(to.BuildProgressInformation);
            string remoteFolder = FtpFolderName;

            ftp.LogIn(ServerName,UserName,Password.PrivateValue,UseActiveConnectionMode);

            if (!FtpFolderName.StartsWith("/"))
            {
                remoteFolder = System.IO.Path.Combine(ftp.CurrentWorkingFolder(), FtpFolderName);
            }

            Modification[] mods =  ftp.ListNewOrUpdatedFilesAtFtpSite(LocalFolderName, remoteFolder, RecursiveCopy);

            ftp.DisConnect();

            return mods;
        }

        /// <summary>
        /// Labels the source control.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
        public override void LabelSourceControl(IIntegrationResult result)
        {
        }

        /// <summary>
        /// Gets the source.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
        public override void GetSource(IIntegrationResult result)
        {
            Util.Log.Info(result.HasModifications().ToString());


            ftp = new FtpLib(result.BuildProgressInformation);
            string remoteFolder = FtpFolderName;

            ftp.LogIn(ServerName, UserName, Password.PrivateValue, UseActiveConnectionMode);


            if (!FtpFolderName.StartsWith("/"))
            {
                remoteFolder = System.IO.Path.Combine(ftp.CurrentWorkingFolder(), FtpFolderName);
            }

            ftp.DownloadFolder( LocalFolderName, remoteFolder, RecursiveCopy);

            ftp.DisConnect();

        }

        /// <summary>
        /// Initializes the specified project.	
        /// </summary>
        /// <param name="project">The project.</param>
        /// <remarks></remarks>
        public override void Initialize(IProject project)
        {
        }

        /// <summary>
        /// Purges the specified project.	
        /// </summary>
        /// <param name="project">The project.</param>
        /// <remarks></remarks>
        public override void Purge(IProject project)
        {
        }
    }
}
