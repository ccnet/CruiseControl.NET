using System.Text;
using Exortech.NetReflector;
using System;
using System.Globalization;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("cvs")]
	public class Cvs : ProcessSourceControl
	{
		// use -C to force get clean copy? should reset tags?
		public const string GET_SOURCE_COMMAND_FORMAT = @"-q update -d -P -C";	// build directories, prune empty directories, get clean copy
		public const string COMMAND_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss 'GMT'";
		private string _executable = "cvs.exe";
		private string _workingDirectory = "";
		private string _cvsRoot = "";
		private bool labelOnSuccess = false;
		private string restrictLogins = "";
		private string branch = "";

		public Cvs() : this(new CvsHistoryParser(), new ProcessExecutor()) { }

		public Cvs(IHistoryParser parser, ProcessExecutor executor) : base(parser, executor) { }
 
		[ReflectorProperty("executable")]
		public string Executable
		{
			get { return _executable; }
			set { _executable = value; }
		}
		
		[ReflectorProperty("cvsroot", Required=false)]
		public string CvsRoot
		{
			get { return _cvsRoot; }
			set { _cvsRoot = value; }
		}

		[ReflectorProperty("workingDirectory")]
		public string WorkingDirectory
		{
			get { return _workingDirectory; }
			set { _workingDirectory = value; }
		}

		[ReflectorProperty("labelOnSuccess", Required=false)] 
		public bool LabelOnSuccess
		{
			get { return labelOnSuccess; }
			set { labelOnSuccess = value; }
		}

		[ReflectorProperty("restrictLogins", Required=false)] 
		public string RestrictLogins
		{
			get { return restrictLogins; }
			set { restrictLogins = value; }
		}

		[ReflectorProperty("webUrlBuilder", InstanceTypeKey="type", Required=false)]
		public IModificationUrlBuilder UrlBuilder;

		[ReflectorProperty("autoGetSource", Required = false)]
		public bool AutoGetSource = false;

		[ReflectorProperty("branch", Required=false)] 
		public string Branch
		{
			get { return branch; }
			set { branch = value; }
		}

		public string FormatCommandDate(DateTime date)
		{
			return date.ToUniversalTime().ToString(COMMAND_DATE_FORMAT, CultureInfo.InvariantCulture);
		}

		public override Modification[] GetModifications(DateTime from, DateTime to)
		{
			Modification[] modifications = GetModifications(CreateHistoryProcessInfo(from, to), from, to);
			if (UrlBuilder != null ) 
			{
				UrlBuilder.SetupModification(modifications);
			}
			return modifications;
		}

		public override void LabelSourceControl(string label, IIntegrationResult result)
		{
			if (LabelOnSuccess && result.Succeeded)
			{
				Execute(CreateLabelProcessInfo(label, result));
			}
		}

		public ProcessInfo CreateHistoryProcessInfo(DateTime from, DateTime to)
		{
			return new ProcessInfo(Executable, BuildHistoryProcessInfoArgs(from), WorkingDirectory);
		}

		public ProcessInfo CreateLabelProcessInfo(string label, IIntegrationResult result) 
		{
			CommandLineBuilder buffer = new CommandLineBuilder();
			buffer.AppendArgument("-d {0}", CvsRoot);
			buffer.AppendArgument("tag ver-{0}", label);
			return new ProcessInfo(Executable, buffer.ToString(), WorkingDirectory);
		}

		public override void GetSource(IIntegrationResult result)
		{
			if (AutoGetSource)
			{
				ProcessInfo info = new ProcessInfo(Executable, GET_SOURCE_COMMAND_FORMAT, WorkingDirectory);
				Log.Info(string.Format("Getting source from CVS: {0} {1}", info.FileName, info.Arguments));
				Execute(info);
			}
		}

		// cvs [-d :ext:mycvsserver:/cvsroot/myrepo] -q log -N "-d>2004-12-24 12:00:00 'GMT'" -rmy_branch (with branch)
		// cvs [-d :ext:mycvsserver:/cvsroot/myrepo] -q log -Nb "-d>2004-12-24 12:00:00 'GMT'" (without branch)
//		public const string HISTORY_COMMAND_FORMAT = @"{0}-q log -N{3} ""-d>{1}""{2}";		// -N means 'do not show tags'

		// in cvs, date 'to' is implicitly now
		// todo: if cvs will accept a 'to' date, it would be nicer to 
		// include that for some harmony with the vss version
		public string BuildHistoryProcessInfoArgs(DateTime from)
		{		
			CommandLineBuilder buffer = new CommandLineBuilder();
			buffer.AppendArgument("-d {0}", CvsRoot);
			buffer.AppendArgument("-q log -N");
			if (StringUtil.IsBlank(branch)) buffer.Append("b");
			buffer.AppendArgument(@"""-d>{0}""", FormatCommandDate(from));
			buffer.AppendArgument("-r{0}", Branch);
            if (! StringUtil.IsBlank(RestrictLogins)) 
            {
                foreach (string login in RestrictLogins.Split(',')) {
                    buffer.AppendArgument("-w{0}", login.Trim());
                }
            }
			return buffer.ToString();
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

			public void Append(string value)
			{
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
		}
	}
}
