using System.Diagnostics;
using System.Reflection;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol 
{
    /// <summary>
    /// SourceGear Vault Source Control Block.
    /// </summary>
    /// <title>SourceGear Vault Source Control Block</title>
    /// <version>1.0</version>
    /// <key name="type">
    /// <description>The type of source control block.</description>
    /// <value>vault</value>
    /// </key>
    /// <example>
    /// <code title="Minimal example">
    /// &lt;sourcecontrol type="vault" /&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;sourcecontrol type="vault" autoGetSource="true" applyLabel="true"&gt;
    /// &lt;executable&gt;c:\program files\sourcegear\vault client\vault.exe&lt;/executable&gt;
    /// &lt;username&gt;my_username&lt;/username&gt;
    /// &lt;password&gt;my_password&lt;/password&gt;
    /// &lt;host&gt;my_buildserver&lt;/host&gt;
    /// &lt;repository&gt;my_repository&lt;/repository&gt;
    /// &lt;folder&gt;$&lt;/folder&gt;
    /// &lt;ssl&gt;true&lt;/ssl&gt;
    /// &lt;timeout units="minutes"&gt;10&lt;/timeout&gt;
    /// &lt;useWorkingDirectory&gt;true&lt;/useWorkingDirectory&gt;
    /// &lt;workingDirectory&gt;project/src&lt;/workingDirectory&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <heading>Plugin available for Vault 4.1+ (or Fortress 1.1+)</heading>
    /// <para>
    /// SourceGear has released a plugin that offers better performance and accuracy by interacting directly with Vault
    /// via its API, rather than the command line. The configuration format is almost identical to this one, making
    /// migration easy. The plugin and its documentation can be downloaded from SourceGear's site
    /// (http://www.sourcegear.com/downloads.html).
    /// </para>
    /// <heading>Vault Working Folder Defined</heading>
    /// <para>
    /// Most version control systems have distinct commands for "get me the source" and "get me the source into a folder
    /// where I may make changes." Vault is no exception. A working folder is a folder where Vault will keep track of
    /// your changes. If you're using CC.NET 1.1.0.2172+, the useWorkingFolder setting determines whether Vault
    /// retrieves source into a working folder. For build purposes, there are typically two situations where you want to
    /// retrieve source into a working folder:
    /// </para>
    /// <para>
    /// 1. Your build script changes source and checks in the change
    /// </para>
    /// <para>
    /// 2. It's taking longer than you'd like to retrieve the source
    /// </para>
    /// <para>
    /// Because of the additional state information kept by Vault for working folders, retrieving source into a working
    /// folder is usually faster than into a non-working folder. The trade-off is that more disk space will be used for
    /// cache and state data.
    /// </para>
    /// <heading>Filtering out Label Changes</heading>
    /// <para>
    /// If you are using Vault 3.x or later, labels will automatically be filtered. However, if you are using an earlier
    /// version of Vault and you apply a label as part of your build process, this will kick off another build. you will
    /// need to use a <link>Filtered Source Control Block</link> to get around this. If your build server uses a
    /// specific user id to integrate with Vault, you can set up a UserFilter to filter out all changes made by that 
    /// user.
    /// </para>
    /// <heading>Problems with CCService and Vault 3.0.2+</heading>
    /// <para>
    /// If you are experiencing problems detecting modifications using CCService after upgrading to Vault 3.0.2, it may
    /// be related to the enhanced security features of the Vault server. You should try the following process to fix
    /// this issue:
    /// </para>
    /// <para>
    /// 1. reinstall the Vault Server to have the IIS Process Model run as System (ie. not Machine)
    /// </para>
    /// <para>
    /// 2. change the CCService to run as an actual user, not as LocalSystem.
    /// </para>
    /// <heading>Problems with releases prior to Vault 3.0</heading>
    /// <para>
    /// For versions of Vault prior to 3.0, the -excludeactions argument is not supported. To get around this problem
    /// you should explicitly specify the &lt;historyArgs&gt; element in your ccnet.config file so that it does not
    /// contain that argument (ie. set it to &lt;historyArgs&gt;-rowlimit 0&lt;/historyArgs&gt;.
    /// </para>
    /// <heading>NAnt Vault Tasks</heading>
    /// <para>
    /// SourceGear has produced some NAnt tasks for Vault that can be downloaded from
    /// http://www.sourcegear.com/vault/downloads.html.
    /// </para>
    /// <heading>Turning off the creation of _sgbak folders</heading>
    /// <para>
    /// Using CC.NET 1.1.0.2172+ and Vault 3.1.7+, _sgbak folders are never created.
    /// </para>
    /// <para>
    /// Using older versions, the use of the _sgbak folder can be turned off using the GUI client. This is a user and
    /// machine-specific setting, so you need to launch the GUI client on the CCNet machine and log in as the same Vault
    /// user that CCNet is using (as specified it ccnet.config).
    /// </para>
    /// <para>
    /// Tools|Options -&gt; Local Files -&gt; Cache/Backup Locations -&gt; Un-check "Save files in backup folder before
    /// overwriting"
    /// </para>
    /// <heading>Contributions</heading>
    /// <para>
    /// Code contributed by Ryan Duffield, Leo von Wyss and Ian Olsen.
    /// </para>
    /// </remarks>
    [ReflectorType("vault")]
	public class VaultVersionChecker 
        : SourceControlBase
	{
		private	Timeout timeout = Timeout.DefaultTimeout;

        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const string DefaultExecutable = @"C:\Program Files\SourceGear\Vault Client\vault.exe";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const string DefaultHistoryArgs = "-excludeactions label,obliterate -rowlimit 0";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const string DefaultFolder = "$";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const string DefaultFileTime = "checkin";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const int DefaultPollRetryWait = 5;
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const int DefaultPollRetryAttempts = 5;

        /// <summary>
        /// 	
        /// </summary>
		public enum EForcedVaultVersion
		{
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
			None,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
			Vault3,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
			Vault317
		}

		private Vault3 _vaultSourceControl = null;
		private EForcedVaultVersion _forcedVaultVersion = EForcedVaultVersion.None;

        /// <summary>
        /// Vault user id that CCNet should use to authenticate.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("username", Required = false)]
        public string Username { get; set; }

        /// <summary>
        /// Password for the Vault user.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("password", typeof(PrivateStringSerialiserFactory), Required = false)]
        public PrivateString Password { get; set; }

        /// <summary>
        /// The name of the Vault server.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("host", Required = false)]
        public string Host { get; set; }

        /// <summary>
        /// The name of the Vault repository to monitor. .
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("repository", Required = false)]
        public string Repository { get; set; }

        /// <summary>
        /// The root folder to be monitored by CCNet.
        /// </summary>
        /// <version>1.0</version>
        /// <default>$</default>
        [ReflectorProperty("folder", Required = false)]
        public string Folder { get; set; }

        /// <summary>
        /// The location of the Vault command-line executable.
        /// </summary>
        /// <version>1.0</version>
        /// <default>C:\Program Files\SourceGear\Vault Client\vault.exe</default>
        [ReflectorProperty("executable", Required = false)]
        public string Executable { get; set; }

        /// <summary>
        /// Should SSL be used to communicate with the Vault server.
        /// </summary>
        /// <version>1.0</version>
        /// <default>false</default>
        [ReflectorProperty("ssl", Required = false)]
        public bool Ssl { get; set; }

        /// <summary>
        /// Specifies if CCNet should automatically retrieve the latest version of the source from the repository.
        /// </summary>
        /// <version>1.0</version>
        /// <default>true</default>
        [ReflectorProperty("autoGetSource", Required = false)]
        public bool AutoGetSource { get; set; }

        /// <summary>
        /// Specifies if CCNet should apply the build label to the repository.
        /// </summary>
        /// <version>1.0</version>
        /// <default>false</default>
        [ReflectorProperty("applyLabel", Required = false)]
        public bool ApplyLabel { get; set; }

        /// <summary>
        /// Extra arguments to be included in the history commandline.
        /// </summary>
        /// <version>1.0</version>
        /// <default>-excludeactions label,obliterate -rowlimit 0</default>
        [ReflectorProperty("historyArgs", Required = false)]
        public string HistoryArgs { get; set; }

        /// <summary>
        /// Sets the timeout period for the source control operation.
        /// </summary>
        /// <version>1.0</version>
        /// <default>10 minutes</default>
        [ReflectorProperty("timeout", typeof(TimeoutSerializerFactory), Required = false)]
		public Timeout Timeout
		{
			get
			{
				return timeout;
			}
			set
			{
				if (value==null) 
					timeout = Timeout.DefaultTimeout;
				else 
					timeout = value;
			}
			
		}

        /// <summary>
        /// <b>CC.NET 1.0</b>: Determines the working directory into which Vault files will be retrieved. Supply true if
        /// you want CCNet to use the Vault folder working directory created for the build user using the Vault GUI
        /// (recommended). Supply false if CCNet should use the CCNet working directory.
        /// <b>CC.NET 1.1</b>: Determines if the source will be retrieved into a Vault Working Folder. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>true</default>
        [ReflectorProperty("useWorkingDirectory", Required = false)]
        public bool UseVaultWorkingDirectory { get; set; }

        /// <summary>
        /// The root folder where the latest source will retrieved from Vault. This path can either be absolute or it
        /// can be relative to the CCNet project working directory.
        /// </summary>
        /// <version>1.0</version>
        /// <default>Project Working Directory</default>
        /// <remarks>
        /// <b>CC.NET 1.1</b>: If this element is missing or empty, Vault will attempt to use the directory set as the
        /// user's working folder. Note that this is simply the destination path on disk. Whether or not this location
        /// is a Vault Working Folder is determined by the useWorkingFolder element. To use the same path as the 
        /// project, it is necessary to use "." (without the quotes) rather than leaving this empty, as you could in 
        /// CC.NET 1.0.
        /// </remarks>
        [ReflectorProperty("workingDirectory", Required = false)]
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// The modification date that retrieved source files will have. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>checkin</default>
        /// <remarks>
        /// Must be one of:
        /// *  checkin - the date/time the file was checked in
        /// * current - the date/time the file was retrieved from Vault
        /// * modification - the date/time the file was last modified
        /// </remarks>
        [ReflectorProperty("setFileTime", Required = false)]
        public string setFileTime { get; set; }

        /// <summary>
        /// If true, the source path will be emptied before retrieving new source. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>false</default>
        [ReflectorProperty("cleanCopy", Required = false)]
        public bool CleanCopy { get; set; }

        /// <summary>
        /// The host name of the HTTP proxy Vault should use. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("proxyServer", Required = false)]
        public string proxyServer { get; set; }

        /// <summary>
        /// The port on the HTTP proxy Vault should use. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("proxyPort", Required = false)]
        public string proxyPort { get; set; }

        /// <summary>
        /// The user name for the HTTP proxy Vault should use. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("proxyUser", Required = false)]
        public string proxyUser { get; set; }

        /// <summary>
        /// The password for the HTTP proxy Vault should use. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("proxyPassword", Required = false)]
        public string proxyPassword { get; set; }

        /// <summary>
        /// The Windows domain of the HTTP proxy Vault should use.  
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("proxyDomain", Required = false)]
        public string proxyDomain { get; set; }

        /// <summary>
        /// Any other aruuments to pass into the executable.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("otherVaultArguments", Required = false)]
        public string otherVaultArguments { get; set; }

        /// <summary>
        /// The number of seconds to wait between retries when a check for modifications fails. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>5</default>
        [ReflectorProperty("pollRetryWait", Required = false)]
        public int pollRetryWait { get; set; }

        /// <summary>
        /// The number of automatic retries when failing to check for modifications before an exception is thrown. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>5</default>
        [ReflectorProperty("pollRetryAttempts", Required = false)]
        public int pollRetryAttempts { get; set; }

        /// <summary>
        /// Gets the vault source control.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public Vault3 VaultSourceControl
		{
			get { return _vaultSourceControl; }
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="VaultVersionChecker" /> class.	
        /// </summary>
        /// <remarks></remarks>
        public VaultVersionChecker()
        {
            this.InitialiseDefaults();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VaultVersionChecker" /> class.	
        /// </summary>
        /// <param name="historyParser">The history parser.</param>
        /// <param name="executor">The executor.</param>
        /// <param name="forceVersion">The force version.</param>
        /// <remarks></remarks>
		public VaultVersionChecker(IHistoryParser historyParser, ProcessExecutor executor, EForcedVaultVersion forceVersion)
		{
            this.InitialiseDefaults();
            _forcedVaultVersion = forceVersion;
			switch ( forceVersion )
			{
				case EForcedVaultVersion.Vault3:
					_vaultSourceControl = new Vault3(this, historyParser, executor);
					break;
				case EForcedVaultVersion.Vault317:
					_vaultSourceControl = new Vault317(this, historyParser, executor);
					break;
				default:
					Debug.Fail("You have to force a version of Vault from the unit tests.");
					break;
			}
        }

        private void InitialiseDefaults()
        {
            this.Folder = DefaultFolder;
            this.Executable = DefaultExecutable;
            this.Ssl = false;
            this.AutoGetSource = true;
            this.ApplyLabel = false;
            this.HistoryArgs = DefaultHistoryArgs;
            this.UseVaultWorkingDirectory = true;
            this.setFileTime = DefaultFileTime;
            this.CleanCopy = false;
            this.pollRetryWait = DefaultPollRetryWait;
            this.pollRetryAttempts = DefaultPollRetryAttempts;
        }

        /// <summary>
        /// Initializes the specified project.	
        /// </summary>
        /// <param name="project">The project.</param>
        /// <remarks></remarks>
        public override void Initialize(IProject project)
		{
			GetCorrectVaultInstance();
			VaultSourceControl.Initialize(project);
		}

        /// <summary>
        /// Gets the modifications.	
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			GetCorrectVaultInstance();
			return VaultSourceControl.GetModifications(from, to);
		}

        /// <summary>
        /// Labels the source control.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
        public override void LabelSourceControl(IIntegrationResult result)
		{
			GetCorrectVaultInstance();
			VaultSourceControl.LabelSourceControl(result);
		}

        /// <summary>
        /// Gets the source.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
        public override void GetSource(IIntegrationResult result)
		{
			GetCorrectVaultInstance();
			VaultSourceControl.GetSource(result);
		}

        /// <summary>
        /// Purges the specified project.	
        /// </summary>
        /// <param name="project">The project.</param>
        /// <remarks></remarks>
        public override void Purge(IProject project)
		{
			GetCorrectVaultInstance();
			VaultSourceControl.Purge(project);
		}

		private void GetCorrectVaultInstance()
		{
			if ( VaultSourceControl != null )
				return;

			if ( VaultVersionIs317OrBetter() )
			{
				Log.Debug("Vault CLC is at least version 3.1.7.");
				_vaultSourceControl = new Vault317(this);
			}
			else
			{
				Log.Debug("Vault CLC is older than version 3.1.7.");
				_vaultSourceControl = new Vault3(this);
			}
		}

		private bool VaultVersionIs317OrBetter()
		{
			switch ( _forcedVaultVersion )
			{
				case EForcedVaultVersion.Vault3:
					Log.Debug("Vault version 3 forced.");
					return false;
				case EForcedVaultVersion.Vault317:
					Log.Debug("Vault version 3.1.7 forced");
					return true;
			}

			Assembly vaultExe = Assembly.LoadFile(Executable);
			AssemblyName vaultExeName = vaultExe.GetName();

			Log.Debug(Executable + " is version " + vaultExeName.Version.ToString());

			// Return true if Vault Command-Line Client is version 3.1.7 or higher, otherwise return false.
			if ( vaultExeName.Version.Major > 3 )
				return true;
			if ( vaultExeName.Version.Major == 3 )
			{
				if ( vaultExeName.Version.Minor > 1 )
					return true;
				if ( vaultExeName.Version.Minor == 1 && vaultExeName.Version.Build >= 7 )
					return true;
			}
			return false;
		}
		
	}
}
