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
			thread.Name = Thread.CurrentThread.Name;
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

		/// <summary>
		/// StreamReader.Peek() does not detect if there are no more characters in the buffer and the stream is blocked.
		/// It will return -1 if the buffer has been read, without checking if the stream is blocked.
		/// Hence, you can't rely on Peek() to determine if the end of the stream has been reached.
		/// </summary>
		private void ReadToEnd()
		{
			string nextLine;
			while ((nextLine = stream.ReadLine()) != null)
			{
				output.WriteLine(nextLine);
				Log.Debug(nextLine);
			}
		}

		void IDisposable.Dispose()
		{
			thread.Abort();
			WaitForExit();
		}
	}
}