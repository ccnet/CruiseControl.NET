namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;

    /// <summary>
    /// <para>
    /// Executes Rake.
    /// </para>
    /// </summary>
    /// <title> Rake Task </title>
    /// <version>1.4</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;rake /&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;rake&gt;
    /// &lt;executable&gt;c:\ruby\bin\rake.bat&lt;/executable&gt;
    /// &lt;baseDirectory&gt;c:\fromcvs\myrepo\myproject&lt;/baseDirectory&gt;
    /// &lt;buildArgs&gt;additional-argument&lt;/buildArgs&gt;
    /// &lt;rakefile&gt;Rakefile&lt;/rakefile&gt;
    /// &lt;targetList&gt;
    /// &lt;target&gt;build&lt;/target&gt;
    /// &lt;/targetList&gt;
    /// &lt;buildTimeoutSeconds&gt;1200&lt;/buildTimeoutSeconds&gt;
    /// &lt;quiet&gt;false&lt;/quiet&gt;
    /// &lt;silent&gt;false&lt;/silent&gt;
    /// &lt;trace&gt;true&lt;/trace&gt;
    /// &lt;/rake&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <heading>Accessing CruiseControl.NET build labels in Rake</heading>
    /// <para>
    /// CCNet will pass the current build label to Rake via the environment variable CCNetLabel. This means that you can access this 
    /// variable too. For example, archive the build results in a folder with the same name as the build label (this is what we do on
    /// CCNetLive  using NAnt. Here's some example Rakefile demonstrating how to do this:
    /// </para>
    /// <code type="none">
    /// #!ruby
    /// require 'rake'
    /// 
    /// task :default =&gt; [:deploy]
    /// 
    /// task :deploy do
    /// 	publishdir="C:/download-area/CCNet-Builds/#{ENV['CCNetLabel']}"
    /// 	mkdir_p publishdir
    /// 	FileList['dist/*'].each do |file|
    /// 		cp file, publishdir
    /// 	end
    /// end
    /// </code>
    /// <para>
    /// See <link>Integration Properties</link> for the values that are passed to the task.
    /// </para>
    /// </remarks>
    [ReflectorType("rake")]
	public class RakeTask
        : BaseExecutableTask
	{
		public const int DefaultBuildTimeout = 600;
		public const string DefaultExecutable = @"rake";

        /// <summary>
        /// Any arguments to pass through to Rake (e.g to specify build properties).
        /// </summary>
        /// <default>None</default>
        /// <version>1.4</version>
		[ReflectorProperty("buildArgs", Required = false)]
		public string BuildArgs =string.Empty;

        /// <summary>
        /// The directory to run the Rake process in. If relative, is a subdirectory of the Project Working Directory.
        /// </summary>
        /// <default>Project Working Directory</default>
        /// <version>1.4</version>
        [ReflectorProperty("baseDirectory", Required = false)]
		public string BaseDirectory =string.Empty;

        /// <summary>
        /// Number of seconds to wait before assuming that the process has hung and should be killed. 
        /// </summary>
        /// <default>600</default>
        /// <version>1.4</version>
        [ReflectorProperty("buildTimeoutSeconds", Required = false)]
		public int BuildTimeoutSeconds = DefaultBuildTimeout;

        /// <summary>
        /// Do not log messages to standard output.
        /// </summary>
        /// <default>false</default>
        /// <version>1.4</version>
        [ReflectorProperty("quiet", Required = false)]
		public bool Quiet;

        /// <summary>
        /// The path of the version of Rake you want to run. If this is relative, then must be relative to either (a) the base directory, 
        /// (b) the CCNet Server application, or (c) if the path doesn't contain any directory details then can be available in the system 
        /// or application's 'path' environment variable.
        /// </summary>
        /// <default>c:\ruby\bin\rake.bat</default>
        /// <version>1.4</version>
        [ReflectorProperty("executable", Required = false)]
		public string Executable = DefaultExecutable;

        /// <summary>
        /// The name of the Rakefile to run, relative to the baseDirectory. 
        /// </summary>
        /// <default>None</default>
        /// <version>1.4</version>
        /// <remarks>
        /// If no rake file is specified Rake will use the default build file in the working directory.
        /// </remarks>
        [ReflectorProperty("rakefile", Required = false)]
		public string Rakefile =string.Empty;

        /// <summary>
        /// Like quiet but also suppresses the 'in directory' announcement. 
        /// </summary>
        /// <default>false</default>
        /// <version>1.4</version>
        [ReflectorProperty("silent", Required = false)]
		public bool Silent;

        /// <summary>
        /// A list of targets to be called. CruiseControl.NET does not call Rake once for each target, it uses the Rake feature of
        /// being able to specify multiple targets. 
        /// </summary>
        /// <remarks>
        /// If no targets are defined Rake will use the default target.
        /// </remarks>
        /// <default>None</default>
        /// <version>1.4</version>
		[ReflectorArray("targetList", Required = false)]
		public string[] Targets = new string[0];

        /// <summary>
        /// Turns on invoke/execute tracing and enables full backtrace.
        /// </summary>
        /// <default>false</default>
        /// <version>1.4</version>
        [ReflectorProperty("trace", Required = false)]
		public bool Trace;

		public RakeTask()
			: this(new ProcessExecutor()) {}

		public RakeTask(ProcessExecutor executor)
		{
			this.executor = executor;
		}

		protected override bool Execute(IIntegrationResult result)
		{
			ProcessInfo processInfo = CreateProcessInfo(result);
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : string.Format("Executing Rake: {0}", processInfo.SafeArguments));
			ProcessResult processResult = TryToRun(processInfo, result);

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

            return (!processResult.Failed);
		}

		protected override string GetProcessArguments(IIntegrationResult result)
		{
			ProcessArgumentBuilder args = new ProcessArgumentBuilder();
			args.AddArgument("--rakefile", Rakefile);

			if (Silent)
				args.AddArgument("--silent");
			else if (Quiet)
				args.AddArgument("--quiet");

			if (Trace)
				args.AddArgument("--trace");

			args.AddArgument(BuildArgs);

			foreach (string t in Targets)
				args.AppendArgument(t);

			return args.ToString();
		}

		protected override string GetProcessBaseDirectory(IIntegrationResult result)
		{
			return result.BaseFromWorkingDirectory(BaseDirectory);
		}

		protected override int GetProcessTimeout()
		{
			return BuildTimeoutSeconds*1000;
		}

		protected override string GetProcessFilename()
		{
			return Executable;
		}

		public string TargetsForPresentation
		{
			get
			{
				return StringUtil.ArrayToNewLineSeparatedString(Targets);
			}
			set
			{
				Targets = StringUtil.NewLineSeparatedStringToArray(value);
			}
		}
	}
}
