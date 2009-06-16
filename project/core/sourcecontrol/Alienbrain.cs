using System;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("alienbrain")]
	public class Alienbrain : ProcessSourceControl
	{
		public static readonly string NO_CHANGE = "No files or folders found!";

		public const string AB_REGISTRY_PATH = @"SOFTWARE\NxN\alienbrain";
		public const string AB_REGISTRY_KEY = "InstallDir";
		public const string AB_COMMMAND_PATH = @"Client\Application\Tools";
		public const string AB_EXE = "ab.exe";

		public const string BRANCH_COMMAND_TEMPLATE = @"setactivebranch ""{0}"" -s ""{1}"" -d ""{2}"" -u ""{3}"" -p ""{4}""";
		public const string MODIFICATIONS_COMMAND_TEMPLATE = @"find ""{0}"" -s ""{1}"" -d ""{2}"" -u ""{3}"" -p ""{4}"" -regex ""SCIT > {5} AND SCIT < {6}""  -format ""#CheckInComment#|#Name#|#DbPath#|#SCIT#|#Mime Type#|#LocalPath#|#Changed By#|#NxN_VersionNumber#""";
		public const string LABEL_COMMAND_TEMPLATE = @"setlabel ""{0}"" -s ""{1}"" -d ""{2}"" -u ""{3}"" -p ""{4}"" -name ""{5}"" -comment ""This label is brought to you by CruiseControl.NET""";
		public const string GET_COMMAND_TEMPLATE = @"getlatest ""{0}"" -s ""{1}"" -d ""{2}"" -u ""{3}"" -p ""{4}"" -localpath ""{5}"" -overwritewritable replace -overwritecheckedout replace -response:GetLatest.PathInvalid y -response:GetLatest.Writable y -response:GetLatest.CheckedOut y";

		private IRegistry registry;
		private string executable;

		public Alienbrain() : this(new AlienbrainHistoryParser(), new ProcessExecutor(), new Registry())
		{
		}

		public Alienbrain(IHistoryParser parser, ProcessExecutor executor, IRegistry registry) : base(parser, executor)
		{
			this.registry = registry;
		}

		[ReflectorProperty("executable", Required = false)]
		public string Executable
		{
			get
			{
                if (string.IsNullOrEmpty(executable))
					executable = GetExecutableFromRegistry();
				return executable;
			}
			set { executable = value; }
		}

		[ReflectorProperty("server")]
		public string Server = string.Empty;

		[ReflectorProperty("database")]
		public string Database = string.Empty;

		[ReflectorProperty("username")]
		public string Username = string.Empty;

		[ReflectorProperty("password")]
		public string Password = string.Empty;

		[ReflectorProperty("branch", Required = false)]
		public string Branch = string.Empty;

		[ReflectorProperty("project", Required = true)]
		public string Project = string.Empty;

		[ReflectorProperty("autoGetSource", Required = false)]
		public bool AutoGetSource = true;

		[ReflectorProperty("workingDirectory", Required = false)]
		public string WorkingDirectory = string.Empty;

		[ReflectorProperty("labelOnSuccess", Required=false)]
		public bool LabelOnSuccess = false;

		// Actions
		// I had to bake something here because Alienbrain return ERRORLEVEL 1 when nothing is found
		// that caused an exception in CruiseControl.NET that think that the process failed (not pretty in the logs)
		// So I'm checking the result to see if there is changes, if not I return nothing
		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			ProcessInfo processInfo = CreateModificationProcess(MODIFICATIONS_COMMAND_TEMPLATE, from.StartTime, to.StartTime);
			processInfo.TimeOut = Timeout.Millis;
			ProcessResult result = executor.Execute(processInfo);
			if (!result.StandardOutput.TrimEnd().EndsWith(NO_CHANGE))
			{
                Modification[] modifications = ParseModifications(result, from.StartTime, to.StartTime);
                base.FillIssueUrl(modifications);
                return modifications;
			}
			else
			{
				return new Modification[0];
			}
		}

		public override void LabelSourceControl(IIntegrationResult result)
		{
			if (LabelOnSuccess && result.Succeeded)
			{
				SelectBranch();

				ProcessInfo process = CreateLabelProcess(LABEL_COMMAND_TEMPLATE, result);
				Execute(process);
			}

		}

		public override void GetSource(IIntegrationResult result)
		{
            result.BuildProgressInformation.SignalStartRunTask("Getting source from AlienBrain");

			if (AutoGetSource)
			{
				SelectBranch();

				if (result.Modifications.Length > 0)
				{
					foreach (Modification m in result.Modifications)
					{
						ProcessInfo process = CreateGetProcess(m.FolderName + "/" + m.FileName);
						Execute(process);
					}
				}
				else
				{
					ProcessInfo process = CreateGetProcess();
					Execute(process);
				}
			}
		}

		private string GetExecutableFromRegistry()
		{
			string comServerPath = registry.GetExpectedLocalMachineSubKeyValue(AB_REGISTRY_PATH, AB_REGISTRY_KEY);
			return Path.Combine(comServerPath, AB_COMMMAND_PATH + "\\" + AB_EXE);
		}

		private void SelectBranch()
		{
            if (!string.IsNullOrEmpty(Branch))
			{
				ProcessInfo process = CreateBranchProcess(BRANCH_COMMAND_TEMPLATE);
				Execute(process);
			}
		}

		public bool HasChanges(ProcessInfo p)
		{
			ProcessResult result = Execute(p);
			return !result.StandardOutput.TrimEnd().EndsWith(NO_CHANGE);
		}

		// Process Creations
		public ProcessInfo CreateModificationProcess(string processCommand, DateTime from, DateTime to)
		{
			string arguments = String.Format(processCommand, Project, Server, Database, Username, Password, from.ToFileTime(), to.ToFileTime());
			return new ProcessInfo(Executable, arguments);
		}

		public ProcessInfo CreateLabelProcess(string processCommand, IIntegrationResult result)
		{
			string arguments = String.Format(processCommand, Project, Server, Database, Username, Password, result.Label);
			return new ProcessInfo(Executable, arguments);
		}

		public ProcessInfo CreateGetProcess()
		{
			return CreateGetProcess(Project);
		}

		public ProcessInfo CreateGetProcess(string filename)
		{
			// @"getlatest ""{0}"" -s ""{1}"" -d ""{2}"" -u ""{3}"" -p ""{4}"" -localpath ""{5}"" -overwritewritable replace -overwritecheckedout replace -response:GetLatest.PathInvalid y -response:GetLatest.Writable y -response:GetLatest.CheckedOut y"
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			builder.AddArgument("getlatest", filename);
			builder.AddArgument("-s", Server);
			builder.AddArgument("-d", Database);
			builder.AddArgument("-u", Username);
			builder.AddHiddenArgument("-p", Password);
			builder.AddArgument("-localpath", WorkingDirectory);
			builder.AppendArgument("-overwritewritable replace -overwritecheckedout replace -response:GetLatest.PathInvalid y -response:GetLatest.Writable y -response:GetLatest.CheckedOut y");
			return new ProcessInfo(Executable, builder.ToString());
		}

		public ProcessInfo CreateBranchProcess(string processCommand)
		{
			string arguments = String.Format(processCommand, Branch, Server, Database, Username, Password);
			return new ProcessInfo(Executable, arguments);
		}
	}
}