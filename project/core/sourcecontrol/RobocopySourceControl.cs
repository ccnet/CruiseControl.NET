using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
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

		public RobocopySourceControl() : this(new RobocopyHistoryParser(), new ProcessExecutor())
		{}

		public RobocopySourceControl(IHistoryParser parser, ProcessExecutor executor) : base(parser, executor)
		{}

		[ReflectorProperty("executable", Required = false)]
		public string Executable = "C:\\Windows\\System32\\robocopy.exe";	 

		[ReflectorProperty("repositoryRoot")]
		public string RepositoryRoot;

		[ReflectorProperty("autoGetSource", Required = false)]
		public bool AutoGetSource = false;

		[ReflectorProperty("workingDirectory", Required = false)]
		public string WorkingDirectory = string.Empty;

		[ReflectorProperty("additionalArguments", Required = false)]
		public string AdditionalArguments = string.Empty;

		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			string destinationDirectory = from.BaseFromWorkingDirectory(WorkingDirectory);

			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();

			AddStandardArguments(builder, destinationDirectory);

			builder.AddArgument("/L");

			Modification[] modifications = GetModifications(new ProcessInfo(Executable, builder.ToString(), null, successExitCodes), from.StartTime, to.StartTime);

			return modifications;
		}

		public override void LabelSourceControl(IIntegrationResult result)
		{}

		public override void GetSource(IIntegrationResult result)
		{
			if (AutoGetSource)
			{
				string destinationDirectory = result.BaseFromWorkingDirectory(WorkingDirectory);
		
				ProcessArgumentBuilder builder = new ProcessArgumentBuilder();

				AddStandardArguments(builder, destinationDirectory);

				Execute(new ProcessInfo(Executable, builder.ToString(), null, successExitCodes));
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