using System;
using System.Globalization;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("vss")]
	public class Vss : ProcessSourceControl
	{
		public const string DefaultProject = "$/";
		public const string SS_DIR_KEY = "SSDIR";
		public const string SS_REGISTRY_PATH = @"Software\\Microsoft\\SourceSafe";
		public const string SS_REGISTRY_KEY = "SCCServerPath";
		public const string SS_EXE = "ss.exe";
		private const string RecursiveCommandLineOption = "-R";

		private IRegistry registry;
		private string ssDir;
		private string executable;
		private string tempLabel;
		private IVssLocale locale;

		public Vss() : this(new VssLocale(CultureInfo.CurrentCulture))
		{}

		private Vss(IVssLocale locale) : this(locale, new VssHistoryParser(locale), new ProcessExecutor(), new Registry())
		{}

		public Vss(IVssLocale locale, IHistoryParser historyParser, ProcessExecutor executor, IRegistry registry) : base(historyParser, executor)
		{
			this.registry = registry;
			this.locale = locale;
		}

		[ReflectorProperty("executable", Required = false)]
		public string Executable
		{
			get
			{
				if (executable == null)
					executable = GetExecutableFromRegistry();
				return executable;
			}
			set { executable = value; }
		}

		[ReflectorProperty("project", Required = false)]
		public string Project = DefaultProject;

		[ReflectorProperty("username", Required = false)]
		public string Username;

		[ReflectorProperty("password", Required = false)]
		public string Password;

		[ReflectorProperty("ssdir", Required = false)]
		public string SsDir
		{
			get { return ssDir; }
			set { ssDir = StringUtil.StripQuotes(value); }
		}

		/// <summary>
		/// Gets or sets whether this repository should be labeled.
		/// </summary>
		[ReflectorProperty("applyLabel", Required = false)]
		public bool ApplyLabel = false;

		[ReflectorProperty("autoGetSource", Required = false)]
		public bool AutoGetSource = true;

		[ReflectorProperty("alwaysGetLatest", Required = false)]
		public bool AlwaysGetLatest = false;

		[ReflectorProperty("workingDirectory", Required = false)]
		public string WorkingDirectory;

		[ReflectorProperty("culture", Required = false)]
		public string Culture
		{
			get { return locale.ServerCulture; }
			set { locale.ServerCulture = value; }
		}

		[ReflectorProperty("cleanCopy", Required = false)]
		public bool CleanCopy = true;

            
	    public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
            string tempOutputFileName = Path.GetTempFileName();
	        return GetModifications(from, to, tempOutputFileName);
        }

        /// <summary>
        /// This method exists only to allow the VssTest unit tests to supply an expected output filename.
        /// DO NOT use this method for normal processing, use <see cref="GetModifications(IIntegrationResult, IIntegrationResult)"/>
        /// instead
        /// </summary>
        public Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to, string tempOutputFileName)
        {
            try
            {
                Execute(CreateHistoryProcessInfo(from, to, tempOutputFileName));
                TextReader outputReader = new StreamReader(tempOutputFileName);
                try
                {
                    return ParseModifications(outputReader, from.StartTime, to.StartTime);
                }
                finally
                {
                    outputReader.Close();
                }
            }
            finally 
            {
                File.Delete(tempOutputFileName);
            }
		}

		public override void LabelSourceControl(IIntegrationResult result)
		{
			if (! ApplyLabel) return;

			Execute(NewProcessInfoWith(LabelProcessInfoArgs(result), result));
			tempLabel = null;
		}

		public override void GetSource(IIntegrationResult result)
		{
			CreateTemporaryLabel(result);

			if (! AutoGetSource) return;

			Log.Info("Getting source from VSS");
			Execute(NewProcessInfoWith(GetSourceArgs(result), result));
		}

		private string GetSourceArgs(IIntegrationResult result)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			builder.AddArgument("get", Project);
			builder.AddArgument(RecursiveCommandLineOption);
			builder.AppendIf(ApplyLabel, "-VL" + tempLabel);
			builder.AppendIf(!AlwaysGetLatest, "-Vd" + locale.FormatCommandDate(result.StartTime));
			AppendUsernameAndPassword(builder);
			builder.AppendArgument("-I-N -W -GF- -GTM");
			builder.AppendIf(CleanCopy, "-GWR");
			return builder.ToString();
		}

		private ProcessInfo CreateHistoryProcessInfo(IIntegrationResult from, IIntegrationResult to, string tempOutputFileName)
		{
			return NewProcessInfoWith(HistoryProcessInfoArgs(from.StartTime, to.StartTime, tempOutputFileName), to);
		}

		private string HistoryProcessInfoArgs(DateTime from, DateTime to, string tempOutputFileName)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			builder.AddArgument("history", Project);
			builder.AddArgument(RecursiveCommandLineOption);
			builder.AddArgument(string.Format("-Vd{0}~{1}", locale.FormatCommandDate(to), locale.FormatCommandDate(from)));
			AppendUsernameAndPassword(builder);
			builder.AddArgument("-I-Y");
            builder.AddArgument(string.Format("-O@{0}", tempOutputFileName));
            return builder.ToString();
		}

		private void CreateTemporaryLabel(IIntegrationResult result)
		{
			if (ApplyLabel)
			{
				tempLabel = CreateTemporaryLabelName(result.StartTime);
				LabelSourceControlWith(tempLabel, result);
			}
		}

		private void LabelSourceControlWith(string label, IIntegrationResult result)
		{
			Execute(NewProcessInfoWith(LabelProcessInfoArgs(label, null), result));
		}

		private string LabelProcessInfoArgs(IIntegrationResult result)
		{
			if (result.Succeeded)
			{
				return LabelProcessInfoArgs(result.Label, tempLabel);
			}
			else
			{
				return DeleteLabelProcessInfoArgs();
			}
		}

		private string DeleteLabelProcessInfoArgs()
		{
			return LabelProcessInfoArgs(string.Empty, tempLabel);
		}

		private string LabelProcessInfoArgs(string label, string oldLabel)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			builder.AddArgument("label", Project);
			builder.AddArgument("-L" + label);
			builder.AddArgument("-VL", "", oldLabel); // only append argument if old label is specified
			AppendUsernameAndPassword(builder);
			builder.AddArgument("-I-Y");
			return builder.ToString();
		}

		private string CreateTemporaryLabelName(DateTime time)
		{
			return "CCNETUNVERIFIED" + time.ToString("MMddyyyyHHmmss");
		}

		private string GetExecutableFromRegistry()
		{
			string comServerPath = registry.GetExpectedLocalMachineSubKeyValue(SS_REGISTRY_PATH, SS_REGISTRY_KEY);
			return Path.Combine(Path.GetDirectoryName(comServerPath), SS_EXE);
		}

		private ProcessInfo NewProcessInfoWith(string args, IIntegrationResult result)
		{
			string workingDirectory = result.BaseFromWorkingDirectory(WorkingDirectory);
			if (! Directory.Exists(workingDirectory)) Directory.CreateDirectory(workingDirectory);

			ProcessInfo processInfo = new ProcessInfo(Executable, args, workingDirectory);
			if (SsDir != null)
			{
				processInfo.EnvironmentVariables[SS_DIR_KEY] = SsDir;
			}
			return processInfo;
		}

		private void AppendUsernameAndPassword(ProcessArgumentBuilder builder)
		{
			builder.AppendIf(! StringUtil.IsBlank(Username), string.Format("-Y{0},{1}", Username, Password));
		}
	}
}
