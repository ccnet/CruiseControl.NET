using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("cvs")]
	public class Cvs : ProcessSourceControl
	{
		public const string GET_SOURCE_COMMAND_FORMAT = @"-q update -d -P"; // build directories, prune empty directories
		public const string COMMAND_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss 'GMT'";
		public const int DIRECTORY_INDEX = 7;

		public Cvs() : this(new CvsHistoryParser(), new ProcessExecutor())
		{}

		public Cvs(IHistoryParser parser, ProcessExecutor executor) : base(parser, executor)
		{}

		[ReflectorProperty("executable")]
		public string Executable = "cvs.exe";

		[ReflectorProperty("cvsroot", Required=false)]
		public string CvsRoot = string.Empty;

		[ReflectorProperty("workingDirectory", Required=false)]
		public string WorkingDirectory = string.Empty;

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
				Log.Info("Using cvs history command");
				dirs = GetDirectoriesContainingChanges(from.StartTime);
			}

			// Get list of target files to run 'cvs log' against
			// Loop through file list update the directory then run the 'cvs log'
			ArrayList mods = new ArrayList();
			foreach (string dir in dirs)
			{
				Log.Info(string.Format("Checking directory {0} for modifications.", dir));
				Modification[] modifications = GetModifications(CreateLogProcessInfo(from.StartTime, dir), from.StartTime, to.StartTime);
				foreach (Modification mod in modifications)
				{
					mods.Add(mod);
				}

				if ((modifications.Length > 0) && UseHistory)
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
				Execute(CreateLabelProcessInfo(result));
			}
		}

		public ProcessInfo CreateLogProcessInfo(DateTime from)
		{
			return CreateLogProcessInfo(from, null); // null?
		}

		public ProcessInfo CreateLogProcessInfo(DateTime from, string dir)
		{
			return new ProcessInfo(Executable, BuildHistoryProcessInfoArgs(from, dir), WorkingDirectory);
		}

		public ProcessInfo CreateLabelProcessInfo(IIntegrationResult result)
		{
			CommandLineBuilder buffer = new CommandLineBuilder();
			buffer.AppendArgument("-d {0}", CvsRoot);
			buffer.AppendArgument("tag ver-{0}", result.Label);
			return new ProcessInfo(Executable, buffer.ToString(), WorkingDirectory);
		}

		public override void GetSource(IIntegrationResult result)
		{
			if (AutoGetSource)
			{
				Execute(CreateGetSourceProcessInfo(null));
			}
		}

		private void UpdateSource(string file)
		{
			Execute(CreateGetSourceProcessInfo(file));
		}

		private ProcessInfo CreateGetSourceProcessInfo(string dir)
		{
			CommandLineBuilder builder = new CommandLineBuilder();
			builder.AppendArgument(GET_SOURCE_COMMAND_FORMAT);
			builder.AppendIf(CleanCopy, "-C");
			builder.AppendIf(UseHistory && dir != null, "-l");
			builder.AppendIf(UseHistory && dir != null, "\"{0}\"", dir);
	
			ProcessInfo info = new ProcessInfo(Executable, builder.ToString(), WorkingDirectory);
			Log.Info(string.Format("Getting source from CVS: {0} {1}", info.FileName, info.Arguments));
			return info;
		}

		// cvs [-d :ext:mycvsserver:/cvsroot/myrepo] -q log -N "-d>2004-12-24 12:00:00 'GMT'" -rmy_branch (with branch)
		// cvs [-d :ext:mycvsserver:/cvsroot/myrepo] -q log -Nb "-d>2004-12-24 12:00:00 'GMT'" (without branch)
//		public const string HISTORY_COMMAND_FORMAT = @"{0}-q log -N{3} ""-d>{1}""{2}";		// -N means 'do not show tags'

		// in cvs, date 'to' is implicitly now
		// todo: if cvs will accept a 'to' date, it would be nicer to 
		// include that for some harmony with the vss version
		private string BuildHistoryProcessInfoArgs(DateTime from, string dir)
		{
			CommandLineBuilder buffer = new CommandLineBuilder();
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

		private string[] GetDirectoriesContainingChanges(DateTime from)
		{
			Log.Info("Get changes in working directory: " + WorkingDirectory);
			ProcessResult result = Execute(CreateHistoryProcessInfo(from));

			StringReader reader = new StringReader(result.StandardOutput);
			ArrayList entryList = new ArrayList();
			string line;
			while ((line = reader.ReadLine()) != null)
			{
				string dir = ParseEntry(line);
				if (dir == null)
				{
					continue;
				}

				// We need to replace forward slash returned by cvs to directory separator of
				// the platform this is running on.
				dir = dir.Replace('/', Path.DirectorySeparatorChar);
				int index;
				if ((index = AcceptEntry(WorkingDirectory, dir)) != -1)
				{
					Log.Debug("Accepted dir: " + dir);
					string entry = dir.Substring(index);
					if (!entryList.Contains(entry))
					{
						Log.Debug("Added entry: " + entry);
						entryList.Add(entry);
					}
				}
			}

			return (string[]) entryList.ToArray(typeof (string));
		}

		private ProcessInfo CreateHistoryProcessInfo(DateTime from)
		{
			CommandLineBuilder buffer = new CommandLineBuilder();
			buffer.AppendArgument("history -x MAR -a -D \"{0}\"", FormatCommandDate(from));
			return new ProcessInfo(Executable, buffer.ToString(), WorkingDirectory);
		}

		/// <summary>
		/// Check to see if the directory passed in is part of the absolute directory
		/// that is passed in.
		/// </summary>
		/// <param name="absolutePath"></param>
		/// <param name="relativePath"></param>
		/// <returns>Relative repo path</returns>
		private int AcceptEntry(string absolutePath, string relativePath)
		{
			char[] delimeter = new char[] {Path.DirectorySeparatorChar};
			string[] absoluteParts = absolutePath.Split(delimeter);
			string[] relativeParts = relativePath.Split(delimeter);

			string relative = relativeParts[0];
			bool addToString = false;
			string newPath = "";
			foreach (string part in absoluteParts)
			{
				if (part.Equals(relative))
				{
					addToString = true;
				}

				if (addToString)
				{
					newPath += part + Path.DirectorySeparatorChar;
				}
			}

			int retVal = -1;
			if (relativePath.IndexOf(newPath) != -1 && (!newPath.Equals("")))
			{
				retVal = newPath.Length;
			}
			return retVal;
		}

		/// <summary>
		/// Find the directory listed by the history command
		/// </summary>
		/// <param name="line"></param>
		/// <returns></returns>
		private string ParseEntry(string line)
		{
			ArrayList items = new ArrayList(9);
			char[] delimeters = new char[] {' ', '\t'};
			string[] tokens = line.Split(delimeters);
			foreach (string token in tokens)
			{
				if (!StringUtil.IsWhitespace(token))
				{
					Log.Debug("Token: " + token);
					items.Add(token);
				}
			}

			if (items.Count > DIRECTORY_INDEX)
				return (string) items[DIRECTORY_INDEX];

			return null;		// under what conditions will this happen.  do we really want to return null?
		}

		private class CommandLineBuilder
		{
			private StringBuilder builder = new StringBuilder();

			public void AppendArgument(string format, string value)
			{
				if (StringUtil.IsBlank(value)) return;

				AppendSpaceIfNotEmpty();
				builder.AppendFormat(format, value);
			}

			public void AppendArgument(string value)
			{
				AppendSpaceIfNotEmpty();
				builder.Append(value);
			}

			private void AppendSpaceIfNotEmpty()
			{
				if (builder.Length > 0) builder.Append(" ");
			}

			public override string ToString()
			{
				return builder.ToString();
			}

			public void AppendIf(bool condition, string value)
			{
				if (condition) AppendArgument(value);
			}

			public void AppendIf(bool condition, string format, string argument)
			{
				if (condition) AppendArgument(format, argument);
			}
		}
	}
}