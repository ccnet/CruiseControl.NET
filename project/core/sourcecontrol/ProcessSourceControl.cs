using System;
using System.Collections;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	public abstract class ProcessSourceControl : ISourceControl
	{
		private ProcessExecutor executor = new ProcessExecutor();

		// todo: make configurable
		public int Timeout
		{
			get { return 30000; }
		}

		protected abstract IHistoryParser HistoryParser
		{
			get;
		}

		public abstract ProcessInfo CreateHistoryProcessInfo(DateTime from, DateTime to);
		public abstract ProcessInfo CreateLabelProcessInfo(string label, DateTime timeStamp);

		public bool ShouldRun(IntegrationResult result)
		{
			return result.Working;
		}

		public void Run(IntegrationResult result)
		{
			result.Modifications = GetModifications(result.LastModificationDate, DateTime.Now);
		}

		public virtual Modification[] GetModifications(DateTime from, DateTime to)
		{
			ProcessInfo processInfo = CreateHistoryProcessInfo(from, to);
			ProcessResult result = Execute(processInfo);
			return ParseModifications(new StringReader(result.StandardOutput), from, to);
		}

		public void LabelSourceControl(string label, DateTime timeStamp) 
		{
			ProcessInfo processInfo = CreateLabelProcessInfo(label, timeStamp);
			executor.Timeout = Timeout;
			Execute(processInfo);
		}

		protected virtual ProcessResult Execute(ProcessInfo processInfo)
		{
			executor.Timeout = Timeout;
			ProcessResult result = executor.Execute(processInfo);

			// check for stderr
			if (result.StandardError != string.Empty) throw new CruiseControlException("Error: " + result.StandardError);
			return result;
		}

		protected Modification[] ParseModifications(TextReader reader, DateTime from, DateTime to)
		{
			return HistoryParser.Parse(reader, from, to);
		}
	}
}
