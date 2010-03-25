namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Mercurial
{
    using System;
    using System.IO;
    using System.Text;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;

    /// <summary>
    /// Provides basic support for Mercurial repositories. Checking for changes, checking out or updating sources, and tagging are supported.
    /// </summary>
    /// <title> Mercurial Source Control Block </title>
    /// <version>1.5</version>
    /// <example>
    /// <code>
    /// &lt;sourcecontrol type="hg"&gt;
    /// &lt;repo&gt;http://hg.mycompany.com/hgwebdir.cgi/myproject/&lt;/repo&gt;
    /// &lt;workingDirectory&gt;c:\dev\ccnet\myproject&lt;/workingDirectory&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// </example>
    /// <key name="type">
    /// <description>The type of source control block.</description>
    /// <value>hg</value>
    /// </key>
    /// <remarks>
    /// <para>
    /// You need to make sure your hg client settings are such that all authentication is automated. Typically you can do this by using anonymous access or appropriate
    /// SSH setups if using hg over SSH.
    /// </para>
    /// <para>
    /// You can link the modifications detected by CruiseControl.NET to the appropriate hgweb page by adding the following additional configuration information to the
    /// Mercurial source control section by using the <link>Mercurial Issue Tracker URL Builder</link>.
    /// </para>
    /// <para>
    /// External contributors: Kent Johnson
    /// </para>
    /// </remarks>
    [ReflectorType("hg")]
    public class Mercurial : ProcessSourceControl
    {
        public const string COMMAND_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
        public const string DefaultExecutable = "hg";
        public const string DefaultTagCommitMessage = "Tagging successful build {0}";
        public const string HistoryTemplate = "<modification><node>{node|short}</node><author>{author|user}</author><date>{date|rfc822date}</date><desc>{desc|escape}</desc><rev>{rev}</rev><email>{author|email|obfuscate}</email><files>{files}</files></modification>";

        private readonly IFileSystem _fileSystem;
        private BuildProgressInformation _buildProgressInformation;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Mercurial"/> class.
        /// </summary>
        public Mercurial()
            : this(new MercurialHistoryParser(), new ProcessExecutor(), new SystemIoFileSystem())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mercurial"/> class.
        /// </summary>
        /// <param name="historyParser">The history parser.</param>
        /// <param name="executor">The executor.</param>
        /// <param name="fileSystem">The file system.</param>
        public Mercurial(IHistoryParser historyParser, ProcessExecutor executor, IFileSystem fileSystem)
            : base(historyParser, executor)
        {
            _fileSystem = fileSystem;
            this.AutoGetSource = true;
            this.Executable = DefaultExecutable;
            this.MultipleHeadsFail = true;
            this.TagCommitMessage = DefaultTagCommitMessage;
            this.TagOnSuccess = false;
        }
        #endregion

        /// <summary>
        /// Whether to update the local working copy from the local repository for a particular build. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>true</default>
        [ReflectorProperty("autoGetSource", Required = false)]
        public bool AutoGetSource { get; set; }

        /// <summary>
        /// Repository branch.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("branch", Required = false)]
        public string Branch { get; set; }

        /// <summary>
        /// The location of the hg executable.
        /// </summary>
        /// <version>1.5</version>
        /// <default>hg</default>
        [ReflectorProperty("executable", Required = false)]
        public string Executable { get; set; }

        /// <summary>
        /// Should the build fail if the local repository has multiple heads?
        /// </summary>
        /// <version>1.5</version>
        /// <default>true</default>
        [ReflectorProperty("multipleHeadsFail", Required = false)]
        public bool MultipleHeadsFail { get; set; }

        /// <summary>
        /// The url for your repository (e.g., http://hgserver/myproject/).
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// This is required if autoGetSource is true.
        /// </remarks>
        [ReflectorProperty("repo", Required = false)]
        public string Repo { get; set; }

        /// <summary>
        /// String format for tags in your repository.
        /// </summary>
        /// <version>1.5</version>
        /// <default>Tagging successful build \{0\}</default>
        [ReflectorProperty("tagCommitMessage", Required = false)]
        public string TagCommitMessage { get; set; }

        /// <summary>
        /// Indicates that the repository should be tagged if the build succeeds.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("tagOnSuccess", Required = false)]
        public bool TagOnSuccess { get; set; }

        /// <summary>
        /// The directory containing the locally checked out workspace.
        /// </summary>
        /// <version>1.5</version>
        /// <default>Project Working Directory</default>
        [ReflectorProperty("workingDirectory", Required = false)]
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Generates a web URL.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("webUrlBuilder", InstanceTypeKey = "type", Required = false)]
        public IModificationUrlBuilder UrlBuilder { get; set; }

        public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
        {
            GetBuildProgressInformation(to).SignalStartRunTask("Checking modifications from Mercurial");

            // enable Stdout monitoring
            ProcessExecutor.ProcessOutput += ProcessExecutor_ProcessOutput;

            string workingDirectory = to.BaseFromWorkingDirectory(WorkingDirectory);
            EnsureWorkingDirectoryExists(workingDirectory);
            InitializeLocalRepository(workingDirectory);
            Execute(GetModificationsFlowCreatePullProcessInfo(workingDirectory));
            int revisionId = GetStartingRevisionId(workingDirectory);
            int tipId = GetTipId(workingDirectory);
            if (tipId == revisionId) //no modifications; working directory is tip
            {
                return new Modification[0];
            }
            ProcessResult modificationResults = Execute(GetModificationsFlowGetLog(revisionId + 1, tipId, workingDirectory));
            Modification[] modifications = ParseModifications(modificationResults, from.StartTime, to.StartTime);

            // remove Stdout monitoring
            ProcessExecutor.ProcessOutput -= ProcessExecutor_ProcessOutput;

            if (UrlBuilder != null)
            {
                UrlBuilder.SetupModification(modifications);
            }
            return modifications;
        }

        private BuildProgressInformation GetBuildProgressInformation(IIntegrationResult result)
        {
            if (_buildProgressInformation == null)
                _buildProgressInformation = result.BuildProgressInformation;

            return _buildProgressInformation;
        }

        private void ProcessExecutor_ProcessOutput(object sender, ProcessOutputEventArgs e)
        {
            if (_buildProgressInformation == null)
                return;

            // ignore error output in the progress information
            if (e.OutputType == ProcessOutputType.ErrorOutput)
                return;

            _buildProgressInformation.AddTaskInformation(e.Data);
        }

        private ProcessInfo GetModificationsFlowGetLog(int revisionId, int tipId, string workingDirectory)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("log");
            buffer.AddArgument("-r", string.Format("{0}:{1}", revisionId, tipId));
            buffer.AddArgument("--template", HistoryTemplate);
            AppendCommonSwitches(buffer);
            return NewProcessInfo(buffer.ToString(), workingDirectory);
        }

        private int GetTipId(string workingDirectory)
        {
            ProcessResult result = Execute(GetModificationsFlowFindTipRevisionNumber(workingDirectory));
            int revisionId;
            int.TryParse(result.StandardOutput.Trim(), out revisionId);
            return revisionId;
        }

        private ProcessInfo GetModificationsFlowFindTipRevisionNumber(string workingDirectory)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("log");
            buffer.AddArgument("-r", "tip");
            buffer.AddArgument("--template", "{rev}");
            AppendCommonSwitches(buffer);
            return NewProcessInfo(buffer.ToString(), workingDirectory);
        }

        private int GetStartingRevisionId(string workingDirectory)
        {
            ProcessResult result = Execute(GetModificationsFlowFindRevisionNumber(workingDirectory));
            int revisionId;
            if (int.TryParse(result.StandardOutput.Trim(), out revisionId))
            {
                return revisionId;
            }
            return -1;
        }

        private ProcessInfo GetModificationsFlowGetFullLog(string workingDirectory)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("log");
            buffer.AddArgument("--template", HistoryTemplate);
            AppendCommonSwitches(buffer);
            return NewProcessInfo(buffer.ToString(), workingDirectory);
        }

        private ProcessInfo GetModificationsFlowFindRevisionNumber(string workingDirectory)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("parents");
            buffer.AddArgument("--template", "{rev}");
            AppendCommonSwitches(buffer);
            return NewProcessInfo(buffer.ToString(), workingDirectory);
        }

        public override void LabelSourceControl(IIntegrationResult result)
        {
            GetBuildProgressInformation(result).SignalStartRunTask("Mercurial: tag current build");

            if (!TagOnSuccess || result.Failed)
            {
                return;
            }

            // enable Stdout monitoring
            ProcessExecutor.ProcessOutput += ProcessExecutor_ProcessOutput;

            Execute(LabelFlowCreateTagProccessInfo(result));
            Execute(LabelFlowCreatePushProcessInfo(result));

            // remove Stdout monitoring
            ProcessExecutor.ProcessOutput -= ProcessExecutor_ProcessOutput;
        }

        public override void GetSource(IIntegrationResult result)
        {
            GetBuildProgressInformation(result).SignalStartRunTask("Getting source from Mercurial");

            if (!AutoGetSource)
            {
                return;
            }

            // enable Stdout monitoring
            ProcessExecutor.ProcessOutput += ProcessExecutor_ProcessOutput;

            if (MultipleHeadsFail)
            {
                ProcessResult headsfound = Execute(GetSourceFlowGetHeadsProcessInfo(result.BaseFromWorkingDirectory(WorkingDirectory)));
                Modification[] headChangSets = ParseModifications(headsfound, DateTime.Now, DateTime.Now);
                if (headChangSets.Length != 1)
                {
                    throw new MultipleHeadsFoundException();
                }
            }
            Execute(GetSourceFlowPerformUpdateProcessInfo(result.BaseFromWorkingDirectory(WorkingDirectory)));

            // remove Stdout monitoring
            ProcessExecutor.ProcessOutput -= ProcessExecutor_ProcessOutput;
        }

        private ProcessInfo GetModificationsFlowCreateInitProcessInfo(string workingDirectory)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("init");
            AppendCommonSwitches(buffer);
            return NewProcessInfo(buffer.ToString(), workingDirectory);
        }

        private ProcessInfo GetModificationsFlowCreatePullProcessInfo(string workingDirectory)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("pull");
            AppendBranch(buffer);
            AppendCommonSwitches(buffer);
            if (!string.IsNullOrEmpty(Repo))
            {
                buffer.AddArgument(Repo);
            }
            return NewProcessInfo(buffer.ToString(), workingDirectory);
        }

        private ProcessInfo LabelFlowCreateTagProccessInfo(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("tag");
            buffer.AddArgument("-m", string.Format(TagCommitMessage, result.Label));
            AppendCommonSwitches(buffer);
            buffer.AddArgument(result.Label);
            return NewProcessInfo(buffer.ToString(), result.BaseFromWorkingDirectory(WorkingDirectory));
        }

        private ProcessInfo LabelFlowCreatePushProcessInfo(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("push");
            AppendCommonSwitches(buffer);
            if (!string.IsNullOrEmpty(Repo))
            {
                buffer.AddArgument(Repo);
            }
            return NewProcessInfo(buffer.ToString(), result.BaseFromWorkingDirectory(WorkingDirectory));
        }

        private ProcessInfo GetSourceFlowGetHeadsProcessInfo(string workingDirectory)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("heads");
            AppendBranch(buffer);
            buffer.AddArgument("--template", HistoryTemplate);
            AppendCommonSwitches(buffer);
            return NewProcessInfo(buffer.ToString(), workingDirectory);
        }

        private ProcessInfo GetSourceFlowPerformUpdateProcessInfo(string workingDirectory)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("update");
            AppendBranch(buffer);
            AppendCommonSwitches(buffer);
            return NewProcessInfo(buffer.ToString(), workingDirectory);
        }

        private ProcessInfo NewProcessInfo(string args, string workingDirectory)
        {
            Log.Info("Calling hg " + args);
            ProcessInfo processInfo = new ProcessInfo(Executable, args, workingDirectory);
            processInfo.StreamEncoding = Encoding.UTF8;
            return processInfo;
        }

        private void EnsureWorkingDirectoryExists(string workingDirectory) { _fileSystem.EnsureFolderExists(workingDirectory); }

        private void InitializeLocalRepository(string workingDirectory)
        {
            if (!_fileSystem.DirectoryExists(Path.Combine(workingDirectory, ".hg")))
            {
                Execute(GetModificationsFlowCreateInitProcessInfo(workingDirectory));
            }
        }

        private void AppendBranch(ProcessArgumentBuilder buffer)
        {
            if (!string.IsNullOrEmpty(Branch))
            {
                buffer.AddArgument("-r", Branch);
            }
        }

        private static void AppendCommonSwitches(ProcessArgumentBuilder buffer) { buffer.AddArgument("--noninteractive"); }
    }
}
