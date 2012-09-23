﻿using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// <para>Runs a FAKE - F# Make script.</para>
    /// <para>
    /// "FAKE - F# Make" is a build automation system. Due to its integration in F#, all benets of the .NET Framework and
    /// functional programming can be used, including the extensive class library,
    /// powerful debuggers and integrated development environments like
    /// Visual Studio 2008 or SharpDevelop, which provide syntax highlighting and code completion.
    /// </para>
    /// <para>
    /// <list type="bullet">
    /// <item>
    /// The Google group can be found at: http://groups.google.com/group/fsharpMake
    /// </item>
    /// <item>
    /// More information on: http://bitbucket.org/forki/fake/wiki/Home
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    /// <title>FAKE - F# Make Task</title>
    /// <version>1.6</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;fake&gt;
    /// &lt;buildFile&gt;build.fsx&lt;/buildFile&gt;
    /// &lt;/fake&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;fake&gt;
    /// &lt;executable&gt;Tools\FAKE.exe&lt;/executable&gt;
    /// &lt;baseDirectory&gt;C:\Build\Project1\&lt;/baseDirectory&gt;
    /// &lt;buildFile&gt;build.fsx&lt;/buildFile&gt;
    /// &lt;buildTimeoutSeconds&gt;1200&lt;/buildTimeoutSeconds&gt;
    /// &lt;/fake&gt;
    /// </code>
    /// </example>
    [ReflectorType("fake")]
    public class FakeTask : BaseExecutableTask
    {
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string defaultExecutable = "FAKE.exe";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const int DefaultBuildTimeout = 600;
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string logFilename = "fake-results-{0}.xml";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public readonly Guid LogFileId = Guid.NewGuid();
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const ProcessPriorityClass DefaultPriority = ProcessPriorityClass.Normal;

        private readonly IFileDirectoryDeleter fileDirectoryDeleter = new IoService();

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

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeTask" /> class.	
        /// </summary>
        /// <remarks></remarks>
        public FakeTask(): 
			this(new ProcessExecutor()){}

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeTask" /> class.	
        /// </summary>
        /// <param name="executor">The executor.</param>
        /// <remarks></remarks>
        public FakeTask(ProcessExecutor executor)
		{
			this.executor = executor;
            Executable = defaultExecutable;
            ConfiguredBaseDirectory = string.Empty;
            Priority = DefaultPriority;
            BuildTimeoutSeconds = DefaultBuildTimeout;
            BuildFile = string.Empty;
        }

        #region Overrides of TaskBase

        /// <summary>
        /// Execute the actual task functionality.
        /// </summary>
        /// <param name="result">The result to use.</param>
        /// <returns><c>true</c> if the task was successful; <c>false</c> otherwise.</returns>
        protected override bool Execute(IIntegrationResult result)
        {
            var fakeOutputFile = GetFakeOutputFile(result);

            //delete old nant output logfile, if exist
            fileDirectoryDeleter.DeleteIncludingReadOnlyObjects(fakeOutputFile);

            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description :
                string.Format(System.Globalization.CultureInfo.CurrentCulture,"Executing FAKE - {0}", ToString()));

						var info = CreateProcessInfo(result);
            var processResult = TryToRun(info, result);

            if (File.Exists(fakeOutputFile))
                result.AddTaskResult(new FileTaskResult(fakeOutputFile));

            result.AddTaskResult(new ProcessTaskResult(processResult, true));

						if (processResult.TimedOut)
							result.AddTaskResult(MakeTimeoutBuildResult(info));

            return processResult.Succeeded;
        }

        #endregion

        #region Overrides of BaseExecutableTask

        /// <summary>
        /// Gets the process filename.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override string GetProcessFilename()
        {
            return Executable;
        }

        /// <summary>
        /// Gets the process arguments.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override string GetProcessArguments(IIntegrationResult result)
        {
            var buffer = new ProcessArgumentBuilder();
            buffer.AppendArgument(StringUtil.AutoDoubleQuoteString(BuildFile));
            buffer.AppendArgument("logfile={0}", StringUtil.AutoDoubleQuoteString(GetFakeOutputFile(result)));
            AppendIntegrationResultProperties(buffer, result);
            return buffer.ToString();
        }

        /// <summary>
        /// Gets the process base directory.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override string GetProcessBaseDirectory(IIntegrationResult result)
        {
            return result.BaseFromWorkingDirectory(ConfiguredBaseDirectory);
        }

        /// <summary>
        /// Gets the process priority class.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override ProcessPriorityClass GetProcessPriorityClass()
        {
            return Priority;
        }

        /// <summary>
        /// Gets the process timeout.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override int GetProcessTimeout()
        {
            return BuildTimeoutSeconds * 1000;
        }

        #endregion

        private static void AppendIntegrationResultProperties(ProcessArgumentBuilder buffer, IIntegrationResult result)
        {
            // We have to sort this alphabetically, else the unit tests
            // that expect args in a certain order are unpredictable
            IDictionary properties = result.IntegrationProperties;
            foreach (string key in properties.Keys)
            {
                object value = result.IntegrationProperties[key];
                if (value != null)
                    buffer.AppendArgument(string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}={1}", key, StringUtil.StripThenEncodeParameterArgument(StringUtil.RemoveTrailingPathDelimeter(StringUtil.IntegrationPropertyToString(value)))));
            }
        }

        /// <summary>
        /// Toes the string.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string ToString()
        {
            string baseDirectory = ConfiguredBaseDirectory ?? string.Empty;
            return string.Format(CultureInfo.CurrentCulture, @" BaseDirectory: {0}, Executable: {1}, BuildFile: {2}", baseDirectory, Executable, BuildFile);
        }

        private string GetFakeOutputFile(IIntegrationResult result)
        {
            return Path.Combine(result.ArtifactDirectory, string.Format(CultureInfo.CurrentCulture, logFilename, LogFileId));
        }
    }
}
