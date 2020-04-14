using System.Diagnostics;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using System.Globalization;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    ///   Source Control Plugin for CruiseControl.NET that talks to git.
    /// </summary>
    /// <title>Git Source Control Block</title>
    /// <version>1.5</version>
    /// <key name="type">
    /// <description>The type of source control block.</description>
    /// <value>git</value>
    /// </key>
    /// <example>
    /// <code title="Minimalist Example">
    /// &lt;sourcecontrol type="git"&gt;
    /// &lt;repository&gt;git://github.com/rails/rails.git&lt;/repository&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// <code title="Full Example">
    /// &lt;sourcecontrol type="git"&gt;
    /// &lt;repository&gt;git://github.com/rails/rails.git&lt;/repository&gt;
    /// &lt;branch&gt;master&lt;/branch&gt;
    /// &lt;autoGetSource&gt;true&lt;/autoGetSource&gt;
    /// &lt;fetchSubmodules&gt;true&lt;/fetchSubmodules&gt;
    /// &lt;executable&gt;git&lt;/executable&gt;
    /// &lt;tagOnSuccess&gt;false&lt;/tagOnSuccess&gt;
    /// &lt;commitBuildModifications&gt;false&lt;/commitBuildModifications&gt;
    /// &lt;commitUntrackedFiles&gt;false&lt;/commitUntrackedFiles&gt;
    /// &lt;tagCommitMessage&gt;CCNet Build {0}&lt;/tagCommitMessage&gt;
    /// &lt;tagNameFormat&gt;CCNet-Build-{0}&lt;/tagNameFormat&gt;
    /// &lt;committerName&gt;Max Mustermann&lt;/committerName&gt;
    /// &lt;committerEMail&gt;max.mustermann@gmx.de&lt;/committerEMail&gt;
    /// &lt;workingDirectory&gt;c:\build\rails&lt;/workingDirectory&gt;
    /// &lt;timeout&gt;60000&lt;/timeout&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <heading>How does this work?</heading>
    /// <para>
    /// <b>Local repository initialization</b>
    /// </para>
    /// <para>
    /// The Git Source Control Block will check whenever the specified working directory exist or not. If it does not exist a "git clone"
    /// command is issued to create and setup the local repository. Also the configuration settings "user.name" and "user.email" for the local
    /// repository will be set with "git config" if both are provided. If 'fetchSubmodules' is set to 'true' git submodules will be initialized.
    /// </para>
    /// <para>
    /// If the working directory exists but is not a git repository (e.g. the .git directory is missing) it will be deleted and the "git clone"
    /// and configuration instructions described above will be issued.
    /// </para>
    /// <para>
    /// If the working directory is already the root of an existing git repository no initialization is done.
    /// </para>
    /// <para>
    /// <b>Checking for modifications</b>
    /// </para>
    /// <para>
    /// Once the repository is initialized the "git fetch origin" command is issued to fetch the remote changes. Next,
    /// "git log $LastIntegrationCommit..origin/$BranchName --name-status -c",
    /// is issued to get a list of commits and their changes, where $LastIntegrationCommit is the commit which was
    /// checked out the last time an integration was run. If the project has not yet been integrated, a
    /// "git log origin/$BranchName --name-status -c"
    /// command is issued instead.
    /// </para>
    /// <para>
    /// <b>Getting the source</b>
    /// </para>
    /// <para>
    /// Once Cruise Control.NET has modifications detected and the 'autoGetSource' property is set to 'true' the "git checkout -f
    /// origin/$NameOfTheBranch" command is issued. Also the "git clean -f -d -x" command to get a clean working copy to start a new build.
    /// If 'fetchSubmodules' is set to 'true' git submodules will be fetched and updated.
    /// </para>
    /// <para>
    /// <b>Tagging a successful build</b>
    /// </para>
    /// <para>
    /// After a successful build and when the 'tagOnSuccess' property is set to 'true' the "git -a -m 'tagCommitMessage' tag 'build label'"
    /// command is issued and a "git push origin tag 'name of the tag'" to push the tag to the remote repository.
    /// </para>
    /// <para>
    /// If 'commitBuildModifications' is set to 'true' then all modified files will be committed before tagging with a "git commit --all
    /// --allow-empty -m 'tagCommitMessage'". If you also set 'commitUntrackedFiles' to 'true' all untracked files that are not ignored by
    /// .gitignores will be added to the git index before committing and tagging with a "git add --all" command.
    /// </para>
    /// <heading>Using Git on Windows</heading>
    /// <para>
    /// Download and install the latest version of msysgit.
    /// </para>
    /// <list type="1">
    /// <item>
    /// Point the "executable" property to the git.cmd file (e.g. C:\Program Files\Git\cmd\git.cmd)
    /// </item>
    /// <item>
    /// Or set the "path" environment variable to the "bin" directory of your msysgit instalation (e.g. C:\Program Files\Git\bin), the "HOME"
    /// environment variable to "%USERPROFILE%" and the "PLINK_PROTOCOL" environment variable to "ssh".
    /// </item>
    /// </list>
    /// <para>
    /// Homepage: http://code.google.com/p/msysgit/
    /// </para>
    /// <heading>Using Git on Unix (CruiseControl.NET with Mono)</heading>
    /// <para>
    /// Make sure that you've the latest Git installed via your distributions packet manager and that git and all its required applications are
    /// in $PATH.
    /// </para>
    /// <heading>Additional Information</heading>
    /// <para>
    /// The default port git uses is 9418. Git over SSH uses port 22 by default. Make sure that your firewall is set up to handle this.
    /// </para>
    /// </remarks>
    [ReflectorType("git")]
    public class Git : ProcessSourceControl
    {
        /// <summary>
        /// Used as the key for storing the most recently integrated commit within
        /// <see cref="IIntegrationResult.SourceControlData"/>.
        /// </summary>
        private const string COMMIT_KEY = "commit";
        private const string historyFormat = "Commit:%H%nTime:%ci%nAuthor:%an%nE-Mail:%ae%nMessage:%s%n%n%b%nChanges:";

        private readonly IFileSystem _fileSystem;
        private readonly IFileDirectoryDeleter _fileDirectoryDeleter;
        private BuildProgressInformation _buildProgressInformation;

        /// <summary>
        /// Whether to fetch the updates from the repository and checkout the branch for a particular build. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>true</default>
        [ReflectorProperty("autoGetSource", Required = false)]
        public bool AutoGetSource { get; set; }

        /// <summary>
        /// Remove untracked files from the working tree
        /// </summary>
        /// <version>1.8.5</version>
        /// <default>true</default>
        [ReflectorProperty("cleanUntrackedFiles", Required = false)]
        public bool CleanUntrackedFiles { get; set; }

        /// <summary>
        /// The location of the Git executable. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>git</default>
        [ReflectorProperty("executable", Required = false)]
        public string Executable { get; set; }

        /// <summary>
        /// The url to the remote repository. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("repository", Required = true)]
        public string Repository { get; set; }

        /// <summary>
        /// Remote repository branch to monitor and checkout. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>master</default>
        [ReflectorProperty("branch", Required = false)]
        public string Branch { get; set; }

        /// <summary>
        /// Indicates that CruiseControl.NET should initialize and fetch git submodules.
        /// </summary>
        /// <version>1.6</version>
        /// <default>false</default>
        [ReflectorProperty("fetchSubmodules", Required = false)]
        public bool FetchSubmodules { get; set; }


        /// <summary>
        /// Maximum amount of commit logs to fetch when checking the history. 
        /// 0 Means no limit, X means limit to that number
        /// </summary>
        /// <version>1.8.5</version>
        /// <default>100</default>
        [ReflectorProperty("maxAmountOfModificationsToFetch", Required = false)]
        public int MaxAmountOfModificationsToFetch { get; set; }

        /// <summary>
        /// Format string for the commit message of each tag. \{0\} is the placeholder for the current build label. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>CCNet Build \{0\}</default>
        [ReflectorProperty("tagCommitMessage", Required = false)]
        public string TagCommitMessage { get; set; }

        /// <summary>
        /// Format string for the name of each tag. Make sure you're only using allowed characters. \{0\} is the placeholder for the current
        /// build label. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>CCNet-Build-\{0\}</default>
        [ReflectorProperty("tagNameFormat", Required = false)]
        public string TagNameFormat { get; set; }

        /// <summary>
        /// Indicates that the repository should be tagged if the build succeeds.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("tagOnSuccess", Required = false)]
        public bool TagOnSuccess { get; set; }

        /// <summary>
        /// Indicates that all modifications during the build process should be committed before tagging. This requires 'tagOnSuccess ' to be
        /// set to 'true'.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("commitBuildModifications", Required = false)]
        public bool CommitBuildModifications { get; set; }

        /// <summary>
        /// Indicates that files created during the build process should be committed before tagging. This requires 'commitBuildModifications'
        /// and 'tagOnSuccess ' to be set to 'true'.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("commitUntrackedFiles", Required = false)]
        public bool CommitUntrackedFiles { get; set; }

        /// <summary>
        /// Used to set the "user.name" configuration setting in the local repository. Required for the 'tagOnSuccess ' feature. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("committerName", Required = false)]
        public string CommitterName { get; set; }

        /// <summary>
        /// Used to set the "user.email" configuration setting in the local repository. Required for the 'tagOnSuccess ' feature. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("committerEMail", Required = false)]
        public string CommitterEMail { get; set; }

        /// <summary>
        /// The directory containing the local git repository. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>Project Working Directory</default>
        [ReflectorProperty("workingDirectory", Required = false)]
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Git" /> class.	
        /// </summary>
        /// <remarks></remarks>
        public Git() : this(new GitHistoryParser(), new ProcessExecutor(), new SystemIoFileSystem(), new IoService()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Git" /> class.	
        /// </summary>
        /// <param name="historyParser">The history parser.</param>
        /// <param name="executor">The executor.</param>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="fileDirectoryDeleter">The file directory deleter.</param>
        /// <remarks></remarks>
        public Git(IHistoryParser historyParser, ProcessExecutor executor, IFileSystem fileSystem, IFileDirectoryDeleter fileDirectoryDeleter)
            : base(historyParser, executor)
        {
            _fileSystem = fileSystem;
            _fileDirectoryDeleter = fileDirectoryDeleter;
            this.AutoGetSource = true;
            this.CleanUntrackedFiles = true;
            this.Executable = "git";
            this.Branch = "master";
            this.TagCommitMessage = "CCNet Build {0}";
            this.TagNameFormat = "CCNet-Build-{0}";
            this.MaxAmountOfModificationsToFetch = 100;
        }

        /// <summary>
        /// Gets the modifications.	
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
        {
            // fetch latest changes from the remote repository
            RepositoryAction result = CreateUpateLocalRepository(to);

            Dictionary<string, string> revisionData = NameValuePair.ToDictionary(from.SourceControlData);
            ProcessResult logResult;
            string lastCommit;
            if (revisionData.TryGetValue(COMMIT_KEY, out lastCommit))
            {
                logResult = GitLogHistory(Branch, lastCommit, to);
            }
            else
            {
                Log.Debug(string.Concat("[Git] last integrated commit not found, using all ancestors of origin/",
                    Branch, " as the set of modifications."));
                logResult = GitLogHistory(Branch, to);
            }

            // Get the hash of the origin head, and store it against the integration result.
            string originHeadHash = GitLogOriginHash(Branch, to);
            revisionData[COMMIT_KEY] = originHeadHash;
            to.SourceControlData.Clear();
            NameValuePair.Copy(revisionData, to.SourceControlData);

            return ParseModifications(logResult, lastCommit);
        }

        /// <summary>
        /// Gets the source.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
        public override void GetSource(IIntegrationResult result)
        {
            if (!AutoGetSource)
                return;

            // checkout remote branch
            GitCheckoutRemoteBranch(Branch, result);

			// update submodules
			if (FetchSubmodules)
				GitUpdateSubmodules(result);

            // clean up the local working copy
			if (CleanUntrackedFiles)
				GitClean(result);
        }

        /// <summary>
        /// Labels the source control.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
        public override void LabelSourceControl(IIntegrationResult result)
        {
            if (!TagOnSuccess || result.Failed)
                return;

            string tagName = string.Format(CultureInfo.CurrentCulture, TagNameFormat, result.Label);
            string commitMessage = string.Format(CultureInfo.CurrentCulture, TagCommitMessage, result.Label);

            if (CommitBuildModifications)
            {
                // add all modified and all untracked files to the git index.
                if (CommitUntrackedFiles)
                    GitAddAll(result);

                // commit all modifications during build before tagging.
                GitCommitAll(commitMessage, result);
            }

            // create a tag and push it.
            GitCreateTag(tagName, commitMessage, result);
            GitPushTag(tagName, result);
        }

        #region private methods

        private string BaseWorkingDirectory(IIntegrationResult result)
        {
            return Path.GetFullPath(result.BaseFromWorkingDirectory(WorkingDirectory));
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

        /// <summary>
        /// Fetches a git repository.
        /// 
        /// If the working directory doesn't exist then a 'git clone' is issued to
        /// initialize the local repo and fetch changes from the remote repo.
        /// Also setup the local repository with some required configuration settings.
        /// 
        /// Else if the .git directory doesn't exist then delete the working directory
        /// and call the method recursive again.
        /// 
        /// Else if the working directory is already a git repository then a 'git fetch'
        /// is issued to fetch changes from the remote repo.
        /// </summary>
        /// <param name="result">IIntegrationResult of the current build.</param>
        /// <returns>The action that was done, repository created or updated.</returns>
        private RepositoryAction CreateUpateLocalRepository(IIntegrationResult result)
        {
            string workingDirectory = BaseWorkingDirectory(result);
            string gitRepositoryDirectory = Path.Combine(workingDirectory, ".git");

            // check whether the working directory exist
            if (!_fileSystem.DirectoryExists(workingDirectory))
            {
                Log.Debug(string.Concat("[Git] Working directory '", workingDirectory, "' does not exist."));

                // if the working does not exist, call git clone
                GitClone(result);

                // init submodules
                if (FetchSubmodules)
                    GitInitSubmodules(result);

                // setup some required configuration settings for the local repository
                SetupLocalRepository(result);

                return RepositoryAction.Created;
            }


            // check whether this is a local git repository or just an existing folder
            if (!_fileSystem.DirectoryExists(gitRepositoryDirectory))
            {
                Log.Debug(string.Concat("[Git] Working directory '", workingDirectory,
                                        "' already exists, but it is not a git repository. Try deleting it and starting again."));

                // delete working directory and call CreateUpateLocalRepository recursive
                _fileDirectoryDeleter.DeleteIncludingReadOnlyObjects(workingDirectory);
                return CreateUpateLocalRepository(result);
            }

            // we are in a local git repository, fetch the latest remote changes
            GitFetch(result);

            return RepositoryAction.Updated;
        }

        /// <summary>
        /// Setup the local repository with some required config settings.
        /// 
        /// For tagging:
        /// - User name
        /// - User e-mail
        /// </summary>
        /// <param name="result">IIntegrationResult of the current build.</param>
        private void SetupLocalRepository(IIntegrationResult result)
        {
            if (!string.IsNullOrEmpty(CommitterName) && !string.IsNullOrEmpty(CommitterEMail))
            {
                GitConfigSet("user.name", CommitterName, result);
                GitConfigSet("user.email", CommitterEMail, result);
            }
            else if (string.IsNullOrEmpty(GitConfigGet("user.name", result)) || string.IsNullOrEmpty(GitConfigGet("user.email", result)))
            {
                Log.Warning("[Git] Properties 'committerName' and 'committerEMail' are not provided. They're required to use the 'TagOnSuccess' feature.");
            }
        }

        private ProcessInfo NewProcessInfo(string args, IIntegrationResult result)
        {
            return NewProcessInfo(args, result, ProcessPriorityClass.Normal, new int[] { 0 });
        }

        private ProcessInfo NewProcessInfo(string args, IIntegrationResult result, ProcessPriorityClass priority, int[] successExitCodes)
        {
            Log.Info(string.Concat("[Git] Calling git ", args));
            var processInfo = new ProcessInfo(Executable, args,
                                                BaseWorkingDirectory(result),
                                                priority,
                                                successExitCodes)
            {
                StreamEncoding = System.Text.Encoding.UTF8,
                StandardInputContent = string.Empty
            };

            return processInfo;
        }

        #region "git commands"

        /// <summary>
        /// Get the hash of the latest commit in the remote repository.
        /// </summary>
        /// <param name="branchName">Name of the branch.</param>
        /// <param name="result">IIntegrationResult of the current build.</param>
        private string GitLogOriginHash(string branchName, IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("log");
            buffer.AddArgument(string.Concat("origin/", branchName));
            buffer.AddArgument("-1");
            buffer.AddArgument("--pretty=format:\"%H\"");
            return Execute(NewProcessInfo(buffer.ToString(), result)).StandardOutput.Trim();
        }

        /// <summary>
        /// Get the commit history including changes between <paramref name="from"/> and origin/<paramref name="branchName"/>
        /// </summary>
        /// <param name="branchName">Name of the branch.</param>
        /// <param name="from">The commit from which to start logging.</param>
        /// <param name="to">IIntegrationResult of the current build.</param>
        /// <returns>Result of the "git log" command.</returns>
        private ProcessResult GitLogHistory(string branchName, string from, IIntegrationResult to)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("log");
            buffer.AddArgument(string.Concat(from, "..origin/", branchName));
            AppendLogOptions(buffer);
            return Execute(NewProcessInfo(buffer.ToString(), to));
        }

        private ProcessResult GitLogHistory(string branchName, IIntegrationResult to)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("log");
            buffer.AddArgument(string.Concat("origin/", branchName));
            AppendLogOptions(buffer);
            return Execute(NewProcessInfo(buffer.ToString(), to));
        }

        private void AppendLogOptions(ProcessArgumentBuilder buffer)
        {
            if (MaxAmountOfModificationsToFetch > 0)
            {
                buffer.AddArgument(string.Concat("-", MaxAmountOfModificationsToFetch.ToString())); // The history may be very long. It may lead to timeout. That's why we need to limit it
            }

            buffer.AddArgument("--name-status");
            buffer.AddArgument(string.Concat("--pretty=format:", '"', historyFormat, '"'));
            buffer.AddArgument("-m"); // for getting the commits seen via merges


        }

        /// <summary>
        /// Clone a repository into a new directory with "git clone 'repository' 'working directory'".
        /// </summary>
        /// <param name="result">IIntegrationResult of the current build.</param>
        private void GitClone(IIntegrationResult result)
        {
            string wd = BaseWorkingDirectory(result);

            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("clone");
            buffer.AddArgument(Repository);
            buffer.AddArgument(wd);

            // initialize progress information
            var bpi = GetBuildProgressInformation(result);
            bpi.SignalStartRunTask(string.Concat("git ", buffer.ToString()));

            // enable Stdout monitoring
            ProcessExecutor.ProcessOutput += ProcessExecutor_ProcessOutput;

            ProcessInfo pi = NewProcessInfo(buffer.ToString(), result);
            // Use upper level of the working directory, because the
            // working directory currently does not exist and
            // will be created by "git clone". "git clone" will fail if
            // the working directory already exist.
            pi.WorkingDirectory = Path.GetDirectoryName(wd.Trim().TrimEnd(Path.DirectorySeparatorChar));
            Execute(pi);

            // remove Stdout monitoring
            ProcessExecutor.ProcessOutput -= ProcessExecutor_ProcessOutput;
        }

        /// <summary>
        /// Call "git config 'name' 'value'" to set local repository properties.
        /// </summary>
        /// <param name="name">Name of the config parameter.</param>
        /// <param name="value">Value of the config parameter.</param>
        /// <param name="result">IIntegrationResult of the current build.</param>
        private void GitConfigSet(string name, string value, IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("config");
            buffer.AddArgument(name);
            buffer.AddArgument(value);
            Execute(NewProcessInfo(buffer.ToString(), result));
        }

        /// <summary>
        /// Call "git config --get 'name'" to get the value of a local repository property.
        /// The command returns error code 1 if the key was not found and error code 2 if multiple key values were found. 
        /// </summary>
        /// <param name="name">Name of the config parameter.</param>
        /// <param name="result">IIntegrationResult of the current build.</param>
        /// <returns>Result of the "git config --get 'name'" command.</returns>
        private string GitConfigGet(string name, IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("config");
            buffer.AddArgument("--get");
            buffer.AddArgument(name);
            return
                Execute(NewProcessInfo(buffer.ToString(), result, ProcessPriorityClass.Normal, new int[] { 0, 1, 2 })).
                    StandardOutput.Trim();
        }

        /// <summary>
        /// Download objects and refs from another repository via the
        /// "git fetch origin" command.
        /// </summary>
        /// <param name="result">IIntegrationResult of the current build.</param>
        private void GitFetch(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("fetch");
            buffer.AddArgument("origin");

            // initialize progress information
            var bpi = GetBuildProgressInformation(result);
            bpi.SignalStartRunTask(string.Concat("git ", buffer.ToString()));

            // enable Stdout monitoring
            ProcessExecutor.ProcessOutput += ProcessExecutor_ProcessOutput;

            Execute(NewProcessInfo(buffer.ToString(), result));

            // remove Stdout monitoring
            ProcessExecutor.ProcessOutput -= ProcessExecutor_ProcessOutput;
        }

        /// <summary>
        /// Checkout a remote branch with the "git checkout -q -f 'origin/branchName'" command.
        /// </summary>
        /// <param name="branchName">Name of the branch to checkout.</param>
        /// <param name="result">IIntegrationResult of the current build.</param>
        private void GitCheckoutRemoteBranch(string branchName, IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("checkout");
            buffer.AddArgument("-q");
            buffer.AddArgument("-f");
            buffer.AddArgument(string.Concat("origin/", branchName));

            // initialize progress information
            var bpi = GetBuildProgressInformation(result);
            bpi.SignalStartRunTask(string.Concat("git ", buffer.ToString()));

            // enable Stdout monitoring
            ProcessExecutor.ProcessOutput += ProcessExecutor_ProcessOutput;

            Execute(NewProcessInfo(buffer.ToString(), result));

            // remove Stdout monitoring
            ProcessExecutor.ProcessOutput -= ProcessExecutor_ProcessOutput;
        }

        /// <summary>
        /// Clean the working tree with "git clean -d -f -x".
        /// </summary>
        /// <param name="result">IIntegrationResult of the current build.</param>
        private void GitClean(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("clean");
            buffer.AddArgument("-d");
            buffer.AddArgument("-f");
            buffer.AddArgument("-x");
            Execute(NewProcessInfo(buffer.ToString(), result));

            if (FetchSubmodules)
            {
                buffer = new ProcessArgumentBuilder();
                buffer.AddArgument("submodule");
                buffer.AddArgument("foreach");
                buffer.AddArgument("--recursive");
                buffer.AddArgument("\"git clean -d -f -x\"");
                Execute(NewProcessInfo(buffer.ToString(), result));
            }
        }

        /// <summary>
        /// Automatically stage files that have been modified and deleted
        /// and commit them with the "git commit --all --allow-empty -m 'message'"
        /// command.
        /// </summary>
        /// <param name="commitMessage">Commit message.</param>
        /// <param name="result">IIntegrationResult of the current build.</param>
        private void GitCommitAll(string commitMessage, IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("commit");
            buffer.AddArgument("--all");
            buffer.AddArgument("--allow-empty");
            buffer.AddArgument("-m", commitMessage);
            Execute(NewProcessInfo(buffer.ToString(), result));
        }

        /// <summary>
        /// Add all modified and all untracked files that are not ignored by .gitignore
        /// to the git index with the "git add --all" command.
        /// </summary>
        /// <param name="result">IIntegrationResult of the current build.</param>
        private void GitAddAll(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("add");
            buffer.AddArgument("--all");
            Execute(NewProcessInfo(buffer.ToString(), result));
        }

        /// <summary>
        /// Create a unsigned tag with "git tag -a -m 'message' 'tag name'".
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="tagMessage">Tag commit message.</param>
        /// <param name="result">IIntegrationResult of the current build.</param>
        private void GitCreateTag(string tagName, string tagMessage, IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("tag");
            buffer.AddArgument("-a");
            buffer.AddArgument("-m", tagMessage);
            buffer.AddArgument(tagName);
            Execute(NewProcessInfo(buffer.ToString(), result));
        }

        /// <summary>
        /// Push a specific tag with "git push origin tag 'tag name'".
        /// </summary>
        /// <param name="tagName">Naem of the tag to push.</param>
        /// <param name="result">IIntegrationResult of the current build.</param>
        private void GitPushTag(string tagName, IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("push");
            buffer.AddArgument("origin");
            buffer.AddArgument("tag");
            buffer.AddArgument(tagName);

            // initialize progress information
            var bpi = GetBuildProgressInformation(result);
            bpi.SignalStartRunTask(string.Concat("git ", buffer.ToString()));

            // enable Stdout monitoring
            ProcessExecutor.ProcessOutput += ProcessExecutor_ProcessOutput;

            Execute(NewProcessInfo(buffer.ToString(), result));

            // remove Stdout monitoring
            ProcessExecutor.ProcessOutput -= ProcessExecutor_ProcessOutput;
        }

        /// <summary>
        /// Initialize the git submodules.
        /// </summary>
        /// <param name="result">IIntegrationResult of the current build.</param>
        private void GitInitSubmodules(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("submodule");
            buffer.AddArgument("init");
            Execute(NewProcessInfo(buffer.ToString(), result));
        }

        /// <summary>
        /// Updates and fetches git submodules.
        /// </summary>
        /// <param name="result"></param>
        private void GitUpdateSubmodules(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("submodule");
            buffer.AddArgument("update");

            // initialize progress information
            var bpi = GetBuildProgressInformation(result);
            bpi.SignalStartRunTask(string.Concat("git ", buffer.ToString()));

            // enable Stdout monitoring
            ProcessExecutor.ProcessOutput += ProcessExecutor_ProcessOutput;

            Execute(NewProcessInfo(buffer.ToString(), result));

            // remove Stdout monitoring
            ProcessExecutor.ProcessOutput -= ProcessExecutor_ProcessOutput;
        }

        #endregion

        #endregion

        /// <summary>
        /// Private enum that is used to determine the action done
        /// by the CreateUpateLocalRepository() method.
        /// </summary>
        private enum RepositoryAction
        {
            Unknown = 0,
            Created = 1,
            Updated = 2
        }
    }
}
