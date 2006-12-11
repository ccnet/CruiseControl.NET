using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("cvs")]
	public class Cvs : ProcessSourceControl
	{
		public const string DefaultCvsExecutable = "cvs.exe";
		public const string COMMAND_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss 'GMT'";
		private readonly IFileSystem fileSystem;
		private readonly CvsHistoryCommandParser historyCommandParser;

		public Cvs() : this(new CvsHistoryParser(), new ProcessExecutor(), new CvsHistoryCommandParser(), new SystemIoFileSystem())
		{
		}

		public Cvs(IHistoryParser parser, ProcessExecutor executor, CvsHistoryCommandParser historyCommandParser, IFileSystem fileSystem)
			: base(parser, executor)
		{
			this.fileSystem = fileSystem;
			this.historyCommandParser = historyCommandParser;
		}

		[ReflectorProperty("executable")]
		public string Executable = DefaultCvsExecutable;

		[ReflectorProperty("cvsroot")]
		public string CvsRoot = string.Empty;

		[ReflectorProperty("module")]
		public string Module;

		[ReflectorProperty("workingDirectory", Required=false)]
		public string WorkingDirectory = string.Empty;

		[ReflectorProperty("labelOnSuccess", Required=false)]
		public bool LabelOnSuccess = false;

		[ReflectorProperty("restrictLogins", Required=false)]
		public string RestrictLogins = string.Empty;

		[ReflectorProperty("webUrlBuilder", InstanceTypeKey="type", Required=false)]
		public IModificationUrlBuilder UrlBuilder = new NullUrlBuilder();

		[ReflectorProperty("autoGetSource", Required = false)]
		public bool AutoGetSource = false;

		[ReflectorProperty("cleanCopy", Required = false)]
		public bool CleanCopy = true;

		[ReflectorProperty("useHistory", Required = false)]
		public bool UseHistory = false;

		[ReflectorProperty("branch", Required=false)]
		public string Branch = string.Empty;

		[ReflectorProperty("tagPrefix", Required=false)]
		public string TagPrefix = "ver-";

		public string FormatCommandDate(DateTime date)
		{
			return date.ToUniversalTime().ToString(COMMAND_DATE_FORMAT, CultureInfo.InvariantCulture);
		}

		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			Modification[] modifications;
			if (UseHistory)
			{
				modifications = GetModificationsUsingHistory(from, to);
			}
			else
			{
				modifications = GetModifications(WorkingDirectory, from, to);
			}

			StripRepositoryRootFromModificationFolderNames(modifications);
			UrlBuilder.SetupModification(modifications);
			return modifications;
		}

		public override void LabelSourceControl(IIntegrationResult result)
		{
			if (LabelOnSuccess && result.Succeeded)
			{
				Execute(NewLabelProcessInfo(result));
			}
		}

		public override void GetSource(IIntegrationResult result)
		{
			if (AutoGetSource && !UseHistory)
			{
				if (DoesCvsDirectoryExist(result))
				{
					UpdateSource(result, null);
				}
				else
				{
					CheckoutSource(result);
				}
			}
		}

		private bool DoesCvsDirectoryExist(IIntegrationResult result)
		{
			string cvsDirectory = Path.Combine(result.BaseFromWorkingDirectory(WorkingDirectory), "CVS");
			return fileSystem.DirectoryExists(cvsDirectory);
		}

		private void CheckoutSource(IIntegrationResult result)
		{
			if (StringUtil.IsBlank(CvsRoot))
				throw new ConfigurationException("<cvsroot> configuration element must be specified in order to automatically checkout source from CVS.");
			Execute(NewCheckoutProcessInfo(result));
		}

		private ProcessInfo NewCheckoutProcessInfo(IIntegrationResult result)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			AppendCvsRoot(builder);
			builder.AddArgument("checkout");
			builder.AddArgument("-R");
			builder.AddArgument("-P");
			builder.AddArgument("-r", Branch);
			builder.AddArgument("-d", result.BaseFromWorkingDirectory(WorkingDirectory));
			builder.AddArgument(Module);
			return NewProcessInfoWithArgs(result, builder.ToString());
		}

		private void UpdateSource(IIntegrationResult result, string file)
		{
			Execute(NewGetSourceProcessInfo(result, file));
		}

		private ProcessInfo NewGetSourceProcessInfo(IIntegrationResult result, string dir)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			AppendCvsRoot(builder);
			builder.AppendArgument("-q update -d -P"); // build directories, prune empty directories
			builder.AppendIf(CleanCopy, "-C");
			builder.AppendIf(UseHistory && dir != null, "-l");
			builder.AppendIf(UseHistory && dir != null, "\"{0}\"", dir);
			builder.AddArgument("-r", Branch);

			return NewProcessInfoWithArgs(result, builder.ToString());
		}

		private Modification[] GetModifications(string directory, IIntegrationResult from, IIntegrationResult to)
		{
			return GetModifications(CreateLogProcessInfo(from, directory), from.StartTime, to.StartTime);
		}

		private Modification[] GetModificationsUsingHistory(IIntegrationResult from, IIntegrationResult to)
		{
			Log.Debug("Using cvs history command");
			string[] dirs = GetDirectoriesContainingChanges(from);

			// Get list of target files to run 'cvs log' against
			// Loop through file list update the directory then run the 'cvs log'
			ArrayList mods = new ArrayList();
			foreach (string dir in dirs)
			{
				string reportDir = Path.Combine(WorkingDirectory, dir);
				Log.Info(String.Format("Checking directory {0} for modifications.", reportDir));
				Modification[] modifications = GetModifications(reportDir, from, to);
				mods.AddRange(modifications);

				// Update the source if there are modifications or the user explicity states to.
				if (modifications.Length > 0)
				{
					UpdateSource(from, dir);
				}
			}
			return (Modification[]) mods.ToArray(typeof (Modification));
		}

		private string[] GetDirectoriesContainingChanges(IIntegrationResult from)
		{
			ProcessResult result = ExecuteHistoryCommand(from);
			historyCommandParser.WorkingDirectory = from.BaseFromWorkingDirectory(WorkingDirectory);
			return historyCommandParser.ParseOutputFrom(result.StandardOutput);
		}

		private ProcessResult ExecuteHistoryCommand(IIntegrationResult from)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			AppendCvsRoot(buffer);
			buffer.AppendArgument(string.Format("history -x MAR -a -D \"{0}\"", FormatCommandDate(from.StartTime)));
			return Execute(NewProcessInfoWithArgs(from, buffer.ToString()));
		}

		private ProcessInfo CreateLogProcessInfo(IIntegrationResult from, string dir)
		{
			return NewProcessInfoWithArgs(from, BuildHistoryProcessInfoArgs(from.StartTime, dir));
		}

		private ProcessInfo NewLabelProcessInfo(IIntegrationResult result)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			AppendCvsRoot(buffer);
			buffer.AppendArgument(string.Format("tag {0}{1}", TagPrefix, ConvertIllegalCharactersInLabel(result)));
			return NewProcessInfoWithArgs(result, buffer.ToString());
		}

		private string ConvertIllegalCharactersInLabel(IIntegrationResult result)
		{
			return Regex.Replace(result.Label, @"\.", "_");
		}

		private ProcessInfo NewProcessInfoWithArgs(IIntegrationResult result, string args)
		{
			return new ProcessInfo(Executable, args, result.BaseFromWorkingDirectory(WorkingDirectory));
		}

		// cvs [-d :ext:mycvsserver:/cvsroot/myrepo] -q log -N "-d>2004-12-24 12:00:00 GMT" -rmy_branch (with branch)
		// cvs [-d :ext:mycvsserver:/cvsroot/myrepo] -q log -Nb "-d>2004-12-24 12:00:00 GMT" (without branch)
		//		public const string HISTORY_COMMAND_FORMAT = @"{0}-q log -N{3} ""-d>{1}""{2}";		// -N means 'do not show tags'
		private string BuildHistoryProcessInfoArgs(DateTime from, string dir)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			AppendCvsRoot(buffer);
			buffer.AddArgument("-q"); // quiet
			buffer.AddArgument("rlog");
			buffer.AddArgument("-N"); // do not show tags
			if (StringUtil.IsBlank(Branch))
			{
				buffer.AddArgument("-b"); // only list revisions on HEAD
			}
			else
			{
				buffer.AppendArgument("-r{0}", Branch); // list revisions on branch
			}
			buffer.AppendIf(UseHistory, "-l");
			buffer.AppendArgument(@"""-d>{0}""", FormatCommandDate(from));
			if (! StringUtil.IsBlank(RestrictLogins))
			{
				foreach (string login in RestrictLogins.Split(','))
				{
					buffer.AppendArgument("-w{0}", login.Trim());
				}
			}
			buffer.AppendIf(UseHistory && dir != null, @"""{0}""", dir);
			buffer.AddArgument(Module);
			return buffer.ToString();
		}

		private void AppendCvsRoot(ProcessArgumentBuilder buffer)
		{
			buffer.AddArgument("-d", CvsRoot);
		}

		private void StripRepositoryRootFromModificationFolderNames(Modification[] modifications)
		{
			foreach (Modification modification in modifications)
			{
				modification.FolderName = StripRepositoryFolder(modification.FolderName);
			}
		}

		private const string LocalCvsProtocolString = ":local:";

		private string StripRepositoryFolder(string rcsFilePath)
		{
			string repositoryFolder = GetRepositoryFolder();
			if (rcsFilePath.StartsWith(repositoryFolder))
			{
				return rcsFilePath.Remove(0, repositoryFolder.Length);
			}
			return rcsFilePath;
		}

		/// <summary>
		/// Get the repository folder in order to strip it from the RCS file.
		/// The repository folder is the last part of the CVSRoot path -- unless the local protocol is used on windows machines.
		/// Examples: 
		///		CvsRoot=":pserver:anonymous@cruisecontrol.cvs.sourceforge.net:/cvsroot/cruisecontrol", Module="cruisecontrol", RepositoryFolder="/cvsroot/cruisecontrol/cruisecontrol"
		///		CvsRoot=":local:C:\dev\CVSRoot", Module="fitwebservice", RepositoryFolder="C:\dev\CVSRoot/fitwebservice"
		/// </summary>
		private string GetRepositoryFolder()
		{
			string modulePath = '/' + Module + '/';
			if (CvsRoot.StartsWith(LocalCvsProtocolString))
				return CvsRoot.Substring(LocalCvsProtocolString.Length) + modulePath;
			return CvsRoot.Substring(CvsRoot.LastIndexOf(':') + 1) + modulePath;
		}
	}
}