using System.IO;
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
		private const string historyFormat = "Commit:%H%nTime:%ci%nAuthor:%an%nE-Mail:%ae%nMessage:%s%n%n%b%nChanges:";

		private readonly IFileSystem _fileSystem;
		private readonly IFileDirectoryDeleter _fileDirectoryDeleter;

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

		[ReflectorProperty("committerName", Required = false)]
		public string CommitterName;

		[ReflectorProperty("committerEMail", Required = false)]
		public string CommitterEMail;

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

			string tagName = result.Label;

			// create a tag with the current build label as name and push it.
			GitCreateTag(tagName, string.Format(TagCommitMessage, tagName), result);
			GitPushTag(tagName, result);
		}

		#region private methods

		private string BaseWorkingDirectory(IIntegrationResult result)
		{
			return Path.GetFullPath(result.BaseFromWorkingDirectory(WorkingDirectory));
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


			// check whenver this is a local git repository or just an existing folder
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
				GitConfig("user.name", CommitterName, result);
				GitConfig("user.email", CommitterEMail, result);
			}
			else
			{
				Log.Warning("[Git] Properties 'committerName' and 'committerEMail' are not provided. They're required to use the 'TagOnSuccess' feature.");
			}
		}

		private ProcessInfo NewProcessInfo(string args, IIntegrationResult result)
		{
			Log.Info(string.Concat("[Git] Calling git ", args));
			ProcessInfo processInfo = new ProcessInfo(Executable, args, BaseWorkingDirectory(result));
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

			ProcessInfo pi = NewProcessInfo(buffer.ToString(), result);
			// Use upper level of the working directory, because the
			// working directory currently does not exist and
			// will be created by "git clone". "git clone" will fail if
			// the working directory already exist.
			pi.WorkingDirectory = Path.GetDirectoryName(wd.Trim().TrimEnd(Path.DirectorySeparatorChar));
			Execute(pi);
		}

		/// <summary>
		/// Call "git config 'name' 'value'" to set local repository properties.
		/// </summary>
		/// <param name="name">Name of the config parameter.</param>
		/// <param name="value">Value of the config parameter.</param>
		/// <param name="result">IIntegrationResult of the current build.</param>
		private void GitConfig(string name, string value, IIntegrationResult result)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AddArgument("config");
			buffer.AddArgument(name);
			buffer.AddArgument(value);
			Execute(NewProcessInfo(buffer.ToString(), result));
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
			Execute(NewProcessInfo(buffer.ToString(), result));
		}

		/// <summary>
		/// Checkout a remote branch with the "git checkout -f 'origin/branchName'" command.
		/// </summary>
		/// <param name="branchName">Name of the branch to checkout.</param>
		/// <param name="result">IIntegrationResult of the current build.</param>
		private void GitCheckoutRemoteBranch(string branchName, IIntegrationResult result)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AddArgument("checkout");
			buffer.AddArgument("-f");
			buffer.AddArgument(string.Concat("origin/", branchName));
			Execute(NewProcessInfo(buffer.ToString(), result));
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
			Execute(NewProcessInfo(buffer.ToString(), result));
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
