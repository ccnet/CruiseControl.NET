namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;

    /// <summary>
    /// Perforce source control block.
    /// </summary>
    /// <title>Perforce Source Control Block</title>
    /// <version>1.0</version>
    /// <key name="type">
    /// <description>The type of source control block.</description>
    /// <value>p4</value>
    /// </key>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;sourcecontrol type="p4"&gt;
    /// &lt;view&gt;//projects/myproject/...&lt;/view&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;sourcecontrol type="p4"&gt;
    /// &lt;view&gt;//projects/ccnet/...,//tools/nant/...&lt;/view&gt;
    /// &lt;executable&gt;c:\perforce\p4.exe&lt;/executable&gt;
    /// &lt;client&gt;ccnet-buildhost&lt;/client&gt;
    /// &lt;user&gt;public&lt;/user&gt;
    /// &lt;password&gt;mypassword&lt;/password&gt;
    /// &lt;port&gt;perforce01.thoughtworks.net:1666&lt;/port&gt;
    /// &lt;timeZoneOffset&gt;-5&lt;/timeZoneOffset&gt;
    /// &lt;applyLabel&gt;true&lt;/applyLabel&gt;
    /// &lt;autoGetSource&gt;true&lt;/autoGetSource&gt;
    /// &lt;forceSync&gt;true&lt;/forceSync&gt;
    /// &lt;p4WebURLFormat&gt;http://perforceWebServer:8080/@md=d&amp;amp;cd=//&amp;amp;c=3IB@/{0}?ac=10&lt;/p4WebURLFormat&gt;
    /// &lt;timeout units="minutes"&gt;10&lt;/timeout&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <heading>Client / User / Password / Port usage</heading>
    /// <para>
    /// You may specify any of the Perforce client, user, password and port (ie 'host:port' in Perforce standards) settings to use. If you
    /// don't specify them, then Cruise Control .NET will use the defaults in your registry (use 'p4 set' to view and edit these.)
    /// </para>
    /// <para>
    /// Note that the client specification is only used for syncing and applying labels, it is not used when checking for changes. This is
    /// significant since it means that the view in the specified client does not effect the plugin's behaviour when checking for changes.
    /// </para>
    /// <heading>Perforce Issues</heading>
    /// <para>
    /// Perforce cannot apply purely numeric labels, which is what CCNet uses by default. Therefore, if you have 'applyLabel' set to true, you
    /// must also setup a custom Labeller in your project, e.g. by using the <link>Default Labeller</link>.
    /// </para>
    /// </remarks>
    [ReflectorType("p4")]
	public class P4 
        : SourceControlBase
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
            this.InitialiseDefaults();
		}

		public P4(ProcessExecutor processExecutor, IP4Initializer initializer, IP4Purger p4Purger, IP4ProcessInfoCreator processInfoCreator)
		{
			this.processExecutor = processExecutor;
			p4Initializer = initializer;
			this.processInfoCreator = processInfoCreator;
			this.p4Purger = p4Purger;
            this.InitialiseDefaults();
        }

        /// <summary>
        /// Initialises the defaults.
        /// </summary>
        private void InitialiseDefaults()
        {
            this.Executable = "p4";
            this.Client = string.Empty;
            this.User = string.Empty;
            this.Password = string.Empty;
            this.Port = string.Empty;
            this.WorkingDirectory = string.Empty;
            this.ApplyLabel = false;
            this.AutoGetSource = true;
            this.ForceSync = false;
            this.TimeZoneOffset = 0;
            this.ErrorPattern = DEFAULT_ERROR_PATTERN;
            this.UseExitCode = false;
            this.AcceptableErrors = new string[1] { FILES_UP_TO_DATE_PATTERN };
		}

        /// <summary>
        /// The location of the Perforce command line client executable.
        /// </summary>
        /// <version>1.0</version>
        /// <default>p4</default>
        [ReflectorProperty("executable", Required = false)]
        public string Executable { get; set; }

        /// <summary>
        /// The perforce 'view' to check for changes. For 'multi-line' views, use a comma-separated list. 'Exclusionary' view lines starting
        /// with - cannot be used. Use a <link>Filtered Source Control Block</link> to achieve this behaviour. Note that this view is not used
        /// for syncing (see below.) 
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("view")]
        public string View { get; set; }

        /// <summary>
        /// The perforce 'client' to use.
        /// </summary>
        /// <version>1.0</version>
        /// <default>Performance environment setting</default>
        [ReflectorProperty("client", Required = false)]
        public string Client { get; set; }

        /// <summary>
        /// The perforce user to use.
        /// </summary>
        /// <version>1.0</version>
        /// <default>Performance environment setting</default>
        [ReflectorProperty("user", Required = false)]
        public string User { get; set; }

        /// <summary>
        /// The perforce password to use.
        /// </summary>
        /// <version>1.0</version>
        /// <default>Performance environment setting</default>
        [ReflectorProperty("password", Required = false)]
        public string Password { get; set; }

        /// <summary>
        /// The perforce hostname and port to use.
        /// </summary>
        /// <version>1.0</version>
        /// <default>Performance environment setting</default>
        [ReflectorProperty("port", Required = false)]
        public string Port { get; set; }

        /// <summary>
        /// The working directory to use.
        /// </summary>
        /// <version>1.0</version>
        /// <default>Project Working Directory</default>
        [ReflectorProperty("workingDirectory", Required = false)]
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Whether to apply a label on a successful build.
        /// </summary>
        /// <version>1.0</version>
        /// <default>false</default>
        [ReflectorProperty("applyLabel", Required = false)]
        public bool ApplyLabel { get; set; }

        /// <summary>
        /// Whether to automatically 'sync' the latest changes from source control before performing the build. The sync target is the entire
        /// view exposed by the specified client - the view has no effect on sycning. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>true</default>
        [ReflectorProperty("autoGetSource", Required = false)]
        public bool AutoGetSource { get; set; }

        /// <summary>
        /// If autoGetSource is set to true, then whether to use the -f option to sync. See
        /// http://www.perforce.com/perforce/doc.042/manuals/cmdref/sync.html for more details.
        /// </summary>
        /// <version>1.0</version>
        /// <default>false</default>
        [ReflectorProperty("forceSync", Required = false)]
        public bool ForceSync { get; set; }

        /// <summary>
        /// Creates a link to the P4Web change list page for each detected modification. The specified value is transformed using String.Format
        /// where the first argument will be the substituted change list number.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty(@"p4WebURLFormat", Required = false)]
        public string P4WebURLFormat { get; set; }

        /// <summary>
        /// How many hours ahead your Perforce Server is from your build server. E.g. if your build server is in London, and your Perforce
        /// server is in New York, this value would be '-5'.
        /// </summary>
        /// <version>1.0</version>
        /// <default>0</default>
        [ReflectorProperty("timeZoneOffset", Required = false)]
        public double TimeZoneOffset { get; set; }

        /// <summary>
        /// The error pattern to use.
        /// </summary>
        /// <version>1.0</version>
        /// <default>^error: .*</default>
        [ReflectorProperty("errorPattern", Required = false)]
        public string ErrorPattern { get; set; }

        /// <summary>
        /// Whether to use exit codes.
        /// </summary>
        /// <version>1.0</version>
        /// <default>false</default>
        [ReflectorProperty("useExitCode", Required = false)]
        public bool UseExitCode { get; set; }

        /// <summary>
        /// The acceptable errors.
        /// </summary>
        /// <version>1.0</version>
        /// <default>file\(s\) up-to-date\.</default>
        [ReflectorProperty("acceptableErrors", Required = false)]
        public string[] AcceptableErrors { get; set; }


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

        public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
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
        public override void LabelSourceControl(IIntegrationResult result)
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
                var viewLineList = new List<string>();
				foreach (string viewLine in View.Split(','))
				{
					viewLineList.Add(viewLine);
				}
				return viewLineList.ToArray();
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

        public override void GetSource(IIntegrationResult result)
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
            Log.Info(String.Format("Perforce {0}: {1} {2}", description, process.FileName, process.PublicArguments));
			ProcessResult result = processExecutor.Execute(process);
            string errorSummary = ParseErrors(result.StandardOutput, result.StandardError);
            if (errorSummary != null)
            {
                string errorMessage =
                    string.Format(
                        "Perforce {0} failed: {1} {2}\r\nError output from process was: \r\n{3}",
                        description, process.FileName, process.PublicArguments, errorSummary);
                Log.Error(errorMessage);
                throw new CruiseControlException(errorMessage);
            }
            return result.StandardOutput.Trim() + Environment.NewLine + result.StandardError.Trim();
		}

        public override void Initialize(IProject project)
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

        public override void Purge(IProject project)
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

        /// <summary>
        /// Converts the comment (or parts from it) into an url pointing to the issue for this build. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("issueUrlBuilder", InstanceTypeKey = "type", Required = false)]
        public IModificationUrlBuilder IssueUrlBuilder { get; set; }


        private void FillIssueUrl(Modification[] modifications)
        {
            if (IssueUrlBuilder != null)
            {
                IssueUrlBuilder.SetupModification(modifications);
            }
        }
	}
}