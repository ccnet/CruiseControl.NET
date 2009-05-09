using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce
{
	[ReflectorType("p4")]
	public class P4 : ISourceControl
	{
		private readonly IP4Purger p4Purger;
		internal static readonly string COMMAND_DATE_FORMAT = "yyyy/MM/dd:HH:mm:ss";
        internal static readonly string EXIT_CODE_PATTERN = @"^exit: (?<ExitCode>\d+)";
        internal static readonly string DEFAULT_ERROR_PATTERN = @"^error: .*";
        internal static readonly string FILES_UP_TO_DATE_PATTERN = @"file\(s\) up-to-date\.";
        internal static readonly RegexOptions DEFAULT_REGEX_OPTIONS = RegexOptions.IgnoreCase | RegexOptions.Multiline;

		private readonly ProcessExecutor processExecutor;
		private readonly IP4Initializer p4Initializer;
		private readonly IP4ProcessInfoCreator processInfoCreator;

		public P4()
		{
			processExecutor = new ProcessExecutor();
			processInfoCreator = new P4ConfigProcessInfoCreator();
			p4Initializer = new ProcessP4Initializer(processExecutor, processInfoCreator);
			p4Purger = new ProcessP4Purger(processExecutor, processInfoCreator);
		}

		public P4(ProcessExecutor processExecutor, IP4Initializer initializer, IP4Purger p4Purger, IP4ProcessInfoCreator processInfoCreator)
		{
			this.processExecutor = processExecutor;
			p4Initializer = initializer;
			this.processInfoCreator = processInfoCreator;
			this.p4Purger = p4Purger;
		}

		[ReflectorProperty("executable", Required=false)]
		public string Executable = "p4";

		[ReflectorProperty("view")]
		public string View;

		[ReflectorProperty("client", Required=false)]
		public string Client = string.Empty;

		[ReflectorProperty("user", Required=false)]
		public string User = string.Empty;

		[ReflectorProperty("password", Required=false)]
		public string Password = string.Empty;

		[ReflectorProperty("port", Required=false)]
		public string Port = string.Empty;

		[ReflectorProperty("workingDirectory", Required=false)]
		public string WorkingDirectory = string.Empty;

		[ReflectorProperty("applyLabel", Required = false)]
		public bool ApplyLabel = false;

		[ReflectorProperty("autoGetSource", Required = false)]
		public bool AutoGetSource = true;

		[ReflectorProperty("forceSync", Required = false)]
		public bool ForceSync = false;

		[ReflectorProperty(@"p4WebURLFormat", Required=false)]
		public string P4WebURLFormat;

		[ReflectorProperty("timeZoneOffset", Required=false)]
		public double TimeZoneOffset = 0;

        [ReflectorProperty("errorPattern", Required = false)]
        public string ErrorPattern = DEFAULT_ERROR_PATTERN;

        [ReflectorProperty("useExitCode", Required = false)]
        public bool UseExitCode = false;

        [ReflectorArray("acceptableErrors", Required = false)]
        public string[] AcceptableErrors = new string[1] { FILES_UP_TO_DATE_PATTERN };


		private string BuildModificationsCommandArguments(DateTime from, DateTime to)
		{
			return string.Format("changes -s submitted {0}", GenerateRevisionsForView(from, to));
		}

		private string GenerateRevisionsForView(DateTime from, DateTime to)
		{
			StringBuilder args = new StringBuilder();
			foreach (string viewline in View.Split(','))
			{
				if (args.Length > 0) args.Append(' ');
				args.Append(viewline);
				if (from == DateTime.MinValue)
				{
					args.Append("@" + FormatDate(to));
				}
				else
				{
					args.Append(string.Format("@{0},@{1}", FormatDate(from), FormatDate(to)));
				}
			}
			return args.ToString();
		}

		private string FormatDate(DateTime date)
		{
			DateTime offsetDate = date.AddHours(TimeZoneOffset);
			return offsetDate.ToString(COMMAND_DATE_FORMAT, CultureInfo.InvariantCulture);
		}

		public virtual ProcessInfo CreateChangeListProcess(DateTime from, DateTime to)
		{
			return processInfoCreator.CreateProcessInfo(this, BuildModificationsCommandArguments(from, to));
		}

		public virtual ProcessInfo CreateDescribeProcess(string changes)
		{
			if (changes.Length == 0)
				throw new Exception("Empty changes list found - this should not happen");

			foreach (char c in changes)
			{
				if (! (Char.IsDigit(c) || c == ' '))
					throw new CruiseControlException("Invalid changes list encountered");
			}

			return processInfoCreator.CreateProcessInfo(this, "describe -s " + changes);
		}

		public Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			P4HistoryParser parser = new P4HistoryParser();
			ProcessInfo process = CreateChangeListProcess(from.StartTime, to.StartTime);
			string processResult = Execute(process, "GetModifications");
			String changes = parser.ParseChanges(processResult);
			if (changes.Length == 0)
			{
				return new Modification[0];
			}
			else
			{
				process = CreateDescribeProcess(changes);
				Modification[] mods = parser.Parse(new StringReader(Execute(process, "GetModifications")), from.StartTime, to.StartTime);
                if (!string.IsNullOrEmpty(P4WebURLFormat))
				{
					foreach (Modification mod in mods)
					{
						mod.Url = string.Format(P4WebURLFormat, mod.ChangeNumber);
					}
				}
                FillIssueUrl(mods);
				return mods;
			}
		}

		/// <summary>
		/// Labelling in Perforce requires 2 activities. First you create a 'label specification' which is the name of the label, and what
		/// part of the source repository it is associated with. Secondly you actually populate the label with files and associated
		/// revisions by performing a 'label sync'. We take the versioned file set as being the versions that are currently 
		/// checked out on the client (In theory this could be refined by using the timeStamp, but it would be better
		/// to wait until CCNet has proper support for atomic-commit change groups, and use that instead)
		/// </summary>
		public void LabelSourceControl(IIntegrationResult result)
		{
			if (ApplyLabel && result.Succeeded)
			{
				if (result.Label == null || result.Label.Length == 0)
					throw new ApplicationException("Internal Exception - Invalid (null or empty) label passed");

				try
				{
					int.Parse(result.Label);
					throw new CruiseControlException("Perforce cannot handle purely numeric labels - you must use a label prefix for your project");
				}
				catch (FormatException)
				{}
				ProcessInfo process = CreateLabelSpecificationProcess(result.Label);
				Execute(process, "LabelSourceControl");

                process = CreateLabelSyncProcess(result.Label);
                Execute(process, "LabelSourceControl");
			}
		}

		private string ParseErrors(string processOutput, string proccessError)
		{
            Match exitCodeMatch = Regex.Match(processOutput, EXIT_CODE_PATTERN, DEFAULT_REGEX_OPTIONS);
            StringBuilder errorSummary = new StringBuilder();

            if (UseExitCode) 
            {
                // "exit: 0" indicates success
                if (exitCodeMatch.Success && Equals(exitCodeMatch.Groups["ExitCode"].Value, "0"))
                {
                    return null;
                }
                errorSummary.Append("Failed since 'exit: 0' string was not found in UseExitCode mode.\r\n");
            }

            // append all standard error data
            if (!StringUtil.IsWhitespace(proccessError))
            {
                errorSummary.Append(proccessError);
                errorSummary.Append("\r\n");
            }

            // append standard output error lines
            if (!StringUtil.IsWhitespace(ErrorPattern))
            {
                foreach (Match errorMatch in Regex.Matches(processOutput, ErrorPattern, DEFAULT_REGEX_OPTIONS))
                {
                    if (!IsAcceptableError(errorMatch.Groups[0].Value))
                    {
                        errorSummary.Append(errorMatch.Groups[0].Value);
                        errorSummary.Append("\n");
                    }
                }
            }

            // no errors indicates sucess
            if (errorSummary.Length == 0)
            {
                return null;
            }

            // append the exit code line last after all other errors
            if (exitCodeMatch.Success)
            {
                errorSummary.AppendLine(exitCodeMatch.Groups[0].Value);
            }

            return errorSummary.ToString();
        }

        private bool IsAcceptableError(string errorLine)
        {
            foreach (string acceptableError in AcceptableErrors)
            {
                if (!StringUtil.IsWhitespace(acceptableError) && Regex.IsMatch(errorLine, acceptableError, DEFAULT_REGEX_OPTIONS))
                {
                    Log.Debug(String.Format("Perforce ignored an acceptable error: {0}", errorLine));
                    return true;
                }
            }
            return false;
		}

		private ProcessInfo CreateLabelSpecificationProcess(string label)
		{
			ProcessInfo processInfo = processInfoCreator.CreateProcessInfo(this, "label -i");
			processInfo.StandardInputContent = string.Format("Label:	{0}\n\nDescription:\n	Created by CCNet\n\nOptions:	unlocked\n\nView:\n{1}", label, ViewForSpecificationsAsNewlineSeparatedString);
			return processInfo;
		}

		public virtual string[] ViewForSpecifications
		{
			get
			{
				ArrayList viewLineList = new ArrayList();
				foreach (string viewLine in View.Split(','))
				{
					viewLineList.Add(viewLine);
				}
				return (string[]) viewLineList.ToArray(typeof (string));
			}
		}

		private string ViewForSpecificationsAsNewlineSeparatedString
		{
			get
			{
				StringBuilder builder = new StringBuilder();
				foreach (string viewLine in ViewForSpecifications)
				{
					builder.Append(" ");
					builder.Append(viewLine);
					builder.Append("\n");
				}
				return builder.ToString();
			}
		}

		public string ViewForDisplay
		{
			get { return View.Replace(",", Environment.NewLine); }
		}

		private ProcessInfo CreateLabelSyncProcess(string label)
		{
			return processInfoCreator.CreateProcessInfo(this, "labelsync -l " + label);
		}

		public void GetSource(IIntegrationResult result)
		{
            result.BuildProgressInformation.SignalStartRunTask("Getting source from Perforce");

			if (AutoGetSource)
			{
                ProcessInfo process = processInfoCreator.CreateProcessInfo(this, CreateSyncCommandLine(result.StartTime));
                Execute(process, "GetSource");
			}
		}

        private string CreateSyncCommandLine(DateTime modificationsToDate)
		{
			string commandline = "sync";
			if (ForceSync)
			{
				commandline += " -f";
			}

            commandline += " @" + FormatDate(modificationsToDate);

			return commandline;
		}

		protected virtual string Execute(ProcessInfo process, string description)
		{
            Log.Info(String.Format("Perforce {0}: {1} {2}", description, process.FileName, process.Arguments));
			ProcessResult result = processExecutor.Execute(process);
            string errorSummary = ParseErrors(result.StandardOutput, result.StandardError);
            if (errorSummary != null)
            {
                string errorMessage =
                    string.Format(
                        "Perforce {0} failed: {1} {2}\r\nError output from process was: \r\n{3}",
                        description, process.FileName, process.Arguments, errorSummary);
                Log.Error(errorMessage);
                throw new CruiseControlException(errorMessage);
            }
            return result.StandardOutput.Trim() + Environment.NewLine + result.StandardError.Trim();
		}

		public void Initialize(IProject project)
		{
            if (string.IsNullOrEmpty(WorkingDirectory))
			{
				p4Initializer.Initialize(this, project.Name, project.WorkingDirectory);
			}
			else
			{
				p4Initializer.Initialize(this, project.Name, WorkingDirectory);
			}
		}

		public void Purge(IProject project)
		{
            if (string.IsNullOrEmpty(WorkingDirectory))
			{
				p4Purger.Purge(this, project.WorkingDirectory);
			}
			else
			{
				p4Purger.Purge(this, WorkingDirectory);
			}
		}

        [ReflectorProperty("issueUrlBuilder", InstanceTypeKey = "type", Required = false)]
        public IModificationUrlBuilder IssueUrlBuilder;


        private void FillIssueUrl(Modification[] modifications)
        {
            if (IssueUrlBuilder != null)
            {
                IssueUrlBuilder.SetupModification(modifications);
            }
        }
	}
}