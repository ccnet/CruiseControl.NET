using System;
using System.IO;
using System.Threading;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	/// <summary>
	/// ProcessReader asynchronously reads from the supplied TextReader stream.  The output of the stream is stored
	/// in the Output property.  The ProcessReader needs to operate in a separate thread, otherwise if the stream filled while
	/// the thread is blocked waiting for the process to exit, deadlock will occur.
	/// </summary>
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
