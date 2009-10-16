//-----------------------------------------------------------------------
// <copyright file="Mks.cs" company="CruiseControl.NET">
//     Copyright (c) 2009 CruiseControl.NET. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;

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
            // Modifications (includes adds and deletes!)
            ProcessInfo info = NewProcessInfoWithArgs(BuildSandboxModsCommand());
            Log.Info(string.Format("Getting Modifications (mods): {0} {1}", info.FileName, info.Arguments));
            Modification[] modifications = GetModifications(info, from.StartTime, to.StartTime);

            AddMemberInfoToModifiedOrAddedModifications(modifications);

            if (!this.CheckpointOnSuccess)
            {
                modifications = FilterOnTimeframe(modifications, from.StartTime, to.StartTime);
            }

		    return modifications;
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
			if (AutoGetSource)
			{
				ProcessInfo resynchProcess = NewProcessInfoWithArgs(BuildResyncCommand());
				ExecuteWithLogging(resynchProcess, "Resynchronizing source");
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

        private Modification[] FilterOnTimeframe(Modification[] modifications, DateTime from, DateTime to)
        {
            List<Modification> mods = new List<Modification>();

            for (int index = 0; index < modifications.Length; index++)
            {
                Modification modification = modifications[index];
                if ( modification.ModifiedTime >= from && modification.ModifiedTime <= to )
                {
                    mods.Add(modification);
                }
            }

            return mods.ToArray();
        }

		private void AddMemberInfo(Modification modification)
		{
            ProcessInfo memberInfoProcess = NewProcessInfoWithArgs(BuildMemberInfoCommandXml(modification));
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
            buffer.AppendArgument("--forceConfirm=yes");
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

	    //VIEEWSANDBOX_TEMPLATE = "viewsandbox -R {SandboxRoot\SandboxFile} --user={user} --password={password} --quiet --xmlapi"
        private string BuildSandboxModsCommand()
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AppendArgument("viewsandbox --nopersist --filter=changed:all --xmlapi");
            AppendCommonArguments(buffer, true);
            return buffer.ToString();
        }

	    //MEMBER_INFO_TEMPLATE = "memberinfo -S {SandboxRoot\SandboxFile} --user={user} --password={password} {member}"
        private string BuildMemberInfoCommandXml(Modification modification)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AppendArgument("memberinfo --xmlapi");
            AppendCommonArguments(buffer, false, true);
            string modificationPath = (modification.FolderName == null) ? SandboxRoot : Path.Combine(SandboxRoot, modification.FolderName);
            buffer.AddArgument(Path.Combine(modificationPath, modification.FileName));
            return buffer.ToString();
        }

		private void AppendCommonArguments(ProcessArgumentBuilder buffer, bool recurse)
		{
            AppendCommonArguments(buffer, recurse, false);
		}

        private void AppendCommonArguments(ProcessArgumentBuilder buffer, bool recurse, bool omitSandbox)
        {
            if (recurse)
            {
                buffer.AppendArgument("-R");
            }

            if (!omitSandbox)
            {
                buffer.AddArgument("-S", Path.Combine(SandboxRoot, SandboxFile));
            }

            buffer.AppendArgument("--user={0}", User);
            buffer.AppendArgument("--password={0}", Password);
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