using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// <para>
    /// Runs a NAnt script.
    /// </para>
    /// <para type="tip">
    /// See <link>Using CruiseControl.NET with NAnt</link> for more information on working with NAnt and CruiseControl.Net.
    /// </para>
    /// </summary>
    /// <title>NAnt Task</title>
    /// <version>1.0</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;nant /&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;nant&gt;
    /// &lt;executable&gt;c:\fromcvs\myrepo\myproject\tools\nant\nant.exe&lt;/executable&gt;
    /// &lt;baseDirectory&gt;c:\fromcvs\myrepo\myproject&lt;/baseDirectory&gt;
    /// &lt;buildArgs&gt;-D:cvs.executable=c:\putty\cvswithplinkrsh.bat&lt;/buildArgs&gt;
    /// &lt;nologo&gt;false&lt;/nologo&gt;
    /// &lt;buildFile&gt;cruise.build&lt;/buildFile&gt;
    /// &lt;logger&gt;My.Other.XmlLogger&lt;/logger&gt;
    /// &lt;targetList&gt;
    /// &lt;target&gt;run&lt;/target&gt;
    /// &lt;/targetList&gt;
    /// &lt;buildTimeoutSeconds&gt;1200&lt;/buildTimeoutSeconds&gt;
    /// &lt;/nant&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <heading>NAnt output in Xml</heading>
    /// <para>
    /// CruiseControl.NET expects NAnt to generate its output as Xml so that the build results can be parsed and rendered appropriately. To
    /// accomplish this, CruiseControl.NET will, by default, launch NAnt using the "-logger:NAnt.Core.XmlLogger" argument. If you want to
    /// override this behaviour, specify the logger property in the NAntBuilder configuration in the ccnet.config file. If this element is
    /// specified but is empty then NAnt will be started with the default logger (though this may cause some problems for CCNet). It is also
    /// possible to instruct NAnt to log its output to an Xml file and then merge the file into the build using the File Merge Task.
    /// </para>
    /// <para type="warning">
    /// The configuration of which NAnt logger to use was orginally specified in the ccnet.exe.config file. This has now been deprecated,
    /// and the "NAnt.Logger" element in the &lt;appSettings&gt; section can now be removed.
    /// </para>
    /// <heading>NUnit and NAnt</heading>
    /// <para>
    /// CruiseControl.NET uses xsl to process the build log and produce html for display on the web page. Since xml is so easy to parse the
    /// nunit2 task in NAnt can produce xml output. The tasks must be configured to do that in order for test results to show up on the web
    /// page. Typically this is done by adding a formatter element to the nunit2 task and setting the type to be "Xml". Additionally the
    /// usefile flag of the formatter element must be set to "false". If it isn't the nunit2 task will try and save the output to a file and
    /// not write it out to the build log.
    /// </para>
    /// <code>
    /// &lt;target name="test.unit" depends="compile" description="runs unit tests"&gt;
    /// &lt;nunit2&gt;
    /// &lt;formatter type="Xml" usefile="false"/&gt;
    /// &lt;test assemblyname="${build.dir}\${core.dll}" fork="true"/&gt;
    /// &lt;test assemblyname="${build.dir}\${console.exe}" fork="true"/&gt;
    /// &lt;/nunit2&gt;
    /// &lt;/target&gt;
    /// </code>
    /// <para>
    /// It would be pretty tedious for developers to read the xml output when they run the build locally. Define a property for the build
    /// output type and set it to "Plain" and use the property in the formatter element..
    /// </para>
    /// <code>
    /// &lt;build&gt;
    /// &lt;property name="outputType" value="Plain"/&gt;
    /// &lt;!-- ... --&gt;
    /// &lt;formatter type="${outputType}" usefile="false"/&gt;
    /// &lt;!-- ... --&gt;
    /// &lt;/build&gt;
    /// </code>
    /// <para>
    /// Then in the ccnet.config file pass in a different value for outputType.
    /// </para>
    /// <code>
    /// &lt;nant&gt;
    /// &lt;!-- ... --&gt;
    /// &lt;buildArgs&gt;"-DoutputType=Xml"&lt;/buildArgs&gt;
    /// &lt;!-- ... --&gt;
    /// &lt;/nant&gt;
    /// </code>
    /// <heading>Accessing CruiseControl.NET build labels in NAnt</heading>
    /// <para>
    /// CCNet will pass the current build label to NAnt via the NAnt property CCNetLabel. This means that you can access use this property to,
    /// for example, archive the newly built assemblies in a folder with the same name as the build label (this is what we do on CCNetLive.
    /// Here's an example NAnt script demonstrating how to do this:
    /// </para>
    /// <code>
    /// &lt;target name="dist.publish" depends="dist"&gt;
    /// &lt;ifnot propertyexists="CCNetLabel"&gt;
    /// &lt;fail message="CCNetLabel property not set, so can't create labelled distribution files" /&gt;
    /// &lt;/ifnot&gt;
    /// &lt;property name="publish.dir" value="D:\download-area\CCNet-Builds\${CCNetLabel}" /&gt;
    /// &lt;mkdir dir="${publish.dir}" /&gt;
    /// &lt;copy todir="${publish.dir}"&gt;
    /// &lt;fileset basedir="dist"&gt;
    /// &lt;includes name="*"/&gt;
    /// &lt;/fileset&gt;
    /// &lt;/copy&gt;
    /// &lt;/target&gt;
    /// </code>
    /// </remarks>
    [ReflectorType("nant")]
	public class NAntTask
        : BaseExecutableTask
	{
		public const int DefaultBuildTimeout = 600;
		public const string logFilename = "nant-results-{0}.xml";
        public readonly Guid LogFileId = Guid.NewGuid();
		public const string defaultExecutable = "nant";
		public const string DefaultLogger = "NAnt.Core.XmlLogger";
		public const string DefaultListener = "NAnt.Core.DefaultLogger";
		public const bool DefaultNoLogo = true;
        public const ProcessPriorityClass DefaultPriority = ProcessPriorityClass.Normal;

	    private readonly IFileDirectoryDeleter fileDirectoryDeleter = new IoService();

		public NAntTask(): 
			this(new ProcessExecutor()){}

		public NAntTask(ProcessExecutor executor)
		{
			this.executor = executor;
        }

        #region Public fields
        #region Targets
        /// <summary>
        /// A list of targets to be called. CruiseControl.NET does not call NAnt once for each target, it uses the NAnt feature of being
        /// able to specify multiple targets.
        /// </summary>
        /// <version>1.0</version>
        /// <default>Default build target</default>
        [ReflectorArray("targetList", Required = false)]
		public string[] Targets = new string[0];
        #endregion

        #region Executable
        /// <summary>
        /// The path of the version of nant.exe you want to run. If this is relative, then must be relative to either (a) the base directory,
        /// (b) the CCNet Server application, or (c) if the path doesn't contain any directory details then can be available in the system or
        /// application's 'path' environment variable
        /// </summary>
        /// <version>1.0</version>
        /// <default>nant</default>
        [ReflectorProperty("executable", Required = false)]
		public string Executable = defaultExecutable;
        #endregion

        #region Priority
        /// <summary>
        /// The priority class of the spawned process.
        /// </summary>
        /// <version>1.5</version>
        /// <default>Normal</default>
        [ReflectorProperty("priority", Required = false)]
        public ProcessPriorityClass Priority = DefaultPriority;
        #endregion

        #region BuildFile
        /// <summary>
        /// The name of the build file to run, relative to the baseDirectory. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>Default build field in the working directory</default>
        [ReflectorProperty("buildFile", Required = false)]
		public string BuildFile = string.Empty;
        #endregion

        #region ConfiguredBaseDirectory
        /// <summary>
        /// The directory to run the NAnt process in. If relative, is a subdirectory of the Project Working Directory.
        /// </summary>
        /// <version>1.0</version>
        /// <default>Project working directory</default>
        [ReflectorProperty("baseDirectory", Required = false)]
		public string ConfiguredBaseDirectory = string.Empty;
        #endregion

        #region BuildArgs
        /// <summary>
        /// Any arguments to pass through to NAnt (e.g to specify build properties).
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("buildArgs", Required = false)]
		public string BuildArgs = string.Empty;
        #endregion

        #region Logger
        /// <summary>
        /// The NAnt logger to use.
        /// </summary>
        /// <version>1.0</version>
        /// <default>NAnt.Core.XmlLogger</default>
        [ReflectorProperty("logger", Required = false)]
		public string Logger = DefaultLogger;
        #endregion

        #region Listener
        /// <summary>
        /// The NAnt listener to use. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>NAnt.Core.DefaultLogger</default>
        [ReflectorProperty("listener", Required = false)]
		public string Listener = DefaultListener;
        #endregion

        #region NoLogo
        /// <summary>
        /// Whether to use the -nologo argument when calling NAnt.
        /// </summary>
        /// <version>1.0</version>
        /// <default>true</default>
        [ReflectorProperty("nologo", Required = false)]
		public bool NoLogo = DefaultNoLogo;
        #endregion

        #region BuildTimeoutSeconds
        /// <summary>
		/// The maximum number of seconds that the build may take.  If the build process takes longer than this period, it will be killed.
        /// Specify this value as zero to disable process timeouts.
		/// </summary>
        /// <version>1.0</version>
        /// <default>600</default>
        [ReflectorProperty("buildTimeoutSeconds", Required = false)]
		public int BuildTimeoutSeconds = DefaultBuildTimeout;
        #endregion
        #endregion

        /// <summary>
		/// Runs the integration using NAnt.  The build number is provided for labelling, build
		/// timeouts are enforced.  The specified targets are used for the specified NAnt build file.
		/// StdOut from nant.exe is redirected and stored.
		/// </summary>
		/// <param name="result">For storing build output.</param>
        protected override bool Execute(IIntegrationResult result)
		{
            string nantOutputFile = GetNantOutputFile(result);
            //delete old nant output logfile, if exist
		    fileDirectoryDeleter.DeleteIncludingReadOnlyObjects(nantOutputFile);

            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : 
                string.Format("Executing Nant :BuildFile: {0} Targets: {1} ", BuildFile, string.Join(", ", Targets)));

			ProcessResult processResult = TryToRun(CreateProcessInfo(result), result);
            
            if (File.Exists(nantOutputFile))
                result.AddTaskResult(new FileTaskResult(nantOutputFile));

		    result.AddTaskResult(new ProcessTaskResult(processResult, true));

		    // is this right?? or should this break the build
			if (processResult.TimedOut)
				throw new BuilderException(this, "NAnt process timed out (after " + BuildTimeoutSeconds + " seconds)");

            return !processResult.Failed;
		}

		protected override string GetProcessFilename()
		{
			return Executable;
		}

		protected override int GetProcessTimeout()
		{
			return BuildTimeoutSeconds * 1000;
		}

		protected override string GetProcessArguments(IIntegrationResult result)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AppendIf(NoLogo, "-nologo");
			buffer.AppendArgument(@"-buildfile:{0}", StringUtil.AutoDoubleQuoteString(BuildFile));
			buffer.AppendArgument("-logger:{0}", Logger);
			buffer.AppendArgument("-logfile:{0}", StringUtil.AutoDoubleQuoteString(GetNantOutputFile(result)));
			buffer.AppendArgument("-listener:{0}", Listener);
			buffer.AppendArgument(BuildArgs);
			AppendIntegrationResultProperties(buffer, result);
			AppendTargets(buffer);
			return buffer.ToString();
		}

		protected override string GetProcessBaseDirectory(IIntegrationResult result)
		{
			return result.BaseFromWorkingDirectory(ConfiguredBaseDirectory);
		}

        protected override ProcessPriorityClass GetProcessPriorityClass()
        {
            return this.Priority;
        }

		private static void AppendIntegrationResultProperties(ProcessArgumentBuilder buffer, IIntegrationResult result)
		{
			// We have to sort this alphabetically, else the unit tests
			// that expect args in a certain order are unpredictable
			IDictionary properties = result.IntegrationProperties;
			foreach (string key in properties.Keys)
			{
				object value = result.IntegrationProperties[key];
				if (value != null)
					buffer.AppendArgument(string.Format("-D:{0}={1}", key, StringUtil.AutoDoubleQuoteString(StringUtil.RemoveTrailingPathDelimeter(StringUtil.IntegrationPropertyToString(value)))));
			}
		}

		private void AppendTargets(ProcessArgumentBuilder buffer)
		{
			foreach(string t in Targets)
			{
				buffer.AppendArgument(t);
			}
		}

		public override string ToString()
		{
			string baseDirectory = ConfiguredBaseDirectory ??string.Empty;
			return string.Format(@" BaseDirectory: {0}, Targets: {1}, Executable: {2}, BuildFile: {3}", baseDirectory, string.Join(", ", Targets), Executable, BuildFile);
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

		private string GetNantOutputFile(IIntegrationResult result)
		{
            return Path.Combine(result.ArtifactDirectory, string.Format(logFilename, LogFileId));
		}
	}
}
