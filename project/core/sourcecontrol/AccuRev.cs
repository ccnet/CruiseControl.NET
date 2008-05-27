using System;
using System.Collections.Specialized;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	/// <summary>
	/// Source control integration for (<a href="www.accurev.com">AccuRev Inc.'s</a> eponymous 
	/// source code control product.
	/// </summary>
	/// <remarks>
	/// This code is based on code\sourcecontrol\ClearCase.cs.
	/// </remarks>
	[ReflectorType("accurev")]
	public class AccuRev : ProcessSourceControl
    {
        #region Properties
        
        /// <summary>
		/// Should we automatically obtain updated source from AccuRev or not? 
		/// </summary>
		/// <remarks>
		/// Optional, default is not to do so.
		/// </remarks>
		[ReflectorProperty("autoGetSource", Required = false)]
		public bool AutoGetSource = false;

		/// <summary>
		/// Name of the AccuRev CLI command.  
		/// </summary>
		/// <remarks>
		/// Optional, defaults to "accurev.exe".
		/// <remarks>
		[ReflectorProperty("executable", Required=false)]
		public string Executable = "accurev.exe";

		/// <summary>
		/// The location of the AccuRev home directory, either absolute or relative to the project artifact 
		/// directory.  If not specified, AccuRev will follow its rules for determining the location.  The 
		/// home directory itself is always called ".accurev", and AccuRev will create it if there isn't 
		/// already one present in the home directory.
		/// </summary>
		/// <remarks>
		/// Optional, default is to let AccuRev decide where the home directory is.
		/// </remarks>
		[ReflectorProperty("homeDir", Required=false)]
		public string AccuRevHomeDir = null;
		
		/// <summary>
		/// If set, the source repository will be tagged with the build label upon successful builds.
		/// </summary>
		/// <remarks>
		/// Optional, default is not to tag.
		/// <remarks>
		[ReflectorProperty("labelOnSuccess", Required=false)]
		public bool LabelOnSuccess = false;

		/// <summary>
		/// If true, log in to AccuRev using the specified principal and password.
		/// </summary>
		/// <remarks>
		/// Optional, default is not to log in.  If set to true, "principal" and "password" must also be set.
		/// </remarks>
		[ReflectorProperty("login", Required=false)]
		public bool LogIn = false;
		
		/// <summary>
		/// The password for the AccuRev "principal" (userid).
		/// </summary>
		/// <remarks>
		/// Optional, default is no password.  Only necessary if "login" is set to "true".
		/// </remarks> 
		[ReflectorProperty("password", Required=false)]
		public string AccuRevPassword = null;

		/// <summary>
		/// The AccuRev "principal" (userid) to run under.  If not specified, AccuRev will follow its rules
		/// for determining the principal.
		/// </summary>
		/// <remarks>
		/// Optional, default is to let AccuRev decide who the principal is.  Must be specified if "login" is
		/// set to "true".
		/// </remarks>
		[ReflectorProperty("principal", Required=false)]
		public string AccuRevPrincipal = null;
		
		/// <summary>
		/// Pathname of the root of the AccuRev workspace to update and/or check, either absolute or relative
		/// to the project working directory.
		/// </summary>
		/// <remarks>
		/// Optional, defaults to the project working directory.
		/// <remarks>
		[ReflectorProperty("workspace", Required=false)]
		public string Workspace = string.Empty;
        
        #endregion

        /// <summary>
		/// Create an instance of the source control integration with the default history parser and 
		/// process executor.
		/// </summary>
		/// <remarks>
        /// Uses <see cref="AccuRev(IHistoryParser, ProcessExecutor)"/> to do the heavy lifting.
		/// <remarks>
		public AccuRev() : this(new AccuRevHistoryParser(), new ProcessExecutor())
		{
		}
		
		/// <summary>
        /// Create an instance of the source control integration with the default history parser.
		/// </summary>
		/// <remarks>
        /// Uses <see cref="AccuRev(IHistoryParser, ProcessExecutor)"/> to do the heavy lifting.
		/// <remarks>
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
			Log.Error(string.Format("No \"Basis:\" line found in output from AccuRev \"accurev info\": {0}", cmdResults.StandardOutput));
			return "";
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
        /// command instead of "accurev hist", as suggested by "dhearing1 <dp_godwin@hotmail.com>" to
        /// the ccnet-user list on 2007-07-09.
		/// </remarks>
		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			PossiblyLogIn(from);
			string args = string.Format("hist -a -s \"{0}\" -t \"{1}-{2}\"", 
				GetBasisStreamName(to),
				FormatCommandDate(to.StartTime),
				FormatCommandDate(from.StartTime));
			ProcessInfo histCommand = PrepCommand(args, from);

			return base.GetModifications(histCommand, from.StartTime, to.StartTime);
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
                if (result.LastChangeNumber != 0)
                    command = command + " -t " + result.LastChangeNumber;
                RunCommand(command, result);
			}
		}
		
		/// <summary>
		/// Label the specified source level.  In AccuRev terms, create a snapshot based on that level.
		/// </summary>
        /// <param name="result">the IntegrationResult containing the label</param>
		public override void LabelSourceControl(IIntegrationResult result)
		{
			if (LabelOnSuccess && result.Succeeded && (result.Label != ""))
			{
				PossiblyLogIn(result);
				string args = string.Format("mksnap -s \"{0}\" -b \"{1}\" -t \"{2}\"",
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
			RunCommand(string.Format("login {0} \"{1}\"", AccuRevPrincipal, AccuRevPassword), result);
		}

		/// <summary>
		/// Prepare an AccuRev command for execution.
		/// </summary>
		/// <param name="args">arguments for the "accurev" command</param>
		/// <param name="result">IntegrationResult for which the command will be run</param>
		/// <returns>a ProcessInfo object primed to execute the specified command</returns>
		private ProcessInfo PrepCommand(string args, IIntegrationResult result)
		{
			Log.Debug(string.Format("Preparing to run AccuRev command: {0} {1}", Executable, args));
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
		private ProcessResult RunCommand(string args, IIntegrationResult result)
		{
			ProcessInfo command = PrepCommand(args, result);
			ProcessResult cmdResults = Execute(command);
			if (cmdResults.Failed)
			{
				Log.Error(string.Format("AccuRev command \"{0} {1}\" failed with RC={2}", 
					Executable, args, cmdResults.ExitCode));
				if ((cmdResults.StandardError != null) && (cmdResults.StandardError != ""))
					Log.Error(string.Format("\tError output: {0}", cmdResults.StandardError));
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
			if (!StringUtil.IsBlank(AccuRevHomeDir))
				environmentVariables["ACCUREV_HOME"] = result.BaseFromArtifactsDirectory(AccuRevHomeDir);
			if (!StringUtil.IsBlank(AccuRevPrincipal))
				environmentVariables["ACCUREV_PRINCIPAL"] = AccuRevPrincipal;
		}
	}
}
