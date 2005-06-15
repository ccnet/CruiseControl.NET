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
		public const string SS_DIR_KEY = "SSDIR";
		public const string SS_REGISTRY_PATH = @"Software\\Microsoft\\SourceSafe";
		public const string SS_REGISTRY_KEY = "SCCServerPath";
		public const string SS_EXE = "ss.exe";

		private const string HISTORY_COMMAND_FORMAT = @"history {0} -R -Vd{1}~{2} -Y{3},{4} -I-Y";
		private const string GET_BY_DATE_COMMAND_FORMAT = @"get {0} -R -Vd{1} -Y{2},{3} -I-N -GTM";
		private const string GET_BY_LABEL_COMMAND_FORMAT = @"get {0} -R -VL{1} -Y{2},{3} -I-N -GTM";
		private const string LABEL_COMMAND_FORMAT = @"label {0} -L{1} -VL{2} -Y{3},{4} -I-Y";
		private const string LABEL_COMMAND_FORMAT_NOTIMESTAMP = @"label {0} -L{1} -Y{2},{3} -I-Y";

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

		[ReflectorProperty("project")]
		public string Project;

		[ReflectorProperty("username")]
		public string Username;

		[ReflectorProperty("password")]
		public string Password;

		[ReflectorProperty("ssdir", Required = false)]
		public string SsDir
		{
			get { return ssDir; }
			set { ssDir = value.Trim('"'); }
		}

		/// <summary>
		/// Gets or sets whether this repository should be labeled.
		/// </summary>
		[ReflectorProperty("applyLabel", Required = false)]
		public bool ApplyLabel = false;

		[ReflectorProperty("autoGetSource", Required = false)]
		public bool AutoGetSource = false;

		[ReflectorProperty("workingDirectory", Required = false)]
		public string WorkingDirectory;

		[ReflectorProperty("culture", Required = false)]
		public string Culture
		{
			get { return locale.CultureName; }
			set { locale.CultureName = value; }
		}

		[ReflectorProperty("cleanCopy", Required = false)]
		public bool CleanCopy = true;

		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			return GetModifications(CreateHistoryProcessInfo(from, to), from.StartTime, to.StartTime);
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
			string cleanCopy = (CleanCopy) ? " -GWR" : string.Empty;
			if (ApplyLabel)
			{
				return string.Format(GET_BY_LABEL_COMMAND_FORMAT, Project, tempLabel, Username, Password) + cleanCopy;
			}
			else
			{
				return string.Format(GET_BY_DATE_COMMAND_FORMAT, Project, locale.FormatCommandDate(result.StartTime), Username, Password) + cleanCopy;
			}
		}

		private ProcessInfo CreateHistoryProcessInfo(IIntegrationResult from, IIntegrationResult to)
		{
			return NewProcessInfoWith(HistoryProcessInfoArgs(from.StartTime, to.StartTime), to);
		}

		private string HistoryProcessInfoArgs(DateTime from, DateTime to)
		{
			return string.Format(HISTORY_COMMAND_FORMAT, 
			                     Project, locale.FormatCommandDate(to), locale.FormatCommandDate(from), Username, Password);
		}

		private string ReplaceLabelProcessInfoArgs(string label, string oldLabel)
		{
			return string.Format(LABEL_COMMAND_FORMAT, Project, label, oldLabel, Username, Password);
		}

		private string CreateTemporaryLabelName(DateTime time)
		{
			return "CCNETUNVERIFIED" + time.ToString("MMddyyyyHHmmss");
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
			Execute(NewProcessInfoWith(string.Format(LABEL_COMMAND_FORMAT_NOTIMESTAMP, Project, label, Username, Password), result));
		}

		private string LabelProcessInfoArgs(IIntegrationResult result)
		{
			if (result.Succeeded)
			{
				return ReplaceLabelProcessInfoArgs(result.Label, tempLabel);
			}
			else
			{
				return DeleteLabelProcessInfoArgs();
			}
		}

		private string DeleteLabelProcessInfoArgs()
		{
			return ReplaceLabelProcessInfoArgs(string.Empty, tempLabel);
		}

		private string GetExecutableFromRegistry()
		{
			string comServerPath = registry.GetExpectedLocalMachineSubKeyValue(SS_REGISTRY_PATH, SS_REGISTRY_KEY);
			return Path.Combine(Path.GetDirectoryName(comServerPath), SS_EXE);
		}

		private ProcessInfo NewProcessInfoWith(string args, IIntegrationResult result)
		{
			Log.Debug(string.Format("VSS: {0} {1}", Executable, args));
			string workingDirectory = result.BaseFromWorkingDirectory(WorkingDirectory);
			if (! Directory.Exists(workingDirectory)) Directory.CreateDirectory(workingDirectory);

			ProcessInfo processInfo = new ProcessInfo(Executable, args, workingDirectory);
			if (SsDir != null)
			{
				processInfo.EnvironmentVariables[SS_DIR_KEY] = SsDir;
			}
			return processInfo;
		}
	}
}