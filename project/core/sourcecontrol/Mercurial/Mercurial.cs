namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Mercurial
{
	using Exortech.NetReflector;
	using System;
	using System.Diagnostics;
	using System.Globalization;
	using System.IO;
	using System.Text;
	using ThoughtWorks.CruiseControl.Core.Util;

	/// <summary>
	/// Provides support for Mercurial repositories. Checking for changes, checking out or updating sources,
	/// committing build modifications and untracked files, tagging, pushing to the remote repository,
	/// reverting modifications and purging untracked and ignored files are supported.
	/// </summary>
	/// <title> Mercurial Source Control Block </title>
	/// <version>1.5</version>
	/// <example>
	/// <code>
	/// &lt;sourcecontrol type="hg"&gt;
	/// &lt;workingDirectory&gt;c:\dev\ccnet\myproject&lt;/workingDirectory&gt;
	/// &lt;timeout&gt;60000&lt;/timeout&gt;
	/// &lt;repo&gt;http://hg.mycompany.com/hgwebdir.cgi/myproject/&lt;/repo&gt;
	/// &lt;branch&gt;trunk&lt;/branch&gt;
	/// &lt;multipleHeadsFail&gt;true&lt;/multipleHeadsFail&gt;
	/// &lt;tagOnSuccess&gt;true&lt;/tagOnSuccess&gt;
	/// &lt;commitModifications&gt;true&lt;/commitModifications&gt;
	/// &lt;commitUntracked&gt;true&lt;/commitUntracked&gt;
	/// &lt;purgeModifications&gt;true&lt;/purgeModifications&gt;
	/// &lt;revertModifications&gt;true&lt;/revertModifications&gt;
	/// &lt;tagCommitMessage&gt;Tagging CC.NET build {0}&lt;/tagCommitMessage&gt;
	/// &lt;modificationsCommitMessage&gt;Modifications of CC.NET build {0}&lt;/modificationsCommitMessage&gt;
	/// &lt;tagNameFormat&gt;ccnet_build_{0}&lt;/tagNameFormat&gt;
	/// &lt;committerName&gt;CruiseControl.NET&lt;/committerName&gt;
	/// &lt;/sourcecontrol&gt;
	/// </code>
	/// </example>
	/// <key name="type">
	/// <description>The type of source control block.</description>
	/// <value>hg</value>
	/// </key>
	/// <remarks>
	/// <para>
	/// You need to make sure your hg client settings are such that all authentication is automated.
	/// Typically you can do this by using anonymous access or appropriate SSH setups if using hg over SSH.
	/// </para>
	/// <para>
	/// You can link the modifications detected by CruiseControl.NET to the appropriate hgweb page by
	/// adding the following additional configuration information to the Mercurial source control section
	/// by using the <link>Mercurial Issue Tracker URL Builder</link>.
	/// </para>
	/// <para>
	/// External contributors:
	/// Bill Barry (initial version),
	/// Aaron Jensen (new history parser)
	/// </para>
	/// </remarks>
	[ReflectorType("hg")]
	public class Mercurial : ProcessSourceControl
	{
		#region Public Constant Fields

		/// <summary>
		/// The default executable of mercurial.
		/// </summary>
		/// <remarks>Usually just "hg"</remarks>
		public const string DefaultExecutable = "hg";

		/// <summary>
		/// The default branch of mercurial repositories.
		/// </summary>
		/// <remarks>Usually "default".</remarks>
		public const string DefaultBranch = "default";

		/// <summary>
		/// The default committer name for commit and tag operations that are done during integration.
		/// </summary>
		/// <remarks></remarks>
		public const string DefaultCommitterName = "CruiseControl.NET";

		/// <summary>
		/// The default format string to build the log message for commits of build modifications.
		/// </summary>
		/// <remarks></remarks>
		public const string DefaultModificationsCommitMessage = "Modifications of CC.NET build {0}";

		/// <summary>
		/// The format string to build the tag (label) names.
		/// </summary>
		/// <remarks></remarks>
		public const string DefaultTagNameFormat = "ccnet_build_{0}";

		/// <summary>
		/// The format string to build the log message for tag (label) operations.
		/// </summary>
		/// <remarks></remarks>
		public const string DefaultTagCommitMessage = "Tagging CC.NET build {0}";

		#endregion

		#region Private Member Fields

		/// <summary>
		/// The _fileSystem is used to create working directories.
		/// </summary>
		/// <remarks>Initialized with an SystemIoFileSystem instance.</remarks>
		private readonly IFileSystem _fileSystem;

		/// <summary>
		/// The _fileDirectoryDeleter is used to delete suspect working directories.
		/// </summary>
		/// <remarks>Initialized with an IoSystem instance.</remarks>
		private readonly IFileDirectoryDeleter _fileDirectoryDeleter;

		/// <summary>
		/// The _buildProgressInformation is a reference to the current integration results BuildProgressInformation.
		/// </summary>
		/// <remarks>Use GetBuildProgressInfomation(result) to initialize and get the reference.</remarks>
		private BuildProgressInformation _buildProgressInformation;

		#endregion

		#region NetReflector Configuration Properties

		/// <summary>
		/// The location of the hg executable.
		/// </summary>
		/// <version>1.5</version>
		/// <default>hg</default>
		[ReflectorProperty("executable", Required = false)]
		public string Executable { get; set; }

		/// <summary>
		/// The url for your repository (e.g., http://hgserver/myproject/).
		/// </summary>
		/// <version>1.5</version>
		/// <default>None</default>
		/// <remarks>This ReflectorProperty should really be named repository but is currently kept as repo for backwards compatibility</remarks>
		[ReflectorProperty("repo", Required = false)]
		public string Repository { get; set; }

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
		/// User name used for commits.
		/// </summary>
		/// <version>1.6</version>
		/// <default>CruiseControl.NET</default>
		[ReflectorProperty("committerName", Required = false)]
		public string CommitterName { get; set; }

		/// <summary>
		/// Whether to commit build modifications.
		/// </summary>
		/// <version>1.6</version>
		/// <default>false</default>
		[ReflectorProperty("commitModifications", Required = false)]
		public bool CommitModifications { get; set; }

		/// <summary>
		/// Whether to commit untracked and removed files along with build modifications.
		/// </summary>
		/// <version>1.6</version>
		/// <default>false</default>
		[ReflectorProperty("commitUntracked", Required = false)]
		public bool CommitUntracked { get; set; }

		/// <summary>
		/// Log message used when committing build modifications.
		/// </summary>
		/// <version>1.6</version>
		/// <default>Modifications of CC.NET build \{0\}</default>
		[ReflectorProperty("modificationsCommitMessage", Required = false)]
		public string ModificationsCommitMessage { get; set; }

		/// <summary>
		/// Whether the source control operation should fail if multiple heads exist in the repository.
		/// </summary>
		/// <version>1.5</version>
		/// <default>false</default>
		[ReflectorProperty("multipleHeadsFail", Required = false)]
		public bool MultipleHeadsFail { get; set; }

		/// <summary>
		/// Indicates that the repository should be tagged if the build succeeds.
		/// </summary>
		/// <version>1.5</version>
		/// <default>false</default>
		[ReflectorProperty("tagOnSuccess", Required = false)]
		public bool TagOnSuccess { get; set; }

		/// <summary>
		/// String format for tags in your repository.
		/// </summary>
		/// <version>1.6</version>
		/// <default>ccnet_build_\{0\}</default>
		[ReflectorProperty("tagNameFormat", Required = false)]
		public string TagNameFormat { get; set; }

		/// <summary>
		/// Log message format to be used for the tag commits.
		/// </summary>
		/// <version>1.5</version>
		/// <default>Tagging CC.NET build \{0\}</default>
		[ReflectorProperty("tagCommitMessage", Required = false)]
		public string TagCommitMessage { get; set; }

		/// <summary>
		/// Whether to purge untracked and ignored files before building.
		/// </summary>
		/// <version>1.6</version>
		/// <default>false</default>
		/// <remarks>The hg purge extension is activated and used for this feature.</remarks>
		[ReflectorProperty("purgeModifications", Required = false)]
		public bool PurgeModifications { get; set; }

		/// <summary>
		/// Whether to push modifications to the remote repository.
		/// </summary>
		/// <version>1.6</version>
		/// <default>false</default>
		[ReflectorProperty("pushModifications", Required = false)]
		public bool PushModifications { get; set; }

		/// <summary>
		/// Whether to revert modifications in tracked files before building.
		/// </summary>
		/// <version>1.6</version>
		/// <default>false</default>
		[ReflectorProperty("revertModifications", Required = false)]
		public bool RevertModifications { get; set; }

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

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Mercurial"/> class.
		/// </summary>
		public Mercurial()
			: this(new MercurialHistoryParser(), new ProcessExecutor(), new SystemIoFileSystem(), new IoService())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Mercurial"/> class.
		/// </summary>
		/// <param name="historyParser">The history parser.</param>
		/// <param name="executor">The executor.</param>
		/// <param name="fileSystem">The file system.</param>
		/// <param name="fileDirectoryDeleter">The file directory deleter.</param>
		public Mercurial(IHistoryParser historyParser, ProcessExecutor executor, IFileSystem fileSystem, IFileDirectoryDeleter fileDirectoryDeleter)
			: base(historyParser, executor)
		{
			Executable = DefaultExecutable;
			AutoGetSource = true;
			// Do not initialize the branch because the default branch does not exist before the first commit
			//Branch = DefaultBranch;
			CommitterName = DefaultCommitterName;
			CommitModifications = false;
			CommitUntracked = false;
			ModificationsCommitMessage = DefaultModificationsCommitMessage;
			MultipleHeadsFail = false;
			TagOnSuccess = false;
			TagNameFormat = DefaultTagNameFormat;
			TagCommitMessage = DefaultTagCommitMessage;
			PurgeModifications = false;
			PushModifications = false;
			RevertModifications = false;

			_fileSystem = fileSystem;
			_fileDirectoryDeleter = fileDirectoryDeleter;
		}

		#endregion

		#region ProcessSourceControl Members

		/// <summary>
		/// Gets the modifications.
		/// </summary>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		/// <returns>List of modifications since the last run.</returns>
		/// <remarks></remarks>
		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			RepositoryStatus result = HgCreateLocalRepository(to);

			if(result == RepositoryStatus.Created)
			{
				Log.Debug("[Mercurial] new repository created.");
				return new Modification[0];
			}

			if(!string.IsNullOrEmpty(Repository))
			{
				HgPull(to);
			}

			int parent = GetSmallestParentId(to);
			int tip = GetTipId(to);

			if (result == RepositoryStatus.AlreadyExists && (parent == tip))
			{
				Log.Debug("[Mercurial] no new changesets detected.");
				return new Modification[0];
			}

			Modification[] modifications = ParseModifications(HgLog(to, parent + 1, tip), from.StartTime, to.StartTime);

			if(UrlBuilder != null)
			{
				UrlBuilder.SetupModification(modifications);
			}

			return modifications;
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

			if(MultipleHeadsFail)
			{
				CheckMultipleHeads(result);
			}
			
			if(RevertModifications)
			{
				HgRevert(result);
			}

			if(PurgeModifications)
			{
				HgPurge(result);
			}

			HgUpdate(result);

			// TODO: update subrepos here?
		}

		/// <summary>
		/// Labels the source control.
		/// </summary>
		/// <param name="result">The result.</param>
		/// <remarks></remarks>
		public override void LabelSourceControl(IIntegrationResult result)
		{
			if (result.Failed)
				return;

			if(CommitModifications)
			{
				string modificationsCommitMessage = string.Format(CultureInfo.CurrentCulture, ModificationsCommitMessage, result.Label);
				HgCommit(CommitUntracked, modificationsCommitMessage, result);
			}

			if(TagOnSuccess)
			{
				string tagName = string.Format(CultureInfo.CurrentCulture, TagNameFormat, result.Label);
				string tagCommitMessage = string.Format(CultureInfo.CurrentCulture, TagCommitMessage, result.Label);
				HgTag(tagName, tagCommitMessage, result);
			}

			if(!string.IsNullOrEmpty(Repository) && (CommitModifications || TagOnSuccess) && PushModifications)
			{
				HgPush(result);
			}
		}

		#endregion

		#region Hg Commands

		/// <summary>
		/// Creates and configures the local repository and initally pulls a set of changes to start with if a remote repository is configured.
		/// </summary>
		/// <param name="result">The integration result.</param>
		/// <returns>The repository status (newly created, already exists or unknown).</returns>
		/// <remarks>The file system and file directory creator are used to delete and create the directory holding the repository.</remarks>
		private RepositoryStatus HgCreateLocalRepository(IIntegrationResult result)
		{
			string workingDirectory = BaseWorkingDirectory(result);
			string hgRepositoryDirectory = Path.Combine(workingDirectory, ".hg");

			if (!_fileSystem.DirectoryExists(workingDirectory))
			{
				Log.Debug(string.Concat("[Mercurial] Working directory '", workingDirectory, "' does not exist."));

				HgInit(result);
				HgConfigureRepository(result);

				if(!string.IsNullOrEmpty(Repository))
				{
					HgPull(result);
				}

				return RepositoryStatus.Created;
			}


			if (!_fileSystem.DirectoryExists(hgRepositoryDirectory))
			{
				Log.Debug(string.Concat("[Mercurial] Working directory '", workingDirectory,
				                        "' already exists, but it is not a hg repository. Try deleting it and starting again."));

				_fileDirectoryDeleter.DeleteIncludingReadOnlyObjects(workingDirectory);
				return HgCreateLocalRepository(result);
			}

			return RepositoryStatus.AlreadyExists;
		}

		/// <summary>
		/// Initialize the working directory with the hg init command
		/// </summary>
		/// <param name="result">The integration result.</param>
		/// <returns>The process result.</returns>
		/// <remarks>Uses the file system to ensure the working directory exists before calling hg init.</remarks>
		private ProcessResult HgInit(IIntegrationResult result)
		{
			string wd = BaseWorkingDirectory(result);
			_fileSystem.EnsureFolderExists(wd);

			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AddArgument("init");
			buffer.AddArgument(wd);

			var bpi = GetBuildProgressInformation(result);
			bpi.SignalStartRunTask(string.Concat("hg ", buffer.ToString()));

			ProcessExecutor.ProcessOutput += ProcessExecutor_ProcessOutput;

			ProcessInfo pi = NewProcessInfo(buffer.ToString(), result);
			pi.WorkingDirectory = Path.GetDirectoryName(wd.Trim().TrimEnd(Path.DirectorySeparatorChar));
			ProcessResult processResult = Execute(pi);

			ProcessExecutor.ProcessOutput -= ProcessExecutor_ProcessOutput;

			return processResult;
		}

		/// <summary>
		/// Creates a local hg configuration file (.hg/hgrc) and an output template (.hg/ccnet.template) in the repository
		/// </summary>
		/// <param name="result">The integration result.</param>
		/// <returns>The process result.</returns>
		/// <remarks>Uses the file system to ensure the working directory exists before calling hg init.</remarks>
		private void HgConfigureRepository(IIntegrationResult result)
		{
			string workingDirectory = BaseWorkingDirectory(result);
			string hgRepositoryDirectory = Path.Combine(workingDirectory, ".hg");
			_fileSystem.EnsureFolderExists(hgRepositoryDirectory);

			TextWriter tw = new StreamWriter(Path.Combine(hgRepositoryDirectory, "hgrc"));
			tw.WriteLine("[extensions]");
			tw.WriteLine("hgext.purge = ");
			tw.Close();
		}

		/// <summary>
		/// Gets list of parents for the current state of the working directory, which is usually the revision that has been updated to on the last run.
		/// </summary>
		/// <param name="result">The integration result.</param>
		/// <returns>The process result.</returns>
		/// <remarks>Uses the output template to render the process output as XML.</remarks>
		private ProcessResult HgParents(IIntegrationResult result)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AddArgument("parents");
			buffer.AddArgument("--template", "{rev}:");

			var bpi = GetBuildProgressInformation(result);
			bpi.SignalStartRunTask(string.Concat("hg ", buffer.ToString()));

			ProcessExecutor.ProcessOutput += ProcessExecutor_ProcessOutput;

			ProcessResult processResult = Execute(NewProcessInfo(buffer.ToString(), result));

			ProcessExecutor.ProcessOutput -= ProcessExecutor_ProcessOutput;

			return processResult;
		}

		/// <summary>
		/// Gets list of heads for the local repository, which is usually a list of revisions including the tip and branch head revisions.
		/// </summary>
		/// <param name="result">The integration result.</param>
		/// <returns>The process result.</returns>
		/// <remarks>Uses the output template to render the process output as XML.</remarks>
		private ProcessResult HgHeads(IIntegrationResult result)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AddArgument("heads");
			if(string.IsNullOrEmpty(Branch))
			{
				buffer.AddArgument(".");
			}
			else
			{
				buffer.AddArgument(Branch);
			}
			buffer.AddArgument("--template", "{rev}:");

			var bpi = GetBuildProgressInformation(result);
			bpi.SignalStartRunTask(string.Concat("hg ", buffer.ToString()));

			ProcessExecutor.ProcessOutput += ProcessExecutor_ProcessOutput;

			ProcessResult processResult = Execute(NewProcessInfo(buffer.ToString(), result, ProcessPriorityClass.Normal, new int[] { 0, 1}));

			ProcessExecutor.ProcessOutput -= ProcessExecutor_ProcessOutput;

			return processResult;
		}

		/// <summary>
		/// Gets the branch head for the current branch or the tip if no branch is configured.
		/// </summary>
		/// <param name="result">The integration result.</param>
		/// <returns>The process result.</returns>
		/// <remarks>Uses the output template to render the process output as XML.</remarks>
		private ProcessResult HgTip(IIntegrationResult result)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AddArgument("log");

			if(!string.IsNullOrEmpty(Branch))
			{
				buffer.AddArgument("-b", Branch);
				buffer.AddArgument("-r", Branch);
			}
			else
			{
				buffer.AddArgument("-r", "tip");
			}

			buffer.AddArgument("--template", "{rev}");

			var bpi = GetBuildProgressInformation(result);
			bpi.SignalStartRunTask(string.Concat("hg ", buffer.ToString()));

			ProcessExecutor.ProcessOutput += ProcessExecutor_ProcessOutput;

			ProcessResult processResult = Execute(NewProcessInfo(buffer.ToString(), result));

			ProcessExecutor.ProcessOutput -= ProcessExecutor_ProcessOutput;

			return processResult;
		}

		/// <summary>
		/// Get the list of changesets within a range of revisions (from, to).
		/// </summary>
		/// <param name="to">The integration result.</param>
		/// <param name="fromId">The start of the range of revisions</param>
		/// <param name="toId">The end of the range of revisions.</param>
		/// <returns>The process result.</returns>
		/// <remarks>Uses the output template to render the process output as XML.</remarks>
		private ProcessResult HgLog(IIntegrationResult to, int fromId, int toId)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AddArgument("log");

			if(!string.IsNullOrEmpty(Branch))
			{
				buffer.AddArgument("-b", Branch);
			}

			buffer.AddArgument("-r", string.Format("{0}:{1}", fromId, toId));
			buffer.AddArgument("--style", "xml");
			buffer.AddArgument("-v");

			var bpi = GetBuildProgressInformation(to);
			bpi.SignalStartRunTask(string.Concat("hg ", buffer.ToString()));

			ProcessExecutor.ProcessOutput += ProcessExecutor_ProcessOutput;

			ProcessResult processResult = Execute(NewProcessInfo(buffer.ToString(), to));

			ProcessExecutor.ProcessOutput -= ProcessExecutor_ProcessOutput;

			return processResult;
		}

		/// <summary>
		/// Pulls the latest changesets from the remote repository into the local repository without updating the files in the working directory.
		/// </summary>
		/// <param name="result">The integration result.</param>
		/// <returns>The process result.</returns>
		/// <remarks></remarks>
		private ProcessResult HgPull(IIntegrationResult result)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AddArgument("pull");

			if(!string.IsNullOrEmpty(Branch))
			{
				buffer.AddArgument("-b", Branch);
			}

			buffer.AddArgument(Repository);

			var bpi = GetBuildProgressInformation(result);
			bpi.SignalStartRunTask(string.Concat("hg ", buffer.ToString()));

			ProcessExecutor.ProcessOutput += ProcessExecutor_ProcessOutput;

			ProcessResult processResult = Execute(NewProcessInfo(buffer.ToString(), result));

			ProcessExecutor.ProcessOutput -= ProcessExecutor_ProcessOutput;

			return processResult;
		}

		/// <summary>
		/// Updates the files in the working directory to a given revision, which is usually the tip or the branch head.
		/// </summary>
		/// <param name="result">The integration result.</param>
		/// <returns>The process result.</returns>
		/// <remarks></remarks>
		private ProcessResult HgUpdate(IIntegrationResult result)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AddArgument("update");

			if(!string.IsNullOrEmpty(Branch))
			{
				buffer.AddArgument(Branch);
			}

			var bpi = GetBuildProgressInformation(result);
			bpi.SignalStartRunTask(string.Concat("hg ", buffer.ToString()));

			ProcessExecutor.ProcessOutput += ProcessExecutor_ProcessOutput;

			ProcessResult processResult = Execute(NewProcessInfo(buffer.ToString(), result));

			ProcessExecutor.ProcessOutput -= ProcessExecutor_ProcessOutput;

			return processResult;
		}

		/// <summary>
		/// Deletes all untracked and ignored files, but leaves modified files alone.
		/// </summary>
		/// <param name="result">The integration result.</param>
		/// <returns>The process result.</returns>
		/// <remarks>Uses the all flag (--all) to also delete ignored files (see .hgignore).</remarks>
		private ProcessResult HgPurge(IIntegrationResult result)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AddArgument("purge");
			buffer.AddArgument("--all");

			var bpi = GetBuildProgressInformation(result);
			bpi.SignalStartRunTask(string.Concat("hg ", buffer.ToString()));

			ProcessExecutor.ProcessOutput += ProcessExecutor_ProcessOutput;

			ProcessResult processResult = Execute(NewProcessInfo(buffer.ToString(), result));

			ProcessExecutor.ProcessOutput -= ProcessExecutor_ProcessOutput;

			return processResult;
		}

		/// <summary>
		/// Commits modifications that have been made during the integration and if configured also newly created files to the local repository.
		/// </summary>
		/// <param name="addRemove">Bool to indicate if the add-remove option should be used.</param>
		/// <param name="commitMessage">The commit message to be used.</param>
		/// <param name="result">The integration result.</param>
		/// <returns>The process result.</returns>
		/// <remarks>Optionally uses the add-remove flag (-A) to allow to commit untracked files and file deletions.</remarks>
		private ProcessResult HgCommit(bool addRemove, string commitMessage, IIntegrationResult result)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AddArgument("commit");

			if(addRemove)
			{
				buffer.AddArgument("-A");
			}

			if(!string.IsNullOrEmpty(CommitterName))
			{
				buffer.AddArgument("-u", CommitterName);
			}

			buffer.AddArgument("-m", commitMessage);

			var bpi = GetBuildProgressInformation(result);
			bpi.SignalStartRunTask(string.Concat("hg ", buffer.ToString()));

			ProcessExecutor.ProcessOutput += ProcessExecutor_ProcessOutput;

			ProcessResult processResult = Execute(NewProcessInfo(buffer.ToString(), result, ProcessPriorityClass.Normal, new int[] {0, 1}));

			ProcessExecutor.ProcessOutput -= ProcessExecutor_ProcessOutput;

			return processResult;
		}

		/// <summary>
		/// Creates a named tag (label) for the currently used revision in the local repository.
		/// </summary>
		/// <param name="tagName">The name of the tag.</param>
		/// <param name="tagMessage">The log message to be used.</param>
		/// <param name="result">The integration result.</param>
		/// <returns>The process result.</returns>
		/// <remarks>Uses the force flag (-f) to even create the tag if a tag with the same name already exists.</remarks>
		private ProcessResult HgTag(string tagName, string tagMessage, IIntegrationResult result)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AddArgument("tag");
			buffer.AddArgument("-m", tagMessage);

			if(!string.IsNullOrEmpty(CommitterName))
			{
				buffer.AddArgument("-u", CommitterName);
			}

			buffer.AddArgument("-f");
			buffer.AddArgument(tagName);

			var bpi = GetBuildProgressInformation(result);
			bpi.SignalStartRunTask(string.Concat("hg ", buffer.ToString()));

			ProcessExecutor.ProcessOutput += ProcessExecutor_ProcessOutput;

			ProcessResult processResult = Execute(NewProcessInfo(buffer.ToString(), result));

			ProcessExecutor.ProcessOutput -= ProcessExecutor_ProcessOutput;

			return processResult;
		}

		/// <summary>
		/// Pushes the committed changesets and created tags to the configured remote repository.
		/// </summary>
		/// <param name="result">The integration result.</param>
		/// <returns>The process result.</returns>
		/// <remarks>Uses the force flag (-f) to push even if new remote heads are created.</remarks>
		private ProcessResult HgPush(IIntegrationResult result)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AddArgument("push");

			if(!string.IsNullOrEmpty(Branch))
			{
				buffer.AddArgument("-b", Branch);
			}

			buffer.AddArgument("-f");
			buffer.AddArgument(Repository);

			var bpi = GetBuildProgressInformation(result);
			bpi.SignalStartRunTask(string.Concat("hg ", buffer.ToString()));

			ProcessExecutor.ProcessOutput += ProcessExecutor_ProcessOutput;

			ProcessResult processResult = Execute(NewProcessInfo(buffer.ToString(), result));

			ProcessExecutor.ProcessOutput -= ProcessExecutor_ProcessOutput;

			return processResult;
		}

		/// <summary>
		/// Recursively reverts modifications in tracked files without keeping backups.
		/// </summary>
		/// <param name="result">The integration result.</param>
		/// <returns>The process result.</returns>
		/// <remarks></remarks>
		private ProcessResult HgRevert(IIntegrationResult result)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AddArgument("revert");
			buffer.AddArgument("--all");
			buffer.AddArgument("--no-backup");

			var bpi = GetBuildProgressInformation(result);
			bpi.SignalStartRunTask(string.Concat("hg ", buffer.ToString()));

			ProcessExecutor.ProcessOutput += ProcessExecutor_ProcessOutput;

			ProcessResult processResult = Execute(NewProcessInfo(buffer.ToString(), result));

			ProcessExecutor.ProcessOutput -= ProcessExecutor_ProcessOutput;

			return processResult;
		}

		#endregion

		#region Helper Methods / Types

		/// <summary>
		/// Gets the tip or branch head revision of the local repository.
		/// </summary>
		/// <param name="to">The integration result.</param>
		/// <returns>The tip or branch head revision.</returns>
		/// <remarks></remarks>
		private int GetTipId(IIntegrationResult to)
		{
			string tip = HgTip(to).StandardOutput.Trim();
			int t;

			if(!int.TryParse(tip, out t))
			{
				t = -1;
			}

			return t;
		}

		/// <summary>
		/// Gets the smallest parent revision of the files in the working directory.
		/// </summary>
		/// <param name="to">The integration result.</param>
		/// <returns>The smallest revision from the list of parents.</returns>
		/// <remarks></remarks>
		private int GetSmallestParentId(IIntegrationResult to)
		{
			string parents = HgParents(to).StandardOutput.Trim().TrimEnd(':');

			if(string.IsNullOrEmpty(parents))
			{
				return -1;
			}

			string[] splitParents = parents.Split(':');

			int parent = -1;
			foreach(string p in splitParents)
			{
				int pp = int.Parse(p);

				if((parent == -1) || (pp < parent))
				{
					parent = pp;
				}
			}

			return parent;
		}

		/// <summary>
		/// Throws an exception if the repository contains multiple head revisions.
		/// </summary>
		/// <param name="to">The integration result.</param>
		/// <remarks></remarks>
		private void CheckMultipleHeads(IIntegrationResult to)
		{
			string heads = HgHeads(to).StandardOutput.Trim().TrimEnd(':');

			if(heads.Contains(":"))
			{
				throw new MultipleHeadsFoundException();
			}
		}

		/// <summary>
		/// Gets the parent directory of the current working directory.
		/// </summary>
		/// <param name="result">The integration result.</param>
		/// <returns>The parent directory.</returns>
		/// <remarks></remarks>
		private string BaseWorkingDirectory(IIntegrationResult result)
		{
			return Path.GetFullPath(result.BaseFromWorkingDirectory(WorkingDirectory));
		}

		/// <summary>
		/// Creates a new ProcessInfo object with the given arguments, normal priority and exit code 0 for indicating success.
		/// </summary>
		/// <param name="args">The command line arguments.</param>
		/// <param name="result">The integration result.</param>
		/// <returns>The process information.</returns>
		/// <remarks></remarks>
		private ProcessInfo NewProcessInfo(string args, IIntegrationResult result)
		{
			return NewProcessInfo(args, result, ProcessPriorityClass.Normal, new int[] {0});
		}

		/// <summary>
		/// Creates a new ProcessInfo object with the given arguments, given priority and given list of exit codes for indicating success.
		/// </summary>
		/// <param name="args">The command line arguments.</param>
		/// <param name="result">The integration result.</param>
		/// <param name="priority">The process priority class.</param>
		/// <param name="successExitCodes">The list of exit codes that indicate success.</param>
		/// <returns>The process information.</returns>
		/// <remarks></remarks>
		private ProcessInfo NewProcessInfo(string args, IIntegrationResult result, ProcessPriorityClass priority, int[] successExitCodes)
		{
			Log.Info(string.Concat("[Mercurial] Calling hg ", args));
			var processInfo = new ProcessInfo(Executable, args, BaseWorkingDirectory(result), priority,
			                                  successExitCodes);
			processInfo.StreamEncoding = Encoding.UTF8;
			return processInfo;
		}

		/// <summary>
		/// Initializes the local reference to the integration results BuildProgressInformation and retzurns it.
		/// </summary>
		/// <param name="result">The integration result.</param>
		/// <returns>The build progress information.</returns>
		/// <remarks></remarks>
		private BuildProgressInformation GetBuildProgressInformation(IIntegrationResult result)
		{
			if (_buildProgressInformation == null)
				_buildProgressInformation = result.BuildProgressInformation;

			return _buildProgressInformation;
		}

		/// <summary>
		/// Event Handler for the ProcessOutput event of the ProcessExecutor.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="args">The event arguments.</param>
		/// <remarks></remarks>
		private void ProcessExecutor_ProcessOutput(object sender, ProcessOutputEventArgs args)
		{
			if (_buildProgressInformation == null)
				return;

			if (args.OutputType == ProcessOutputType.ErrorOutput)
				return;

			_buildProgressInformation.AddTaskInformation(args.Data);
		}

		#endregion
	}
}
