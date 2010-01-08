using System.Diagnostics;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

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
    /// repository will be set with "git config" if both are provided.
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
    /// One the repository is initialized the "git fetch origin" command is issued to fetch the remote changes. In the next step the sha-1 hash
    /// of the specified remote branch and the local checkout is compared. If they're identical Cruise Control.NET will expect that there are
    /// no changes.
    /// </para>
    /// <para>
    /// If the 2 sha-1 hashes are different a "git log --name-status --before=... --after=..." command is issued to get a list of the new
    /// commits and their changes.
    /// </para>
    /// <para>
    /// <b>Getting the source</b>
    /// </para>
    /// <para>
    /// Once Cruise Control.NET has modifications detected and the 'autoGetSource' property is set to 'true' the "git checkout -f
    /// origin/$NameOfTheBranch" command is issued. Also the "git clean -f -d -x" command to get a clean working copy to start a new build.
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
    /// <para></para>
    /// <para></para>
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
		public bool AutoGetSource = true;

        /// <summary>
        /// The location of the Git executable. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>git</default>
        [ReflectorProperty("executable", Required = false)]
		public string Executable = "git";

        /// <summary>
        /// The url to the remote repository. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("repository", Required = true)]
		public string Repository;

        /// <summary>
        /// Remote repository branch to monitor and checkout. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>master</default>
        [ReflectorProperty("branch", Required = false)]
		public string Branch = "master";

        /// <summary>
        /// Format string for the commit message of each tag. \{0\} is the placeholder for the current build label. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>CCNet Build \{0\}</default>
        [ReflectorProperty("tagCommitMessage", Required = false)]
		public string TagCommitMessage = "CCNet Build {0}";

        /// <summary>
        /// Format string for the name of each tag. Make sure you're only using allowed characters. \{0\} is the placeholder for the current
        /// build label. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>CCNet-Build-\{0\}</default>
        [ReflectorProperty("tagNameFormat", Required = false)]
		public string TagNameFormat = "CCNet-Build-{0}";

        /// <summary>
        /// Indicates that the repository should be tagged if the build succeeds.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("tagOnSuccess", Required = false)]
		public bool TagOnSuccess;

        /// <summary>
        /// Indicates that all modifications during the build process should be committed before tagging. This requires 'tagOnSuccess ' to be
        /// set to 'true'.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("commitBuildModifications", Required = false)]
		public bool CommitBuildModifications;

        /// <summary>
        /// Indicates that files created during the build process should be committed before tagging. This requires 'commitBuildModifications'
        /// and 'tagOnSuccess ' to be set to 'true'.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("commitUntrackedFiles", Required = false)]
		public bool CommitUntrackedFiles;

        /// <summary>
        /// Used to set the "user.name" configuration setting in the local repository. Required for the 'tagOnSuccess ' feature. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("committerName", Required = false)]
		public string CommitterName;

        /// <summary>
        /// Used to set the "user.email" configuration setting in the local repository. Required for the 'tagOnSuccess ' feature. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("committerEMail", Required = false)]
		public string CommitterEMail;

        /// <summary>
        /// The directory containing the local git repository. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>Project Working Directory</default>
        [ReflectorProperty("workingDirectory", Required = false)]
		public string WorkingDirectory;

		public Git() : this(new GitHistoryParser(), new ProcessExecutor(), new SystemIoFileSystem(), new IoService()) { }

		public Git(IHistoryParser historyParser, ProcessExecutor executor, IFileSystem fileSystem, IFileDirectoryDeleter fileDirectoryDeleter)
			: base(historyParser, executor)
		{
			_fileSystem = fileSystem;
			_fileDirectoryDeleter = fileDirectoryDeleter;
		}

		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			// fetch lates changes from the remote repository
			RepositoryAction result = CreateUpateLocalRepository(to);

			// check whenever the remote hash has changed after a "git fetch" command
			string originHeadHash = GitLogOriginHash(Branch, to);
			if (result == RepositoryAction.Updated && (originHeadHash == GitLogLocalHash(to)))
			{
				Log.Debug(string.Concat("[Git] Local and origin hash of branch '", Branch,
										"' matches, no modifications found. Current hash is '", originHeadHash, "'"));
				return new Modification[0];
			}

			// parse git log history
			return ParseModifications(GitLogHistory(Branch, from, to), from.StartTime, to.StartTime);
		}

		public override void GetSource(IIntegrationResult result)
		{
			if (!AutoGetSource)
				return;

			// checkout remote branch
			GitCheckoutRemoteBranch(Branch, result);

			// clean up the local working copy
			GitClean(result);
		}

		public override void LabelSourceControl(IIntegrationResult result)
		{
			if (!TagOnSuccess || result.Failed)
				return;

			string tagName = string.Format(TagNameFormat, result.Label);
			string commitMessage = string.Format(TagCommitMessage, result.Label);

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
		    return NewProcessInfo(args, result, ProcessPriorityClass.Normal, new int[] {0});
		}

        private ProcessInfo NewProcessInfo(string args, IIntegrationResult result, ProcessPriorityClass priority, int[] successExitCodes)
        {
            Log.Info(string.Concat("[Git] Calling git ", args));
            var processInfo = new ProcessInfo(Executable, args, BaseWorkingDirectory(result), priority,
                                                      successExitCodes);
            //processInfo.StreamEncoding = Encoding.UTF8;
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
			buffer.AddArgument("--date-order");
			buffer.AddArgument("-1");
			buffer.AddArgument("--pretty=format:\"%H\"");
			return Execute(NewProcessInfo(buffer.ToString(), result)).StandardOutput.Trim();
		}

		/// <summary>
		/// Get the hash of the latest commit in the local repository
		/// </summary>
		/// <param name="result">IIntegrationResult of the current build.</param>
		private string GitLogLocalHash(IIntegrationResult result)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AddArgument("log");
			buffer.AddArgument("--date-order");
			buffer.AddArgument("-1");
			buffer.AddArgument("--pretty=format:\"%H\"");

			string hash = null;
			try
			{
				hash = Execute(NewProcessInfo(buffer.ToString(), result)).StandardOutput.Trim();
			}
			catch (CruiseControlException ex)
			{
				if (!ex.Message.Contains("fatal: bad default revision 'HEAD'"))
					throw;
			}
			return hash;
		}

		/// <summary>
		/// Get the commit history including changes in date order in the provided upper and lower time limit.
		/// </summary>
		/// <param name="branchName">Name of the branch.</param>
		/// <param name="from">IIntegrationResult of the last build.</param>
		/// <param name="to">IIntegrationResult of the current build.</param>
		/// <returns>Result of the "git log" command.</returns>
		private ProcessResult GitLogHistory(string branchName, IIntegrationResult from, IIntegrationResult to)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AddArgument("log");
			buffer.AddArgument(string.Concat("origin/", branchName));
			buffer.AddArgument("--date-order");
			buffer.AddArgument("--name-status");
			buffer.AddArgument(string.Concat("--after=", from.StartTime.ToUniversalTime().ToString("R")));
			buffer.AddArgument(string.Concat("--before=", to.StartTime.ToUniversalTime().ToString("R")));
			buffer.AddArgument(string.Concat("--pretty=format:", '"', historyFormat, '"'));

			return Execute(NewProcessInfo(buffer.ToString(), to));
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
		        Execute(NewProcessInfo(buffer.ToString(), result, ProcessPriorityClass.Normal, new int[] {0, 1, 2})).
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
