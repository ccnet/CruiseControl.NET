using System;
using System.Collections;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Util;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	public abstract class ProcessSourceControl : ISourceControl
	{
		private const int DEFAULT_TIMEOUT = 600000;
		private ProcessExecutor _executor;
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

		public bool ShouldRun(IntegrationResult result)
		{
			return result.Working;
		}

		public void Run(IntegrationResult result)
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

			if (result.HasError)
				throw new CruiseControlException(string.Format("Source control operation caused an error: {0}\nOutput: {1}", result.StandardError, result.StandardOutput));
			else if (result.TimedOut)
				throw new CruiseControlException("Source control operation has timed out.");

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
	}
}