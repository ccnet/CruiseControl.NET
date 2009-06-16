using System;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	public abstract class ProcessSourceControl : ISourceControl
	{
		protected ProcessExecutor executor;
		protected IHistoryParser historyParser;
		private Timeout timeout = Timeout.DefaultTimeout;

		public ProcessSourceControl(IHistoryParser historyParser) : this(historyParser, new ProcessExecutor())
		{
		}

		public ProcessSourceControl(IHistoryParser historyParser, ProcessExecutor executor)
		{
			this.executor = executor;
			this.historyParser = historyParser;
		}

		[ReflectorProperty("timeout", typeof (TimeoutSerializerFactory))]
		public Timeout Timeout
		{
			get { return timeout; }
			set { timeout = (value == null) ? Timeout.DefaultTimeout : value; }
		}

		public abstract Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to);

		public abstract void LabelSourceControl(IIntegrationResult result);

		protected Modification[] GetModifications(ProcessInfo info, DateTime from, DateTime to)
		{
			ProcessResult result = Execute(info);
			return ParseModifications(result, from, to);
		}

		protected ProcessResult Execute(ProcessInfo processInfo)
		{
			processInfo.TimeOut = Timeout.Millis;
			ProcessResult result = executor.Execute(processInfo);

			if (result.TimedOut)
			{
				throw new CruiseControlException("Source control operation has timed out.");
			}
			else if (result.Failed)
			{
				throw new CruiseControlException(string.Format("Source control operation failed: {0}. Process command: {1} {2}",
				                                               result.StandardError, processInfo.FileName, processInfo.SafeArguments));
			}
			else if (result.HasErrorOutput)
			{
				Log.Warning(string.Format("Source control wrote output to stderr: {0}", result.StandardError));
			}
			return result;
		}

		protected Modification[] ParseModifications(ProcessResult result, DateTime from, DateTime to)
		{
			return ParseModifications(new StringReader(result.StandardOutput), from, to);
		}

		protected Modification[] ParseModifications(TextReader reader, DateTime from, DateTime to)
		{
			return historyParser.Parse(reader, from, to);
		}

		public virtual void GetSource(IIntegrationResult result)
		{
		}

		public virtual void Initialize(IProject project)
		{
		}

		public void Purge(IProject project)
		{
		}


        // rw issue
        [ReflectorProperty("issueUrlBuilder", InstanceTypeKey = "type", Required = false)]
        public IModificationUrlBuilder IssueUrlBuilder;


        protected void FillIssueUrl(Modification[] modifications)
        {
            if ((IssueUrlBuilder != null) && (modifications != null))
            {
                IssueUrlBuilder.SetupModification(modifications);
            }
        }

	}
}