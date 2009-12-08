using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.BitKeeper
{
    /// <summary>
    /// <para>
    /// Source control integration for the BitKeeper source control product.
    /// </para>
    /// </summary>
    /// <title>BitKeeper Source Control Block</title>
    /// <version>1.0</version>
    /// <example>
    /// <code>
    /// &lt;sourcecontrol type="bitkeeper"&gt;
    /// &lt;workingDirectory&gt;c:\build\dev-1.0&lt;/workingDirectory&gt;
    /// &lt;autoGetSource&gt;true&lt;/autoGetSource&gt;
    /// &lt;cloneTo&gt;..\Source&lt;/cloneTo&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// </example>
    /// <key name="type">
    /// <description>The type of source control block.</description>
    /// <value>bitkeeper</value>
    /// </key>
    /// <remarks>
    /// <para type="warning">
    /// <title>SSH Access Not Supported</title>
    /// Your permanent BK repository must have a parent accessed via bkd or the local filesystem; ssh access is not supported at this time.
    /// </para>
    /// <heading>Contributions</heading>
    /// <para>
    /// BitKeeper support added by Harold L Hunt II of StarNet Communications Corp.
    /// </para>
    /// </remarks>
    [ReflectorType("bitkeeper")]
	public class BitKeeper : ProcessSourceControl
	{
		public const string DefaultExecutable = @"C:\Program Files\BitKeeper\bk.exe";

		public BitKeeper(IHistoryParser parser, ProcessExecutor executor) : base(parser, executor)
		{}

		public BitKeeper() : base(new BitKeeperHistoryParser())
		{}

		/// <summary>
		/// Absolute, DOS-style, path to bk.exe.
		/// </summary>
        /// <version>1.0</version>
        /// <default>c:\Program Files\BitKeeper\bk.exe</default>
		[ReflectorProperty("executable", Required=false)]
		public string Executable = DefaultExecutable;

		/// <summary>
		/// Absolute, DOS-style, path to permanent BK repository.
		/// </summary>
        /// <version>1.0</version>
        /// <default>Project Working Directory</default>
        [ReflectorProperty("workingDirectory", Required = true)]
		public string WorkingDirectory = string.Empty;

		/// <summary>
		/// Add BK tag on successful build.
		/// </summary>
        /// <version>1.0</version>
        /// <default>false</default>
        [ReflectorProperty("tagOnSuccess", Required = false)]
		public bool TagOnSuccess = false;

		/// <summary>
		/// Automatically pull latest source into permanent BK repository.
		/// </summary>
        /// <version>1.0</version>
        /// <default>true</default>
        [ReflectorProperty("autoGetSource", Required = false)]
		public bool AutoGetSource = true;

		/// <summary>
		/// Include history of each file, rather than just ChangeSets.
		/// </summary>
        /// <version>1.0</version>
        /// <default>false</default>
        [ReflectorProperty("fileHistory", Required = false)]
		public bool FileHistory = false;

		/// <summary>
		/// Make a clone of the permanent BK repository into the designated path. The DOS-style path can be relative to WorkingDirectory or
        /// absolute.
		/// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("cloneTo", Required = false)]
		public string CloneTo = string.Empty;

		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			ProcessResult result = Execute(NewProcessInfo(BuildHistoryProcessArgs(), to));
			Modification[] modifications = ParseModifications(result, from.StartTime, to.StartTime);
            base.FillIssueUrl(modifications);
            return modifications;
		}

		public override void LabelSourceControl(IIntegrationResult result)
		{
			if (TagOnSuccess && result.Succeeded)
			{
				Execute(NewLabelProcessInfo(result));
				Execute(NewProcessInfo(BuildPushProcessArgs(), result));
			}
		}

		public override void GetSource(IIntegrationResult result)
		{
            result.BuildProgressInformation.SignalStartRunTask("Getting source from BitKeeper");

			if (!AutoGetSource)
				return;

			// Get the latest source
			ProcessInfo info = NewProcessInfo(BuildGetSourceArguments(), result);
			Log.Info(string.Format("Getting source from BitKeeper: {0} {1}", info.FileName, info.SafeArguments));
			Execute(info);

			// Push any pending labels that failed to push due to remote side having additional revisions
			Execute(NewProcessInfo(BuildPushProcessArgs(), result));

			if (CloneTo != string.Empty) CloneSource(result);
		}

		private void CloneSource(IIntegrationResult result)
		{
			string clonePath = CloneTo;
	
			// Prepend working directory if path was not absolute
			if (!Path.IsPathRooted(clonePath))
				clonePath = Path.Combine(WorkingDirectory, CloneTo);
			clonePath = Path.GetFullPath(clonePath);
	
			// Delete old destination
			DirectoryInfo di = new DirectoryInfo(clonePath);
			try
			{
				if (di.Exists)
					new IoService().DeleteIncludingReadOnlyObjects(clonePath);
			}
			catch
			{}
	
			ProcessInfo ctInfo = NewProcessInfo(BuildCloneToArguments(), result);
			Log.Info(string.Format("Cloning source to: {0}", clonePath));
			Execute(ctInfo);
		}

		private ProcessInfo NewLabelProcessInfo(IIntegrationResult result)
		{
			return NewProcessInfo(BuildTagProcessArgs(result.Label), result);
		}

		private string BuildTagProcessArgs(string label)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AppendArgument("tag");
			buffer.AppendArgument(label);
			return buffer.ToString();
		}

		private string BuildPushProcessArgs()
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AppendArgument("push");
			return buffer.ToString();
		}

		private string BuildHistoryProcessArgs()
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AppendArgument("changes");
			buffer.AppendArgument("-R");
			if (FileHistory)
				buffer.AppendArgument("-v");
			return buffer.ToString();
		}

		private string BuildGetSourceArguments()
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.Append("pull");
			return buffer.ToString();
		}

		private string BuildCloneToArguments()
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AppendArgument("clone");
			buffer.AppendArgument(".");
			buffer.AppendArgument(CloneTo);
			return buffer.ToString();
		}

		private ProcessInfo NewProcessInfo(string args, IIntegrationResult result)
		{
			ProcessInfo pi = new ProcessInfo(Executable, args, result.BaseFromWorkingDirectory(WorkingDirectory));
			// Needed to disable the pager for bk commands, which causes infinite hangs
			pi.EnvironmentVariables.Add("PAGER", "cat");
			return pi;
		}
	}
}