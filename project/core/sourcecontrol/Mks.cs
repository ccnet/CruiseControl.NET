using System;
using System.Collections;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("mks")]
	public class Mks : ProcessSourceControl
	{
		public const string DefaultExecutable = "si.exe";
		public const int DefaultPort = 8722;
		public const bool DefaultAutoGetSource = true;

		public Mks() : this(new MksHistoryParser(), new ProcessExecutor())
		{
		}

		public Mks(IHistoryParser parser, ProcessExecutor executor) : base(parser, executor)
		{
		}

		[ReflectorProperty("executable")]
		public string Executable = DefaultExecutable;

		[ReflectorProperty("user", Required = false)]
		public string User;

		[ReflectorProperty("password", Required = false)]
		public string Password;

		[ReflectorProperty("checkpointOnSuccess", Required = false)]
		public bool CheckpointOnSuccess;

		[ReflectorProperty("autoGetSource", Required=false)]
		public bool AutoGetSource = DefaultAutoGetSource;

		[ReflectorProperty("hostname")]
		public string Hostname;

		[ReflectorProperty("port", Required=false)]
		public int Port = DefaultPort;

		[ReflectorProperty("sandboxroot")]
		public string SandboxRoot;

		[ReflectorProperty("sandboxfile")]
		public string SandboxFile;

		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			ProcessInfo info = NewProcessInfoWithArgs(BuildModsCommand());
			Log.Info(string.Format("Getting Modifications: {0} {1}", info.FileName, info.SafeArguments));
			Modification[] modifications = GetModifications(info, from.StartTime, to.StartTime);
			AddMemberInfoToModifiedOrAddedModifications(modifications);
            base.FillIssueUrl(modifications);
			return ValidModifications(modifications, from.StartTime, to.StartTime);
		}

		/* as the "mods" command gets modifications to a project between checkpoints on the working project,
		 * if CheckpointOnSuccess is set to false the modifications are filtered according to the modified time of the files
		 * */

		private Modification[] ValidModifications(Modification[] modifications, DateTime from, DateTime to)
		{
			if (CheckpointOnSuccess) return modifications;
			ArrayList validModifications = new ArrayList();
			for (int i = 0; i < modifications.Length; i++)
			{
				if (from <= modifications[i].ModifiedTime && to >= modifications[i].ModifiedTime)
				{
					validModifications.Add(modifications[i]);
				}
			}
			return (Modification[]) validModifications.ToArray(typeof (Modification));
		}

		public override void LabelSourceControl(IIntegrationResult result)
		{
			if (CheckpointOnSuccess && result.Succeeded)
			{
				ProcessInfo checkpointProcess = NewProcessInfoWithArgs(BuildCheckpointCommand(result.Label));
				ExecuteWithLogging(checkpointProcess, "Adding Checkpoint");
			}
		}

		public override void GetSource(IIntegrationResult result)
		{
            result.BuildProgressInformation.SignalStartRunTask("Getting source from MKS");

			if (AutoGetSource)
			{
				ProcessInfo resynchProcess = NewProcessInfoWithArgs(BuildResyncCommand());
				ExecuteWithLogging(resynchProcess, "Resynchronising source");
				RemoveReadOnlyAttribute();
			}
		}

		private void AddMemberInfoToModifiedOrAddedModifications(Modification[] modifications)
		{
			for (int index = 0; index < modifications.Length; index++)
			{
				Modification modification = modifications[index];
				if ("Deleted" != modification.Type)
				{
					AddMemberInfo(modification);
				}
			}
		}

		private void AddMemberInfo(Modification modification)
		{
			ProcessInfo memberInfoProcess = NewProcessInfoWithArgs(BuildMemberInfoCommand(modification));
			ProcessResult result = Execute(memberInfoProcess);
			((MksHistoryParser) historyParser).ParseMemberInfoAndAddToModification(modification, new StringReader(result.StandardOutput));
		}

		private void ExecuteWithLogging(ProcessInfo processInfo, string comment)
		{
			Log.Info(string.Format(comment + " : {0} {1}", processInfo.FileName, processInfo.SafeArguments));
			Execute(processInfo);
		}

		//RESYNC_TEMPLATE = "resync --overwriteChanged --restoreTimestamp-R -S {SandboxRoot\SandboxFile} --user={user} --password={password} --quiet"
		private string BuildResyncCommand()
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AppendArgument("resync");
			buffer.AppendArgument("--overwriteChanged");
			buffer.AppendArgument("--restoreTimestamp");
			AppendCommonArguments(buffer, true);
			return buffer.ToString();
		}

		//CHECKPOINT_TEMPLATE = "checkpoint -d "Cruise Control.Net Build -{lebel}" -L "CCNET Build - {lebel}" -R -S {SandboxRoot\SandboxFile} --user={user} --password={password} --quiet"
		private string BuildCheckpointCommand(string label)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AppendArgument("checkpoint");
			buffer.AppendArgument("-d \"Cruise Control.Net Build - {0}\"", label);
			buffer.AppendArgument("-L \"Build - {0}\"", label);
			AppendCommonArguments(buffer, true);
			return buffer.ToString();
		}

		//MODS_TEMPLATE = "mods -R -S {SandboxRoot\SandboxFile} --user={user} --password={password} --quiet"
		private string BuildModsCommand()
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AppendArgument("mods");
			AppendCommonArguments(buffer, true);
			return buffer.ToString();
		}

		//MEMBER_INFO_TEMPLATE = "memberinfo -S {SandboxRoot\SandboxFile} --user={user} --password={password} {member}"
		private string BuildMemberInfoCommand(Modification modification)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AppendArgument("memberinfo");
			AppendCommonArguments(buffer, false);
			string modificationPath = (modification.FolderName == null) ? SandboxRoot : Path.Combine(SandboxRoot, modification.FolderName);
			buffer.AddArgument(Path.Combine(modificationPath, modification.FileName));
			return buffer.ToString();
		}

		private void AppendCommonArguments(ProcessArgumentBuilder buffer, bool recurse)
		{
			if (recurse)
			{
				buffer.AppendArgument("-R");
			}
			buffer.AddArgument("-S", Path.Combine(SandboxRoot, SandboxFile));
			buffer.AppendArgument("--user={0}", User);
			buffer.AppendHiddenArgument("--password={0}", Password);
			buffer.AppendArgument("--quiet");
		}

		private void RemoveReadOnlyAttribute()
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AddArgument("-R");
			buffer.AddArgument("/s", SandboxRoot + "\\*");
			Execute(new ProcessInfo("attrib", buffer.ToString()));
		}

		private ProcessInfo NewProcessInfoWithArgs(string args)
		{
			return new ProcessInfo(Executable, args);
		}
	}
}