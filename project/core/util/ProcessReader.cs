using System;
using System.IO;
using System.Threading;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	/// <summary>
	/// ProcessReader asynchronously reads from the supplied TextReader stream.  The output of the stream is stored
	/// in the Output property.  The ProcessReader needs to operate in a separate thread, otherwise if the stream filled while
	/// the thread is blocked waiting for the process to exit, deadlock will occur.
	/// The ProcessReader implements IDisposable to ensure that the thread will be terminated and the stream will be closed when
	/// the reader goes out of scope.
	/// </summary>
	public class ProcessReader : IDisposable
	{
		private Thread thread;
		private TextReader stream;
		private TextWriter output;

		public ProcessReader(TextReader stream)
		{
			this.stream = stream;
			output = new StringWriter();
			thread = new Thread(new ThreadStart(ReadToEnd));
			thread.Priority = ThreadPriority.BelowNormal;
			thread.Start();
		}

		public void WaitForExit()
		{
			thread.Join();
			stream.Close();
		}

		public string Output
		{
			get { return output.ToString(); }
		}

		private void ReadToEnd()
		{
			int nextChar;
			while ((nextChar = stream.Read()) >= 0)
			{
				output.Write((char)nextChar);
			}
		}

		void IDisposable.Dispose()
		{
			thread.Abort();
			stream.Close();
		}
	}
}
