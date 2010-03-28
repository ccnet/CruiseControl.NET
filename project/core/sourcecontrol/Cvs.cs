using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// <para>
    /// Please refer to <link>Using CruiseControl.NET with CVS</link> for an overview of this block.
    /// </para>
    /// <para>
    /// For CVS you must define where the CVS executable (if you give a relative path, it must be relative to the ccnet.exe application) is and the working directory for checked out code.
    /// </para>
    /// </summary>
    /// <title> CVS Source Control Block </title>
    /// <version>1.2</version>
    /// <example>
    /// <code title="pserver authentication example">
    /// &lt;sourcecontrol type="cvs"&gt;
    /// &lt;executable&gt;..\tools\cvs.exe&lt;/executable&gt;
    /// &lt;cvsroot&gt;:pserver:anonymous@cvs.sourceforge.net:/cvsroot/ccnet&lt;/cvsroot&gt;
    /// &lt;module&gt;ccnet&lt;/module&gt;
    /// &lt;workingDirectory&gt;c:\projects\ccnet&lt;/workingDirectory&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// <code title="SSH via putty example">
    /// &lt;sourcecontrol type="cvs"&gt;
    /// &lt;executable&gt;c:\putty\cvswithplinkrsh.bat&lt;/executable&gt;
    /// &lt;cvsroot&gt;:ext:mycvsserver:/cvsroot/myrepo&lt;/cvsroot&gt;
    /// &lt;module&gt;mymodule&lt;/module&gt;
    /// &lt;workingDirectory&gt;c:\fromcvs\myrepo&lt;/workingDirectory&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// </example>
    /// <key name="type">
    /// <description>The type of source control block.</description>
    /// <value>cvs</value>
    /// </key>
    [ReflectorType("cvs")]
	public class Cvs : ProcessSourceControl
	{
		public const string DefaultCvsExecutable = "cvs";
		public const string COMMAND_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss 'GMT'";
		private readonly IFileSystem fileSystem;
        private readonly IExecutionEnvironment executionEnvironment;
        private BuildProgressInformation _buildProgressInformation;

		public Cvs() : this(new CvsHistoryParser(), new ProcessExecutor(), new SystemIoFileSystem(), new ExecutionEnvironment())
		{
		}

        public Cvs(IHistoryParser parser, ProcessExecutor executor, IFileSystem fileSystem, IExecutionEnvironment executionEnvironment)
            : base(parser, executor)
		{
			this.fileSystem = fileSystem;
            this.executionEnvironment = executionEnvironment;
            this.Executable = DefaultCvsExecutable;
            this.CvsRoot = string.Empty;
            this.WorkingDirectory = string.Empty;
            this.LabelOnSuccess = false;
            this.RestrictLogins = string.Empty;
            this.UrlBuilder = new NullUrlBuilder();
            this.AutoGetSource = true;
            this.CleanCopy = true;
            this.ForceCheckout = false;
            this.Branch = string.Empty;
            this.TagPrefix = "ver-";
		}

        /// <summary>
        /// The location of the cvs.exe executable. 
        /// </summary>
        /// <version>1.2</version>
        /// <default>cvs</default>
        [ReflectorProperty("executable", Required = false)]
        public string Executable { get; set; }

        /// <summary>
        /// The cvs connection string. If this is unspecified and your working directory contains a previous checkout, then the CVS client
        /// will attempt to determine the correct root based on the CVS folder in your working directory. If the working directory does not
        /// contain the source, then this element must be specfied.
        /// </summary>
        /// <version>1.2</version>
        /// <default>n/a</default>
        [ReflectorProperty("cvsroot")]
        public string CvsRoot { get; set; }

        /// <summary>
        /// The cvs module to monitor. This element is used both when checking for modifications and when checking out the source into an
        /// empty working directory.
        /// </summary>
        /// <version>1.2</version>
        /// <default>n/a</default>
        [ReflectorProperty("module")]
        public string Module { get; set; }

        /// <summary>
        /// The folder that the source has been checked out into. 
        /// </summary>
        /// <version>1.2</version>
        /// <default>Project Working Directory</default>
        [ReflectorProperty("workingDirectory", Required = false)]
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Specifies whether or not the repository should be labelled after a successful build.
        /// </summary>
        /// <version>1.2</version>
        /// <default>false</default>
        [ReflectorProperty("labelOnSuccess", Required = false)]
        public bool LabelOnSuccess { get; set; }

        /// <summary>
        /// Only list modifications checked in by specified logins.
        /// </summary>
        /// <version>1.2</version>
        /// <default>None</default>
        [ReflectorProperty("restrictLogins", Required = false)]
        public string RestrictLogins { get; set; }

        /// <summary>
        /// Converts the comment (or parts from it) into an url pointing to the issue for this build. See <link>IssueUrlBuilder</link> for
        /// more details 
        /// </summary>
        /// <version>1.2</version>
        /// <default>false</default>
        [ReflectorProperty("webUrlBuilder", InstanceTypeKey = "type", Required = false)]
        public IModificationUrlBuilder UrlBuilder { get; set; }

        /// <summary>
        /// Specifies whether the current version of the source should be retrieved from CVS. 
        /// </summary>
        /// <version>1.2</version>
        /// <default>true</default>
        [ReflectorProperty("autoGetSource", Required = false)]
        public bool AutoGetSource { get; set; }

        /// <summary>
        /// Specifies whether or not a clean copy should be retrieved.
        /// </summary>
        /// <version>1.2</version>
        /// <default>true</default>
        [ReflectorProperty("cleanCopy", Required = false)]
        public bool CleanCopy { get; set; }

        /// <summary>
        /// Specifies whether the checkout command should be used instead of update.
        /// </summary>
        /// <version>1.2</version>
        /// <default>false</default>
        [ReflectorProperty("forceCheckout", Required = false)]
        public bool ForceCheckout { get; set; }

        /// <summary>
        /// The branch to check for modifications on. 
        /// </summary>
        /// <version>1.2</version>
        /// <default>None</default>
        [ReflectorProperty("branch", Required = false)]
        public string Branch { get; set; }

        /// <summary>
        /// By default the CVS tag name used when labelOnSuccess is set to true is ver-BuildLabel. If you specify this property, the
        /// prefix ver- will be replaced with the value you specify. 
        /// </summary>
        /// <version>1.2</version>
        /// <default>ver-</default>
        [ReflectorProperty("tagPrefix", Required = false)]
        public string TagPrefix { get; set; }

        /// <summary>
        /// Suppresses headers that do not have revisions within the specified modification window. Setting this option to true will reduce
        /// the time that it takes for CCNet to poll CVS for changes. Only fairly recent versions of CVS support this option. Run cvs --help
        /// log to see if the -S option is listed.
        /// </summary>
        /// <version>1.2</version>
        /// <default>false</default>
        [ReflectorProperty("suppressRevisionHeader", Required = false)]
        public bool SuppressRevisionHeader { get; set; }

		public string FormatCommandDate(DateTime date)
		{
			return date.ToUniversalTime().ToString(COMMAND_DATE_FORMAT, CultureInfo.InvariantCulture);
		}

		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			Modification[] modifications = GetModifications(CreateLogProcessInfo(from), from.StartTime, to.StartTime);

			StripRepositoryRootFromModificationFolderNames(modifications);
			UrlBuilder.SetupModification(modifications);

            base.FillIssueUrl(modifications);
            return modifications;
        }

		public override void LabelSourceControl(IIntegrationResult result)
		{
			if (LabelOnSuccess && result.Succeeded)
			{
				Execute(NewLabelProcessInfo(result));
			}
		}

		public override void GetSource(IIntegrationResult result)
		{
            GetBuildProgressInformation(result).SignalStartRunTask("Getting source from CVS");

			if (!AutoGetSource) return;

			if (!ForceCheckout && DoesCvsDirectoryExist(result))
			{
				UpdateSource(result);
			}
			else
			{
				CheckoutSource(result);
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

		private bool DoesCvsDirectoryExist(IIntegrationResult result)
		{
			string cvsDirectory = Path.Combine(result.BaseFromWorkingDirectory(WorkingDirectory), "CVS");
			return fileSystem.DirectoryExists(cvsDirectory);
		}

		private void CheckoutSource(IIntegrationResult result)
		{
            if (string.IsNullOrEmpty(CvsRoot))
				throw new ConfigurationException("<cvsroot> configuration element must be specified in order to automatically checkout source from CVS.");

            // enable Stdout monitoring
            ProcessExecutor.ProcessOutput += ProcessExecutor_ProcessOutput;

			Execute(NewCheckoutProcessInfo(result));

            // remove Stdout monitoring
            ProcessExecutor.ProcessOutput -= ProcessExecutor_ProcessOutput;
		}

		private ProcessInfo NewCheckoutProcessInfo(IIntegrationResult result)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			AppendCvsRoot(builder);
			builder.AddArgument("-q");
			builder.AddArgument("checkout");
			builder.AddArgument("-R");
			builder.AddArgument("-P");
			builder.AddArgument("-r", Branch);
			builder.AddArgument(Module);
			return NewProcessInfoWithArgs(result, builder.ToString());
		}

		private void UpdateSource(IIntegrationResult result)
		{
            // enable Stdout monitoring
            ProcessExecutor.ProcessOutput += ProcessExecutor_ProcessOutput;

			Execute(NewGetSourceProcessInfo(result));

            // remove Stdout monitoring
            ProcessExecutor.ProcessOutput -= ProcessExecutor_ProcessOutput;
		}

		private ProcessInfo NewGetSourceProcessInfo(IIntegrationResult result)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			AppendCvsRoot(builder);
			builder.AppendArgument("-q update -d -P"); // build directories, prune empty directories
			builder.AppendIf(CleanCopy, "-C");
			builder.AddArgument("-r", Branch);

			return NewProcessInfoWithArgs(result, builder.ToString());
		}

		private ProcessInfo CreateLogProcessInfo(IIntegrationResult from)
		{
			return NewProcessInfoWithArgs(from, BuildLogProcessInfoArgs(from.StartTime));
		}

		private ProcessInfo NewLabelProcessInfo(IIntegrationResult result)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			AppendCvsRoot(buffer);
			buffer.AppendArgument(string.Format("tag {0}{1}", TagPrefix, ConvertIllegalCharactersInLabel(result)));
			return NewProcessInfoWithArgs(result, buffer.ToString());
		}

		private string ConvertIllegalCharactersInLabel(IIntegrationResult result)
		{
			return Regex.Replace(result.Label, @"\.", "_");
		}

		private ProcessInfo NewProcessInfoWithArgs(IIntegrationResult result, string args)
		{
		    var wd = result.BaseFromWorkingDirectory(WorkingDirectory);
            // ensure working directory exists
		    fileSystem.EnsureFolderExists(wd);

            var pi = new ProcessInfo(Executable, args, wd);
            SetEnvironmentVariables(pi, result);
            return pi;
		}

        /// <summary>
        /// Set default environment variables for CVS
        /// </summary>
        /// <param name="pi">The command.</param>
        /// <param name="result">IntegrationResult for which the command is being run.</param>
        private void SetEnvironmentVariables(ProcessInfo pi, IIntegrationResult result)
        {
            if(executionEnvironment.IsRunningOnWindows)
            {
                var cvsHomePath = result.ArtifactDirectory.TrimEnd(Path.DirectorySeparatorChar);

                // set %HOME% env var (see http://jira.public.thoughtworks.org/browse/CCNET-1793)
                pi.EnvironmentVariables["HOME"] = cvsHomePath;

                //ensure %HOME%\.cvspass file exists (see http://jira.public.thoughtworks.org/browse/CCNET-1795)
                fileSystem.EnsureFileExists(Path.Combine(cvsHomePath, ".cvspass"));
                Log.Debug("[CVS] Set %HOME% environment variable to '{0}'.", cvsHomePath);
            }
        }

		// cvs [-d :ext:mycvsserver:/cvsroot/myrepo] -q log -N "-d>2004-12-24 12:00:00 GMT" -rmy_branch (with branch)
		// cvs [-d :ext:mycvsserver:/cvsroot/myrepo] -q log -Nb "-d>2004-12-24 12:00:00 GMT" (without branch)
		//		public const string HISTORY_COMMAND_FORMAT = @"{0}-q log -N{3} ""-d>{1}""{2}";		// -N means 'do not show tags'
		private string BuildLogProcessInfoArgs(DateTime from)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			AppendCvsRoot(buffer);
			buffer.AddArgument("-q"); // quiet
			buffer.AddArgument("rlog");
			buffer.AddArgument("-N"); // do not show tags
			buffer.AppendIf(SuppressRevisionHeader, "-S");
            if (string.IsNullOrEmpty(Branch))
			{
				buffer.AddArgument("-b"); // only list revisions on HEAD
			}
			else
			{
				buffer.AppendArgument("-r{0}", Branch); // list revisions on branch
			}
			buffer.AppendArgument(@"""-d>{0}""", FormatCommandDate(from));
            if (!string.IsNullOrEmpty(RestrictLogins))
			{
				foreach (string login in RestrictLogins.Split(','))
				{
					buffer.AppendArgument("-w{0}", login.Trim());
				}
			}
			buffer.AddArgument(Module);
			return buffer.ToString();
		}

		private void AppendCvsRoot(ProcessArgumentBuilder buffer)
		{
			buffer.AddArgument("-d", CvsRoot);
		}

		private void StripRepositoryRootFromModificationFolderNames(Modification[] modifications)
		{
			foreach (Modification modification in modifications)
			{
				modification.FolderName = StripRepositoryFolder(modification.FolderName);
			}
		}

		private const string LocalCvsProtocolString = ":local:";

		private string StripRepositoryFolder(string rcsFilePath)
		{
			string repositoryFolder = GetRepositoryFolder();
			if (rcsFilePath.StartsWith(repositoryFolder))
			{
				return rcsFilePath.Remove(0, repositoryFolder.Length);
			}
			return rcsFilePath;
		}

		/// <summary>
		/// Get the repository folder in order to strip it from the RCS file.
		/// The repository folder is the last part of the CVSRoot path -- unless the local protocol is used on windows machines.
		/// Examples: 
		///		CvsRoot=":pserver:anonymous@cruisecontrol.cvs.sourceforge.net:/cvsroot/cruisecontrol", Module="cruisecontrol", RepositoryFolder="/cvsroot/cruisecontrol/cruisecontrol"
		///		CvsRoot=":local:C:\dev\CVSRoot", Module="fitwebservice", RepositoryFolder="C:\dev\CVSRoot/fitwebservice"
		/// </summary>
		private string GetRepositoryFolder()
		{
			string modulePath = '/' + Module + '/';
			if (CvsRoot.StartsWith(LocalCvsProtocolString))
				return CvsRoot.Substring(LocalCvsProtocolString.Length) + modulePath;
			return CvsRoot.Substring(CvsRoot.LastIndexOf(':') + 1) + modulePath;
		}
	}
}
