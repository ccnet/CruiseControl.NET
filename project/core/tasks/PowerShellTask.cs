using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	/// <summary>
	/// This is a specialized Exec for PowerShell
	/// </summary>
	[ReflectorType("powershell")]
	public class PowerShellTask 
        : TaskBase
	{
		public const int DefaultBuildTimeOut = 600;
        public const string PowerShellExe = "powershell.exe";
        public const string regkeypowershell1 = @"SOFTWARE\Microsoft\PowerShell\1\PowerShellEngine";
        public const string regkeypowershell2 = @"SOFTWARE\Microsoft\PowerShell\2\PowerShellEngine";
        public const string regkeyholder = @"ApplicationBase";
        public static string DefaultScriptsDirectory = System.Environment.GetEnvironmentVariable("USERPROFILE") + @"\Documents\WindowsPowerShell\";

        private string executable;
		private ProcessExecutor executor;

		public PowerShellTask() : this(new Registry(), new ProcessExecutor()) { }
		
		public PowerShellTask(IRegistry registry, ProcessExecutor executor)
		{
            this.Registry = registry;
			this.executor = executor;
		}

        /// <summary>
        /// Expose the registry so the unit tests can change it if necessary.
        /// </summary>
        public IRegistry Registry { get; set; }

        [ReflectorProperty("script", Required = true)]
        public string Script;

		[ReflectorProperty("executable", Required = false)]
        public string Executable
        {
            get 
			{ 
				if (executable == null)
				{
					executable = ReadPowerShellFromRegistry();
				}
				return executable; 
			}	
			set { executable = value; }
		}

		[ReflectorProperty("scriptsDirectory", Required = false)]
        public string ConfiguredScriptsDirectory = DefaultScriptsDirectory;

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

		private int[] successExitCodes = null;

        /// <summary>
        /// The list of exit codes that indicate success, separated by commas.
        /// </summary>
		[ReflectorProperty("successExitCodes", Required = false)]
		public string SuccessExitCodes
		{
			get 
            {
                string result = string.Empty;
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
                if (!string.IsNullOrEmpty(value))
                {
                    string[] codes = value.Split(',');

                    successExitCodes = new int[codes.Length];

                    for (int i = 0; i < codes.Length; ++i)
                    {
                        successExitCodes[i] = Int32.Parse(codes[i]);
                    }
                }
                else
                {
                    successExitCodes = null;
                }
			}
		}
		
		/// <summary>
		/// Gets and sets the maximum number of seconds that the build may take.  If the build process takes longer than
		/// this period, it will be killed.  Specify this value as zero to disable process timeouts.
		/// </summary>
		[ReflectorProperty("buildTimeoutSeconds", Required = false)]
		public int BuildTimeoutSeconds = DefaultBuildTimeOut;

        /// <summary>
        /// Run the specified PowerShell and add its output to the build results.
        /// </summary>
        /// <param name="result">the IIntegrationResult object for the build</param>
        protected override bool Execute(IIntegrationResult result)
		{
            result.BuildProgressInformation.SignalStartRunTask(string.Format("Executing {0}", Executable));
            
			ProcessInfo processInfo = NewProcessInfoFrom(result);

			ProcessResult processResult = AttemptToExecute(processInfo);

            if (!StringUtil.IsWhitespace(processResult.StandardOutput) || !StringUtil.IsWhitespace(processResult.StandardError))
            {
                // The PowerShell produced some output.  We need to transform it into an XML build report 
                // fragment so the rest of CC.Net can process it.
                ProcessResult newResult = new ProcessResult(
                    MakeBuildResult(processResult.StandardOutput,string.Empty), 
                    MakeBuildResult(processResult.StandardError, "Error"), 
                    processResult.ExitCode, 
                    processResult.TimedOut,
					processResult.Failed);

                processResult = newResult;
            }
            result.AddTaskResult(new ProcessTaskResult(processResult));

			if (processResult.TimedOut)
			{
				throw new BuilderException(this, "Command Line Build timed out (after " + BuildTimeoutSeconds + " seconds)");
			}

            return !processResult.Failed;
		}

		private ProcessInfo NewProcessInfoFrom(IIntegrationResult result)
		{
            ProcessInfo info = new ProcessInfo( executable, Args(result), BaseDirectory(result), successExitCodes);
			info.TimeOut = BuildTimeoutSeconds*1000;
            SetConfiguredEnvironmentVariables(info.EnvironmentVariables, this.EnvironmentVariables);
            IDictionary properties = result.IntegrationProperties;
			foreach (string key in properties.Keys)
			{
				info.EnvironmentVariables[key] = StringUtil.IntegrationPropertyToString(properties[key]);
			}

			return info;
		}

		private string BaseDirectory(IIntegrationResult result)
		{
            return result.BaseFromWorkingDirectory(ConfiguredScriptsDirectory);
		}

		protected ProcessResult AttemptToExecute(ProcessInfo info)
		{
			try
			{
				return executor.Execute(info);
			}
			catch (IOException e)
			{
				throw new BuilderException(this, string.Format("Unable to execute: {0}\n{1}", info, e), e);
			}
		}

		public override string ToString()
		{
			return string.Format(@" BaseDirectory: {0}, PowerShell: {1}", ConfiguredScriptsDirectory, PowerShellExe);
		}

        /// <summary>
        /// Convert a stream of text lines separated with newline sequences into an XML build result.
        /// </summary>
        /// <param name="input">the text stream</param>
        /// <param name="msgLevel">the message level, if any.  Values are "Error" and "Warning".</param>
        /// <returns>the build result string</returns>
        /// <remarks>If there are any non-blank lines in the input, they are each wrapped in a
        /// <code>&lt;message&gt</code> element and the entire set is wrapped in a
        /// <code>&lt;buildresults&gt;</code> element and returned.  Each line of the input is encoded
        /// as XML CDATA rules require.  If the input is empty or contains only whitspace, an 
        /// empty string is returned.
        /// Note: If we can't manage to understand the input, we just return it unchanged.
        /// </remarks>
        private static string MakeBuildResult(string input, string msgLevel)
        {
            StringBuilder output = new StringBuilder();

            // Pattern for capturing a line of text, exclusive of the line-ending sequence.
            // A "line" is an non-empty unbounded sequence of characters followed by some 
            // kind of line-ending sequence (CR, LF, or any combination thereof) or 
            // end-of-string.
            Regex linePattern = new Regex(@"([^\r\n]+)");

            MatchCollection lines = linePattern.Matches(input);
            if (lines.Count > 0)
            {
                output.Append(System.Environment.NewLine);
                output.Append("<buildresults>");
                output.Append(System.Environment.NewLine);
                foreach (Match line in lines)
                {
                    output.Append("  <message");
                    if (msgLevel !=string.Empty)
                        output.AppendFormat(" level=\"{0}\"", msgLevel);
                    output.Append(">");
                    output.Append(XmlUtil.EncodePCDATA(line.ToString()));
                    output.Append("</message>");
                    output.Append(System.Environment.NewLine);
                }
                output.Append("</buildresults>");
                output.Append(System.Environment.NewLine);
            }
            else
                output.Append(input);       // All of that stuff failed, just return our input
            return output.ToString();
        }

        /// <summary>
        /// Pass the project's environment variables to the process.
        /// </summary>
        /// <param name="variablePool">The collection of environment variables to be updated.</param>
        /// <param name="varsToSet">An array of environment variables to set.</param>
        /// <remarks>
        /// Any variable without a value will be set to an empty string.
        /// </remarks>
        private static void SetConfiguredEnvironmentVariables(StringDictionary variablePool, EnvironmentVariable[] varsToSet)
        {
            foreach (EnvironmentVariable item in varsToSet)
                variablePool[item.name] = item.value;
        }

         /// <summary>
        /// Get the name of the PowerShell executable for the highest version installed on this machine.
        /// </summary>
        /// <returns>The fully-qualified pathname of the executable.</returns>
		private string ReadPowerShellFromRegistry()
		{
			string registryValue = null;

            registryValue = Registry.GetLocalMachineSubKeyValue(regkeypowershell2, regkeyholder);

			if (registryValue == null)
			{
                registryValue = Registry.GetLocalMachineSubKeyValue(regkeypowershell1, regkeyholder);
			}

            if (registryValue == null)
            {
                Log.Debug("Unable to find PowerShell and it was not defined in Executable Parameter");
                throw new BuilderException(this, "Unable to find PowerShell and it was not defined in Executable Parameter");
            }

			return Path.Combine(registryValue, PowerShellExe);
		}

        private string Args(IIntegrationResult result)
        {
            ProcessArgumentBuilder builder = new ProcessArgumentBuilder();

            if (!string.IsNullOrEmpty(Script))
            {
                if (ConfiguredScriptsDirectory.EndsWith("\\"))
                {
                    builder.AppendArgument(ConfiguredScriptsDirectory + Script);
                }
                else
                {
                    builder.AppendArgument(ConfiguredScriptsDirectory + "\\" + Script);
                }
            }

            if (!string.IsNullOrEmpty(BuildArgs)) builder.AppendArgument(BuildArgs);
            return builder.ToString();
        }  

    }
}
