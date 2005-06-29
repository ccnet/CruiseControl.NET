using System;
using System.Collections;
using System.Globalization;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("cvs")]
	public class Cvs : ProcessSourceControl
	{
		public const string DefaultCvsExecutable = "cvs.exe";
		public const string GET_SOURCE_COMMAND_FORMAT = @"-q update -d -P"; // build directories, prune empty directories
		public const string COMMAND_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss 'GMT'";
		public const int DIRECTORY_INDEX = 7;

		public Cvs() : this(new CvsHistoryParser(), new ProcessExecutor())
		{}

		public Cvs(IHistoryParser parser, ProcessExecutor executor) : base(parser, executor)
		{}

		[ReflectorProperty("executable")]
		public string Executable = DefaultCvsExecutable;

		[ReflectorProperty("cvsroot", Required=false)]
		public string CvsRoot = string.Empty;

		[ReflectorProperty("workingDirectory", Required=false)]
		public string WorkingDirectory = string.Empty;

		// what's the purpose of this property?
		[ReflectorProperty("localOnly", Required=false)]
		public bool LocalOnly = false;

		[ReflectorProperty("labelOnSuccess", Required=false)]
		public bool LabelOnSuccess = false;

		[ReflectorProperty("restrictLogins", Required=false)]
		public string RestrictLogins = string.Empty;

		[ReflectorProperty("webUrlBuilder", InstanceTypeKey="type", Required=false)]
		public IModificationUrlBuilder UrlBuilder;

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
			// by default use the WorkingDirectory to keep backward compatibility
			string[] dirs = {WorkingDirectory};
			// Run a true 'cvs history' command
			if (UseHistory)
			{
				Log.Debug("Using cvs history command");
				CvsHistoryCommandParser history = new CvsHistoryCommandParser(_executor, Executable, WorkingDirectory);
				history.LocalOnly = LocalOnly;
				dirs = history.GetDirectoriesContainingChanges(from.StartTime);
			}

			// Get list of target files to run 'cvs log' against
			// Loop through file list update the directory then run the 'cvs log'
			ArrayList mods = new ArrayList();
			foreach (string dir in dirs)
			{
				string reportDir = Path.Combine(WorkingDirectory, dir);
				Log.Info(string.Format("Checking directory {0} for modifications.", reportDir));
				Modification[] modifications = GetModifications(CreateLogProcessInfo(from.StartTime, dir), from.StartTime, to.StartTime);
				mods.AddRange(modifications);

				// Update the source if there are modifications or the user explicity states to.
 				if ((modifications.Length > 0) && UseHistory)
//				if (modifications.Length > 0 || AutoGetSource)	// do we really want to do this?
				{
					UpdateSource(dir);
				}
			}

			Modification[] modArray = (Modification[]) mods.ToArray(typeof (Modification));
			if (UrlBuilder != null)
			{
				UrlBuilder.SetupModification(modArray);
			}
			return modArray;
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
				UpdateSource(null);
			}
		}

		private void UpdateSource(string file)
		{
			Execute(NewGetSourceProcessInfo(file));
		}

		private ProcessInfo NewGetSourceProcessInfo(string dir)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			builder.AppendArgument(GET_SOURCE_COMMAND_FORMAT);
			builder.AppendIf(CleanCopy, "-C");
//			builder.AppendIf(UseHistory && dir != null, "-l");
//			builder.AppendIf(UseHistory && dir != null, "\"{0}\"", dir);
 			builder.AppendIf((UseHistory && dir != null) || LocalOnly, "-l");
 			builder.AppendIf(dir != null, "\"{0}\"", dir);

			ProcessInfo info = NewProcessInfoWithArgs(builder.ToString());
			Log.Info(string.Format("Getting source from CVS: {0} {1}", info.FileName, info.Arguments));
			return info;
		}

		private ProcessInfo CreateLogProcessInfo(DateTime from, string dir)
		{
			return NewProcessInfoWithArgs(BuildHistoryProcessInfoArgs(from, dir));
		}

		private ProcessInfo NewLabelProcessInfo(IIntegrationResult result)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AppendArgument("-d {0}", CvsRoot);
			buffer.AppendArgument(string.Format("tag {0}{1}", TagPrefix, result.Label));
			return NewProcessInfoWithArgs(buffer.ToString());
		}

		private ProcessInfo NewProcessInfoWithArgs(string args)
		{
			return new ProcessInfo(Executable, args, WorkingDirectory);
		}

		// cvs [-d :ext:mycvsserver:/cvsroot/myrepo] -q log -N "-d>2004-12-24 12:00:00 'GMT'" -rmy_branch (with branch)
		// cvs [-d :ext:mycvsserver:/cvsroot/myrepo] -q log -Nb "-d>2004-12-24 12:00:00 'GMT'" (without branch)
		//		public const string HISTORY_COMMAND_FORMAT = @"{0}-q log -N{3} ""-d>{1}""{2}";		// -N means 'do not show tags'

		// in cvs, date 'to' is implicitly now
		// todo: if cvs will accept a 'to' date, it would be nicer to 
		// include that for some harmony with the vss version
		private string BuildHistoryProcessInfoArgs(DateTime from, string dir)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AppendArgument("-d {0}", CvsRoot);
			buffer.AppendArgument("-q log -N");
			buffer.AppendIf(UseHistory, "-l");
			buffer.AppendIf(StringUtil.IsBlank(Branch), "-b");
			buffer.AppendArgument(@"""-d>{0}""", FormatCommandDate(from));
			buffer.AppendArgument("-r{0}", Branch);
			if (! StringUtil.IsBlank(RestrictLogins))
			{
				foreach (string login in RestrictLogins.Split(','))
				{
					buffer.AppendArgument("-w{0}", login.Trim());
				}
			}
			buffer.AppendIf(UseHistory && dir != null, @"""{0}""", dir);
			return buffer.ToString();
		}
	}
}