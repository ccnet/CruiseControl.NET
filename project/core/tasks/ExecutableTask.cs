using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// <para>
    /// The Executable Task lets you invoke any command line executable. It doesn't offer as much specific
    /// integration as (for example) the <link>NAnt Task</link>, but does allow you to hook almost anything
    /// up as a build process to CCNet. CCNet will examine the exit code when the executable ends and act
    /// accordingly.
    /// </para>
    /// </summary>
    /// <title>Executable Task</title>
    /// <version>1.0</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;exec executable="c:\projects\myproject\build.bat" /&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;exec&gt;
    /// &lt;executable&gt;make&lt;/executable&gt;
    /// &lt;baseDirectory&gt;D:\dev\MyProject&lt;/baseDirectory&gt;
    /// &lt;buildArgs&gt;all&lt;/buildArgs&gt;
    /// &lt;buildTimeoutSeconds&gt;10&lt;/buildTimeoutSeconds&gt;
    /// &lt;successExitCodes&gt;0,1,3,5&lt;/successExitCodes&gt;
    /// &lt;environment&gt;
    /// &lt;variable&gt;
    /// &lt;name&gt;MyVar1&lt;/name&gt;
    /// &lt;value&gt;Var1Value&lt;/value&gt;
    /// &lt;/variable&gt;
    /// &lt;variable name="MyVar2" value="Var2Value"/&gt;
    /// &lt;/environment&gt;
    /// &lt;/exec&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <para type="note">
    /// An exit code of -1 is always treated as the operation has timed out. This will fail the build.
    /// </para>
    /// <para type="warning">
    /// Windows seems to change the case of environment variables occasionally. If your task target doesn't
    /// find one of these properties, try using all upper case or all lower case versions of these properties.
    /// </para>
    /// <heading>Frequently Asked Questions</heading>
    /// <para>
    /// <b>Does the exec task pass the integration properties via the command line?</b>
    /// </para>
    /// <para>
    /// No. The integration properties are only available as environment variables. As there is no way of
    /// knowing the way in which the external program expects these properties to be formatted as command line
    /// arguments, environment variables are a simple, common medium for making these values accessible. To
    /// pass these environment variables into an external program, have the exec task call a batch file instead
    /// that will pick up the environment variables, format them and pass them as command line arguments to the
    /// external program.
    /// </para>
    /// <para>
    /// <b>Using built in shell commands</b>
    /// </para>
    /// <para>
    /// In Windows use cmd.exe as the executable, and pass the wanted command as an argument, preceded with /c.
    /// This allows to execute del *.* and the like. For example :
    /// </para>
    /// <code>
    /// &lt;exec&gt;
    /// &lt;executable&gt;c:\Windows\System32\cmd.exe&lt;/executable&gt;
    /// &lt;buildArgs&gt;/C NET STOP "Service name"&lt;/buildArgs&gt;
    /// &lt;/exec&gt;
    /// </code>
    /// <para>
    /// The following parameters are passed to the external program using environment variables, in addition to those you specify in
    /// the &lt;environment&gt; element.:
    /// </para>
    /// <includePage>Integration Properties</includePage>
    /// </remarks>
    [ReflectorType("exec")]
	public class ExecutableTask
        : BaseExecutableTask
	{
		public const int DEFAULT_BUILD_TIMEOUT = 600;
        public const ProcessPriorityClass DEFAULT_PRIORITY = ProcessPriorityClass.Normal;

		public ExecutableTask() : this(new ProcessExecutor())
		{}

		public ExecutableTask(ProcessExecutor executor)
		{
			this.executor = executor;
		}

        /// <summary>
        /// The path of the program to run. If this is relative, then must be relative to either (a) the base
        /// directory, (b) the CCNet Server application, or (c) if the path doesn't contain any directory
        /// details then can be available in the system or application's 'path' environment variable.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
		[ReflectorProperty("executable", Required = true)]
		public string Executable = string.Empty;

        /// <summary>
        /// The priority class of the spawned process.
        /// </summary>
        /// <version>1.5</version>
        /// <default>Normal</default>
        [ReflectorProperty("priority", Required = false)]
        public ProcessPriorityClass Priority = DEFAULT_PRIORITY;

        /// <summary>
        /// The directory to run the process in. If relative, is a subdirectory of the Project Working
        /// Directory.
        /// </summary>
        /// <version>1.0</version>
        /// <default>Project working directory</default>
        [ReflectorProperty("baseDirectory", Required = false)]
		public string ConfiguredBaseDirectory = string.Empty;

        /// <summary>
        /// Any command line arguments to pass in.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("buildArgs", Required = false)]
		public string BuildArgs = string.Empty;

		/// <summary>
		/// A set of environment variables set for commands that are executed.
		/// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorArray("environment", Required = false)]
		public EnvironmentVariable[] EnvironmentVariables = new EnvironmentVariable[0];

		private int[] successExitCodes;

        /// <summary>
        /// The list of exit codes that indicate success, separated by commas.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("successExitCodes", Required = false)]
		public string SuccessExitCodes
		{
			get 
            {
                string result =string.Empty;
                if (successExitCodes != null)
                {
                    foreach (int code in successExitCodes)
                    {
                        if (result !=string.Empty)
                            result = result + ",";
                        result = result + code;
                    }
                }
                return result;
            }

			set
			{
				string[] codes = value.Split(',');

				if (codes.Length == 0)
				{
					successExitCodes = null;
					return;
				}

				successExitCodes = new int[codes.Length];

				for (int i = 0; i < codes.Length; ++i)
				{
					successExitCodes[i] = Int32.Parse(codes[i]);
				}
			}
		}
		
		/// <summary>
        /// Number of seconds to wait before assuming that the process has hung and should be killed.  If the 
        /// build process takes longer than this period, it will be killed.  Specify this value as zero to 
        /// disable process timeouts.
		/// </summary>
        /// <version>1.0</version>
        /// <default>600</default>
        [ReflectorProperty("buildTimeoutSeconds", Required = false)]
		public int BuildTimeoutSeconds = DEFAULT_BUILD_TIMEOUT;

        /// <summary>
        /// Run the specified executable and add its output to the build results.
        /// </summary>
        /// <param name="result">the IIntegrationResult object for the build</param>
        protected override bool Execute(IIntegrationResult result)
		{
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : string.Format("Executing {0}", Executable));

			ProcessInfo info = CreateProcessInfo(result);
			SetConfiguredEnvironmentVariables(info.EnvironmentVariables, EnvironmentVariables);

			ProcessResult processResult = TryToRun(info, result);

            if (!StringUtil.IsWhitespace(processResult.StandardOutput) || !StringUtil.IsWhitespace(processResult.StandardError))
            {
                // The executable produced some output.  We need to transform it into an XML build report 
                // fragment so the rest of CC.Net can process it.
                ProcessResult newResult = new ProcessResult(
                    StringUtil.MakeBuildResult(processResult.StandardOutput,string.Empty),
					StringUtil.MakeBuildResult(processResult.StandardError, "Error"), 
                    processResult.ExitCode, 
                    processResult.TimedOut,
					processResult.Failed);

                processResult = newResult;
            }
            result.AddTaskResult(new ProcessTaskResult(processResult));

        	if (processResult.TimedOut)
        		throw new BuilderException(this, "Command Line Build timed out (after " + BuildTimeoutSeconds + " seconds)");

            return !processResult.Failed;
		}

		protected override string GetProcessFilename()
		{
			return Executable;
		}

		protected override string GetProcessArguments(IIntegrationResult result)
		{
			return BuildArgs;
		}

		protected override string GetProcessBaseDirectory(IIntegrationResult result)
		{
			return result.BaseFromWorkingDirectory(ConfiguredBaseDirectory);
		}

		protected override int[] GetProcessSuccessCodes()
		{
			return successExitCodes;
		}

		protected override int GetProcessTimeout()
		{
			return BuildTimeoutSeconds*1000;
		}

        protected override ProcessPriorityClass GetProcessPriorityClass()
        {
            return this.Priority;
        }

		public override string ToString()
		{
			return string.Format(@" BaseDirectory: {0}, Executable: {1}", ConfiguredBaseDirectory, Executable);
		}

		/// <summary>
        /// Pass the project's environment variables to the process.
        /// </summary>
        /// <param name="variablePool">The collection of environment variables to be updated.</param>
        /// <param name="varsToSet">An array of environment variables to set.</param>
        /// <remarks>
        /// Any variable without a value will be set to an empty string.
        /// </remarks>
        private static void SetConfiguredEnvironmentVariables(StringDictionary variablePool, IEnumerable<EnvironmentVariable> varsToSet)
        {
            foreach (EnvironmentVariable item in varsToSet)
                variablePool[item.name] = item.value;
        }
    }
}
