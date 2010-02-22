using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// A source control implementation for use when the source control system 
    /// doesn't integrate directly with CCNet.
    /// </summary>
    /// <title>External Source Control Configuration</title>
    /// <version>1.3</version>
    /// <key name="type">
    /// <description>The type of source control block.</description>
    /// <value>external</value>
    /// </key>
    /// <example>
    /// <code title="Minimal Configuration">
    /// &lt;sourcecontrol type="external"&gt;
    /// &lt;executable&gt;path to command-line application&lt;/executable&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// <code title="Full Example">
    /// &lt;sourcecontrol type="external"&gt;
    /// &lt;executable&gt;path to command-line application&lt;/executable&gt;
    /// &lt;args&gt;arguments for the command-line application&lt;/args&gt;
    /// &lt;autoGetSource&gt;false&lt;/autoGetSource&gt;
    /// &lt;labelOnSuccess&gt;false&lt;/labelOnSuccess&gt;
    /// &lt;environment&gt;
    /// &lt;var&gt;name=value&lt;/var&gt;
    /// &lt;var&gt;name=value&lt;/var&gt;
    /// &lt;/environment&gt;
    /// &lt;timeout units="minutes"&gt;10&lt;/timeout&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>
    /// Each of the three sourcecontrol operations (GetModifications(), GetSource(), and LabelSourceControl()) are passed to the source control
    /// command as a command line.
    /// </para>
    /// <heading>GetModifications</heading>
    /// <para>
    /// The GetModifications function is invoked as the GETMODS operation, and specifying a starting and ending timestamp:
    /// </para>
    /// <code type="None">
    /// executable GETMODS "fromtimestamp" "totimestamp" args
    /// </code>
    /// <para>
    /// The source control command should search for modifications between these two times inclusively, write their details to the standard
    /// output stream in the XML format used by the <link>Modification Writer Task</link>, and exit with exit status 0 (any other status
    /// indicates an error and will fail the build). For example, the following represents two modifications, numbered 12244 and 12245.
    /// </para>
    /// <code>
    /// &lt;ArrayOfModification&gt;
    /// &lt;Modification&gt;
    /// &lt;ChangeNumber&gt;12245&lt;/ChangeNumber&gt;
    /// &lt;Comment&gt;New Project for testing stuff&lt;/Comment&gt;
    /// &lt;EmailAddress&gt;JUser@Example.Com&lt;/EmailAddress&gt;
    /// &lt;FileName&gt;AssemblyInfo.cs&lt;/FileName&gt;
    /// &lt;FolderName&gt;Dev\Server\Interface\Properties\&lt;/FolderName&gt;
    /// &lt;ModifiedTime&gt;2006-11-22T11:11:00-0500&lt;/ModifiedTime&gt;
    /// &lt;Type&gt;add&lt;/Type&gt;
    /// &lt;UserName&gt;joe_user&lt;/UserName&gt;
    /// &lt;Url&gt;http://www.example.com/index.html&lt;/Url&gt;
    /// &lt;Version&gt;5&lt;/Version&gt;
    /// &lt;/Modification&gt;
    /// &lt;Modification&gt;
    /// &lt;ChangeNumber&gt;12244&lt;/ChangeNumber&gt;
    /// &lt;Comment&gt;New Project for accessing web services&lt;/Comment&gt;
    /// &lt;EmailAddress&gt;SSpade@Example.Com&lt;/EmailAddress&gt;
    /// &lt;FileName&gt;Interface&lt;/FileName&gt;
    /// &lt;FolderName&gt;Dev\Server\&lt;/FolderName&gt;
    /// &lt;ModifiedTime&gt;2006-11-22T11:10:44-0500&lt;/ModifiedTime&gt;
    /// &lt;Type&gt;add&lt;/Type&gt;
    /// &lt;UserName&gt;sam_spade&lt;/UserName&gt;
    /// &lt;Url&gt;http://www.example.com/index.html&lt;/Url&gt;
    /// &lt;Version&gt;4&lt;/Version&gt;
    /// &lt;/Modification&gt;
    /// &lt;/ArrayOfModification&gt;
    /// </code>
    /// <heading>GetSource</heading>
    /// <para>
    /// The GetSource function is invoked as the GETSOURCE operation, and specifying a working directory path and the target timestamp:
    /// </para>
    /// <code type="None">
    /// executable GETSOURCE "workingdirectory" "timestamp" args
    /// </code>
    /// <para>
    /// The source control command should update the files in the specified working directory to the versions current as of the specified time
    /// stamp and exit with exit status 0 (any other status indicates an error and will fail the build).
    /// </para>
    /// <heading>LabelSourceControl</heading>
    /// <para>
    /// The LabelSourceControl function is invoked as the SETLABEL operation, and specifying a label to be applied and the target timestamp:
    /// </para>
    /// <code type="None">
    /// executable SETLABEL "label" "sourcetimestamp" args
    /// </code>
    /// <para>
    /// The source control command should add the label to source repository and exit with exit status 0 (any other status indicates an error 
    /// and will fail the build).
    /// </para>
    /// <para type="warning">
    /// <para>
    /// Watch out for the comment tag, if this contains dodgy charatecters eg.: &lt;   it will fail the getsource. Be sure to escape these
    /// characters.  So replace these with there XML equivalents : &amp;amp;lt;
    /// </para>
    /// <para>
    /// Be careful of the &lt;ModifiedTime&gt;, this MUST be more than the fromtimestamp if it is &lt;= then the modification will not be
    /// detected.
    /// </para>
    /// <para>
    /// You don't need the following parameters for this to work:
    /// </para>
    /// <list type="1">
    /// <item>&lt;Type /&gt;</item>
    /// <item>&lt;FileName /&gt;</item>
    /// <item>&lt;FolderName /&gt;</item>
    /// <item>&lt;Version /&gt;</item>
    /// <item>&lt;EmailAddress /&gt;</item>
    /// </list>
    /// </para>
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
        /// </remarks>
        public ExternalSourceControl()
            : this(new ExternalSourceControlHistoryParser(), new ProcessExecutor())
        {
        }

        /// <summary>
        /// Create an instance of the source control integration with the default history parser.
        /// </summary>
        /// <remarks>
        /// Uses <see cref="ExternalSourceControl(IHistoryParser, ProcessExecutor)"/> to do the heavy lifting.
        /// </remarks>
        public ExternalSourceControl(ProcessExecutor executor)
            : this(new ExternalSourceControlHistoryParser(), executor)
        {
        }

        /// <summary>
        /// Create an instance of the source control integration.
        /// </summary>
        public ExternalSourceControl(IHistoryParser parser, ProcessExecutor executor)
            : base(parser, executor)
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
        /// <version>1.3</version>
        /// <default>None</default>
        [ReflectorProperty("args", Required = false)]
        public string ArgString = string.Empty;

        /// <summary>
        /// Should we automatically obtain updated source from the source control system or not? 
        /// </summary>
        /// <version>1.3</version>
        /// <default>false</default>
        [ReflectorProperty("autoGetSource", Required = false)]
        public bool AutoGetSource = false;

        /// <summary>
        /// A set of environment variables set for commands that are executed.
        /// </summary>
        /// <version>1.3</version>
        /// <default>None</default>
        [ReflectorArray("environment", Required = false)]
        public EnvironmentVariable[] EnvironmentVariables = new EnvironmentVariable[0];

        /// <summary>
        /// Name of the source control system executable to run.
        /// </summary>
        /// <version>1.3</version>
        /// <default>n/a</default>
        [ReflectorProperty("executable", Required = true)]
        public string Executable;

        /// <summary>
        /// If set, the source repository will be tagged with the build label upon successful builds.
        /// </summary>
        /// <version>1.3</version>
        /// <default>false</default>
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


            Modification[] modifications = base.GetModifications(command, from.StartTime, to.StartTime);
            base.FillIssueUrl(modifications);
            return modifications;
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
        /// </remarks>
        public override void LabelSourceControl(IIntegrationResult result)
        {
            if (LabelOnSuccess && result.Succeeded && (result.Label != string.Empty))
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
                if ((cmdResults.StandardError != null) && (cmdResults.StandardError !=string.Empty))
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
