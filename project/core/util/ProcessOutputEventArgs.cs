
using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class ProcessOutputEventArgs : EventArgs
	{
		public ProcessOutputEventArgs(ProcessOutputType outputType, string data)
		{
			this.OutputType = outputType;
			this.Data = data;
		}

		public ProcessOutputType OutputType { get; set; }
		public string Data { get; set; }
	}
}
