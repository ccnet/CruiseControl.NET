using System;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	[ReflectorType("nunit")]
	public class NUnitTask : ITask
	{
		public const string DefaultPath = @"C:\Program Files\NUnit 2.2\bin\nunit-console.exe";
		public const int DefaultTimeout = 600;
		private const string DefaultOutputFile = "nunit-results.xml";
		private ProcessExecutor processExecutor;

		public NUnitTask() : this(new ProcessExecutor())
		{}

		public NUnitTask(ProcessExecutor exec)
		{
			processExecutor = exec;
		}

		[ReflectorArray("assemblies")]
		public string[] Assemblies = new string[0];

		[ReflectorProperty("path", Required=false)]
		public string NUnitPath = DefaultPath;

		[ReflectorProperty("outputfile", Required=false)]
		public string OutputFile = DefaultOutputFile;

		[ReflectorProperty("timeout", Required=false)]
		public int Timeout = DefaultTimeout;

		public virtual void Run(IIntegrationResult result)
		{
            Util.ListenerFile.WriteInfo(result.ListenerFile, "Executing NUnit"); 

			string outputFile = result.BaseFromArtifactsDirectory(OutputFile);

			ProcessResult nunitResult = processExecutor.Execute(NewProcessInfo(outputFile, result), ProcessMonitor.GetProcessMonitorByProject(result.ProjectName));
			result.AddTaskResult(new ProcessTaskResult(nunitResult));
			if (File.Exists(outputFile))
			{
				result.AddTaskResult(new FileTaskResult(outputFile));				
			}
			else
			{
				Log.Warning(string.Format("NUnit test output file {0} was not created", outputFile));
			}

            Util.ListenerFile.RemoveListenerFile(result.ListenerFile);

		}

		private ProcessInfo NewProcessInfo(string outputFile, IIntegrationResult result)
		{
			string args = new NUnitArgument(Assemblies, outputFile).ToString();
			Log.Debug(string.Format("Running unit tests: {0} {1}", NUnitPath, args));
	
			ProcessInfo info = new ProcessInfo(NUnitPath, args, result.WorkingDirectory);
			info.TimeOut = Timeout * 1000;
			return info;
		}
	}
}
