using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;


namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    ///   Source Control Plugin for CruiseControl.NET that talks to git.
    /// </summary>
    [ReflectorType("git")]
    public class Git : ProcessSourceControl
    {
		private const string historyFormat = "'Commit:%H%nTime:%ci%nAuthor:%an%nE-Mail:%ae%nMessage:%s%n%n%b%nChanges:'";

        private readonly IFileSystem _fileSystem;

        [ReflectorProperty("autoGetSource", Required = false)]
		public bool AutoGetSource = true;

        [ReflectorProperty("executable", Required = false)]
        public string Executable = "git";

        [ReflectorProperty("repository", Required = true)]
        public string Repository;

        [ReflectorProperty("branch", Required = false)]
        public string Branch = "master";

        [ReflectorProperty("tagCommitMessage", Required = false)]
		public string TagCommitMessage = "CCNET build {0}";

        [ReflectorProperty("tagOnSuccess", Required = false)]
        public bool TagOnSuccess;

        [ReflectorProperty("workingDirectory", Required = false)]
        public string WorkingDirectory;

        public Git() : this(new GitHistoryParser(), new ProcessExecutor(), new SystemIoFileSystem()) { }

        public Git(IHistoryParser historyParser, ProcessExecutor executor, IFileSystem fileSystem)
            : base(historyParser, executor)
        {
            _fileSystem = fileSystem;
        }

        public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
        {
            if (!Fetch(to) && GitLogOriginHash(to) == GitLogLocalHash(to))
				return new Modification[0];

			return ParseModifications(GitLogHistory(from, to), from.StartTime, to.StartTime);
        }

        public override void GetSource(IIntegrationResult result)
        {
            if (!AutoGetSource)
				return;

            GitClean(result);

            if (GitLogLocalHash(result) != null)
				GitReset(result);

            GitMerge(result);
        }

        public override void LabelSourceControl(IIntegrationResult result)
        {
            if (!TagOnSuccess || result.Failed)
				return;

            GitTag(result);
            GitPushTags(result);
        }

        private string BaseWorkingDirectory(IIntegrationResult result)
        {
            return result.BaseFromWorkingDirectory(WorkingDirectory);
        }

        /// <summary>
        /// Fetches a git repository.  
        /// 
        /// If the working directory doesn't exist then a 'git clone' is issued to 
        /// initialize the local repo and fetch changes from the remote repo.
        /// 
        /// Else if the .git directory doesn't exist then 'git init' initializes 
        /// the working directory, 'git config' sets up the required configuration 
        /// properties, and a 'git fetch' is issued to fetch changes from the remote 
        /// repo.
        /// 
        /// Else if the working directory is already a git repository then a 'git fetch'
        /// is issued to fetch changes from the remote repo.
        /// </summary>
        /// <returns>
        /// Returns true if we needed to create the local repository
        /// </returns>
        private bool Fetch(IIntegrationResult result)
        {
            string wd = BaseWorkingDirectory(result);
            bool first = false;

            if (_fileSystem.DirectoryExists(wd))
            {
                if (!_fileSystem.DirectoryExists(Path.Combine(wd, ".git")))
                {
                    // Initialise the existing directory 
                    GitInit(result);

                    // Set config options 
                    GitConfig("remote.origin.url", Repository, result);
                    GitConfig("remote.origin.fetch", "+refs/heads/*:refs/remotes/origin/*", result);
                    GitConfig(string.Format("branch.{0}.remote", Branch), "origin", result);
                    GitConfig(string.Format("branch.{0}.merge", Branch), string.Concat("refs/heads/", Branch), result);

                    first = true;
                }

                // Fetch changes from the remote repository
                GitFetch(result);
            }
            else
            {
                // Cloning will setup the working directory as a git repository and do a fetch for us
                GitClone(result);
                first = true;
            }
            return first;
        }

        private ProcessInfo NewProcessInfo(string args, string dir)
        {
            Log.Info("Calling git " + args);
            ProcessInfo processInfo = new ProcessInfo(Executable, args, dir);
            processInfo.StreamEncoding = Encoding.UTF8;
            return processInfo;
        }

        #region "git commands"

        /// <summary>
        /// Get the hash of the latest commit in the remote repository
        /// </summary>
        private string GitLogOriginHash(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("log");
            buffer.AddArgument("origin/master");
            buffer.AddArgument("--date-order");
            buffer.AddArgument("-1");
            buffer.AddArgument("--pretty=format:'%H'");
            return Execute(NewProcessInfo(buffer.ToString(), BaseWorkingDirectory(result))).StandardOutput.Trim();
        }

        /// <summary>
        /// Get the hash of the latest commit in the local repository
        /// </summary>
        private string GitLogLocalHash(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("log");
            buffer.AddArgument("--date-order");
            buffer.AddArgument("-1");
            buffer.AddArgument("--pretty=format:'%H'");

            string hash = null;
            try
            {
                hash = Execute(NewProcessInfo(buffer.ToString(), BaseWorkingDirectory(result))).StandardOutput.Trim();
            }
            catch (CruiseControlException ex)
            {
                if (!ex.Message.Contains("fatal: bad default revision 'HEAD'"))
					throw;
            }
            return hash;
        }

        /// <summary>
        /// Get a list of all commits in date order.  The position of each commit in the list is used as the ChangeNumber.
        /// </summary>
        private ProcessResult GitLogHistory(IIntegrationResult from, IIntegrationResult to)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("log");
            buffer.AddArgument("origin/master");
            buffer.AddArgument("--date-order");
			buffer.AddArgument(string.Concat("--after=", from.StartTime.ToUniversalTime().ToString("R")));
			buffer.AddArgument(string.Concat("--before=", to.StartTime.ToUniversalTime().ToString("R")));
            buffer.AddArgument(string.Concat("--pretty=format:", historyFormat));

            return Execute(NewProcessInfo(buffer.ToString(), BaseWorkingDirectory(to)));
        }

        private void GitClone(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("clone");
            buffer.AddArgument(Repository);
            buffer.AddArgument(BaseWorkingDirectory(result));
            Execute(NewProcessInfo(buffer.ToString(), BaseWorkingDirectory(result)));
        }

        private void GitInit(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("init");
            Execute(NewProcessInfo(buffer.ToString(), BaseWorkingDirectory(result)));
        }

        private void GitConfig(string name, string value, IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("config");
            buffer.AddArgument(name);
            buffer.AddArgument(value);
            Execute(NewProcessInfo(buffer.ToString(), BaseWorkingDirectory(result)));
        }

        private void GitFetch(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("fetch");
            Execute(NewProcessInfo(buffer.ToString(), BaseWorkingDirectory(result)));
        }

        private void GitTag(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("tag");
            buffer.AddArgument("-a");
            buffer.AddArgument("-m", string.Format(TagCommitMessage, result.Label));
            buffer.AddArgument(result.Label);
            Execute(NewProcessInfo(buffer.ToString(), BaseWorkingDirectory(result)));
        }

        private void GitClean(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("clean");
            buffer.AddArgument("-d");
            buffer.AddArgument("-f");
            buffer.AddArgument("-x");
            Execute(NewProcessInfo(buffer.ToString(), BaseWorkingDirectory(result)));
        }

        private void GitReset(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("reset");
            buffer.AddArgument("HEAD");
            buffer.AddArgument("--hard");
            Execute(NewProcessInfo(buffer.ToString(), BaseWorkingDirectory(result)));
        }

        private void GitMerge(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("merge");
            buffer.AddArgument(string.Format("origin/{0}", Branch));
            Execute(NewProcessInfo(buffer.ToString(), BaseWorkingDirectory(result)));
        }

        private void GitPushTags(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("push");
            buffer.AddArgument("--tags");
            Execute(NewProcessInfo(buffer.ToString(), BaseWorkingDirectory(result)));
        }
        #endregion

    }

}
