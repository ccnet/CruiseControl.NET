using System.Diagnostics;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// <para>
    /// Uses RoboCopy as Source Control.
    /// </para>
    /// </summary>	
    /// <title>RoboCopy Source Control Block</title>
    /// <version>1.4.4</version>
    /// <key name="type">
    /// <description>The type of source control block.</description>
    /// <value>robocopy</value>
    /// </key>
    /// <example>
    /// <code>
    /// &lt;sourcecontrol type="repositoryRoot"&gt;
    /// &lt;repositoryRoot&gt;C:\Somewhere&lt;/repositoryRoot&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// </example>
    [ReflectorType("robocopy")]
	public class RobocopySourceControl : ProcessSourceControl
	{
		private static int[] GenerateExitCodes()
		{
			int[] exitCodes = new int[4];

			exitCodes[0] = 0;			// All OK, nothing to do
			exitCodes[1] = 1;			// Some files copied
			exitCodes[2] = 2;			// Some extra files in destination tree
			exitCodes[3] = 3;			// Copied and some extra files in destination tree

			// Note that we COULD want to have 4-7 as valid success codes, but i've not been 
			// able to cause them to occur yet and so havent included them here.
			
			return exitCodes;
		}

		private static readonly int[] successExitCodes = GenerateExitCodes();

        /// <summary>
        /// Initializes a new instance of the <see cref="RobocopySourceControl" /> class.	
        /// </summary>
        /// <remarks></remarks>
		public RobocopySourceControl() : this(new RobocopyHistoryParser(), new ProcessExecutor())
		{}

        /// <summary>
        /// Initializes a new instance of the <see cref="RobocopySourceControl" /> class.	
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="executor">The executor.</param>
        /// <remarks></remarks>
        public RobocopySourceControl(IHistoryParser parser, ProcessExecutor executor)
            : base(parser, executor)
        {
            this.Executable = "C:\\Windows\\System32\\robocopy.exe";
            this.AutoGetSource = false;
            this.WorkingDirectory = string.Empty;
            this.AdditionalArguments = string.Empty;
        }

        /// <summary>
        /// The executable location.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>C:\\Windows\\System32\\robocopy.exe</default>
        [ReflectorProperty("executable", Required = false)]
        public string Executable { get; set; }

        /// <summary>
        /// The repository root.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>n/a</default>
        [ReflectorProperty("repositoryRoot")]
        public string RepositoryRoot { get; set; }

        /// <summary>
        /// Whether to automatically get the source.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>false</default>
        [ReflectorProperty("autoGetSource", Required = false)]
        public bool AutoGetSource { get; set; }

        /// <summary>
        /// The working directory to use.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>Project Working Directory</default>
        [ReflectorProperty("workingDirectory", Required = false)]
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Any additional arguments.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>None</default>
        [ReflectorProperty("additionalArguments", Required = false)]
        public string AdditionalArguments { get; set; }

        /// <summary>
        /// Gets the modifications.	
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			string destinationDirectory = from.BaseFromWorkingDirectory(WorkingDirectory);

			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();

			AddStandardArguments(builder, destinationDirectory);

			builder.AddArgument("/L");

			Modification[] modifications = GetModifications(new ProcessInfo(Executable, builder.ToString(), null, ProcessPriorityClass.Normal, successExitCodes), from.StartTime, to.StartTime);

			return modifications;
		}

        /// <summary>
        /// Labels the source control.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
		public override void LabelSourceControl(IIntegrationResult result)
		{}

        /// <summary>
        /// Gets the source.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
		public override void GetSource(IIntegrationResult result)
		{
			if (AutoGetSource)
			{
				string destinationDirectory = result.BaseFromWorkingDirectory(WorkingDirectory);
		
				ProcessArgumentBuilder builder = new ProcessArgumentBuilder();

				AddStandardArguments(builder, destinationDirectory);

                Execute(new ProcessInfo(Executable, builder.ToString(), null, ProcessPriorityClass.Normal, successExitCodes));
			}
		}

		// /MIR - MIRror a directory tree (equivalent to /E plus /PURGE).
		// /NP	- No Progress - don't display % copied.
		// /X	- Report all eXtra files, not just those selected.
		// /TS	- Include source file Time Stamps in the output.
		// /FP	- Include Full Pathname of files in the output.
		// /NDL	- No Directory List - don't log directory names.
		// /NS	- No Size - don't log file sizes.
		// /NJH	- No Job Header.
		// /NJS	- No Job Summary.

		private readonly static string standardArguments = " /MIR /NP /X /TS /FP /NDL /NS /NJH /NJS ";

		private void AddStandardArguments(
			ProcessArgumentBuilder builder,
			string destinationDirectory)
		{
			builder.AddArgument(RepositoryRoot);
			builder.AddArgument(destinationDirectory);
			builder.Append(standardArguments);
			builder.Append(AdditionalArguments);
		}
	}
}