using System.Globalization;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// This supports Códice Software's Plastic SCM source control system.
    /// </summary>
    /// <title>PlasticSCM Source Control Block</title>
    /// <version>1.3</version>
    /// <key name="type">
    /// <description>The type of source control block.</description>
    /// <value>plasticscm</value>
    /// </key>
    /// <example>
    /// <code title="Basic Example">
    /// &lt;sourcecontrol type="plasticscm"&gt;
    /// &lt;workingDirectory&gt;c:\workspace&lt;/workingDirectory&gt;
    /// &lt;branch&gt;br:/main&lt;/branch&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// <code title="Full Example">
    /// &lt;sourcecontrol type="plasticscm"&gt;
    /// &lt;executable&gt;c:\Program Files\PlasticSCM\client\cm.exe&lt;/executable&gt;
    /// &lt;workingDirectory&gt;c:\workspace&lt;/workingDirectory&gt;
    /// &lt;branch&gt;br:/main&lt;/branch&gt;
    /// &lt;repository&gt;mainrep&lt;/repository&gt;
    /// &lt;forced&gt;true&lt;/forced&gt;
    /// &lt;labelOnSuccess&gt;true&lt;/labelOnSuccess&gt;
    /// &lt;labelPrefix&gt;BL&lt;/labelPrefix&gt;
    /// &lt;timeout units="minutes"&gt;10&lt;/timeout&gt;
    /// &lt;/sourcecontrol&gt;
    /// </code>
    /// </example>
	[ReflectorType("plasticscm")]
	public class PlasticSCM : ProcessSourceControl
	{
		public const string DefaultPlasticExecutable = "cm";
		public const char DELIMITER = '?';

		//Format used in the query to Plastic SCM
		public const string DATEFORMAT = "dd/MM/yyyy HH:mm:ss";
		public static string FORMAT = DELIMITER + "{item}" + DELIMITER + "{owner}" + DELIMITER + "{date}" + DELIMITER + "{changeset}";


		public PlasticSCM() : this(new PlasticSCMHistoryParser(), new ProcessExecutor())
		{
		}

		public PlasticSCM(IHistoryParser parser, ProcessExecutor executor)
			: base(parser, executor)
		{
            this.AutoGetSource = true;
            this.Executable = DefaultPlasticExecutable;
            this.Branch = string.Empty;
            this.Repository = string.Empty;
            this.WorkingDirectory = string.Empty;
            this.LabelOnSuccess = false;
            this.LabelPrefix = "ccver-";
            this.Forced = false;
		}

        /// <summary>
        /// Should we automatically obtain updated source from PlasticSCM or not? 
        /// </summary>
        /// <version>1.3</version>
        /// <default>true</default>
        [ReflectorProperty("autoGetSource", Required = false)]
        public bool AutoGetSource { get; set; }
        
        /// <summary>
        /// Name of the PlasticSCM executable.  
        /// </summary>
        /// <version>1.3</version>
        /// <default>cm</default>
        [ReflectorProperty("executable", Required = false)]
        public string Executable { get; set; }

        /// <summary>
        /// The Plastic SCM branch to monitor. 
        /// </summary>
        /// <version>1.3</version>
        /// <default>n/a</default>
        [ReflectorProperty("branch", Required = true)]
        public string Branch { get; set; }

        /// <summary>
        /// The Plastic SCM repository to monitor. 
        /// </summary>
        /// <version>1.3</version>
        /// <default>Current in workspace</default>
        [ReflectorProperty("repository", Required = false)]
        public string Repository { get; set; }

        /// <summary>
        /// Valid Plastic SCM workspace path. 
        /// </summary>
        /// <version>1.3</version>
        /// <default>Project Working Directory</default>
        [ReflectorProperty("workingDirectory", Required = false)]
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Specifies whether or not CCNet should create an Plastic SCM baseline when the build is successful. 
        /// </summary>
        /// <version>1.3</version>
        /// <default>false</default>
        [ReflectorProperty("labelOnSuccess", Required = false)]
        public bool LabelOnSuccess { get; set; }

        /// <summary>
        /// Specifies the prefix label name. 
        /// </summary>
        /// <version>1.3</version>
        /// <default>ccver-</default>
        [ReflectorProperty("labelPrefix", Required = false)]
        public string LabelPrefix { get; set; }

        /// <summary>
        /// Do the update with the "--forced" option.
        /// </summary>
        /// <version>1.3</version>
        /// <default>false</default>
        [ReflectorProperty("forced", Required = false)]
        public bool Forced { get; set; }

		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			// Without the stb if the selector is pointing to a different repository
			// it can't solve path correctly
			Execute(GoToBranchProcessInfo(from));
			
			//Get and parse the modified files.
            Modification[] modifications = GetModifications(CreateQueryProcessInfo(from, to), from.StartTime, to.StartTime);
            base.FillIssueUrl(modifications);
            return modifications;
		}

		public override void LabelSourceControl(IIntegrationResult result)
		{
			if (LabelOnSuccess && result.Succeeded)
			{
                //The label could exist or the label process find private elements
                Execute(CreateLabelProcessInfo(result));
                Execute(LabelProcessInfo(result));
			}
		}

		public override void GetSource(IIntegrationResult result)
		{
            result.BuildProgressInformation.SignalStartRunTask("Getting source from PlasticSCM");

            if (AutoGetSource)
            {
                Execute(GoToBranchProcessInfo(result));
                Execute(NewGetSourceProcessInfo(result));
            }
		}

		public ProcessInfo NewGetSourceProcessInfo(IIntegrationResult result)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
            builder.AppendArgument(string.Format(System.Globalization.CultureInfo.CurrentCulture,"update {0}", result.BaseFromWorkingDirectory(WorkingDirectory)));
			if (Forced)
			{
				builder.AppendArgument("--forced");
			}
			return NewProcessInfoWithArgs(result, builder.ToString());
		}

		public ProcessInfo GoToBranchProcessInfo(IIntegrationResult result)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			builder.AppendArgument(string.Format(System.Globalization.CultureInfo.CurrentCulture,"stb {0}", Branch));
			if (Repository != string.Empty)
			{
				builder.AppendArgument(string.Format(System.Globalization.CultureInfo.CurrentCulture,"-repository={0}", Repository));
			}
			builder.AppendArgument("--noupdate");
			return NewProcessInfoWithArgs(result, builder.ToString());
		}

		public ProcessInfo CreateQueryProcessInfo(IIntegrationResult from, IIntegrationResult to)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			builder.AppendArgument(
				string.Format(System.Globalization.CultureInfo.CurrentCulture,"find revision where branch = '{0}' "+
							  "and revno != 'CO' "+
							  "and date between '{1}' and '{2}'",
				Branch, from.StartTime.ToString(DATEFORMAT, CultureInfo.InvariantCulture), to.StartTime.ToString(DATEFORMAT, CultureInfo.InvariantCulture)));
        
			if (Repository != string.Empty) 
			{
				builder.AppendArgument(string.Format(System.Globalization.CultureInfo.CurrentCulture,"on repository '{0}'", Repository));
			}

			builder.AppendArgument(string.Format(System.Globalization.CultureInfo.CurrentCulture,"--dateformat=\"{0}\"", DATEFORMAT));
			builder.AppendArgument(string.Format(System.Globalization.CultureInfo.CurrentCulture,"--format=\"{0}\"", FORMAT));

			return NewProcessInfoWithArgs(from, builder.ToString());
		}

		public ProcessInfo CreateLabelProcessInfo(IIntegrationResult result)
		{
			string labelName = LabelPrefix + result.Label;
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AppendArgument(string.Format(System.Globalization.CultureInfo.CurrentCulture,"mklb {0}", labelName));
			return NewProcessInfoWithArgs(result, buffer.ToString());
		}

		public ProcessInfo LabelProcessInfo(IIntegrationResult result)
		{
			string labelName = LabelPrefix + result.Label;
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AppendArgument(string.Format(System.Globalization.CultureInfo.CurrentCulture,"label -R lb:{0} .", labelName));
			return NewProcessInfoWithArgs(result, buffer.ToString());
		}

		private ProcessInfo NewProcessInfoWithArgs(IIntegrationResult result, string args)
		{
			return new ProcessInfo(Executable, args, result.BaseFromWorkingDirectory(WorkingDirectory));
		}
	}
}
