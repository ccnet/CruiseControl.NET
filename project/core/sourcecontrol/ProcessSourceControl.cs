using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using tw.ccnet.core.util;

namespace tw.ccnet.core.sourcecontrol
{
	public abstract class ProcessSourceControl : ISourceControl
	{
		// todo: make configurable
		public int Timeout
		{
			get { return 30000; }
		}

		protected abstract IHistoryParser HistoryParser
		{
			get;
		}

		// TODO: is it necessary to specify to date -- just want changes after from date
		public Modification[] GetModifications(DateTime from, DateTime to)
		{
			Process process = CreateHistoryProcess(from, to);
			TextReader reader = null;
			try
			{
				reader = Execute(process);
				return ParseModifications(reader);
			}
			finally
			{
				Close(reader, process);
			}
		}

		public void LabelSourceControl(string label, DateTime timeStamp) 
		{
//			Process process = CreateLabelProcess(label, timeStamp);
//			TextReader reader = null;
//			try
//			{
//				reader = Execute(process);
//			}
//			finally
//			{
//				Close(reader, process);
//			}
		}

		protected virtual TextReader Execute(Process process)
		{
			TextReader reader = ProcessUtil.ExecuteRedirected(process);
			string result = reader.ReadToEnd();
			process.WaitForExit(120000);
			return new StringReader(result);
		}

		public abstract Process CreateHistoryProcess(DateTime from, DateTime to);
		public abstract Process CreateLabelProcess(string label, DateTime timeStamp);

		protected Modification[] ParseModifications(TextReader reader)
		{
			return HistoryParser.Parse(reader);
		}

		private void Close(TextReader reader, Process process)
		{
			if (reader != null)
			{
				reader.Close();
			}

			if (process != null)
			{
				process.WaitForExit(Timeout);
				process.Close();
			}
		}
	}
}
