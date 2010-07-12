using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace ThoughtWorks.CruiseControl.Core.tasks
{
    /// <summary>
    /// <title>FAKE Task</title>
    /// <para>Runs a FAKE - F# Make script.</para>
    /// <version>1.6</version>
    /// <para>
    /// "FAKE - F# Make" is a build automation system. Due to its integration in F#, all benets of the .NET Framework and
    /// functional programming can be used, including the extensive class library,
    /// powerful debuggers and integrated development environments like
    /// Visual Studio 2008 or SharpDevelop, which provide syntax highlighting and code completion.
    /// 
    /// The Google group can be found at: http://groups.google.com/group/fsharpMake
    /// More information on: http://bitbucket.org/forki/fake/wiki/Home
    /// </para>
    /// </summary>
    [ReflectorType("fake")]
    public class FakeTask : BaseExecutableTask
    {
        public const string defaultExecutable = "FAKE.exe";
        public const int DefaultBuildTimeout = 600;
        public const string logFilename = "fake-results-{0}.xml";
        public readonly Guid LogFileId = Guid.NewGuid();
        public const ProcessPriorityClass DefaultPriority = ProcessPriorityClass.Normal;

        /// <summary>
        /// The location of the FAKE executable.
        /// </summary>
        /// <version>1.6</version>
        /// <default>FAKE.exe</default>
        [ReflectorProperty("executable", Required = false)]
        public string Executable { get; set; }

        /// <summary>
        /// The directory to run FAKE in.
        /// </summary>
        /// <version>1.6</version>
        /// <default>Project Working Directory</default>
        [ReflectorProperty("baseDirectory", Required = false)]
        public string ConfiguredBaseDirectory { get; set; }

        /// <summary>
        /// The priority class of the spawned process.
        /// </summary>
        /// <version>1.6</version>
        /// <default>Normal</default>
        [ReflectorProperty("priority", Required = false)]
        public ProcessPriorityClass Priority { get; set; }

        /// <summary>
        /// The maximum number of seconds that the build may take.  If the build process takes longer than this period, it will be killed.
        /// Specify this value as zero to disable process timeouts.
        /// </summary>
        /// <version>1.6</version>
        /// <default>600</default>
        [ReflectorProperty("buildTimeoutSeconds", Required = false)]
        public int BuildTimeoutSeconds { get; set; }

        /// <summary>
        /// The name of the build file to run, relative to the baseDirectory. 
        /// </summary>
        /// <version>1.6</version>
        /// <default>Default build field in the working directory</default>
        [ReflectorProperty("buildFile", Required = false)]
        public string BuildFile { get; set; }

        #region Overrides of TaskBase

        /// <summary>
        /// Execute the actual task functionality.
        /// </summary>
        /// <param name="result">The result to use.</param>
        /// <returns><c>true</c> if the task was successful; <c>false</c> otherwise.</returns>
        protected override bool Execute(IIntegrationResult result)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Overrides of BaseExecutableTask

        protected override string GetProcessFilename()
        {
            return Executable;
        }

        protected override string GetProcessArguments(IIntegrationResult result)
        {
            throw new NotImplementedException();
        }

        protected override string GetProcessBaseDirectory(IIntegrationResult result)
        {
            return result.BaseFromWorkingDirectory(ConfiguredBaseDirectory);
        }

        protected override ProcessPriorityClass GetProcessPriorityClass()
        {
            return Priority;
        }

        protected override int GetProcessTimeout()
        {
            return BuildTimeoutSeconds * 1000;
        }

        #endregion
    }
}
