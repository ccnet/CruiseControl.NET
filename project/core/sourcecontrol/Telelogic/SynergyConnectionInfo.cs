using System;
using System.Globalization;
using System.IO;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Telelogic
{
	/// <summary>
	/// A CM Synergy client session.
	/// </summary>
    /// <title>Synergy Client Session</title>
    /// <version>1.0</version>
    /// <example>
    /// <code>
    /// &lt;connection&gt;
    /// &lt;host&gt;myserver&lt;/host&gt;
    /// &lt;database&gt;\\myserver\share\mydatabase&lt;/database&gt;
    /// &lt;!-- store values in an environmental variable--&gt;
    /// &lt;username&gt;%CCM_USER%&lt;/username&gt;
    /// &lt;password&gt;%CCM_PWD%&lt;/password&gt;
    /// &lt;role&gt;build_mgr&lt;/role&gt;
    /// &lt;homeDirectory&gt;D:\cmsynergy\%CCM_USER%&lt;/homeDirectory&gt;
    /// &lt;clientDatabaseDirectory&gt;D:\cmsynergy\uidb&lt;/clientDatabaseDirectory&gt;
    /// &lt;polling&gt;true&lt;/polling&gt;
    /// &lt;timeout&gt;3600&lt;/timeout&gt;
    /// &lt;/connection&gt;
    /// </code>
    /// </example>
	[ReflectorType("synergyConnection")]
	public class SynergyConnectionInfo
	{
		private string username;
		private string password;
		private string homeDirectory;
		private string clientDatabaseDirectory;
		private string executable;
		private string workingDirectory;

		public SynergyConnectionInfo()
		{
			Executable = "ccm.exe";
			Timeout = 3600;
			Host = "localhost";
			Database = null;
			Username = "%USERNAME%";
			Password = String.Empty;
			Role = "build_mgr";
			HomeDirectory = @"%SystemDrive%\cmsynergy\%USERNAME%";
			ClientDatabaseDirectory = @"%SystemDrive%\cmsynergy\uidb";
			WorkingDirectory = @"%ProgramFiles%\Telelogic\CM Synergy 6.3\bin";

			Reset();
		}

		/// <summary>
		/// The identitifer for the Synergy client side process.
		/// Required to have multiple Synergy processes.
		/// </summary>
		/// <example>
		/// <c>COMPUTERNAME:1234:127.0.0.1</c>
		/// </example>
		/// <value>
		/// Defaults to <see langword="null" />.
		/// </value>
        /// <version>1.0</version>
        /// <default>None</default>
		public string SessionId;

		/// <summary>
		/// The executable filename/path for the CM Synergy command line interface.
		/// </summary>
        /// <remarks>
        /// Can include environmental variables to be replaced.
        /// </remarks>
		/// <value>
		/// Defaults to <c>ccm.exe</c>.
		/// </value>
        /// <version>1.0</version>
        /// <default>ccm.exe</default>
        [ReflectorProperty("executable")]
		public string Executable
		{
			get { return executable; }
			set { executable = Environment.ExpandEnvironmentVariables(value); }
		}

		/// <summary>
		/// The directory to execute all CM Synergy commands from.
		/// </summary>
        /// <remarks>
        /// Can include environmental variables to be replaced.
        /// </remarks>
		/// <value>
		/// Defaults to <c>%PROGRAMFILES%\Telelogic\CM Synergy 6.3\bin</c>
		/// </value>
        /// <version>1.0</version>
        /// <default>%PROGRAMFILES%\Telelogic\CM Synergy 6.3\bin</default>
        [ReflectorProperty("workingDirectory", Required = false)]
		public string WorkingDirectory
		{
			get { return workingDirectory; }
			set { workingDirectory = Environment.ExpandEnvironmentVariables(value); }
		}

		/// <summary>
		/// Hostname of the Synergy server
		/// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("host")]
		public string Host;

		/// <summary>
		/// Network path to the Synergy database instance
		/// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("database")]
		public string Database;

		/// <summary>
		/// The configured database delimiter for object and project specifications.
		/// </summary>
		/// <value>
		///     Defaults to <c>-</c>.
		/// </value>
		public char Delimiter;

		/// <summary>
		///     Extracts the name of the database from the <see cref="Database"/> full
		///     physical path.
		/// </summary>
		public string DatabaseName
		{
			get
			{
				string databaseName;

				// HACK: assume the database name always matches the directory name for the path
				//       to the database.
				databaseName = Path.GetFileName(Database);
				if (databaseName.Length == 0)
				{
					// System.IO.Path parses "\\server\share\folder\" as dir,
					// but parses            "\\server\share\folder"  as a file
					databaseName = Directory.GetParent(Database).Name;
				}

				return (databaseName);
			}
		}

		/// <summary>
		/// Poll the server every minute when the <c>ccm_admin</c> has protected the database for the purpose of issuing backup commands.
		/// </summary>
		/// <remarks>
		/// This is useful if a long runing inadventently enters the scheduled time window for routine downtime, generally for server maintenance jobs like backups.
		/// </remarks>
		/// <value>
		///     Defaults to <see langword="false" />
		/// </value>
        /// <version>1.0</version>
        /// <default>false</default>
        [ReflectorProperty("polling", Required = false)]
		public bool PollingEnabled;

		/// <summary>
		/// The username for the Synergy session. Can include environmental variables to be replaced.
		/// </summary>
		/// <value>
		/// Defaults to <c>("%USERNAME%")</c>.
		/// </value>
        /// <version>1.0</version>
        /// <default>%USERNAME%</default>
        [ReflectorProperty("username", Required = false)]
		public string Username
		{
			get { return username; }
			set { username = Environment.ExpandEnvironmentVariables(value); }
		}

		/// <summary>
		/// The Synergy password for the associate <see cref="Username"/> value.
		/// </summary>
		/// <value>
		/// Defaults to <see cref="String.Empty"/>.
		/// </value>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("password", Required = false)]
		public string Password
		{
			get { return password; }
			set { password = Environment.ExpandEnvironmentVariables(value); }
		}

		/// <summary>
		/// The role to use for the Synergy session.
		/// </summary>
		/// <value>
		/// Defaults to <c>build_mgr</c>.
		/// </value>
        /// <version>1.0</version>
        /// <default>build_mgr</default>
        [ReflectorProperty("role", Required = false)]
		public string Role;

		/// <summary>
		/// The full physical path of the home directory for the associated Username on the client machine. Can include environmental variables to be replaced.
		/// </summary>
		/// <remarks>
		/// This role must have sufficient permissions to modify task folders, change reconfigure properties, and create baselines.
		/// </remarks>
		/// <value>Defaults to <c>%SystemDrive%\cmsynergy\%USERNAME%</c>.</value>
        /// <version>1.0</version>
        /// <default>%SystemDrive%\cmsynergy\%USERNAME%</default>
        [ReflectorProperty("homeDirectory", Required = false)]
		public string HomeDirectory
		{
			get { return homeDirectory; }
			set { homeDirectory = Environment.ExpandEnvironmentVariables(value); }
		}

		/// <summary>
		/// Path for the remote client session to copy database information to. Can include environmental variables to be replaced.
		/// </summary>
		/// <value>Defaults to <c>%SystemDrive%\cmsynergy\uidb</c>.</value>
        /// <version>1.0</version>
        /// <default>%SystemDrive%\cmsynergy\uidb</default>
        [ReflectorProperty("clientDatabaseDirectory", Required = false)]
		public string ClientDatabaseDirectory
		{
			get { return clientDatabaseDirectory; }
			set { clientDatabaseDirectory = Environment.ExpandEnvironmentVariables(value); }
		}

		/// <summary>
		/// Timeout in seconds for all Synergy commands.
		/// </summary>
		/// <value>Defaults to <c>3600</c> seconds (one hour).</value>
        /// <version>1.0</version>
        /// <default>3600</default>
        [ReflectorProperty("timeout", Required = false)]
		public int Timeout;

		public IFormatProvider FormatProvider = CultureInfo.CurrentCulture;
		
		/// <summary>
		///     Resets session variables back to default values.
		///     Useful for when a connection is closed or reestablished.
		/// </summary>
		public void Reset()
		{
			SessionId = null;
			Delimiter = '-';
		}
	}
}