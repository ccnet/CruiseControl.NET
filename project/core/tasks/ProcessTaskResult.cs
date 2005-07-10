using System;
using System.Xml;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	public class ProcessTaskResult : ITaskResult
	{
		protected readonly ProcessResult result;

		public ProcessTaskResult(ProcessResult result)
		{
			this.result = result;
			if (Failed())
			{
				Log.Info("Task execution failed");
				Log.Info("Task output: " + result.StandardOutput);
				if (! StringUtil.IsBlank(result.StandardError)) Log.Info("Task error: " + result.StandardError);
			}
		}

		public virtual string Data
		{
			get { return StringUtil.Join(Environment.NewLine, result.StandardOutput, result.StandardError); }
		}

		public virtual void WriteTo(XmlWriter writer)
		{
			writer.WriteStartElement("task");
			if (result.Failed) writer.WriteAttributeString("failed", true.ToString());
			if (result.TimedOut) writer.WriteAttributeString("timedout", true.ToString());
			writer.WriteElementString("standardOutput", result.StandardOutput);
			writer.WriteElementString("standardError", result.StandardError);
			writer.WriteEndElement();
		}

		public bool Succeeded()
		{
			return ! Failed();
		}

		public bool Failed()
		{
			return result.Failed;
		}
	}
}