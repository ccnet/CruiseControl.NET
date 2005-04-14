using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	[ReflectorType("nunit")]
	public class NUnitTask : ITask
	{
		private const string DefaultOutputFile = "nunit-results.xml";
		private ProcessExecutor _processExecutor;

		public NUnitTask() : this(new ProcessExecutor())
		{}

		public NUnitTask(ProcessExecutor exec)
		{
			_processExecutor = exec;
		}

		[ReflectorArray("assemblies")]
		public string[] Assemblies = new string[0];

		[ReflectorProperty("path")]
		public string NUnitPath;

		[ReflectorProperty("outputfile", Required=false)]
		public string OutputFile;

		public virtual void Run(IIntegrationResult result)
		{
			string outputFile = result.BaseFromArtifactsDirectory(OutputFile, DefaultOutputFile);
			string args = new NUnitArgument(Assemblies, outputFile).ToString();
			Log.Debug(string.Format("Running unit tests: {0} {1}", NUnitPath, args));

			ProcessResult nunitResult = _processExecutor.Execute(new ProcessInfo(NUnitPath, args, result.WorkingDirectory));
			result.AddTaskResult(new FileTaskResult(outputFile));

			/// TODO: this is wrong.  do not throw exception.
			if (nunitResult.Failed) throw new CruiseControlException("NUnit tests failed!");
		}
	}
}