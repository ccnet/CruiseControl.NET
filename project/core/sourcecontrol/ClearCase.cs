using System;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// Rational ClearCase source control block.
    /// </summary>
    /// <title>Rational ClearCase Source Control Block</title>
    /// <version>1.0</version>
    /// <key name="type">
    /// <description>The type of source control block.</description>
    /// <value>clearCase</value>
    /// </key>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;sourcecontrol type="clearCase"&gt;
    /// &lt;viewPath&gt;C:\PATH\TO\SOURCE&lt;/viewPath&gt;
    /// &lt;autoGetSource&gt;true&lt;/autoGetSource&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;sourcecontrol type="clearCase"&gt;
    /// &lt;viewPath&gt;C:\PATH\TO\SOURCE&lt;/viewPath&gt;
    /// &lt;branch&gt;main&lt;/branch&gt;
    /// &lt;autoGetSource&gt;false&lt;/autoGetSource&gt;
    /// &lt;useLabel&gt;true&lt;/useLabel&gt;
    /// &lt;useBaseline&gt;false&lt;/useBaseline&gt;
    /// &lt;projectVobName&gt;PROJECT_VOB_NAME&lt;/projectVobName&gt;
    /// &lt;viewName&gt;PROJECT_VIEW_NAME&lt;/viewName&gt;
    /// &lt;executable&gt;cleartool.exe&lt;/executable&gt;
    /// &lt;timeout&gt;50000&lt;/timeout&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <heading>Common Problems</heading>
    /// <para>
    /// <b>The build is initiated when users check in on private branches</b>
    /// </para>
    /// <para>
    /// By default, ClearCase returns a history for every file in every branch, even if the config spec limits to a
    /// single branch. You must specify &lt;branch&gt; in order to limit which changes CCNet can see.
    /// </para>
    /// <para>
    /// <b>After the build is successful I get a "Baseline not found" error message.</b>
    /// </para>
    /// <para>
    /// An example of this message is:
    /// </para>
    /// <code type="None">
    /// ThoughtWorks.CruiseControl.Core.CruiseControlException: Source control operation failed:
    /// cleartool: Error: Baseline not found: "CruiseControl.NETTemporaryBaseline_05-06-2004-16-34-15".
    /// </code>
    /// <para>
    /// This happens when &lt;projectVobName&gt; is not set to the project VOB. Typically this happens when the user
    /// specifies the UCM VOB instead of the project VOB.
    /// </para>
    /// <para>
    /// To correct the problem, change the value in that element to the name of the project VOB.
    /// </para>
    /// <heading>Known Bugs</heading>
    /// <para>
    /// <b>When I view my baselines, I see that they're called CruiseControl.NET[something] instead of v1.0.0.4.</b>
    /// </para>
    /// <para>
    /// This is a bug in ClearCase; Rational is aware of it. It only occurs if you're using baselines.
    /// </para>
    /// <para>
    /// CCNet creates a temporary baseline with the prefix CruiseControl.NET before renaming it to the final value, such
    /// as v1.5.2.3. Depending on how you view baselines in ClearCase, you may see the temporary or real name.
    /// </para>
    /// <para>
    /// For example, if you use the admin console, you'll see the old, temporary value. If use use cleartool lsbl,
    /// you'll see the correct one:
    /// </para>
    /// <code type="None">
    /// M:\gsmith_GS_Project_int\GS_UCM_VOB&gt;cleartool lsbl
    /// 06-May-04.16:28:27  v1.0.0.1  gsmith   "CruiseControlTemporaryBaseline_05-06-200
    /// 4-16-28-26"
    ///   stream: GS_Project_Integration@\GS_PVOB
    ///   component: GS_UCM_VOB@\GS_PVOB
    /// 06-May-04.16:34:16  v1.0.0.2  gsmith   "CruiseControl.NETTemporaryBaseline_05-06
    /// -2004-16-34-15"
    ///   stream: GS_Project_Integration@\GS_PVOB
    ///   component: GS_UCM_VOB@\GS_PVOB
    /// </code>
    /// <para>
    /// <b>CruiseControl.NET sees checkins on all branches, not just the one specified in my config spec</b>
    /// </para>
    /// <para>
    /// This is due to the fact that the ClearCase history command (lshist) returns a complete history for the file, not
    /// just the history that can be seen by the config spec.
    /// </para>
    /// <para>
    /// The workaround is to make sure you include a &lt;branch&gt; element in your configuration. This will force ccnet
    /// to just see changes on that branch.
    /// </para>
    /// <para>
    /// <b>CruiseControl.NET doesn't see my changes</b>
    /// </para>
    /// <para>
    /// Make sure the clock of your build server is synchronised to the clock of your ClearCase server.
    /// </para>
    /// </remarks>
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

        /// <summary>
        /// Specifies the path to the ClearCase command line tool. You should only have to include this element if the
        /// tool isn't in your path. By default, the ClearCase client installation puts cleartool in your path. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>cleartool.exe</default>
        [ReflectorProperty("executable", Required = false)]
		public string Executable = "cleartool.exe";

        /// <summary>
        /// The name of the project VOB that the view path uses. 
        /// </summary>
        /// <remarks>
        /// This is required if useBaseline="true".
        /// </remarks>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("projectVobName", Required = false)]
		public string ProjectVobName;

        /// <summary>
        /// Specifies whether a baseline should be applied when the build is successful. Requires the VOB your view
        /// references to be a UCM VOB. 
        /// </summary>
        /// <remarks>
        /// Requires that you specify viewName and projectVobName. 
        /// </remarks>
        /// <version>1.0</version>
        /// <default>false</default>
        [ReflectorProperty("useBaseline", Required = false)]
		public bool UseBaseline = false;

        /// <summary>
        /// Specifies whether a label should be applied when the build is successful. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>true</default>
        [ReflectorProperty("useLabel", Required = false)]
		public bool UseLabel = true;

        /// <summary>
        /// The name of the view that you're using. 
        /// </summary>
        /// <remarks>
        /// This is required if useBaseline="true".
        /// </remarks>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("viewName", Required = false)]
		public string ViewName;

        /// <summary>
        /// The path that CCNet will check for modifications and use to apply the label. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        /// <remarks>
        /// Specifies a directory on your filesystem that CCNet monitors for changes. The path must be a versioned
        /// object. CCNet checks the actual VOB for changes, not the local filesystem.
        /// This doesn't have to be the root of the local ClearCase view. It may be any of the root's children or even
        /// a single object.
        /// </remarks>
		[ReflectorProperty("viewPath", Required=false)]
		public string ViewPath;

        /// <summary>
        /// Specifies whether the current version of the source should be retrieved from ClearCase.
        /// </summary>
        /// <version>1.0</version>
        /// <default>true</default>
        [ReflectorProperty("autoGetSource", Required = false)]
		public bool AutoGetSource = true;

        /// <summary>
        /// The name of the branch that CCNet will monitor for modifications. Note that the config spec of the view
        /// being built from must also be set up to reference this branch.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("branch", Required = false)]
		public string Branch;

		public string TempBaseline;

		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
            Modification[] modifications = base.GetModifications(CreateHistoryProcessInfo(from.StartTime, to.StartTime), from.StartTime, to.StartTime);
            base.FillIssueUrl(modifications);
            return modifications;
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
			info.TimeOut = Timeout.Millis;
			ProcessResult result = executor.Execute(info);

			if (result.TimedOut)
			{
				throw new CruiseControlException("Source control operation has timed out.");
			}
			else if (result.Failed && HasFatalError(result.StandardError))
			{
				throw new CruiseControlException(string.Format("Source control operation failed: {0}. Process command: {1} {2}",
				                                               result.StandardError, info.FileName, info.SafeArguments));
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
            result.BuildProgressInformation.SignalStartRunTask("Getting source from ClearCase");

			CreateTemporaryLabel();
			if (AutoGetSource)
			{
				ProcessInfo info = new ProcessInfo(Executable, BuildGetSourceArguments());
				Log.Info(string.Format("Getting source from ClearCase: {0} {1}", info.FileName, info.SafeArguments));
				Execute(info);
			}
		}

		private string BuildGetSourceArguments()
		{
			return string.Format(@"update -force -overwrite ""{0}""", ViewPath);
		}
	}
}