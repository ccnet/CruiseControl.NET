using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Mercurial
{
    [ReflectorType("hg")]
    public class Mercurial : ProcessSourceControl
    {
        public const string COMMAND_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
        public const string DefaultExecutable = "hg";
        public const string DefaultTagCommitMessage = "Tagging successful build {0}";
        public const string HistoryTemplate = "<modification><node>{node|short}</node><author>{author|user}</author><date>{date|rfc822date}</date><desc>{desc|escape}</desc><rev>{rev}</rev><email>{author|email|obfuscate}</email><files>{files}</files></modification>";

        private readonly IFileSystem _fileSystem;

        [ReflectorProperty("autoGetSource", Required = false)] public bool AutoGetSource = true;

        [ReflectorProperty("branch", Required = false)] public string Branch;

        [ReflectorProperty("executable", Required = false)] public string Executable = DefaultExecutable;

        [ReflectorProperty("multipleHeadsFail", Required = false)] public bool MultipleHeadsFail = true;

        [ReflectorProperty("repo", Required = false)] public string Repo;

        [ReflectorProperty("tagCommitMessage", Required = false)] public string TagCommitMessage = DefaultTagCommitMessage;

        [ReflectorProperty("tagOnSuccess", Required = false)] public bool TagOnSuccess = false;

        [ReflectorProperty("workingDirectory", Required = false)] public string WorkingDirectory;

        [ReflectorProperty("webUrlBuilder", InstanceTypeKey = "type", Required = false)] public IModificationUrlBuilder UrlBuilder;

        public Mercurial() : this(new MercurialHistoryParser(), new ProcessExecutor(), new SystemIoFileSystem()) { }

        public Mercurial(IHistoryParser historyParser, ProcessExecutor executor, IFileSystem fileSystem)
            : base(historyParser, executor) { _fileSystem = fileSystem; }

        public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
        {
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

            if (UrlBuilder != null)
            {
                UrlBuilder.SetupModification(modifications);
            }
            return modifications;
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
            if (!TagOnSuccess || result.Failed)
            {
                return;
            }
            Execute(LabelFlowCreateTagProccessInfo(result));
            Execute(LabelFlowCreatePushProcessInfo(result));
        }

        public override void GetSource(IIntegrationResult result)
        {
            if (!AutoGetSource)
            {
                return;
            }
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
