using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// <para>
    /// Source control integration for Accurev's source control product (http://www.accurev.com).
    /// </para>
    /// </summary>
    /// <title> AccuRev Source Control Block </title>
    /// <version>1.3</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;sourcecontrol type="accurev"&gt;
    /// &lt;autoGetSource&gt;true&lt;/autoGetSource&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;sourcecontrol type="accurev"&gt;
    /// &lt;autoGetSource&gt;true&lt;/autoGetSource&gt;
    /// &lt;executable&gt;accurev.exe&lt;/executable&gt;
    /// &lt;homeDir&gt;.&lt;/homeDir&gt;
    /// &lt;labelOnSuccess&gt;false&lt;/labelOnSuccess&gt;
    /// &lt;login&gt;false&lt;/login&gt;
    /// &lt;password&gt;banana&lt;/password&gt;
    /// &lt;principal&gt;joe_user&lt;/principal&gt;
    /// &lt;timeout units="minutes"&gt;10&lt;/timeout&gt;
    /// &lt;workspace&gt;.&lt;/workspace&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// </example>
    /// <key name="type">
    /// <description>The type of source control block.</description>
    /// <value>accurev</value>
    /// </key>
    /// <remarks>
    /// <heading>Known Issues</heading>
    /// <para>
    /// <b>CruiseControl.NET doesn't see my changes</b>
    /// </para>
    /// <para>
    /// AccuRev depends on the clocks of the server and its clients ticking together. Make sure the clock of your build server is synchronized
    /// to the clock of your AccuRev server. See CCNET-271 for details on a similar problem with Rational ClearCase.
    /// </para>
    /// <para>
    /// <b>AccuRev says I'm "unknown" or "not authenticated"</b>
    /// </para>
    /// <para>
    /// AccuRev needs to know the userid that owns the workspace, and stores that information in files in the AccuRev home directory, which
    /// defaults to %HOMEDRIVE%%HOMEPATH%\.accurev. If there isn't any such directory, or if CCNet is running under a userid that isn't an
    /// AccuRev user, AccuRev will not be able to function correctly and the accurev info command may report that the user is unknown or not
    /// authenticated. You can use the homeDir element to force AccuRev to look for the .accurev directory in a particular location, such as
    /// the project's artifact directory.
    /// </para>
    /// </remarks>
	[ReflectorType("accurev")]
	public class AccuRev : ProcessSourceControl
    {
        #region Properties
        
        /// <summary>
        /// Specifies whether the current version of the source should be retrieved from AccuRev. 
		/// </summary>
		/// <version>1.3</version>
        /// <default>false</default>
        [ReflectorProperty("autoGetSource", Required = false)]
        public bool AutoGetSource { get; set; }

		/// <summary>
        /// Specifies the path to the AccuRev command line tool. You should only have to include this element if the tool isn't in your
        /// path. By default, the AccuRev client installation process names it accurev.exe and puts it in C:\Program Files\AccuRev\bin. 
		/// </summary>
        /// <version>1.3</version>
        /// <default>accurev.exe</default>
        [ReflectorProperty("executable", Required = false)]
        public string Executable { get; set; }

		/// <summary>
        /// Specifies the location of the AccuRev home directory. The pathname can be either absolute or relative to the project artifact
        /// directory. If not specified, AccuRev will follow its rules for determining the location. The home directory itself is always
        /// named ".accurev". 
		/// </summary>
		/// <remarks>
		/// Optional, default is to let AccuRev decide where the home directory is.
		/// </remarks>
        /// <version>1.3</version>
        /// <default>None</default>
        [ReflectorProperty("homeDir", Required = false)]
        public string AccuRevHomeDir { get; set; }
		
		/// <summary>
        /// Specifies whether or not CCNet should create an AccuRev snapshot when the build is successful. If set to true, CCNet will create
        /// a snapshot of the workspace's basis stream as of the starting time of the build, naming it according to the build label.
        /// </summary>
        /// <version>1.3</version>
        /// <default>false</default>
        [ReflectorProperty("labelOnSuccess", Required = false)]
        public bool LabelOnSuccess { get; set; }

		/// <summary>
        /// Specifies whether or not CCNet should log in to AccuRev using the specified principal and password. If set to true, the principal
        /// and password elements are required, and CCNet will use them to log in to AccuRev before executing any AccuRev commands. 
        /// </summary>
        /// <remarks>
        /// If this is set to true, then both principal and password must be set.
        /// </remarks>
        /// <version>1.3</version>
        /// <default>false</default>
        [ReflectorProperty("login", Required = false)]
        public bool LogIn { get; set; }
		
		/// <summary>
        /// Specifies the password for the AccuRev "principal" (userid). 
        /// </summary>
        /// <version>1.3</version>
        /// <default>None</default>
        [ReflectorProperty("password", typeof(PrivateStringSerialiserFactory), Required = false)]
        public PrivateString AccuRevPassword { get; set; }

		/// <summary>
        /// Specifies the AccuRev "principal" (userid) to run under. If not specified, AccuRev will follow its rules for determining the
        /// principal. 
        /// </summary>
        /// <version>1.3</version>
        /// <default>false</default>
        [ReflectorProperty("principal", Required = false)]
        public string AccuRevPrincipal { get; set; }
		
		/// <summary>
        /// Specifies the location on disk of the AccuRev workspace that CCNet monitors for changes. The pathname can be either absolute or
        /// relative to the project working directory, and must identify the top-level directory of the workspace. Note that this is not the
        /// same as the workspace name - AccuRev will determine the workspace name from the disk pathname.
        /// </summary>
        /// <version>1.3</version>
        /// <default>false</default>
        [ReflectorProperty("workspace", Required = false)]
        public string Workspace { get; set; }
        
        #endregion

        /// <summary>
        /// Modifications discovered by this instance of the source control interface.
        /// </summary>
        internal Modification[] mods = new Modification[0];

        /// <summary>
		/// Create an instance of the source control integration with the default history parser and 
		/// process executor.
		/// </summary>
		/// <remarks>
        /// Uses <see cref="AccuRev(IHistoryParser, ProcessExecutor)"/> to do the heavy lifting.
		/// </remarks>
		public AccuRev() : this(new AccuRevHistoryParser(), new ProcessExecutor())
		{
		}
		
		/// <summary>
        /// Create an instance of the source control integration with the default history parser.
		/// </summary>
		/// <remarks>
        /// Uses <see cref="AccuRev(IHistoryParser, ProcessExecutor)"/> to do the heavy lifting.
		/// </remarks>
		public AccuRev(ProcessExecutor executor) : this(new AccuRevHistoryParser(), executor)
		{
		}

		/// <summary>
		/// Create an instance of the source control integration.
		/// </summary>
		public AccuRev(IHistoryParser parser, ProcessExecutor executor) : base(parser, executor)
		{
			if (LogIn & ((AccuRevPrincipal == null) || (AccuRevPassword == null)))
			{
				Log.Error("login=true requires principal= and password= to be specified.");
			}

            this.AutoGetSource = false;
            this.Executable = "accurev.exe";
            this.AccuRevHomeDir = null;
            this.LabelOnSuccess = false;
            this.LogIn = false;
            this.AccuRevPassword = null;
            this.AccuRevPrincipal = null;
            this.Workspace = string.Empty;
		}

		/// <summary>
		/// Format a timestamp the way AccuRev's commands want to see it.
		/// </summary>
		/// <param name="date">the timestamp to format.</param>
		/// <returns>the timestamp as a string in "yyyy/mm/dd hh:mm:ss" form in local time</returns>
		private static string FormatCommandDate(DateTime date)
		{
			return date.ToString("yyyy\\/MM\\/dd HH\\:mm\\:ss", CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Determine the AccuRev basis stream name for the specified workspace directory.
		/// </summary>
		/// <param name="result">the IIntegrationResult object contaiing the directory name 
		/// of the workspace</param>
		/// <returns>the name of the basis stream</returns>
		private string GetBasisStreamName(IIntegrationResult result)
		{
			string line;
			Regex findBasisRegex = new Regex(@"^\s*Basis:\s+(.+)$");
			PossiblyLogIn(result);
			ProcessResult cmdResults = RunCommand("info", result);
			StringReader infoStdOut = new StringReader(cmdResults.StandardOutput);
			while ((line = infoStdOut.ReadLine()) != null)
			{
				Match parsed = findBasisRegex.Match(line);
				if (parsed.Success)
					return parsed.Groups[1].ToString().Trim();	
				// Format is: "Basis:          __stream_name__"
			}
			Log.Error(string.Format(System.Globalization.CultureInfo.CurrentCulture,"No \"Basis:\" line found in output from AccuRev \"accurev info\": {0}", cmdResults.StandardOutput));
			return string.Empty;
		}

		/// <summary>
		/// Obtain a list of modified files between the specified points on the revision history.
		/// </summary>
        /// <param name="from">the IntegrationResult containing the starting timestamp</param>
        /// <param name="to">the IntegrationResult containing the ending timestamp</param>
        /// <remarks>
        /// This method creates an AccuRev command to list all the modifications in the specified 
        /// timespan, and defers the execution and parsing to AccuRevHistoryParser.Parse() (via 
        /// ProcessSourceControl.GetModifications() et al.)
        /// <b>Note:</b> The technique used by this method is only aware of changes in the workspace's
		/// parent stream, not in streams that are ancestors of that stream.  This method should
		/// probably be changed to detect such changes, possibly through use of the "accurev update -i"
        /// command instead of "accurev hist", as suggested by "dhearing1 &lt;dp_godwin@hotmail.com&gt;" to
        /// the ccnet-user list on 2007-07-09.
		/// </remarks>
		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			PossiblyLogIn(from);
			string args = string.Format(System.Globalization.CultureInfo.CurrentCulture,"hist -a -s \"{0}\" -t \"{1}-{2}\"", 
				GetBasisStreamName(to),
				FormatCommandDate(to.StartTime),
				FormatCommandDate(from.StartTime));
			ProcessInfo histCommand = PrepCommand(args, from);

            Modification[] mods = base.GetModifications(histCommand, from.StartTime, to.StartTime);
            base.FillIssueUrl(mods);
            return mods;
		}

		/// <summary>
		/// Obtain the specified level on the source code. 
		/// </summary>
        /// <param name="result">the IntegrationResult indicating the source level to get</param>
        /// <remarks>
        /// If the integration result doesn't specify a last change number, we update to the most-current level,
        /// because AccuRev doesn't have the ability to update to a specific timestamp, only to a specific transaction.
        /// </remarks>
		public override void GetSource(IIntegrationResult result)
		{
            result.BuildProgressInformation.SignalStartRunTask("Getting source from AccuRev");

			if (AutoGetSource)
			{
			    string command = "update";
				PossiblyLogIn(result);
                var lastChangeNumber = Modification.GetLastChangeNumber(mods);
			    int lastChange = int.Parse(lastChangeNumber ?? "0");
                if (lastChange != 0) command = command + " -t " + lastChange;
                RunCommand(command, result);
			}
		}
		
		/// <summary>
		/// Label the specified source level.  In AccuRev terms, create a snapshot based on that level.
		/// </summary>
        /// <param name="result">the IntegrationResult containing the label</param>
		public override void LabelSourceControl(IIntegrationResult result)
		{
			if (LabelOnSuccess && result.Succeeded && (result.Label != string.Empty))
			{
				PossiblyLogIn(result);
				string args = string.Format(System.Globalization.CultureInfo.CurrentCulture,"mksnap -s \"{0}\" -b \"{1}\" -t \"{2}\"",
					result.Label,
					GetBasisStreamName(result),
					FormatCommandDate(result.StartTime));
				RunCommand(args, result);
			}
		}

		/// <summary>
		/// Log in to AccuRev if we're supposed to do so.
		/// </summary>
        /// <param name="result">IntegrationResult for which the command will be run</param>
        private void PossiblyLogIn(IIntegrationResult result)
		{
			if (!LogIn)
				return;		// Nothing to do.
			if ((AccuRevPrincipal == null) || (AccuRevPassword == null))
			{
				Log.Error("login=true requires principal= and password= to be specified.");
				return;
			}
            RunCommand(new PrivateArguments("login", AccuRevPrincipal, AccuRevPassword), result);
		}

		/// <summary>
		/// Prepare an AccuRev command for execution.
		/// </summary>
		/// <param name="args">arguments for the "accurev" command</param>
		/// <param name="result">IntegrationResult for which the command will be run</param>
		/// <returns>a ProcessInfo object primed to execute the specified command</returns>
        private ProcessInfo PrepCommand(PrivateArguments args, IIntegrationResult result)
		{
			Log.Debug(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Preparing to run AccuRev command: {0} {1}", Executable, args));
			ProcessInfo command = new ProcessInfo(Executable, args, result.BaseFromWorkingDirectory(Workspace));
			SetEnvironmentVariables(command.EnvironmentVariables, result);
			return command;
		}

		/// <summary>
		/// Execute an AccuRev command and check the results.
		/// </summary>
		/// <param name="args">arguments for the "accurev" command</param>
		/// <param name="result">IntegrationResult for which the command is being run</param>
		/// <returns>a ProcessResult object with the results from the command</returns>
		private ProcessResult RunCommand(PrivateArguments args, IIntegrationResult result)
		{
			ProcessInfo command = PrepCommand(args, result);
			ProcessResult cmdResults = Execute(command);
			if (cmdResults.Failed)
			{
				Log.Error(string.Format(System.Globalization.CultureInfo.CurrentCulture,"AccuRev command \"{0} {1}\" failed with RC={2}", 
					Executable, args, cmdResults.ExitCode));
				if ((cmdResults.StandardError != null) && (cmdResults.StandardError != string.Empty))
					Log.Error(string.Format(System.Globalization.CultureInfo.CurrentCulture,"\tError output: {0}", cmdResults.StandardError));
			}
			return cmdResults;
		}
		
		/// <summary>
		///  Make sure this IIntegrationResult object has our environment variables set in it.
		///  </summary>
		///  <param name="environmentVariables">The collection of environment variables to be updated.</param>
		/// <param name="result">IntegrationResult for the command whose variables we are updating.</param>
		private void SetEnvironmentVariables(StringDictionary environmentVariables, IIntegrationResult result)
		{
            if (!string.IsNullOrEmpty(AccuRevHomeDir))
				environmentVariables["ACCUREV_HOME"] = result.BaseFromArtifactsDirectory(AccuRevHomeDir);
            if (!string.IsNullOrEmpty(AccuRevPrincipal))
				environmentVariables["ACCUREV_PRINCIPAL"] = AccuRevPrincipal;
		}
	}
}
