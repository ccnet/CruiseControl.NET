using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// CruiseControl.NET provides basic support for Subversion repositories. Checking for changes, checking out or updating sources, and 
    /// tagging-by-copying are supported, but more advanced features such as using Subversion revision numbers are not yet supported.
    /// Subversion support is under active development and will improve over time.
    /// </summary>
    /// <title>Subversion Source Control Block</title>
    /// <version>1.0</version>
    /// <key name="type">
    /// <description>The type of source control block.</description>
    /// <value>svn</value>
    /// </key>
    /// <example>
    /// <code>
    /// &lt;sourcecontrol type="svn"&gt;
    /// &lt;trunkUrl&gt;svn://svn.mycompany.com/myfirstproject/trunk&lt;/trunkUrl&gt;
    /// &lt;workingDirectory&gt;c:\dev\ccnet&lt;/workingDirectory&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>
    /// The &lt;trunkUrl&gt; tag should specify the URL to use to determine if changes have occurred in your repository.
    /// </para>
    /// <para>
    /// You need to make sure your SVN client settings are such that all authentication is automated. Typically you can do this by using 
    /// anonymous access or appropriate SSH setups if using SVN over SSH.
    /// </para>
    /// <heading>Linking modifications to WebSVN</heading>
    /// <para>
    /// You can link the modifications detected by CruiseControl.NET to the appropriate WebSVN page by adding the following additional
    /// configuration information to the Subversion source control section:
    /// </para>
    /// <code>
    /// &lt;webUrlBuilder type="websvn"&gt;
    /// &lt;url&gt;http://localhost:7899/websvn/diff.php?repname=MiniACE&amp;amp;path={0}&amp;amp;rev={1}&amp;amp;sc=1&lt;/url&gt;
    /// &lt;/webUrlBuilder&gt;
    /// </code>
    /// <para>
    /// Change the &lt;url&gt; element to point to the root url for the WebSVN site. The path and rev parameters will be filled in by
    /// CruiseControl.NET when it generates the link to the code file page.
    /// </para>
    /// <para>
    /// The standard url for WebSVN 1.38 contains rep=3 r instead of repname=MiniACE. Adding a new repository to the SvnParentPath will change
    /// the number of the rep parameter, so you may need to make the following change to diff.php to decode the repname parameter:
    /// </para>
    /// <code type="java">
    /// $repname = @$_REQUEST["repname"];
    /// 
    /// if (isset($repname))
    /// {
    /// $rep = $config->findRepository($repname);
    /// }
    /// </code>
    /// <para>
    /// The WebSVN WebUrlBuilder can also be used to connect to a ViewCV site. Here is an example used by CCNet to link to the file revision
    /// pages on SourceForge:
    /// </para>
    /// <code>
    /// &lt;webUrlBuilder type="websvn"&gt;
    /// &lt;url&gt;http://svn.sourceforge.net/viewvc/ccnet/{0}?view=markup&amp;amp;pathrev={1}&lt;/url&gt;
    /// &lt;/webUrlBuilder&gt;
    /// </code>
    /// <heading>SVN over SSL</heading>
    /// <para>
    /// When connecting to a Subversion repository via SSL (https), you may be required to accept an issued server certificate. This generally
    /// requires responding to a command-line prompt that the certificate should be accepted permanently. For CruiseControl.NET, all Subversion
    /// commands are executed using the --non-interactive switch, which will cause this prompt to be skipped and the subsequent Subversion
    /// command to fail with a message like this:
    /// </para>
    /// <code type="none">
    /// svn: PROPFIND request failed on '/svnroot/ccnet'
    /// svn: PROPFIND of '/svnroot/ccnet': Server certificate verification failed: issuer is not trusted (https://ccnet.svn.sourceforge.net)
    /// </code>
    /// <para>
    /// As the failure will happen on the first Subversion command to be executed, this failure will not show up as a broken build, but will
    /// instead show up in the server log.
    /// </para>
    /// <para>
    /// One way to resolve this problem is to execute a command against the Subversion repository from the command-line logged in as the user
    /// that you are using to run CCNet (certificates are cached by user account, so you must accept the certificate for the appropriate user).
    /// For example, try executing the following command (where trunk_url is the svn url for your repository):
    /// </para>
    /// <code type="none">
    /// svn list [trunk_url]
    /// </code>
    /// <para>
    /// When prompted to accept the certificate, type 'P' to permanently accept it.
    /// </para>
    /// <para>
    /// If you are running CCService under the LocalSystem account, you will need to accept the certificate for this user. Check out the
    /// CCService page for information about how to diagnose problems as the LocalSystem account.
    /// </para>
    /// <para>
    /// External contributors: Matt Petteys
    /// </para>
    /// <heading>SVN over svn+ssh</heading>
    /// <para>
    /// To connect to Subversion with the svn+ssh protocol, here is an excerpt from the article HowTo: Configure SVN+SSH with Subclipse on
    /// Windows by Martin Woodward. For more details, see the complete article (http://www.woodwardweb.com/archive/200511.html).
    /// </para>
    /// <para>
    /// <b>HowTo: Configure SVN+SSH with Subclipse on Windows</b>
    /// </para>
    /// <para>
    /// You need to create an environment variable called "SVN_SSH" that points to an executable file that accepts the same command line
    /// arguments as ssh on unix. I did this by doing the following:-
    /// </para>
    /// <para>
    /// 1: Set up ssh keys. Not going to cover that here as you can easily Google for that. You need to end up with your public key on the SVN
    /// server and your private key loaded into Paegent locally.
    /// </para>
    /// <para>
    /// 2: Download and installed the excellent TortoiseSVN client for Windows.
    /// </para>
    /// <para>
    /// 3: Set the following environment variable (by right-clicking on My Computer, Properties, Advanced, Environment Variables, New):- 
    /// </para>
    /// <code type="none">
    /// Variable name: SVN_SSH
    /// Variable value: C:\\Program Files\\TortoiseSVN\\bin\\TortoisePlink.exe
    /// </code>
    /// <para type="info">
    /// The "\ \"(double-back-slash) is very important, otherwise it won't work. Equally, you cannot use the plink.exe that comes with putty as
    /// that fires up a command shell window which is really annoying. The TortoisePlink.exe is a windows implementation of plink that doesn't
    /// bring up any UI.
    /// </para>
    /// <heading>Known Issues</heading>
    /// <b>CruiseControl.NET doesn't see my changes</b>
    /// <para>
    /// The Subversion interface depends on the clocks of the CruiseControl.Net and Subversion servers being set within a small difference. Due
    /// to a long-standing Subversion bug (Bugzilla #1642) that appears unlikely to ever be fixed, CruiseControl.Net must filter the list of
    /// modifications returned by Subversion, looking for only those that fall within a specific time range. When the clocks of the two servers
    /// are significantly different, the filter may ignore modifications that should not be ignored. To prevent this problem, keep the clocks 
    /// of the two servers set as closely together as possible.
    /// </para>
    /// <heading>Dashboard Localization (issues with SVN)</heading>
    /// <para>
    /// I've found that svn has a trouble with --xml parameter. My russian Log Messages were not readable.
    /// </para>
    /// <para>
    /// I found a quick solution for it, next items could be customized to any languges. But be sure this is not the panacea. So in all .xsl
    /// files where comments or filename in Russian local I've added following "translation"
    /// </para>
    /// <code>
    /// &lt;xsl:value-of select="translate(comment,'??????????????????????????????????????????????????????????????????',
    /// '????????????????????????????????????????????????????????????????')"/&gt;
    /// </code>
    /// <para>
    /// Where comment from /cruisecontrol/modifications/modification, and "??" in second parameter are symbols to delete, all others are mathed
    /// with third parameters. As I found xsl could only translate char strings , if some one know the way to translate double-char symbols,
    /// please note about it. In this way you can help your self for modification for any charset.
    /// </para>
    /// <para>
    /// I agree with everyone that it is a rude solution. But for my no time to wait untill svn or cc.net will be customized for that feature.
    /// </para>
    /// </remarks>
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
            this.InitialiseDefaults();
        }

        public Svn()
            : this(new ProcessExecutor(), new SvnHistoryParser(), new SystemIoFileSystem())
        {
            this.InitialiseDefaults();
        }

        /// <summary>
        /// Initialises the defaults.
        /// </summary>
        private void InitialiseDefaults()
        {
            this.AuthCaching = AuthCachingMode.None;
        }

        /// <summary>
        /// The root url for the WebSVN site.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("webUrlBuilder", InstanceTypeKey = "type", Required = false)]
        public IModificationUrlBuilder UrlBuilder;

        /// <summary>
        /// The location of the svn executable.
        /// </summary>
        /// <version>1.0</version>
        /// <default>svn</default>
        [ReflectorProperty("executable", Required = false)]
        public string Executable = DefaultExecutable;

        /// <summary>
        /// The url for your repository (e.g., svn://svnserver/).
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("trunkUrl", Required = false)]
        public string TrunkUrl;

        /// <summary>
        /// The directory containing the locally checked out workspace. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>Project Working Directory</default>
        [ReflectorProperty("workingDirectory", Required = false)]
        public string WorkingDirectory;

        /// <summary>
        /// Indicates that the repository should be tagged if the build succeeds. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>false</default>
        [ReflectorProperty("tagOnSuccess", Required = false)]
        public bool TagOnSuccess = false;

        /// <summary>
        /// Should any detected obstructions be deleted prior to getting modifications?
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("deleteObstructions", Required = false)]
        public bool DeleteObstructions = false;

        /// <summary>
        /// The base url for tags in your repository. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("tagBaseUrl", Required = false)]
        public string TagBaseUrl;

        /// <summary>
        /// The username to use for authentication when connecting to the repository. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("username", Required = false)]
        public string Username;

        /// <summary>
        /// The password to use for authentication when connecting to the repository. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("password", typeof(PrivateStringSerialiserFactory), Required = false)]
        public PrivateString Password;

        /// <summary>
        /// Whether to retrieve the updates from Subversion for a particular build. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>true</default>
        [ReflectorProperty("autoGetSource", Required = false)]
        public bool AutoGetSource = true;

        /// <summary>
        /// Whether to check the paths specified in the svn:externals property for modifications. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>false</default>
        [ReflectorProperty("checkExternals", Required = false)]
        public bool CheckExternals = false;

        /// <summary>
        /// Whether to check for modifications of svn:externals recursively. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>true</default>
        [ReflectorProperty("checkExternalsRecursive", Required = false)]
        public bool CheckExternalsRecursive = true;

        /// <summary>
        /// Whether to delete the working copy before updating the source. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>false</default>
        [ReflectorProperty("cleanCopy", Required = false)]
        public bool CleanCopy = false;

        /// <summary>
        /// Reverts any local changes to a file or directory and resolves any conflicted states. svn revert will not only revert the contents
        /// of an item in your working copy, but also any property changes. Finally, you can use it to undo any scheduling operations that you
        /// may have done (e.g. files scheduled for addition or deletion can be "unscheduled").
        /// </summary>
        /// <version>1.4.3</version>
        /// <default>false</default>
        [ReflectorProperty("revert", Required = false)]
        public bool Revert = false;

        /// <summary>
        /// Recursively clean up the working copy, removing locks resuming unfinished operations. If you ever get a "working copy locked"
        /// error, run this command to remove stale locks and get your working copy into a usable state again.
        /// </summary>
        /// <version>1.4.3</version>
        /// <default>false</default>
        [ReflectorProperty("cleanUp", Required = false)]
        public bool CleanUp = false;

        /// <summary>
        /// Whether to use revision numbers for fetching the modifications.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>false</default>
        [ReflectorProperty("revisionNumbers", Required = false)]
        public bool UserRevsionNumbers { get; set; }

        /// <summary>
        /// Defines the auth caching mode to use.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("authCaching", Required = false)]
        public AuthCachingMode AuthCaching { get; set; }

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

        /// <summary>
        /// Lists any obstructed files or folders.
        /// </summary>
        /// <param name="result">The current result.</param>
        private IList<string> ListObstructions(IIntegrationResult result)
        {
            var args = new PrivateArguments("status", "--xml");
            var info = this.NewProcessInfo(args, result);
            var processResult = Execute(info);

            var obstructions = new List<string>();
            var svnData = new XmlDocument();
            svnData.LoadXml(processResult.StandardOutput);
            var nodes = svnData.SelectNodes("//entry[wc-status/@item=\"obstructed\"]");
            foreach (XmlElement node in nodes)
            {
                obstructions.Add(node.GetAttribute("path"));
            }

            return obstructions;
        }

        /// <summary>
        /// Deletes any obstructions from the working directory.
        /// </summary>
        /// <param name="result">The current result.</param>
        private void DeleteObstructionsFromWorking(IIntegrationResult result)
        {
            // Check if there are any obstructions
            Log.Info("Retrieving obstructions");
            var obstructions = this.ListObstructions(result);
            if (obstructions.Count == 0)
            {
                Log.Info("No obstructions found");
            }
            else
            {
                // Delete the obstructions
                Log.Info(obstructions.Count.ToString() + " obstruction(s) found - deleting");
                var basePath = Path.GetFullPath(result.BaseFromWorkingDirectory(this.WorkingDirectory)); ;
                foreach (var obstruction in obstructions)
                {
                    // Get the full path to the folder
                    var path = Path.Combine(basePath, obstruction);
                    Log.Info("Deleting folder " + path);
                    this.fileSystem.DeleteDirectory(path, true);
                }
            }
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
                {
                    Execute(CleanupWorkingCopy(to));
                }

                if (Revert)
                {
                    Execute(RevertWorkingCopy(to));
                }

                if (this.DeleteObstructions)
                {
                    this.DeleteObstructionsFromWorking(to);
                }
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
            var buffer = new PrivateArguments("propget");
            buffer.AddIf(CheckExternalsRecursive, "-R");
            AppendCommonSwitches(buffer);
            buffer.Add("svn:externals");
            buffer.Add(TrunkUrl);
            return NewProcessInfo(buffer, result);
        }

        private ProcessInfo RevertWorkingCopy(IIntegrationResult result)
        {
            var buffer = new PrivateArguments("revert", "--recursive");
            buffer.Add(null, Path.GetFullPath(result.BaseFromWorkingDirectory(WorkingDirectory)), true);

            return NewProcessInfo(buffer, result);
        }


        private ProcessInfo CleanupWorkingCopy(IIntegrationResult result)
        {
            var buffer = new PrivateArguments("cleanup");
			buffer.Add(null, Path.GetFullPath(result.BaseFromWorkingDirectory(WorkingDirectory)), true);

            return NewProcessInfo(buffer, result);
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
            var buffer = new PrivateArguments("checkout");
            buffer.Add(string.Empty, TrunkUrl, true);
            buffer.Add(null, Path.GetFullPath(result.BaseFromWorkingDirectory(WorkingDirectory)), true);
            AppendCommonSwitches(buffer);
            return NewProcessInfo(buffer, result);
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
            var buffer = new PrivateArguments("update");
            buffer.Add(null, Path.GetFullPath(result.BaseFromWorkingDirectory(WorkingDirectory)), true);
            // Do not use Modification.GetLastChangeNumber() here directly.
            AppendRevision(buffer, latestRevision);
            AppendCommonSwitches(buffer);
            return NewProcessInfo(buffer, result);
        }

        //		TAG_COMMAND_FORMAT = "copy --message "CCNET build label" "trunkUrl" "tagBaseUrl/label"
        private ProcessInfo NewLabelProcessInfo(IIntegrationResult result)
        {
            var buffer = new PrivateArguments("copy");
            buffer.Add(null, TagMessage(result.Label), true);
            buffer.Add(null, TagSource(result), true);
            buffer.Add(null, TagDestination(result.Label), true);
            // Do not use Modification.GetLastChangeNumber() here directly.
            AppendRevision(buffer, latestRevision);
            AppendCommonSwitches(buffer);
            return NewProcessInfo(buffer, result);
        }

        //		HISTORY_COMMAND_FORMAT = "log url --revision \"{{{StartDate}}}:{{{EndDate}}}\" --verbose --xml --non-interactive";
        private ProcessInfo NewHistoryProcessInfo(IIntegrationResult from, IIntegrationResult to, string url)
        {
            var buffer = new PrivateArguments("log");
            buffer.Add(null, url, true);
            buffer.Add(string.Format("-r \"{{{0}}}:{{{1}}}\"", FormatCommandDate(from.StartTime), FormatCommandDate(to.StartTime)));
            buffer.Add("--verbose --xml");
            AppendCommonSwitches(buffer, url != this.TrunkUrl);
            return NewProcessInfo(buffer, to);
        }

        //		HISTORY_COMMAND_FORMAT = "log url --revision {LastRevision}:HEAD --verbose --xml --non-interactive";
        private ProcessInfo NewHistoryProcessInfoFromRevision(string lastRevision, IIntegrationResult to, string url)
        {
            var buffer = new PrivateArguments("log");
            buffer.Add(null, url, true);
            buffer.Add(string.Format("-r {0}:HEAD", string.IsNullOrEmpty(lastRevision) ? "0" : lastRevision));
            buffer.Add("--verbose --xml");
            AppendCommonSwitches(buffer, url != this.TrunkUrl);
            return NewProcessInfo(buffer, to);
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

        private void AppendCommonSwitches(PrivateArguments buffer)
        {
            this.AppendCommonSwitches(buffer, false);
        }

        private void AppendCommonSwitches(PrivateArguments buffer, bool isExternal)
        {
            buffer.AddIf(!string.IsNullOrEmpty(this.Username), "--username ", this.Username, true);
            buffer.AddIf(this.Password != null, "--password ", this.Password, true);
            buffer.Add("--non-interactive");
            if (!isExternal || (this.AuthCaching == AuthCachingMode.None))
            {
                buffer.Add("--no-auth-cache");
            }
        }

        private static void AppendRevision(PrivateArguments buffer, int revision)
        {
            buffer.AddIf(revision > 0, "--revision ", revision.ToString());
        }

        private ProcessInfo NewProcessInfo(PrivateArguments args, IIntegrationResult result)
        {
            string workingDirectory = Path.GetFullPath(result.BaseFromWorkingDirectory(WorkingDirectory));
            if (!Directory.Exists(workingDirectory)) Directory.CreateDirectory(workingDirectory);

            ProcessInfo processInfo = new ProcessInfo(Executable, args, workingDirectory);
            processInfo.StreamEncoding = Encoding.UTF8;
            return processInfo;
        }

        /// <summary>
        /// Defies the type of auth caching to use.
        /// </summary>
        public enum AuthCachingMode
        {
            /// <summary>
            /// No auth caching.
            /// </summary>
            None,

            /// <summary>
            /// Use auth caching for externals.
            /// </summary>
            ExternalsOnly
        }
    }
}
