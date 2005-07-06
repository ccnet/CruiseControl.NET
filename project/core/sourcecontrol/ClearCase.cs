using System;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("clearCase")]
	public class ClearCase : ProcessSourceControl
	{
		private const string _TEMPORARY_BASELINE_PREFIX = "CruiseControl.NETTemporaryBaseline_";
		public const string DATETIME_FORMAT = "dd-MMM-yyyy.HH:mm:ss";

		public ClearCase() : base(new ClearCaseHistoryParser())
		{}

		public ClearCase(ProcessExecutor executor)
			: base(new ClearCaseHistoryParser(), executor)
		{}

		[ReflectorProperty("executable", Required=false)]
		public string Executable = "cleartool.exe";

		[ReflectorProperty("projectVobName", Required=false)]
		public string ProjectVobName;

		[ReflectorProperty("useBaseline", Required=false)]
		public bool UseBaseline = false;

		[ReflectorProperty("useLabel", Required=false)]
		public bool UseLabel = true;

		[ReflectorProperty("viewName", Required=false)]
		public string ViewName;

		[ReflectorProperty("viewPath", Required=false)]
		public string ViewPath;

		[ReflectorProperty("autoGetSource", Required = false)]
		public bool AutoGetSource = false;

		[ReflectorProperty("branch", Required = false)]
		public string Branch;

		public string TempBaseline;

		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			return base.GetModifications(CreateHistoryProcessInfo(from.StartTime, to.StartTime), from.StartTime, to.StartTime);
		}

		/// <summary>
		/// Executes the two processes needed to label the source tree in ClearCase.
		/// </summary>
		/// <remarks>
		///	ClearCase needs to execute two processes to label a source tree; most source control systems
		///	take only one.
		/// </remarks>
		/// <param name="result">the timestamp of the label; ignored for this implementation</param>
		public override void LabelSourceControl(IIntegrationResult result)
		{
			if (result.Succeeded)
			{
				if (UseBaseline)
				{
					RenameBaseline(result.Label);
				}
				if (UseLabel)
				{
					ProcessResult processResult = base.Execute(CreateLabelTypeProcessInfo(result.Label));
					Log.Debug("standard output from label: " + processResult.StandardOutput);
					ExecuteIgnoreNonVobObjects(CreateMakeLabelProcessInfo(result.Label));
				}
			}
			else
			{
				DeleteTemporaryLabel();
			}
		}

		private void CreateTemporaryLabel()
		{
			if (UseBaseline)
			{
				TempBaseline = CreateTemporaryBaselineName();
				ValidateBaselineConfiguration();
				base.Execute(CreateTempBaselineProcessInfo(TempBaseline));
			}
		}

		public void DeleteTemporaryLabel()
		{
			if (UseBaseline)
			{
				ValidateBaselineConfiguration();
				RemoveBaseline();
			}
		}

		public ProcessInfo CreateTempBaselineProcessInfo(string name)
		{
			string args = string.Format("mkbl -view {0} -identical {1}", ViewName, name);
			Log.Debug(string.Format("command line is: {0} {1}", Executable, args));
			return new ProcessInfo(Executable, args);
		}

		internal string CreateTemporaryBaselineName()
		{
			return _TEMPORARY_BASELINE_PREFIX + DateTime.Now.ToString("MM-dd-yyyy-HH-mm-ss");
		}

		// This is a HACK.  ProcessSourceControl.Execute doesn't allow the flexibility ClearCase needs
		// to allow nonzero exit codes and to selectively ignore certian error messages.
		private void ExecuteIgnoreNonVobObjects(ProcessInfo info)
		{
			info.TimeOut = Timeout;
			ProcessResult result = executor.Execute(info);

			if (result.TimedOut)
			{
				throw new CruiseControlException("Source control operation has timed out.");
			}
			else if (result.Failed && HasFatalError(result.StandardError))
			{
				throw new CruiseControlException(string.Format("Source control operation failed: {0}. Process command: {1} {2}",
				                                               result.StandardError, info.FileName, info.Arguments));
			}
			else if (result.HasErrorOutput)
			{
				Log.Warning(string.Format("Source control wrote output to stderr: {0}", result.StandardError));
			}
		}

		/// <summary>
		/// Returns true if there is an error indicating the operation did not complete successfully.
		/// </summary>
		/// <remarks>
		/// Currently, a fatal error is any error output line that is not <c>Error: Not a vob object:</c>.
		/// We ignore this error because it occurs any time there is a non-versioned (i.e. compiled .DLL) file
		/// in the viewpath.  But the make label operation completed successfully.
		/// </remarks>
		/// <param name="standardError">the standard error from the process</param>
		/// <returns><c>true</c> if there is a fatal error</returns>
		public bool HasFatalError(string standardError)
		{
			if (standardError == null)
			{
				return false;
			}
			StringReader reader = new StringReader(standardError);
			try
			{
				String line = null;
				while ((line = reader.ReadLine()) != null)
				{
					if (line.IndexOf("Error: Not a vob object:") == -1)
					{
						return true;
					}
				}
				return false;
			}
			finally
			{
				reader.Close();
			}
		}

		public ProcessInfo CreateHistoryProcessInfo(DateTime from, DateTime to)
		{
			string fromDate = from.ToString(DATETIME_FORMAT);
			string args = CreateHistoryArguments(fromDate);
			Log.Debug(string.Format("cleartool commandline: {0} {1}", Executable, args));
			ProcessInfo processInfo = new ProcessInfo(Executable, args);
			return processInfo;
		}

		/// <summary>
		/// Creates a process info object for the process that creates a new label type.
		/// </summary>
		/// <param name="label">the label to apply</param>
		/// <returns>the process execution info</returns>
		public ProcessInfo CreateLabelTypeProcessInfo(string label)
		{
			string args = string.Format(" mklbtype -c \"CRUISECONTROL Comment\" \"{0}\"", label);
			Log.Debug(string.Format("mklbtype: {0} {1}; [working dir: {2}]", Executable, args, ViewPath));
			return new ProcessInfo(Executable, args, ViewPath);
		}

		/// <summary>
		/// Creates a process info object for the process that applies a label.
		/// </summary>
		/// <param name="label">the label to apply</param>
		/// <returns>the process execution info</returns>
		public ProcessInfo CreateMakeLabelProcessInfo(string label)
		{
			string args = string.Format(@" mklabel -recurse ""{0}"" ""{1}""", label, ViewPath);
			Log.Debug(string.Format("mklabel: {0} {1}", Executable, args));
			return new ProcessInfo(Executable, args);
		}

		public ProcessInfo CreateRemoveBaselineProcessInfo()
		{
			string args = string.Format("rmbl -force {0}@\\{1}", TempBaseline, ProjectVobName);
			Log.Debug(string.Format("remove baseline: {0} {1}", Executable, args));
			return new ProcessInfo(Executable, args);
		}

		public ProcessInfo CreateRenameBaselineProcessInfo(string name)
		{
			string args = string.Format("rename baseline:{0}@\\{1} \"{2}\"", TempBaseline, ProjectVobName, name);
			Log.Debug(string.Format("rename baseline: {0} {1}", Executable, args));
			return new ProcessInfo(Executable, args);
		}

		public void ValidateBaselineName(string name)
		{
			if (name == null
				|| name.Length == 0
				|| name.IndexOf(" ") > -1)
			{
				throw new CruiseControlException(string.Format("invalid baseline name: \"{0}\" (Does your prefix have a space in it?)", name));
			}
		}

		private string CreateHistoryArguments(string fromDate)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			builder.AppendArgument("lshist -r -nco");
			builder.AppendIf(Branch != null, "-branch \"{0}\"", Branch);
			builder.AppendArgument("-since {0}", fromDate);
			builder.AppendArgument("-fmt \"%u{0}%Vd{0}%En{0}%Vn{0}%o{0}!%l{0}!%a{0}%Nc", ClearCaseHistoryParser.DELIMITER);
			builder.Append(ClearCaseHistoryParser.END_OF_LINE_DELIMITER + "\\n\"");
			builder.AppendArgument("\"{0}\"", ViewPath);
			return builder.ToString();
		}

		private void RemoveBaseline()
		{
			base.Execute(CreateRemoveBaselineProcessInfo());
		}

		private void RenameBaseline(string name)
		{
			ValidateBaselineConfiguration();
			ValidateBaselineName(name);
			base.Execute(CreateRenameBaselineProcessInfo(name));
		}

		private void ValidateBaselineConfiguration()
		{
			if (UseBaseline
				&& (ProjectVobName == null
					|| ViewName == null))
			{
				throw new CruiseControlException("you must specify the project VOB and view name if UseBaseLine is true");
			}
		}

		public override void GetSource(IIntegrationResult result)
		{
			CreateTemporaryLabel();
			if (AutoGetSource)
			{
				ProcessInfo info = new ProcessInfo(Executable, BuildGetSourceArguments());
				Log.Info(string.Format("Getting source from ClearCase: {0} {1}", info.FileName, info.Arguments));
				Execute(info);
			}
		}

		private string BuildGetSourceArguments()
		{
			return string.Format(@"update -force -overwrite ""{0}""", ViewPath);
		}
	}
}