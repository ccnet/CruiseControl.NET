using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	[ReflectorType("nunit")]
	public class NUnitTask
        : TaskBase, ITask
	{
		public const string DefaultPath = @"nunit-console";
		public const int DefaultTimeout = 600;
		private const string DefaultOutputFile = "nunit-results.xml";
		private readonly ProcessExecutor executor;

		public NUnitTask() : this(new ProcessExecutor())
		{}

		public NUnitTask(ProcessExecutor exec)
		{
			executor = exec;
		}

		[ReflectorArray("assemblies")]
		public string[] Assemblies = new string[0];

		[ReflectorProperty("path", Required=false)]
		public string NUnitPath = DefaultPath;

		[ReflectorProperty("outputfile", Required=false)]
		public string OutputFile = DefaultOutputFile;

		[ReflectorProperty("timeout", Required=false)]
		public int Timeout = DefaultTimeout;

        [ReflectorArray("excludedCategories", Required = false)]
        public string[] ExcludedCategories = new string[0];

        [ReflectorArray("includedCategories", Required = false)]
        public string[] IncludedCategories = new string[0];

        /// <summary>
        /// Description used for the visualisation of the buildstage, if left empty the process name will be shown
        /// </summary>
        [ReflectorProperty("description", Required = false)]
        public string Description = string.Empty;


		public virtual void Run(IIntegrationResult result)
		{
            result.BuildProgressInformation.SignalStartRunTask(Description != string.Empty ? Description : "Executing NUnit"); 

			string outputFile = result.BaseFromArtifactsDirectory(OutputFile);

			ProcessResult nunitResult = executor.Execute(NewProcessInfo(outputFile, result));
			result.AddTaskResult(new ProcessTaskResult(nunitResult));
			if (File.Exists(outputFile))
			{
				result.AddTaskResult(new FileTaskResult(outputFile));				
			}
			else
			{
				Log.Warning(string.Format("NUnit test output file {0} was not created", outputFile));
			}           
		}

		private ProcessInfo NewProcessInfo(string outputFile, IIntegrationResult result)
		{
            NUnitArgument nunitArgument = new NUnitArgument(Assemblies, outputFile);
            nunitArgument.ExcludedCategories = ExcludedCategories;
            nunitArgument.IncludedCategories = IncludedCategories;
            string args = nunitArgument.ToString();

			Log.Debug(string.Format("Running unit tests: {0} {1}", NUnitPath, args));
	
			ProcessInfo info = new ProcessInfo(NUnitPath, args, result.WorkingDirectory);
			info.TimeOut = Timeout * 1000;
			return info;
		}
	}
}
