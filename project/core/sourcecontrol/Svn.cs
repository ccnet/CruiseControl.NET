using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    [ReflectorType("svn")]
    public class Svn : ProcessSourceControl
    {
        public const string DefaultExecutable = "svn";
        public static readonly string UtcXmlDateFormat = "yyyy-MM-ddTHH:mm:ssZ";
		private BuildProgressInformation _buildProgressInformation;

        public Svn(ProcessExecutor executor, IHistoryParser parser, IFileSystem fileSystem)
            : base(parser, executor)
        {
            this.fileSystem = fileSystem;
        }

        public Svn()
            : this(new ProcessExecutor(), new SvnHistoryParser(), new SystemIoFileSystem())
        {
        }

        [ReflectorProperty("webUrlBuilder", InstanceTypeKey = "type", Required = false)]
        public IModificationUrlBuilder UrlBuilder;

        [ReflectorProperty("executable", Required = false)]
        public string Executable = DefaultExecutable;

        [ReflectorProperty("trunkUrl", Required = false)]
        public string TrunkUrl;

        [ReflectorProperty("workingDirectory", Required = false)]
        public string WorkingDirectory;

        [ReflectorProperty("tagOnSuccess", Required = false)]
        public bool TagOnSuccess = false;

        [ReflectorProperty("tagBaseUrl", Required = false)]
        public string TagBaseUrl;

        [ReflectorProperty("username", Required = false)]
        public string Username;

        [ReflectorProperty("password", Required = false)]
        public string Password;

        [ReflectorProperty("autoGetSource", Required = false)]
        public bool AutoGetSource = true;

        [ReflectorProperty("checkExternals", Required = false)]
        public bool CheckExternals = false;

        [ReflectorProperty("checkExternalsRecursive", Required = false)]
        public bool CheckExternalsRecursive = true;

        [ReflectorProperty("cleanCopy", Required = false)]
        public bool CleanCopy = false;

        [ReflectorProperty("revert", Required = false)]
        public bool Revert = false;

        [ReflectorProperty("cleanUp", Required = false)]
        public bool CleanUp = false;

        [ReflectorProperty("revisionNumbers", Required = false)]
        public bool UserRevsionNumbers { get; set; }

        private readonly IFileSystem fileSystem;

        /// <summary>
        /// Modifications discovered by this instance of the source control interface.
        /// This is needed for the Multi Source Control block. (See CCNET-639/CCNET-1307)
        /// </summary>
        internal Modification[] mods = new Modification[0];

        // Non-private for testing only.
        internal int latestRevision;

        public string FormatCommandDate(DateTime date)
        {
            return date.ToUniversalTime().ToString(UtcXmlDateFormat, CultureInfo.InvariantCulture);
        }


        private bool WorkingFolderIsKnownAsSvnWorkingFolder(string workingDirectory)
        {
            Log.Debug("Checking if {0} is a svn working folder", workingDirectory);

			if (!Directory.Exists(workingDirectory))
				return false;
            
            return Directory.GetDirectories(workingDirectory, ".svn").Length != 0 ||
                   Directory.GetDirectories(workingDirectory, "_svn").Length != 0;
        }



        public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
        {
            var revisionData = NameValuePair.ToDictionary(from.SourceControlData);

            if (to.LastIntegrationStatus == IntegrationStatus.Unknown)
            {
                ((SvnHistoryParser)historyParser).IntegrationStatusUnknown = true;
            }

            string wd = Path.GetFullPath(to.BaseFromWorkingDirectory(WorkingDirectory));

            if (WorkingFolderIsKnownAsSvnWorkingFolder(wd) )
            {
                if (CleanUp)
                    Execute(CleanupWorkingCopy(to));

                if (Revert)
                    Execute(RevertWorkingCopy(to));
            }
            else
            {
                Util.Log.Warning(string.Format("{0} is not a svn working folder", wd));
            }

            List<Modification> modifications = new List<Modification>();
            List<string> repositoryUrls = new List<string>();
            repositoryUrls.Add(TrunkUrl);

            if (CheckExternals)
            {
                ProcessResult resultOfSvnPropget = Execute(PropGetProcessInfo(to));
                List<string> externals = ParseExternalsDirectories(resultOfSvnPropget);
                foreach (string external in externals)
                {
                    if (!repositoryUrls.Contains(external)) repositoryUrls.Add(external);
                }
            }

            foreach (string repositoryUrl in repositoryUrls)
            {
                var lastRepositoryRevisionName = "SVN:LastRevision:" + repositoryUrl;
                Modification[] modsInRepository;
                string lastRepositoryRevision = null;
                if (UserRevsionNumbers)
                {
                    // Since we are using the last revision number, see if there is any number stored to use
                    lastRepositoryRevision = revisionData.ContainsKey(lastRepositoryRevisionName)
                        ? revisionData[lastRepositoryRevisionName]
                        : null;
                    ProcessResult result = Execute(NewHistoryProcessInfoFromRevision(lastRepositoryRevision, to, repositoryUrl));
                    modsInRepository = ParseModifications(result, lastRepositoryRevision);
                }
                else
                {
                    // Use use the date range
                    ProcessResult result = Execute(NewHistoryProcessInfo(from, to, repositoryUrl));
                    modsInRepository = ParseModifications(result, from.StartTime, to.StartTime);
                }

                // If there are modifications, get the number and add them to the output
                if (modsInRepository != null)
                {
                    lastRepositoryRevision = Modification.GetLastChangeNumber(modsInRepository)
                        ?? lastRepositoryRevision;
                    modifications.AddRange(modsInRepository);
                    revisionData[lastRepositoryRevisionName] = lastRepositoryRevision;
                }

                // Set the latest revision - this always need to be done just in case an external has triggered a build
                if (repositoryUrl == TrunkUrl)
                {
                    latestRevision = int.Parse(lastRepositoryRevision ?? "0");
                }
            }

            mods = modifications.ToArray();
            if (UrlBuilder != null)
            {
                UrlBuilder.SetupModification(mods);
            }
            FillIssueUrl(mods);

            // Store the latest revision number
            to.SourceControlData.Clear();
            NameValuePair.Copy(revisionData, to.SourceControlData);

            return mods;
        }

        public override void LabelSourceControl(IIntegrationResult result)
        {
            if (TagOnSuccess && result.Succeeded)
            {
                Execute(NewLabelProcessInfo(result));
            }
        }

		private BuildProgressInformation GetBuildProgressInformation(IIntegrationResult result)
		{
			if (_buildProgressInformation == null)
                _buildProgressInformation = result.BuildProgressInformation;

			return _buildProgressInformation;
		}

		private void ProcessExecutor_ProcessOutput(object sender, ProcessOutputEventArgs e)
		{
			if (_buildProgressInformation == null)
				return;

			// ignore error output in the progress information
			if (e.OutputType == ProcessOutputType.ErrorOutput)
				return;

			_buildProgressInformation.AddTaskInformation(e.Data);
		}

        private ProcessInfo PropGetProcessInfo(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("propget");
            if (CheckExternalsRecursive)
            {
                buffer.AddArgument("-R");
            }
            AppendCommonSwitches(buffer);
            buffer.AddArgument("svn:externals");
            buffer.AddArgument(TrunkUrl);
            return NewProcessInfo(buffer.ToString(), result);
        }

        private ProcessInfo RevertWorkingCopy(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("revert");
            buffer.AddArgument("--recursive");
			buffer.AddArgument(StringUtil.AutoDoubleQuoteString(Path.GetFullPath(result.BaseFromWorkingDirectory(WorkingDirectory))));

            return NewProcessInfo(buffer.ToString(), result);
        }


        private ProcessInfo CleanupWorkingCopy(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("cleanup");
			buffer.AddArgument(StringUtil.AutoDoubleQuoteString(Path.GetFullPath(result.BaseFromWorkingDirectory(WorkingDirectory))));

            return NewProcessInfo(buffer.ToString(), result);
        }


        public override void GetSource(IIntegrationResult result)
        {
            result.BuildProgressInformation.SignalStartRunTask("Getting source from SVN");

            if (!AutoGetSource) return;

            if (DoesSvnDirectoryExist(result) && !CleanCopy)
            {
                UpdateSource(result);
            }
            else
            {
                if (CleanCopy)
                {
                    if (WorkingDirectory == null)
                    {
                        DeleteSource(result.WorkingDirectory);
                    }
                    else
                    {
                        DeleteSource(WorkingDirectory);
                    }
                }

                CheckoutSource(result);
            }
        }

        private void DeleteSource(string workingDirectory)
        {
            if (fileSystem.DirectoryExists(workingDirectory))
            {
                new IoService().DeleteIncludingReadOnlyObjects(workingDirectory);
            }
        }

        private void CheckoutSource(IIntegrationResult result)
        {
            if (string.IsNullOrEmpty(TrunkUrl))
                throw new ConfigurationException("<trunkurl> configuration element must be specified in order to automatically checkout source from SVN.");

			// initialize progress information
			var bpi = GetBuildProgressInformation(result);
			bpi.SignalStartRunTask("Calling svn checkout ...");

			// enable Stdout monitoring
			ProcessExecutor.ProcessOutput += ProcessExecutor_ProcessOutput;

            Execute(NewCheckoutProcessInfo(result));

			// remove Stdout monitoring
			ProcessExecutor.ProcessOutput -= ProcessExecutor_ProcessOutput;
        }

        private ProcessInfo NewCheckoutProcessInfo(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("checkout");
            buffer.AddArgument(TrunkUrl);
            buffer.AddArgument(StringUtil.AutoDoubleQuoteString(Path.GetFullPath(result.BaseFromWorkingDirectory(WorkingDirectory))));
            AppendCommonSwitches(buffer);
            return NewProcessInfo(buffer.ToString(), result);
        }

        private void UpdateSource(IIntegrationResult result)
        {
			// initialize progress information
			var bpi = GetBuildProgressInformation(result);
			bpi.SignalStartRunTask("Calling svn update ...");

			// enable Stdout monitoring
			ProcessExecutor.ProcessOutput += ProcessExecutor_ProcessOutput;

            Execute(NewGetSourceProcessInfo(result));

			// remove Stdout monitoring
			ProcessExecutor.ProcessOutput -= ProcessExecutor_ProcessOutput;
        }

        private bool DoesSvnDirectoryExist(IIntegrationResult result)
        {
            string svnDirectory = Path.Combine(result.BaseFromWorkingDirectory(WorkingDirectory), ".svn");
            string underscoreSvnDirectory = Path.Combine(result.BaseFromWorkingDirectory(WorkingDirectory), "_svn");
            return fileSystem.DirectoryExists(svnDirectory) || fileSystem.DirectoryExists(underscoreSvnDirectory);
        }

        private ProcessInfo NewGetSourceProcessInfo(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("update");
            buffer.AddArgument(StringUtil.AutoDoubleQuoteString(Path.GetFullPath(result.BaseFromWorkingDirectory(WorkingDirectory))));
            // Do not use Modification.GetLastChangeNumber() here directly.
            AppendRevision(buffer, latestRevision);
            AppendCommonSwitches(buffer);
            return NewProcessInfo(buffer.ToString(), result);
        }

        //		TAG_COMMAND_FORMAT = "copy --message "CCNET build label" "trunkUrl" "tagBaseUrl/label"
        private ProcessInfo NewLabelProcessInfo(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("copy");
            buffer.AppendArgument(TagMessage(result.Label));
            buffer.AddArgument(TagSource(result));
            buffer.AddArgument(TagDestination(result.Label));
            // Do not use Modification.GetLastChangeNumber() here directly.
            AppendRevision(buffer, latestRevision);
            AppendCommonSwitches(buffer);
            return NewProcessInfo(buffer.ToString(), result);
        }

        //		HISTORY_COMMAND_FORMAT = "log url --revision \"{{{StartDate}}}:{{{EndDate}}}\" --verbose --xml --non-interactive";
        private ProcessInfo NewHistoryProcessInfo(IIntegrationResult from, IIntegrationResult to, string url)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("log");
            buffer.AddArgument(url);
            buffer.AppendArgument(string.Format("-r \"{{{0}}}:{{{1}}}\"", FormatCommandDate(from.StartTime), FormatCommandDate(to.StartTime)));
            buffer.AppendArgument("--verbose --xml");
            AppendCommonSwitches(buffer);
            return NewProcessInfo(buffer.ToString(), to);
        }

        //		HISTORY_COMMAND_FORMAT = "log url --revision {LastRevision}:HEAD --verbose --xml --non-interactive";
        private ProcessInfo NewHistoryProcessInfoFromRevision(string lastRevision, IIntegrationResult to, string url)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("log");
            buffer.AddArgument(url);
            buffer.AppendArgument(string.Format("-r {0}:HEAD", string.IsNullOrEmpty(lastRevision) ? "0" : lastRevision));
            buffer.AppendArgument("--verbose --xml");
            AppendCommonSwitches(buffer);
            return NewProcessInfo(buffer.ToString(), to);
        }

        private static List<string> ParseExternalsDirectories(ProcessResult result)
        {
            List<string> externalDirectories = new List<string>();

            using (StringReader reader = new StringReader(result.StandardOutput))
            {
                string externalsDefinition;

                while ((externalsDefinition = reader.ReadLine()) != null)
                {
                    // If this external is not a specific revision and is not an empty string
                    if (!externalsDefinition.Contains("-r") && !externalsDefinition.Equals(string.Empty))
                    {
                        int Pos = GetSubstringPosition(externalsDefinition);

                        if (Pos > 0)
                        {
                            externalsDefinition = externalsDefinition.Substring(Pos);
                        }

                        Pos = externalsDefinition.IndexOf(" ");

                        if (Pos > 0)
                        {
                            externalsDefinition = externalsDefinition.Substring(0, Pos);
                        }

                        if (!externalDirectories.Contains(externalsDefinition))
                        {
                            externalDirectories.Add(externalsDefinition);
                        }
                    }
                }
            }
            return externalDirectories;
        }

        private static int GetSubstringPosition(string externalsDefinition)
        {
            int pos = 0;
            string[] urlTypes = { "file:/", "http:/", "https:/", "svn:/", "svn+ssh:/" };

            foreach (string type in urlTypes)
            {
                int tmp = externalsDefinition.LastIndexOf(type);
                if (tmp > pos) pos = tmp;
            }

            return pos;
        }

        private static string TagMessage(string label)
        {
            return string.Format("-m \"CCNET build {0}\"", label);
        }

        private string TagSource(IIntegrationResult result)
        {
            if (Modification.GetLastChangeNumber(mods) == null)
            {
                return Path.GetFullPath(result.BaseFromWorkingDirectory(WorkingDirectory)).TrimEnd(Path.DirectorySeparatorChar);
            }
            return TrunkUrl;
        }

        private string TagDestination(string label)
        {
            return string.Format("{0}/{1}", TagBaseUrl, label);
        }

        private void AppendCommonSwitches(ProcessArgumentBuilder buffer)
        {
            buffer.AddArgument("--username", Username);
            buffer.AddHiddenArgument("--password", Password);
            buffer.AddArgument("--non-interactive");
            buffer.AddArgument("--no-auth-cache");
        }

        private static void AppendRevision(ProcessArgumentBuilder buffer, int revision)
        {
            buffer.AppendIf(revision > 0, "--revision {0}", revision.ToString());
        }

        private ProcessInfo NewProcessInfo(string args, IIntegrationResult result)
        {
            string workingDirectory = Path.GetFullPath(result.BaseFromWorkingDirectory(WorkingDirectory));
            if (!Directory.Exists(workingDirectory)) Directory.CreateDirectory(workingDirectory);

            ProcessInfo processInfo = new ProcessInfo(Executable, args, workingDirectory);
            processInfo.StreamEncoding = Encoding.UTF8;
            return processInfo;
        }
    }
}
