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
		public const string TEMP_SOURCE_DIRECTORY = "VssSource";

		internal static readonly string HISTORY_COMMAND_FORMAT = @"history {0} -R -Vd{1}~{2} -Y{3},{4} -I-Y";
		internal static readonly string GET_BY_DATE_COMMAND_FORMAT = @"get {0} -R -Vd{1} -Y{2},{3} -I-N -GWR -GTM";
		internal static readonly string GET_BY_LABEL_COMMAND_FORMAT = @"get {0} -R -VL{1} -Y{2},{3} -I-N -GWR -GTM";
		internal static readonly string LABEL_COMMAND_FORMAT = @"label {0} -L{1} -VL{2} -Y{3},{4} -I-Y";
		internal static readonly string LABEL_COMMAND_FORMAT_NOTIMESTAMP = @"label {0} -L{1} -Y{2},{3} -I-Y";

		private IRegistry _registry;
		private string _ssDir;
		private string _executable;
		private string _lastTempLabel;
		private IVssLocale _locale;

		public Vss() : this(new VssLocale(CultureInfo.CurrentCulture))
		{}

		private Vss(IVssLocale locale) : this(locale, new VssHistoryParser(locale), new ProcessExecutor(), new Registry())
		{}

		public Vss(IVssLocale locale, IHistoryParser historyParser, ProcessExecutor executor, IRegistry registry) : base(historyParser, executor)
		{
			_registry = registry;
			_locale = locale;
		}

		[ReflectorProperty("executable", Required = false)]
		public string Executable
		{
			get
			{
				if (_executable == null)
					_executable = GetExecutable();
				return _executable;
			}
			set { _executable = value; }
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
			get { return _ssDir; }
			set { _ssDir = value.Trim('"'); }
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
			get { return _locale.CultureName; }
			set { _locale.CultureName = value; }
		}

		public override Modification[] GetModifications(DateTime from, DateTime to)
		{
			return GetModifications(CreateHistoryProcessInfo(from, to), from, to);
		}

		public override void LabelSourceControl(string newLabel, IIntegrationResult result)
		{
			if (! ApplyLabel) return;

			if (result.Succeeded)
			{
				LabelSourceControl(newLabel, _lastTempLabel);
			}
			else
			{
				DeleteTemporaryLabel();
			}
			_lastTempLabel = null;
		}

		private void LabelSourceControl(string newLabel, string oldLabel)
		{
			Execute(CreateLabelProcessInfo(newLabel, oldLabel));
		}

		public void CreateTemporaryLabel()
		{
			if (ApplyLabel)
			{
				_lastTempLabel = CreateTemporaryLabelName(DateTime.Now);
				LabelSourceControl(_lastTempLabel);
			}
		}

		private void DeleteTemporaryLabel()
		{
			if (ApplyLabel && WasTempLabelApplied())
			{
				DeleteLatestLabel();
			}
			_lastTempLabel = null;
		}

		public ProcessInfo CreateHistoryProcessInfo(DateTime from, DateTime until)
		{
			return CreateProcessInfo(BuildHistoryProcessInfoArgs(from, until));
		}

		public virtual ProcessInfo CreateLabelProcessInfo(string label)
		{
			return CreateProcessInfo(string.Format(LABEL_COMMAND_FORMAT_NOTIMESTAMP, Project, label, Username, Password));
		}

		public ProcessInfo CreateLabelProcessInfo(string label, string oldLabel)
		{
			return CreateProcessInfo(string.Format(LABEL_COMMAND_FORMAT, Project, label, oldLabel, Username, Password));
		}

		private ProcessInfo CreateProcessInfo(string args)
		{
			Log.Debug(string.Format("VSS: {0} {1}", Executable, args));
			ProcessInfo processInfo = new ProcessInfo(Executable, args, WorkingDirectory);
			if (SsDir != null)
			{
				processInfo.EnvironmentVariables[SS_DIR_KEY] = SsDir;
			}
			return processInfo;
		}

		private string CreateTemporaryLabelName(DateTime time)
		{
			return "CCNETUNVERIFIED" + time.ToString("MMddyyyyHHmmss");
		}

		private void LabelSourceControl(string label)
		{
			Execute(CreateLabelProcessInfo(label));
		}

		internal string BuildHistoryProcessInfoArgs(DateTime from, DateTime to)
		{
			return string.Format(HISTORY_COMMAND_FORMAT, Project, _locale.FormatCommandDate(to), _locale.FormatCommandDate(from), Username, Password);
		}

		private string GetExecutable()
		{
			string comServerPath = _registry.GetExpectedLocalMachineSubKeyValue(SS_REGISTRY_PATH, SS_REGISTRY_KEY);
			return Path.Combine(Path.GetDirectoryName(comServerPath), SS_EXE);
		}

		public override void GetSource(IIntegrationResult result)
		{
			CreateTemporaryLabel();

			if (! AutoGetSource)
				return;

			if (WorkingDirectory == null)
			{
				WorkingDirectory = TempFileUtil.CreateTempDir(TEMP_SOURCE_DIRECTORY);
			}
			else if (! Directory.Exists(WorkingDirectory))
			{
				Directory.CreateDirectory(WorkingDirectory);
			}

			Log.Info("Getting source from VSS");
			ProcessInfo processInfo = CreateProcessInfo(DetermineGetSourceCommand(result));
			Execute(processInfo);
		}

		internal string DetermineGetSourceCommand(IIntegrationResult result)
		{
			if (ApplyLabel && WasTempLabelApplied())
			{
				return string.Format(GET_BY_LABEL_COMMAND_FORMAT, Project, _lastTempLabel, Username, Password);
			}
			else if (!ApplyLabel)
			{
				return string.Format(GET_BY_DATE_COMMAND_FORMAT, Project, _locale.FormatCommandDate(result.StartTime), Username, Password);
			}
			else
			{
				throw new CruiseControlException("illegal state: applylabel true but no temp label");
			}
		}

		private void DeleteLatestLabel()
		{
			LabelSourceControl(string.Empty, _lastTempLabel);
		}

		private bool WasTempLabelApplied()
		{
			return (_lastTempLabel != null);
		}
	}
}