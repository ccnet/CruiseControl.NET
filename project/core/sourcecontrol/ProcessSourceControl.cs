using System;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Util;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	public abstract class ProcessSourceControl : ISourceControl
	{
		protected ProcessExecutor _executor;
		private const int DEFAULT_TIMEOUT = 600000;
		private IHistoryParser _historyParser;

		public ProcessSourceControl(IHistoryParser historyParser) : this(historyParser, new ProcessExecutor())
		{
		}

		public ProcessSourceControl(IHistoryParser historyParser, ProcessExecutor executor)
		{
			_executor = executor;
			_historyParser = historyParser;
		}

		[ReflectorProperty("timeout", Required=false)]
		public int Timeout = DEFAULT_TIMEOUT;

		public bool ShouldRun(IIntegrationResult result)
		{
			return result.Working;
		}

		public void Run(IIntegrationResult result)
		{
			result.Modifications = GetModifications(result.LastModificationDate, DateTime.Now);
		}

		public abstract Modification[] GetModifications(DateTime from, DateTime to);

		public abstract void LabelSourceControl(string label, DateTime timeStamp);

		protected Modification[] GetModifications(ProcessInfo info, DateTime from, DateTime to)
		{
			ProcessResult result = Execute(info);
			return ParseModifications(result, from, to);
		}

		protected ProcessResult Execute(ProcessInfo processInfo)
		{
			processInfo.TimeOut = Timeout;
			ProcessResult result = _executor.Execute(processInfo);

			if (result.TimedOut)
			{
				throw new CruiseControlException("Source control operation has timed out.");
			}
			else if (result.Failed)
			{
				throw new CruiseControlException(string.Format("Source control operation failed: {0}. Process command: {1} {2}", 
					result.StandardError, processInfo.FileName, processInfo.Arguments));
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
			return _historyParser.Parse(reader, from, to);
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
	}
}