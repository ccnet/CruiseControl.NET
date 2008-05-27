using System;
using System.Collections;
using System.Globalization;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using System.Collections.Specialized;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// A source control implementation for use when the source control system 
    /// doesn't integrate directly with CCNet.
    /// </summary>
    /// <remarks>
    /// This source control implementation acts as a proxy for an external system
    /// of unspecified vintage.  The system (or more likely a batch file created
    /// by the administrator that issues source control commands) must implement
    /// the following commands:
    /// <list type="u">
    /// <item> <param>executable</param> <code>GETMODS</code> "<param>fromtimestamp</param>" "<param>totimestamp</param>" <param>argstring</param> </item>
    /// <item>  <param>executable</param> <code>GETSOURCE</code> "<param>workingdirectory</param>" "<param>timestamp</param>" <param>argstring</param> </item>
    /// <item>  <param>executable</param> <code>SETLABEL</code> "<param>label</param>" "<param>sourcetimestamp</param>" <param>argstring</param> </item>
    /// </list>
    /// </remarks>
	[ReflectorType("external")]
	public class ExternalSourceControl : ProcessSourceControl
    {
        #region Fields
        // None yet
        #endregion

        #region Constructors

        /// <summary>
		/// Create an instance of the source control integration with the default history parser and
		/// process executor.
		/// </summary>
		/// <remarks>
		/// Uses <see cref="ExternalSourceControl(IHistoryParser, ProcessExecutor)"/> to do the heavy lifting.
		/// <remarks>
		public ExternalSourceControl() : this(new ExternalSourceControlHistoryParser(), new ProcessExecutor())
		{
		}
		
		/// <summary>
        /// Create an instance of the source control integration with the default history parser.
		/// </summary>
		/// <remarks>
		/// Uses <see cref="ExternalSourceControl(IHistoryParser, ProcessExecutor)"/> to do the heavy lifting.
		/// <remarks>
		public ExternalSourceControl(ProcessExecutor executor) : this(new ExternalSourceControlHistoryParser(), executor)
		{
		}
      
		/// <summary>
		/// Create an instance of the source control integration.
		/// </summary>
		public ExternalSourceControl(IHistoryParser parser, ProcessExecutor executor) : base(parser, executor)
		{
		}

        #endregion

        #region Properties
        /// <summary>
        /// A string to be passed to the external source control program in commands.
        /// </summary>
        /// <remarks>
        /// The string will be passed exactly as specified.  In particular, it will not be enclosed
        /// quotation marks, thus allowing you to specify what the executable will see as multiple
        /// parameters.
        /// </remarks>
        [ReflectorProperty("args", Required=false)]
		public string ArgString = "";

        /// <summary>
        /// Should we automatically obtain updated source from the source control system or not? 
        /// </summary>
        /// <remarks>
        /// Optional, default is not to do so.
        /// </remarks>
        [ReflectorProperty("autoGetSource", Required = false)]
		public bool AutoGetSource = false;

        /// <summary>
        /// A set of environment variables set for commands that are executed.
        /// </summary>
        /// <remarks>
        /// Each variable should be specified as <code>&lt;variable name="name" value="value"/&gt;</code>.
        /// </remarks>
        [ReflectorArray("environment", Required = false)]
        public EnvironmentVariable[] EnvironmentVariables = new EnvironmentVariable[0];

        /// <summary>
        /// Name of the source control system executable to run.
        /// </summary>
        [ReflectorProperty("executable", Required = true)]
        public string Executable;

        /// <summary>
        /// If set, the source repository will be tagged with the build label upon successful builds.
        /// </summary>
        /// <remarks>
        /// Optional, default is not to tag.
        /// <remarks>
        [ReflectorProperty("labelOnSuccess", Required = false)]
        public bool LabelOnSuccess = false;

        #endregion

        #region Interface methods
        /// <summary>
        /// Obtain a list of modified files between the specified points on the revision history.
        /// </summary>
        /// <param name="from">the IntegrationResult containing the starting timestamp</param>
        /// <param name="to">the IntegrationResult containing the ending timestamp</param>
        /// <remarks>
        /// This method creates and runs a command to list all the modifications in the specified 
        /// timespan, and expects the modifications to be returned in the serialized form of the
        /// ThoughtWorks.CruiseControl.Core.Modification class <i>a la</i> 
        /// <see cref="System.Xml.Serialization.XmlSerializer.Serialize(Stream, object)"/>.
        /// 
        /// The command executed is:
        /// <param>executable</param> <code>GETMODS</code> "<param>fromtimestamp</param>" "<param>totimestamp</param>" <param>argstring</param>
        /// with timestamps represented as "<i>yyyy</i>-<i>mm</i>-<i>dd</i> <i>hh</i>:<i>mm</i>:<i>ss</i>"
        /// in local 24-hour time.
        /// The command must return the modification list as its standard output.
        /// </remarks>
        public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
            string args = string.Format(@"GETMODS ""{0}"" ""{1}"" {2}",
                FormatCommandDate(to.StartTime),
                FormatCommandDate(from.StartTime),
                ArgString);
            ProcessInfo command = PrepCommand(Executable, args, from);

			return base.GetModifications(command, from.StartTime, to.StartTime);
        }

        /// <summary>
        /// Obtain the specified level on the source code. 
        /// </summary>
        /// <param name="result">the the IntegrationResult containing the timestamp</param>
        /// <remarks>
        /// The command executed is:
        /// <param>executable</param> <code>GETSOURCE</code> "<param>workingdirectory</param>" "<param>timestamp</param>" <param>argstring</param>
        /// with the timestamp represented as "<i>yyyy</i>-<i>mm</i>-<i>dd</i> <i>hh</i>:<i>mm</i>:<i>ss</i>"
        /// in local 24-hour time.
        /// </remarks>
        public override void GetSource(IIntegrationResult result)
        {
            result.BuildProgressInformation.SignalStartRunTask("Getting source from External Source Control");

            if (AutoGetSource)
            {
                string args = string.Format(@"GETSOURCE ""{0}"" ""{1}"" {2}",
                    result.WorkingDirectory,
                    FormatCommandDate(result.StartTime),
                    ArgString);
                RunCommand(Executable, args, result);
            }
        }

        /// <summary>
        /// Label the specified source level.
        /// </summary>
        /// <param name="result">the IntegrationResult containing the label</param>
        /// <remarks>
        /// The command executed is:
        /// <param>executable</param> <code>SETLABEL</code> "<param>label</param>" "<param>sourcetimestamp</param>" <param>argstring</param>
        /// with the source timestamp represented as "<i>yyyy</i>-<i>mm</i>-<i>dd</i> <i>hh</i>:<i>mm</i>:<i>ss</i>"
        /// in local 24-hour time.
        public override void LabelSourceControl(IIntegrationResult result)
        {
            if (LabelOnSuccess && result.Succeeded && (result.Label != ""))
			{
				string args = string.Format(@"SETLABEL ""{0}"" ""{1}"" {2}",
					result.Label,
					FormatCommandDate(result.StartTime),
                    ArgString);
				RunCommand(Executable, args, result);
			}
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Format a timestamp the way the commands need to see it.
        /// </summary>
        /// <param name="date">the timestamp to format.</param>
        /// <returns>the timestamp as a string in "yyyy-mm-dd hh:mm:ss" form in local time</returns>
        private static string FormatCommandDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Prepare a command for execution.
        /// </summary>
        /// <param name="executable">name of command to run</param>
        /// <param name="args">arguments for the command</param>
        /// <param name="result">IntegrationResult for which the command will be run</param>
        /// <returns>a ProcessInfo object primed to execute the specified command</returns>
        private ProcessInfo PrepCommand(string executable, string args, IIntegrationResult result)
        {
            Log.Debug(string.Format("Preparing to run source control command: {0} {1}", Executable, args));
            ProcessInfo command = new ProcessInfo(executable, args, result.WorkingDirectory);
            SetConfiguredEnvironmentVariables(command.EnvironmentVariables, this.EnvironmentVariables);
            SetCCNetEnvironmentVariables(command.EnvironmentVariables, result.IntegrationProperties);
            return command;
        }

        /// <summary>
        /// Execute a command and check the results.
        /// </summary>
        /// <param name="executable">name of command to run</param>
        /// <param name="args">arguments for the "accurev" command</param>
        /// <param name="result">IntegrationResult for which the command is being run</param>
        /// <returns>a ProcessResult object with the results from the command</returns>
        private ProcessResult RunCommand(string executable, string args, IIntegrationResult result)
        {
            ProcessInfo command = PrepCommand(executable, args, result);
            ProcessResult cmdResults = Execute(command);
            if (cmdResults.Failed)
            {
                Log.Error(string.Format(@"Source control command ""{0} {1}"" failed with RC={2}",
                    Executable, args, cmdResults.ExitCode));
                if ((cmdResults.StandardError != null) && (cmdResults.StandardError != ""))
                    Log.Error(string.Format("\tError output: {0}", cmdResults.StandardError));
            }
            return cmdResults;
        }

        /// <summary>
        /// Pass CCNet's standard environment variables to the process.
        /// </summary>
        /// <param name="variablePool">The collection of environment variables to be updated.</param>
        /// <param name="varsToSet">The collection of variables to set.</param>
        /// <remarks>
        /// Any variable without a value will be set to null (just as in 
        /// <see cref="ExecutableTask.NewProcessInfoFrom"/>).
        /// </remarks>
        private static void SetCCNetEnvironmentVariables(StringDictionary variablePool, IDictionary varsToSet)
        {
            foreach (string key in varsToSet.Keys)
            {
                variablePool[key] = (varsToSet[key] == null) ? null : varsToSet[key].ToString();
            }

        }

        /// <summary>
        /// Pass the project's environment variables to the process.
        /// </summary>
        /// <param name="variablePool">The collection of environment variables to be updated.</param>
        /// <param name="varsToSet">An array of environment variables to set.</param>
        /// <remarks>
        /// Any variable without a value will be set to an empty string.
        /// </remarks>
        private static void SetConfiguredEnvironmentVariables(StringDictionary variablePool, EnvironmentVariable[] varsToSet)
        {
            foreach (EnvironmentVariable item in varsToSet)
                variablePool[item.name] = item.value;
        }

        #endregion
    }
}
