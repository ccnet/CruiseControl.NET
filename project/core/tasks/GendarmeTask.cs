using System.IO;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// <para>
    /// Gendarme is a extensible rule-based tool to find problems in .NET applications and libraries. Gendarme inspects programs and libraries
    /// that contain code in ECMA CIL format (Mono and .NET) and looks for common problems with the code, problems that compiler do not
    /// typically check or have not historically checked. Website: http://mono-project.com/Gendarme
    /// </para>
    /// <para type="tip">
    /// See <link>Using CruiseControl.NET with Gendarme</link> for more details.
    /// </para>
    /// </summary>
    /// <title>Gendarme Task</title>
    /// <version>1.4.3</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;gendarme&gt;
    /// &lt;assemblies&gt;
    /// &lt;assemblyMatch expr='*.dll' /&gt;
    /// &lt;assemblyMatch expr='*.exe' /&gt;
    /// &lt;/assemblies&gt;
    /// &lt;/gendarme&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;gendarme&gt;
    /// &lt;executable&gt;Tools\gendarme.exe&lt;/executable&gt;
    /// &lt;baseDirectory&gt;C:\Build\Project1\Bin\Debug\&lt;/baseDirectory&gt;
    /// &lt;configFile&gt;rules.xml&lt;/configFile&gt;
    /// &lt;ruleSet&gt;*&lt;/ruleSet&gt;
    /// &lt;ignoreFile&gt;C:\Build\Project1\gendarme.ignore.list.txt&lt;/ignoreFile&gt;
    /// &lt;limit&gt;200&lt;/limit&gt;
    /// &lt;severity&gt;medium+&lt;/severity&gt;
    /// &lt;confidence&gt;normal+&lt;/confidence&gt;
    /// &lt;quiet&gt;FALSE&lt;/quiet&gt;
    /// &lt;verbose&gt;TRUE&lt;/verbose&gt;
    /// &lt;failBuildOnFoundDefects&gt;TRUE&lt;/failBuildOnFoundDefects&gt;
    /// &lt;verifyTimeoutSeconds&gt;600&lt;/verifyTimeoutSeconds&gt;
    /// &lt;assemblyListFile&gt;C:\Build\Project1\gendarme.assembly.list.txt&lt;/assemblyListFile&gt;
    /// &lt;description&gt;Test description&lt;/description&gt;
    /// &lt;/gendarme&gt;
    /// </code>
    /// </example>
	[ReflectorType("gendarme")]
	public class GendarmeTask : BaseExecutableTask
	{
		public const string defaultExecutable = "gendarme";
		public const string logFilename = "gendarme-results.xml";
		public const int defaultLimit = -1;
		public const bool defaultQuiet = false;
		public const bool defaultVerbose = false;
		public const bool defaultFailBuildOnFoundDefects = false;
		public const int defaultVerifyTimeout = 0;

        private readonly IFileDirectoryDeleter fileDirectoryDeleter = new IoService();

		public GendarmeTask(): 
			this(new ProcessExecutor()){}

		public GendarmeTask(ProcessExecutor executor)
		{
			this.executor = executor;
		}

		#region public properties

		/// <summary>
        /// The location of the Gendarme executable.
		/// </summary>
        /// <version>1.4.3</version>
        /// <default>gendarme</default>
		[ReflectorProperty("executable", Required = false)]
		public string Executable = defaultExecutable;

		/// <summary>
        /// The directory to run Gendarme in.
		/// </summary>
        /// <version>1.4.3</version>
        /// <default>Project Working Directory</default>
        [ReflectorProperty("baseDirectory", Required = false)]
		public string ConfiguredBaseDirectory = string.Empty;

		/// <summary>
        /// Specify the configuration file.
		/// </summary>
        /// <version>1.4.3</version>
        /// <default>rules.xml</default>
        /// <remarks>
        /// <b>Maps to "--config configfile"</b>
        /// </remarks>
        [ReflectorProperty("configFile", Required = false)]
		public string ConfigFile = string.Empty;

		/// <summary>
		/// Specify the set of rules to verify.
		/// </summary>
        /// <version>1.4.3</version>
        /// <default>*</default>
        /// <remarks>
        /// <b>Maps to "--set ruleset"</b>
        /// </remarks>
        [ReflectorProperty("ruleSet", Required = false)]
		public string RuleSet = string.Empty;

		/// <summary>
		/// Do not report the known defects that are part of the specified file.
		/// </summary>
        /// <version>1.4.3</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to "--ignore ignore-file"</b>
        /// </remarks>
        [ReflectorProperty("ignoreFile", Required = false)]
		public string IgnoreFile = string.Empty;

		/// <summary>
		/// Stop reporting after N defects are found.
		/// </summary>
        /// <version>1.4.3</version>
        /// <default>-1</default>
        /// <remarks>
        /// <b>Maps to "--limit N"</b>
        /// </remarks>
        [ReflectorProperty("limit", Required = false)]
		public int Limit = defaultLimit;

		/// <summary>
		/// Filter the reported defects to include the specified severity levels.
		/// </summary>
        /// <version>1.4.3</version>
        /// <default>Medium+</default>
        /// <remarks>
        /// <b>Maps to "--severity [all | audit[+] | low[+|-] | medium[+|-] | high[+|-] | critical[-]],..."</b>
        /// </remarks>
        [ReflectorProperty("severity", Required = false)]
		public string Severity = string.Empty;

		/// <summary>
		/// Filter the reported defects to include the specified confidence levels.
		/// </summary>
        /// <version>1.4.3</version>
        /// <default>normal+</default>
        /// <remarks>
        /// <b>"--confidence [all | low[+] | normal[+|-] | high[+|-] | total[-]],..."</b>
        /// </remarks>
        [ReflectorProperty("confidence", Required = false)]
		public string Confidence = string.Empty;

		/// <summary>
		/// If true, display minimal output (results) from the runner.
		/// </summary>
        /// <version>1.4.3</version>
        /// <default>false</default>
        [ReflectorProperty("quiet", Required = false)]
		public bool Quiet = defaultQuiet;

		/// <summary>
		/// Enable debugging output.
		/// </summary>
        /// <version>1.4.3</version>
        /// <default>false</default>
        [ReflectorProperty("verbose", Required = false)]
		public bool Verbose = defaultVerbose;

		/// <summary>
		/// Specify whenver the build should fail if some defects are found by Gendarme.
		/// </summary>
        /// <version>1.4.3</version>
        /// <default>false</default>
        [ReflectorProperty("failBuildOnFoundDefects", Required = false)]
		public bool FailBuildOnFoundDefects = defaultFailBuildOnFoundDefects;

		/// <summary>
		/// Specify the assemblies to verify. You can specify multiple filenames, including masks (? and *).
		/// </summary>
        /// <version>1.4.3</version>
        /// <default>None</default>
        [ReflectorArray("assemblies", Required = false)]
		public AssemblyMatch[] Assemblies = new AssemblyMatch[0];

		/// <summary>
		/// Specify a file that contains the assemblies to verify. You can specify multiple filenames, including masks (? and *), one by line.
		/// </summary>
        /// <version>1.4.3</version>
        /// <default>None</default>
        [ReflectorProperty("assemblyListFile", Required = false)]
		public string AssemblyListFile = string.Empty;

		/// <summary>
		/// The maximum number of seconds that the build may take.  If the build process takes longer than this period, it will be killed.  Specify this value as zero to disable process timeouts.
		/// </summary>
        /// <version>1.4.3</version>
        /// <default>0</default>
        [ReflectorProperty("verifyTimeoutSeconds", Required = false)]
		public int VerifyTimeoutSeconds = defaultVerifyTimeout;
		#endregion

		#region BaseExecutableTask overrides

		protected override string GetProcessFilename()
		{
			return Executable;
		}

		protected override string GetProcessArguments(IIntegrationResult result)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AppendIf(!string.IsNullOrEmpty(ConfigFile), "--config {0}", StringUtil.AutoDoubleQuoteString(ConfigFile));
			buffer.AppendIf(!string.IsNullOrEmpty(RuleSet), "--set {0}", RuleSet);
			buffer.AppendIf(!string.IsNullOrEmpty(IgnoreFile), "--ignore {0}", StringUtil.AutoDoubleQuoteString(IgnoreFile));
			buffer.AppendIf(Limit > 0, "--limit {0}", Limit.ToString());
			buffer.AppendIf(!string.IsNullOrEmpty(Severity), "--severity {0}", Severity);
			buffer.AppendIf(!string.IsNullOrEmpty(Confidence), "--confidence {0}", Confidence);
			buffer.AppendIf(Quiet, "--quiet");
			buffer.AppendIf(Verbose, "--verbose");

			// append output xml file
			buffer.AppendArgument("--xml {0}", StringUtil.AutoDoubleQuoteString(GetGendarmeOutputFile(result)));

			// append assembly list or list file
			CreateAssemblyList(buffer);

			return buffer.ToString();
		}

		protected override string GetProcessBaseDirectory(IIntegrationResult result)
		{
			return result.BaseFromWorkingDirectory(ConfiguredBaseDirectory);
		}

		protected override int GetProcessTimeout()
		{
			return VerifyTimeoutSeconds * 1000;
		}

        protected override bool Execute(IIntegrationResult result)
		{
            string gendarmeOutputFile = GetGendarmeOutputFile(result);
            //delete old nant output logfile, if exist
            fileDirectoryDeleter.DeleteIncludingReadOnlyObjects(gendarmeOutputFile);

            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description :
				"Executing Gendarme to verifiy assemblies.");

			ProcessResult processResult = TryToRun(CreateProcessInfo(result), result);


            if (File.Exists(gendarmeOutputFile))
            {
                result.AddTaskResult(new FileTaskResult(gendarmeOutputFile));
            }

            result.AddTaskResult(new ProcessTaskResult(processResult, true));

			if (processResult.TimedOut)
				throw new BuilderException(this, string.Concat("Gendarme process timed out (after ", VerifyTimeoutSeconds, " seconds)"));

            return !processResult.Failed;
		}

		/// <summary>
		/// Gendarme returns the following codes:
		/// - 0 for success
		/// - 1 if some defects are found
		/// - 2 if some parameters are bad
		/// - 3 if a problem is related to the xml configuration file
		/// - 4 if an uncaught exception occured
		/// </summary>
		/// <returns>Defects should not break the build, so return an array of 0 and 1.</returns>
		protected override int[] GetProcessSuccessCodes()
		{
			if (FailBuildOnFoundDefects)
				return new int[] {0};

			return new int[] {0, 1};
		}

		#endregion

		#region private methods

		private static string GetGendarmeOutputFile(IIntegrationResult result)
		{
			return Path.Combine(result.ArtifactDirectory, logFilename);
		}

		private void CreateAssemblyList(ProcessArgumentBuilder buffer)
		{
			if (string.IsNullOrEmpty(AssemblyListFile) && (Assemblies == null || Assemblies.Length == 0))
				throw new ConfigurationException("[GendarmeTask] Neither 'assemblyListFile' nor 'assemblies' are specified. Please specify one of them.");

			// append the assembly list file if set
			if (!string.IsNullOrEmpty(AssemblyListFile))
				buffer.AppendArgument(string.Concat("@", StringUtil.AutoDoubleQuoteString(AssemblyListFile)));

			// build the assembly list by the assembly match collection
			foreach (AssemblyMatch asm in Assemblies)
				buffer.AppendArgument(asm.Expression);
		}

		#endregion
	}
}
