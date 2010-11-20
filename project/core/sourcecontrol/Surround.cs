using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// <para>
    /// Source Controller for Seapine Surround SCM
    /// </para>
    /// <para type="info">
    /// The Seapine Surround provider is designed to work with Surround 4.1. It may not work with earlier versions of
    /// Surround.
    /// </para>
    /// </summary>	
    /// <title>Seapine Surround Source Control Block</title>
    /// <version>1.0</version>
    /// <key name="type">
    /// <description>The type of source control block.</description>
    /// <value>pvcs</value>
    /// </key>
    /// <example>
    /// <code>
    /// &lt;sourcecontrol type="surround"&gt;
    /// &lt;executable&gt;C:\Program Files\Seapine\Surround SCM\sscm.exe&lt;/executable&gt;
    /// &lt;serverconnect&gt;127.0.0.1:4900&lt;/serverconnect&gt;
    /// &lt;serverlogin&gt;build:buildpw&lt;/serverlogin&gt;
    /// &lt;branch&gt;mybranch&lt;/branch&gt;
    /// &lt;repository&gt;myrepository/myproject&lt;/repository&gt;
    /// &lt;workingDirectory&gt;C:\myproject&lt;/workingDirectory&gt;
    /// &lt;recursive&gt;1&lt;/recursive&gt;
    /// &lt;file&gt;*.cpp&lt;/file&gt;
    /// &lt;searchregexp&gt;0&lt;/searchregexp&gt;
    /// &lt;timeout units="minutes"&gt;10&lt;/timeout&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>
    /// Contributed by Yan Shapochnik and Pete Vasiliauskas at Seapine Software.
    /// </para>
    /// </remarks>
    [ReflectorType("surround")]
	public class Surround : ProcessSourceControl
	{
		public const string TO_SSCM_DATE_FORMAT = "yyyyMMddHHmmss";
		private const string DefaultServerConnection = "127.0.0.1:4900";
		private const string DefaultServerLogin = "Administrator:";

        /// <summary>
        /// Initializes a new instance of the <see cref="Surround"/> class.
        /// </summary>
        public Surround()
            : base(new SurroundHistoryParser(), new ProcessExecutor())
        {
            this.Executable = "sscm";
            this.ServerConnect = DefaultServerConnection;
            this.ServerLogin = DefaultServerLogin;
            this.SearchRegExp = 0;
            this.Recursive = 0;
        }

        /// <summary>
        /// The local path for the Surround SCM command-line client 
        /// (eg. C:\Program Files\Seapine\Surround SCM\sscm.exe).
        /// </summary>
        /// <version>1.0</version>
        /// <default>sscm</default>
        [ReflectorProperty("executable")]
        public string Executable { get; set; }

        /// <summary>
        /// The Surround SCM branch to monitor. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("branch")]
        public string Branch { get; set; }

        /// <summary>
        /// The Surround SCM repository to monitor. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("repository")]
        public string Repository { get; set; }

        /// <summary>
        /// A filename pattern to match to monitor and retrieve files.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("file", Required = false)]
        public string File { get; set; }

        /// <summary>
        /// The local path to get files from Surround SCM to. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("workingDirectory")]
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// The IP address or machine name and port number of the Surround SCM server. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>127.0.0.1:4900</default>
        [ReflectorProperty("serverconnect", Required = false)]
        public string ServerConnect { get; set; }

        /// <summary>
        /// Surround SCM login:password that CCNet should use. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>Administrator</default>
        [ReflectorProperty("serverlogin", Required = false)]
        public string ServerLogin { get; set; }

        /// <summary>
        /// Treat the filename pattern as a regular expression. (Value 1 = true, 0 = false) 
        /// </summary>
        /// <version>1.0</version>
        /// <default>0</default>
        [ReflectorProperty("searchregexp", Required = false)]
        public int SearchRegExp { get; set; }

        /// <summary>
        /// Monitor and retrieve all files in child repositories of the specified repository. (Value 1 = true, 
        /// 0 = false).
        /// </summary>
        /// <version>1.0</version>
        /// <default>0</default>
        [ReflectorProperty("recursive", Required = false)]
        public int Recursive { get; set; }

		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			string command = string.Format(System.Globalization.CultureInfo.CurrentCulture,"cc {0} -d{1}:{2} {3} -b{4} -p{5} {6} -z{7} -y{8}",
			                               File,
			                               from.StartTime.ToString(TO_SSCM_DATE_FORMAT),
			                               to.StartTime.ToString(TO_SSCM_DATE_FORMAT),
			                               (Recursive == 0) ?string.Empty : "-r",
			                               Branch,
			                               Repository,
			                               (SearchRegExp == 0) ? "-x-" : "-x",
			                               ServerConnect,
			                               ServerLogin);

            Modification[] modifications = GetModifications(CreateSSCMProcessInfo(command), from.StartTime, to.StartTime);
            base.FillIssueUrl(modifications);
            return modifications;
        }

		private ProcessInfo CreateSSCMProcessInfo(string command)
		{
			return new ProcessInfo(Executable, command);
		}

		public override void LabelSourceControl(IIntegrationResult result)
		{}

		public override void Initialize(IProject project)
		{
			Execute(CreateSSCMProcessInfo("workdir " + WorkingDirectory + " " + Repository + " -z" + ServerConnect + " -y" + ServerLogin));
		}

		public override void GetSource(IIntegrationResult result)
		{
			Log.Info("Getting source from Surround SCM");
            result.BuildProgressInformation.SignalStartRunTask("Getting source from Surround SCM");

			string command = string.Format(System.Globalization.CultureInfo.CurrentCulture,"get * -q -tcheckin -wreplace {0} -d{1} -b{2} -p{3} -z{4} -y{5}",
			                               (Recursive == 0) ?string.Empty : "-r",
			                               WorkingDirectory,
										   Branch,
			                               Repository,
			                               ServerConnect,
			                               ServerLogin);
			Execute(CreateSSCMProcessInfo(command));
		}
	}
}