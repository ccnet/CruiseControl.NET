using System;
using System.Globalization;
using System.IO;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// For Visual Source Safe you must specify the executable, project, username and password. You may also specify the SSDIR. If SSDIR is
    /// not set the default or the SSDIR environment variable will be used.
    /// </summary>
    /// <title>VSS Configuration Example</title>
    /// <version>1.0</version>
    /// <example>
    /// <code title="Minimal example">
    /// &lt;sourcecontrol type="vss" /&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;sourcecontrol type="vss"&gt;
    /// &lt;executable&gt;C:\Program Files\Microsoft Visual Studio\VSS\win32\SS.EXE&lt;/executable&gt;
    /// &lt;project&gt;$/CCNET&lt;/project&gt;
    /// &lt;username&gt;buildguy&lt;/username&gt;
    /// &lt;password&gt;buildguypw&lt;/password&gt;
    /// &lt;ssdir&gt;c:\repos\&lt;/ssdir&gt;
    /// &lt;applyLabel&gt;false&lt;/applyLabel&gt;
    /// &lt;autoGetSource&gt;true&lt;/autoGetSource&gt;
    /// &lt;alwaysGetLatest&gt;false&lt;/alwaysGetLatest&gt;
    /// &lt;workingDirectory&gt;c:\myBuild&lt;/workingDirectory&gt;
    /// &lt;culture&gt;fr-FR&lt;/culture&gt;
    /// &lt;cleanCopy&gt;false&lt;/cleanCopy&gt;
    /// &lt;timeout units="minutes"&gt;10&lt;/timeout&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// </example>
    /// <key name="type">
    /// <description>The type of source control block.</description>
    /// <value>vss</value>
    /// </key>
    /// <remarks>
    /// <heading>Getting the latest source with VSS</heading>
    /// <para>
    /// VSS does not automatically remove files from the local workspace that have been deleted from the VSS database. This does not cause a
    /// problem if you are using the &lt;solution&gt; task or the <link>Visual Studio Task</link> to compile your project. However, if you are
    /// packaging the source for deployment or if you are using the &lt;csc&gt; task to produce a custom build, you may end up compiling these
    /// deleted files in your assembly. To be on the safe side, it might be a good idea to clear the contents of the local workspace after each
    /// build.
    /// </para>
    /// <heading>Using a US English VSS in a non-English culture</heading>
    /// <para>
    /// If you use an English VSS with machines configured to use a non-English culture, it may happen that CCNet will not detect any
    /// modifications after you check-in some code. The reason for this behaviour is that CCNet uses the selected culture on the build server
    /// to determine the language it expects VSS will output for parsing. For example, with fr-CA, CCNet looks for French keywords in the VSS
    /// output. Hence, if your VSS installation does not use the same language, CCNET will not be able to detect any modification.
    /// </para>
    /// <para>
    /// If you're using a US VSS installation, the first step in solving this problem is to include a configuration block set to the US english
    /// culture (&lt;culture&gt;en-US&lt;/culture&gt;). This will make CCNet look for English VSS keywords, and eventually detect
    /// modifications.
    /// </para>
    /// <heading>VSS Issues</heading>
    /// <b>
    /// CCNet periodically reports the following error when connecting to VSS: "Unable to open user login file 
    /// \SourceSafe\Vss60\data\loggedin\&lt;userid&gt;.log." What gives?
    /// </b>
    /// <para>
    /// If you have set CCNet up to manage multiple projects that all connect to the VSS repository using the same user id then you may
    /// sporadically receive this failure. Our analysis suggests that the root of the problem is caused by the fact that VSS will create the
    /// &lt;userid&gt;.log file when a user logs into VSS and delete it when the user logs out again. If a second build is using the repository
    /// concurrently with the same user, when that second build logs out it looks for &lt;userid&gt;.log, but it's gone. Hence the error.
    /// </para>
    /// <para>
    /// There are three approaches that you can take to deal with this:
    /// </para>
    /// <list type="1">
    /// <item>Log into VSS using different users for each CCNet project.</item>
    /// <item>You can keep CCNet from publishing exceptions , so this exception will just get logged into the ccnet.log file.</item>
    /// <item>Leave the VSS GUI open on the integration server. This will mean the userid.log file never gets deleted.</item>
    /// </list>
    /// <para></para>
    /// <para></para>
    /// <b>
    /// If you're using a labeller that returns a label equal to one already applied in the repository, the old label will be deleted when the
    /// new one is added.
    /// </b>
    /// <para>
    /// This is because of a quirk in how VSS deals with labels of the same name; it should not be a problem with the default labeller.
    /// </para>
    /// <para>
    /// This problem usually occurs when someone is using a custom labeller (a class that implements ILabeller) and that custom labeller returns
    /// a constant value.
    /// </para>
    /// <para>
    /// Workaround: If you use a custom labeller, make sure each label is unique.
    /// </para>
    /// <b>
    /// When I try to connect to use the &lt;vss&gt; NAntContrib tasks from <link>The Server Service Application</link> I get this error: 
    /// Failed to open database \\someserver\someshare\vssrep\srcsafe.ini
    /// </b>
    /// <para>
    /// There are a number of known issues with SourceSafe 6.0c. Make sure that you upgrade to the 6.0d version.
    /// </para>
    /// <b>
    /// When I try connecting to VSS when using <link>The Server Service Application</link> I get the error: No VSS database (srcsafe.ini)
    /// found. Use the SSDIR environment variable or run netsetup.
    /// </b>
    /// <para>
    /// Make sure that you are running ccservice using an account that has the necessary permissions to access the network share where your VSS
    /// database is set up. By default ccservice will run as the LocalSystem account, which does not have the necessary priviledges to access
    /// other machines.
    /// </para>
    /// <b>
    /// When using VSS with a <link>Filtered Source Control</link> Block, newly added or removed files don't show up as modifications
    /// </b>
    /// <para>
    /// VSS does not output the paths for added or deleted files. As a result, the modifications returned by CCNet do not have any specified
    /// path information. If a Filtered Source Control Block is used with a path filter then these modifications are likely to be filtered out.
    /// This is an outstanding issue.
    /// </para>
    /// <heading>Useful links</heading>
    /// <list type="1">
    /// <item>Visual SourceSafe Best Practices Guide - http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnvss/html/vssbest.asp</item>
    /// <item>Using VSS With Multiple Timezones - http://support.microsoft.com/default.aspx?scid=kb;en-us;248240&amp;Product=vss</item>
    /// <item>OLE Automation interface Get method behaves differently with VSSVersion and with VSSItem - http://support.microsoft.com/default.aspx?scid=kb;en-us;837417</item>
    /// </list>
    /// </remarks>
	[ReflectorType("vss")]
	public class Vss : ProcessSourceControl
	{
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const string DefaultProject = "$/";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const string SS_DIR_KEY = "SSDIR";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const string SS_REGISTRY_PATH = @"Software\\Microsoft\\SourceSafe";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const string SS_REGISTRY_KEY = "SCCServerPath";
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const string SS_EXE = "ss.exe";
		private const string RecursiveCommandLineOption = "-R";

		private IRegistry registry;
		private string ssDir;
		private string executable;
		private string tempLabel;
		private IVssLocale locale;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vss" /> class.	
        /// </summary>
        /// <remarks></remarks>
		public Vss() : this(new VssLocale(CultureInfo.CurrentCulture))
		{}

		private Vss(IVssLocale locale) : this(locale, new VssHistoryParser(locale), new ProcessExecutor(), new Registry())
		{}

        /// <summary>
        /// Initializes a new instance of the <see cref="Vss" /> class.	
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="historyParser">The history parser.</param>
        /// <param name="executor">The executor.</param>
        /// <param name="registry">The registry.</param>
        /// <remarks></remarks>
		public Vss(IVssLocale locale, IHistoryParser historyParser, ProcessExecutor executor, IRegistry registry) : base(historyParser, executor)
		{
			this.registry = registry;
			this.locale = locale;
            this.Project = DefaultProject;
            this.ApplyLabel = false;
            this.AutoGetSource = true;
            this.AlwaysGetLatest = false;
            this.CleanCopy = true;
		}

        /// <summary>
        /// The location of SS.EXE. If VSS is installed on the integration server, the location of VSS will be read from the registry and this
        /// element may be omitted.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
		[ReflectorProperty("executable", Required = false)]
		public string Executable
		{
			get
			{
				if (executable == null)
					executable = GetExecutableFromRegistry();
				return executable;
			}
			set { executable = value; }
		}

        /// <summary>
        /// The project in the repository to be monitored. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>$/</default>
        [ReflectorProperty("project", Required = false)]
        public string Project { get; set; }

        /// <summary>
        /// VSS user ID that CCNet should use to authenticate. If the username is unspecified, the VSS client will attempt to authenticate
        /// using the NT user.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("username", Required = false)]
        public string Username { get; set; }

        /// <summary>
        /// Password for the VSS user ID.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("password", typeof(PrivateStringSerialiserFactory), Required = false)]
        public PrivateString Password { get; set; }

        /// <summary>
        /// The directory containing SRCSAFE.INI. If this SSDIR environment variable is already set then this property may be omitted. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("ssdir", Required = false)]
		public string SsDir
		{
			get { return ssDir; }
			set { ssDir = StringUtil.StripQuotes(value); }
		}

        /// <summary>
        /// Specifies whether the current CCNet label should be applied to all source files under the current project in VSS.
        /// </summary>
        /// <version>1.0</version>
        /// <default>false</default>
        /// <remarks>
        /// The specified VSS username must have write access to the repository.
        /// </remarks>
        [ReflectorProperty("applyLabel", Required = false)]
        public bool ApplyLabel { get; set; }

        /// <summary>
        /// Specifies whether the current version of the source should be retrieved from VSS.
        /// </summary>
        /// <version>1.0</version>
        /// <default>true</default>
        [ReflectorProperty("autoGetSource", Required = false)]
        public bool AutoGetSource { get; set; }

        /// <summary>
        /// Specifies whether the most recent version of the source should be retrieved from VSS. If not, CCNet will obtain the source as of
        /// the time the build began.
        /// </summary>
        /// <version>1.0</version>
        /// <default>false</default>
        [ReflectorProperty("alwaysGetLatest", Required = false)]
        public bool AlwaysGetLatest { get; set; }

        /// <summary>
        /// The folder into which the source should be retrived from VSS. If this folder does not exist, it will be automatically created.
        /// </summary>
        /// <version>1.0</version>
        /// <default>Project Working Directory</default>
        [ReflectorProperty("workingDirectory", Required = false)]
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// The culture under which VSS is running. This value must match the culture of the VSS installation for CCNet to work with VSS. Most
        /// of the time the default is OK and you may omit this item. If you are using the US version of VSS on a machine that is not set to
        /// the US culture, you should include the configuration block &lt;culture&gt;en-US&lt;/culture&gt;.
        /// </summary>
        /// <version>1.0</version>
        /// <default>The culture of the VSS installation</default>
        [ReflectorProperty("culture", Required = false)]
		public string Culture
		{
			get { return locale.ServerCulture; }
			set { locale.ServerCulture = value; }
		}

        /// <summary>
        /// Controls whether or not VSS gets a clean copy (overwrites modified files) when getting the latest source. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>true</default>
        [ReflectorProperty("cleanCopy", Required = false)]
        public bool CleanCopy { get; set; }


        /// <summary>
        /// Gets the modifications.	
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        /// <remarks></remarks>
	    public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
            string tempOutputFileName = Path.GetTempFileName();
	        return GetModifications(from, to, tempOutputFileName);
        }

        /// <summary>
        /// This method exists only to allow the VssTest unit tests to supply an expected output filename.
        /// DO NOT use this method for normal processing, use <see cref="GetModifications(IIntegrationResult, IIntegrationResult)"/>
        /// instead
        /// </summary>
        public Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to, string tempOutputFileName)
        {
            try
            {
                Execute(CreateHistoryProcessInfo(from, to, tempOutputFileName));
                TextReader outputReader = new StreamReader(tempOutputFileName, Encoding.Default);
                try
                {
                    // return ParseModifications(outputReader, from.StartTime, to.StartTime);
                    Modification[] modifications = ParseModifications(outputReader, from.StartTime, to.StartTime);
                    base.FillIssueUrl(modifications);
                    return modifications;
                }
                finally
                {
                    outputReader.Close();
                }
            }
            finally 
            {
                File.Delete(tempOutputFileName);
            }
            
		}

        /// <summary>
        /// Labels the source control.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
		public override void LabelSourceControl(IIntegrationResult result)
		{
			if (! ApplyLabel) return;

			Execute(NewProcessInfoWith(LabelProcessInfoArgs(result), result));
			tempLabel = null;
		}

        /// <summary>
        /// Gets the source.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
		public override void GetSource(IIntegrationResult result)
		{
            result.BuildProgressInformation.SignalStartRunTask("Getting source from VSS");

			CreateTemporaryLabel(result);

			if (! AutoGetSource) return;

			Log.Info("Getting source from VSS");
			Execute(NewProcessInfoWith(GetSourceArgs(result), result));
		}

        private PrivateArguments GetSourceArgs(IIntegrationResult result)
		{
            var builder = new PrivateArguments();
			builder.Add("get ", Project + "/*?*", true);
			builder.Add(RecursiveCommandLineOption);
			builder.AddIf(ApplyLabel, "-VL", tempLabel);
			builder.AddIf(!AlwaysGetLatest, "-Vd", locale.FormatCommandDate(result.StartTime));
			AppendUsernameAndPassword(builder);
			builder.Add("-I-Y -W -GF- -GTM");
			builder.AddIf(CleanCopy, "-GWR");
			return builder;
		}

		private ProcessInfo CreateHistoryProcessInfo(IIntegrationResult from, IIntegrationResult to, string tempOutputFileName)
		{
			return NewProcessInfoWith(HistoryProcessInfoArgs(from.StartTime, to.StartTime, tempOutputFileName), to);
		}

        private PrivateArguments HistoryProcessInfoArgs(DateTime from, DateTime to, string tempOutputFileName)
		{
			var builder = new PrivateArguments();
			builder.Add("history ", Project, true);
			builder.Add(RecursiveCommandLineOption);
			builder.Add(string.Format(System.Globalization.CultureInfo.CurrentCulture,"-Vd{0}~{1}", locale.FormatCommandDate(to), locale.FormatCommandDate(from)));
			AppendUsernameAndPassword(builder);
			builder.Add("-I-Y");
            builder.Add(null, "-O@" + tempOutputFileName, true);
            return builder;
		}

		private void CreateTemporaryLabel(IIntegrationResult result)
		{
			if (ApplyLabel)
			{
				tempLabel = CreateTemporaryLabelName(result.StartTime);
				LabelSourceControlWith(tempLabel, result);
			}
		}

		private void LabelSourceControlWith(string label, IIntegrationResult result)
		{
			Execute(NewProcessInfoWith(LabelProcessInfoArgs(label, null), result));
		}

        private PrivateArguments LabelProcessInfoArgs(IIntegrationResult result)
		{
			if (result.Succeeded)
			{
				return LabelProcessInfoArgs(result.Label, tempLabel);
			}
			else
			{
				return DeleteLabelProcessInfoArgs();
			}
		}

        private PrivateArguments DeleteLabelProcessInfoArgs()
		{
			return LabelProcessInfoArgs(string.Empty, tempLabel);
		}

		private PrivateArguments LabelProcessInfoArgs(string label, string oldLabel)
		{
            var builder = new PrivateArguments();
			builder.Add("label ", Project, true);
			builder.Add("-L", label);
            builder.AddIf(!string.IsNullOrEmpty(oldLabel), "-VL", oldLabel);
			AppendUsernameAndPassword(builder);
			builder.Add("-I-Y");
            return builder;
		}

		private string CreateTemporaryLabelName(DateTime time)
		{
			return "CCNETUNVERIFIED" + time.ToString("MMddyyyyHHmmss");
		}

		private string GetExecutableFromRegistry()
		{
			string comServerPath = registry.GetExpectedLocalMachineSubKeyValue(SS_REGISTRY_PATH, SS_REGISTRY_KEY);
			return Path.Combine(Path.GetDirectoryName(comServerPath), SS_EXE);
		}

		private ProcessInfo NewProcessInfoWith(PrivateArguments args, IIntegrationResult result)
		{
			string workingDirectory = result.BaseFromWorkingDirectory(WorkingDirectory);
			if (! Directory.Exists(workingDirectory)) Directory.CreateDirectory(workingDirectory);

			ProcessInfo processInfo = new ProcessInfo(Executable, args, workingDirectory);
			if (SsDir != null)
			{
				processInfo.EnvironmentVariables[SS_DIR_KEY] = SsDir;
			}
			return processInfo;
		}

        private void AppendUsernameAndPassword(PrivateArguments builder)
		{
            if (!string.IsNullOrEmpty(Username))
            {
                PrivateString userPlusPass = "\"-Y" + Username + "," + Password.PrivateValue + "\"";
                builder.Add(userPlusPass);
            }
		}
	}
}
