using System;
using System.IO;
using System.Threading;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class ProcessReader
	{
		private Thread thread;
		private TextReader stream;
		private string output;

		public ProcessReader(TextReader stream)
		{
			this.thread = new Thread(new ThreadStart(ReadToEnd));
			this.stream = stream;
		}

		public void Start()
		{
			thread.Start();
		}

		public void WaitForExit()
		{
			thread.Join();
			stream.Close();
		}

		public string Output
		{
			get { return output; }
		}

		private void ReadToEnd()
		{
			output = stream.ReadToEnd();
		}
	}
}
