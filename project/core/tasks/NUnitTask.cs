using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	[ReflectorType("nunit")]
	public class NUnitTask : ITask
	{
		private ProcessExecutor _processExecutor = new ProcessExecutor();
		private string[] _assemblies = new string[0];
		private string _nunitPath;

		public NUnitTask()
		{}

		public NUnitTask(ProcessExecutor exec)
		{
			_processExecutor = exec;
		}

		[ReflectorArray("assemblies")]
		public string[] Assembly
		{
			get { return _assemblies; }
			set { _assemblies = value; }
		}

		[ReflectorProperty("path")]
		public string NUnitPath
		{
			get { return _nunitPath; }
			set { _nunitPath = value; }
		}

		public virtual void Run(IIntegrationResult result)
		{
			string args = new NUnitArgument(Assembly).ToString();
			if (StringUtil.IsBlank(args)) 
				throw new CruiseControlException("No unit test assemblies are specified. Please use the <assemblies> element to specify the test assemblies to run.");

			Log.Debug(string.Format("Running unit tests: {0} {1}", NUnitPath, args));
			ProcessResult nunitResult = _processExecutor.Execute(new ProcessInfo(NUnitPath, args));
			if (nunitResult.Failed) throw new CruiseControlException("NUnit tests failed!");
			result.AddTaskResult(nunitResult.StandardOutput);
		}
	}
}