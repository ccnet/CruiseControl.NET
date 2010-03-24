using System.Diagnostics;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// <para type="tip">
    ///  See <link>Using CruiseControl.NET with NUnit</link> for more details.
    /// </para>
    /// <para>
    ///  This task enables you to instruct CCNet to run the unit tests contained within a collection of assemblies. The results of the unit
    ///  tests will be automatically included in the CCNet build results. This can be useful if you have some unit tests that you want to
    ///  run as part of the integration process, but you don't need as part of your developer build process. For example, if you have a set
    ///  of integration tests that you want to run in a separate build process, it is easy to set up a project to use this task.
    /// </para>
    /// <para>
    ///  If you are using the <link>Visual Studio Task</link> and you want to run unit tests then you probably want to use this task.
    ///  Alternatively you can run NUnit using post-build tasks in your Visual Studio project properties.
    /// </para>
    /// <para type="warning">
    ///  We recommend not using this task, and using your builder to run your tests if possible. This way if the tests fail and you don't
    ///  know why, it is a lot easier to try and replicate the problem on another machine.
    /// </para>
    /// <para type="warning">
    ///  When using this task,do NOT merge an xml file from bin folder of your app with the merge task, or the results will be save twice in
    ///  the buildlog file.
    /// </para>
    /// </summary>
    /// <title>NUnit Task</title>
    /// <version>1.0</version>
    /// <example>
    /// <code>
    /// &lt;nunit&gt;
    /// &lt;path&gt;D:\dev\ccnet\ccnet\tools\nunit\nunit-console.exe&lt;/path&gt;
    /// &lt;assemblies&gt;
    /// &lt;assembly&gt;D:\dev\Refactoring\bin\Debug\Refactoring.exe&lt;/assembly&gt;
    /// &lt;assembly&gt;D:\dev\Refactoring\bin\Debug\Refactoring.Core.dll&lt;/assembly&gt;
    /// &lt;/assemblies&gt;
    /// &lt;excludedCategories&gt;
    /// &lt;excludedCategory&gt;LongRunning&lt;/excludedCategory&gt;
    /// &lt;/excludedCategories&gt;
    /// &lt;/nunit&gt;
    /// </code>
    /// </example>
    [ReflectorType("nunit")]
	public class NUnitTask
        : TaskBase
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

        #region Public fields
        #region Assemblies
        /// <summary>
        /// List of the paths to the assemblies containing the NUnit tests to be run.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("assemblies")]
		public string[] Assemblies = new string[0];
        #endregion

        #region NUnitPath
        /// <summary>
        /// Path of <b>nunit-console.exe</b> application. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>nunit-console</default>
        [ReflectorProperty("path", Required = false)]
		public string NUnitPath = DefaultPath;
        #endregion

        #region OutputFile
        /// <summary>
        /// The file that NUnit will write the test results to.
        /// </summary>
        /// <version>1.0</version>
        /// <default>nunit-results.xml</default>
        [ReflectorProperty("outputfile", Required = false)]
		public string OutputFile = DefaultOutputFile;
        #endregion

        #region Timeout
        /// <summary>
        /// The number of seconds that the nunit process will run before timing out.
        /// </summary>
        /// <version>1.0</version>
        /// <default>600</default>
        [ReflectorProperty("timeout", Required = false)]
		public int Timeout = DefaultTimeout;
        #endregion

        #region Priority
        /// <summary>
        /// The priority class of the spawned process.
        /// </summary>
        /// <version>1.5</version>
        /// <default>Normal</default>
        [ReflectorProperty("priority", Required = false)]
        public ProcessPriorityClass Priority = ProcessPriorityClass.Normal;
        #endregion

        #region ExcludedCategories
        /// <summary>
        /// List of the test categories to be excluded from the NUnit run. The tests need to have the CategoryAttribute set. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("excludedCategories", Required = false)]
        public string[] ExcludedCategories = new string[0];
        #endregion

        #region IncludedCategories
        /// <summary>
        /// List of the test categories to be included in the NUnit run. The tests need to have the CategoryAttribute set. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("includedCategories", Required = false)]
        public string[] IncludedCategories = new string[0];
        #endregion
        #endregion

        protected override bool Execute(IIntegrationResult result)
		{
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : "Executing NUnit"); 

			string outputFile = result.BaseFromArtifactsDirectory(OutputFile);

			ProcessResult nunitResult = executor.Execute(NewProcessInfo(outputFile, result));
		    result.AddTaskResult(new ProcessTaskResult(nunitResult, true));
			if (File.Exists(outputFile))
			{
				result.AddTaskResult(new FileTaskResult(outputFile));				
			}
			else
			{
				Log.Warning(string.Format("NUnit test output file {0} was not created", outputFile));
			}
            return !nunitResult.Failed;
		}

		private ProcessInfo NewProcessInfo(string outputFile, IIntegrationResult result)
		{
            NUnitArgument nunitArgument = new NUnitArgument(Assemblies, outputFile);
            nunitArgument.ExcludedCategories = ExcludedCategories;
            nunitArgument.IncludedCategories = IncludedCategories;
            string args = nunitArgument.ToString();

			Log.Debug(string.Format("Running unit tests: {0} {1}", NUnitPath, args));
	
			ProcessInfo info = new ProcessInfo(NUnitPath, args, result.WorkingDirectory, Priority);
			info.TimeOut = Timeout * 1000;
			return info;
		}
	}
}
