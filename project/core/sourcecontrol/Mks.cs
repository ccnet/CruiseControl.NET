//-----------------------------------------------------------------------
// <copyright file="Mks.cs" company="CruiseControl.NET">
//     Copyright (c) 2009 CruiseControl.NET. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// MKS Source Integrity Source Control Block.
    /// </summary>
    /// <title>MKS Source Integrity Source Control Block</title>
    /// <version>1.0</version>
    /// <key name="type">
    /// <description>The type of source control block.</description>
    /// <value>mks</value>
    /// </key>
    /// <example>
    /// <code>
    /// &lt;sourceControl type="mks"&gt;
    /// &lt;executable&gt;C:\MKS\bin\si.exe&lt;/executable&gt;
    /// &lt;user&gt;CCNetUser&lt;/user&gt;
    /// &lt;password&gt;CCNetPassword&lt;/password&gt;
    /// &lt;hostname&gt;hostname&lt;/hostname&gt;
    /// &lt;port&gt;8722&lt;/port&gt;
    /// &lt;sandboxroot&gt;C:\MyProject&lt;/sandboxroot&gt;
    /// &lt;sandboxfile&gt;myproject.pj&lt;/sandboxfile&gt;
    /// &lt;autoGetSource&gt;true&lt;/autoGetSource&gt;
    /// &lt;timeout units="minutes"&gt;10&lt;/timeout&gt;
    /// &lt;/sourceControl&gt;
    /// </code>
    /// </example>
    [ReflectorType("mks")]
	public class Mks : ProcessSourceControl
	{
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const string DefaultExecutable = "si.exe";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const int DefaultPort = 8722;
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const bool DefaultAutoGetSource = true;
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const bool DefaultAutoDisconnect = false;
		
		private static int usageCount/* = 0*/;
		private static object usageCountLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="Mks" /> class.	
        /// </summary>
        /// <remarks></remarks>
		public Mks() : this(new MksHistoryParser(), new ProcessExecutor())
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="Mks" /> class.	
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="executor">The executor.</param>
        /// <remarks></remarks>
		public Mks(IHistoryParser parser, ProcessExecutor executor) : base(parser, executor)
		{
            this.Executable = DefaultExecutable;
            this.AutoGetSource = DefaultAutoGetSource;
            this.AutoDisconnect = DefaultAutoDisconnect;
            this.Port = DefaultPort;
		}

        /// <summary>
        /// The local path for the MKS source integrity command-line client (eg. c:\Mks\bin\si.exe).
        /// </summary>
        /// <version>1.0</version>
        /// <default>si.exe</default>
        [ReflectorProperty("executable")]
        public string Executable { get; set; }

        /// <summary>
        /// MKS Source Integrity user ID that CCNet should use.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("user", Required = false)]
        public string User { get; set; }

        /// <summary>
        /// Password for the MKS Source Integrity user ID.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("password", Required = false)]
        public string Password { get; set; }

        /// <summary>
        /// Whether to set a checkpoint on success or not.
        /// </summary>
        /// <version>1.0</version>
        /// <default>false</default>
        [ReflectorProperty("checkpointOnSuccess", Required = false)]
        public bool CheckpointOnSuccess { get; set; }

        /// <summary>
        /// Instruct CCNet whether or not you want it to automatically retrieve the latest source from the repository.
        /// </summary>
        /// <version>1.0</version>
        /// <default>true</default>
        [ReflectorProperty("autoGetSource", Required = false)]
        public bool AutoGetSource { get; set; }
		
		/// <summary>
        /// Whether or not CCNet should automatically disconnect after the sourcecontrol operation has finished.
        /// </summary>
        /// <version>1.0</version>
        /// <default>false</default>
        [ReflectorProperty("autoDisconnect", Required = false)]
        public bool AutoDisconnect { get; set; }

        /// <summary>
        /// The IP address or machine name of the MKS Source Integrity server. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("hostname")]
        public string Hostname { get; set; }

        /// <summary>
        /// The port on the MKS Source Integrity server to connect to. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>8722</default>
        [ReflectorProperty("port", Required = false)]
        public int Port { get; set; }

        /// <summary>
        /// The local path MKS Source Integrity sandbox root corresponds to.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("sandboxroot")]
        public string SandboxRoot { get; set; }

        /// <summary>
        /// The project file.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("sandboxfile")]
        public string SandboxFile { get; set; }

        /// <summary>
        /// Gets the modifications.	
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			IncreaseUsageCount();
			
            // Modifications (includes adds and deletes!)
            ProcessInfo info = NewProcessInfoWithArgs(BuildSandboxModsCommand());
            Log.Info(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Getting Modifications (mods): {0} {1}", info.FileName, info.Arguments));
            Modification[] modifications = GetModifications(info, from.StartTime, to.StartTime);

            AddMemberInfoToModifiedOrAddedModifications(modifications);

            if (!this.CheckpointOnSuccess)
            {
                modifications = FilterOnTimeframe(modifications, from.StartTime, to.StartTime);
            }
 
            DecreaseUsageCount();
		    
		    return modifications;		   
		}

        /// <summary>
        /// Labels the source control.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
		public override void LabelSourceControl(IIntegrationResult result)
		{
			IncreaseUsageCount();
			
			if (CheckpointOnSuccess && result.Succeeded)
			{
				ProcessInfo checkpointProcess = NewProcessInfoWithArgs(BuildCheckpointCommand(result.Label));
				ExecuteWithLogging(checkpointProcess, "Adding Checkpoint");
			}
			
			DecreaseUsageCount();
		}

        /// <summary>
        /// Gets the source.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
		public override void GetSource(IIntegrationResult result)
		{
			IncreaseUsageCount();
			
			if (AutoGetSource)
			{
				ProcessInfo resynchProcess = NewProcessInfoWithArgs(BuildResyncCommand());
				ExecuteWithLogging(resynchProcess, "Resynchronizing source");
				RemoveReadOnlyAttribute();
			}
			
			DecreaseUsageCount();
		}

		private void AddMemberInfoToModifiedOrAddedModifications(Modification[] modifications)
		{
			for (int index = 0; index < modifications.Length; index++)
			{
				Modification modification = modifications[index];
				if ("deleted" != modification.Type)
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
                else if (modification.Type == "deleted")
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
			Log.Info(string.Format(CultureInfo.CurrentCulture, comment + " : {0} {1}", processInfo.FileName, processInfo.PublicArguments));
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
            buffer.AppendArgument("--includeDropped");
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
			buffer.AddArgument("/s", System.IO.Path.Combine(SandboxRoot, "*"));
			Execute(new ProcessInfo("attrib", buffer.ToString()));
		}

		private ProcessInfo NewProcessInfoWithArgs(string args)
		{
			return new ProcessInfo(Executable, args);
		}
		
		private void IncreaseUsageCount()
		{
			lock(usageCountLock)
			{
				usageCount++;
			}
		}
		
		private void DecreaseUsageCount()
		{
			lock(usageCountLock)
			{
				usageCount--;
				
				if(AutoDisconnect && (usageCount == 0))
				{
						ProcessInfo info = NewProcessInfoWithArgs(BuildDisconnectCommand());
						ExecuteWithLogging(info, "Disconnecting from server");
				}
			}			
		}		
		
		private string BuildDisconnectCommand()
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AppendArgument("disconnect");
			buffer.AppendArgument("--user={0}", User);
			buffer.AppendArgument("--password={0}", Password);
			buffer.AppendArgument("--quiet");
			buffer.AppendArgument("--forceConfirm=yes");
			return buffer.ToString();
		}
	}
}