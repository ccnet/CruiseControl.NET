using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
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
		}

        /// <summary>
        /// Should we automatically obtain updated source from PlasticSCM or not? 
        /// </summary>
        /// <remarks>
        /// Optional, default is not to do so.
        /// </remarks>
        [ReflectorProperty("autoGetSource", Required = false)]
        public bool AutoGetSource = true;
        
        /// <summary>
        /// Name of the PlasticSCM executable.  
        /// </summary>
        /// <remarks>
        /// Optional, defaults to "cm".
        /// <remarks>
        [ReflectorProperty("executable", Required=false)]
		public string Executable = DefaultPlasticExecutable;

		[ReflectorProperty("branch", Required=true)]
		public string Branch = string.Empty;

		[ReflectorProperty("repository", Required=false)]
		public string Repository = string.Empty;

        /// <summary>
        /// Pathname of the PlasticSCM working directory, either absolute or relative
        /// to the project working directory.
        /// </summary>
        /// <remarks>
        /// Optional, defaults to the project working directory.
        /// <remarks>
        [ReflectorProperty("workingDirectory", Required = false)]
		public string WorkingDirectory = string.Empty;

        /// <summary>
        /// If set, the source repository will be tagged with the build label upon successful builds.
        /// </summary>
        /// <remarks>
        /// Optional, default is not to tag.
        /// <remarks>
        [ReflectorProperty("labelOnSuccess", Required = false)]
		public bool LabelOnSuccess = false;

		[ReflectorProperty("labelPrefix", Required=false)]
		public string LabelPrefix = "ccver-";

		[ReflectorProperty("forced", Required=false)]
		public bool Forced = false;

		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			// Without the stb if the selector is pointing to a different repository
			// it can't solve path correctly
			Execute(GoToBranchProcessInfo(from));
			
			//Get and parse the modified files.
			return GetModifications(CreateQueryProcessInfo(from, to), from.StartTime, to.StartTime);
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
            if (AutoGetSource)
            {
                Execute(GoToBranchProcessInfo(result));
                Execute(NewGetSourceProcessInfo(result));
            }
		}

		public ProcessInfo NewGetSourceProcessInfo(IIntegrationResult result)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
            builder.AppendArgument(string.Format("update {0}", result.BaseFromWorkingDirectory(WorkingDirectory)));
			if (Forced)
			{
				builder.AppendArgument("--forced");
			}
			return NewProcessInfoWithArgs(result, builder.ToString());
		}

		public ProcessInfo GoToBranchProcessInfo(IIntegrationResult result)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			builder.AppendArgument(string.Format("stb {0}", Branch));
			if (Repository != string.Empty)
			{
				builder.AppendArgument(string.Format("-repository={0}", Repository));
			}
			builder.AppendArgument("--noupdate");
			return NewProcessInfoWithArgs(result, builder.ToString());
		}

		public ProcessInfo CreateQueryProcessInfo(IIntegrationResult from, IIntegrationResult to)
		{
			ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
			builder.AppendArgument(
				string.Format("find revision where branch = '{0}' "+
							  "and revno != 'CO' "+
							  "and date between '{1}' and '{2}'", 
				Branch, from.StartTime.ToString(DATEFORMAT), to.StartTime.ToString(DATEFORMAT)));
        
			if (Repository != string.Empty) 
			{
				builder.AppendArgument(string.Format("on repository '{0}'", Repository));
			}

			builder.AppendArgument(string.Format("--dateformat=\"{0}\"", DATEFORMAT));
			builder.AppendArgument(string.Format("--format=\"{0}\"", FORMAT));

			return NewProcessInfoWithArgs(from, builder.ToString());
		}

		public ProcessInfo CreateLabelProcessInfo(IIntegrationResult result)
		{
			string labelName = LabelPrefix + result.Label;
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AppendArgument(string.Format("mklb {0}", labelName));
			return NewProcessInfoWithArgs(result, buffer.ToString());
		}

		public ProcessInfo LabelProcessInfo(IIntegrationResult result)
		{
			string labelName = LabelPrefix + result.Label;
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AppendArgument(string.Format("label -R lb:{0} .", labelName));
			return NewProcessInfoWithArgs(result, buffer.ToString());
		}

		private ProcessInfo NewProcessInfoWithArgs(IIntegrationResult result, string args)
		{
			return new ProcessInfo(Executable, args, result.BaseFromWorkingDirectory(WorkingDirectory));
		}
	}
}