using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	/// <summary>
	/// This is a builder that can run any command line process. We capture standard out and standard error
	/// and include them in the Integration Result. We use the process exit code to set whether the build has failed.
	/// TODO: Passing through build label
	/// TODO: This is very similar to the NAntBuilder, so refactoring required (can we have subclasses with reflector properties?)
	/// </summary>
	[ReflectorType("exec")]
	public class ExecutableTask
        : BaseExecutableTask
	{
		public const int DEFAULT_BUILD_TIMEOUT = 600;

		public ExecutableTask() : this(new ProcessExecutor())
		{}

		public ExecutableTask(ProcessExecutor executor)
		{
			this.executor = executor;
		}

		[ReflectorProperty("executable", Required = true)]
		public string Executable = string.Empty;

		[ReflectorProperty("baseDirectory", Required = false)]
		public string ConfiguredBaseDirectory = string.Empty;

		[ReflectorProperty("buildArgs", Required = false)]
		public string BuildArgs = string.Empty;

		/// <summary>
		/// A set of environment variables set for commands that are executed.
		/// </summary>
		/// <remarks>
		/// Each variable should be specified as <code>&lt;variable name="name" value="value"/&gt;</code>.
		/// </remarks>
		[ReflectorArray("environment", Required = false)]
		public EnvironmentVariable[] EnvironmentVariables = new EnvironmentVariable[0];

		private int[] successExitCodes;

        /// <summary>
        /// The list of exit codes that indicate success, separated by commas.
        /// </summary>
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
		/// Gets and sets the maximum number of seconds that the build may take.  If the build process takes longer than
		/// this period, it will be killed.  Specify this value as zero to disable process timeouts.
		/// </summary>
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

			ProcessResult processResult = TryToRun(info);
            
			if (!StringUtil.IsWhitespace(processResult.StandardOutput + processResult.StandardError))
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
