using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	public abstract class ProcessSourceControl : ISourceControl
	{
		// todo: make configurable
		public int Timeout
		{
			get { return 30000; }
		}

		#region Abstract property and methods

		protected abstract IHistoryParser HistoryParser
		{
			get;
		}

		public abstract Process CreateHistoryProcess(DateTime from, DateTime to);
		public abstract Process CreateLabelProcess(string label, DateTime timeStamp);

		#endregion

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
			Process process = CreateLabelProcess(label, timeStamp);
			if (process != null) 
			{
				TextReader reader = null;
				try
				{
					reader = Execute(process);
				}
				finally
				{
					Close(reader, process);
				}
			}
		}

		protected virtual TextReader Execute(Process process)
		{
			TextReader reader = ProcessUtil.ExecuteRedirected(process);

			// TODO: this call will block until the process ends (so there's no point calling WaitForExit below)
			// to do this pattern properly, the output must be read in another thread
			// see the class NAntBuilder.StdOutReader (which could be made a public utility class)
			string result = reader.ReadToEnd();
			
			process.WaitForExit(120000);
			return new StringReader(result);
		}

		protected Modification[] ParseModifications(TextReader reader)
		{
			return HistoryParser.Parse(reader);
		}

		private void Close(TextReader reader, Process process)
		{
			if (reader != null)
				reader.Close();

			if (process != null)
			{
				process.WaitForExit(Timeout);
				process.Close();
			}
		}
	}
}
