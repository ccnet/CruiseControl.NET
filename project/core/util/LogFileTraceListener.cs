#pragma warning disable 1591
using System;
using System.Diagnostics;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class LogFileTraceListener : TraceListener
	{
		private TextWriterTraceListener listener;

		public LogFileTraceListener(string logfile) : base(logfile) 
		{ 
			listener = new TextWriterTraceListener(logfile);
		}

		private string CreateMessage()
		{
			return string.Format("{0}", DateTime.Now.ToString());
		}

		private string CreateMessage(string category)
		{
			return string.Format("{0}: {1}", DateTime.Now.ToString(), category);
		}

		public override void Write(string message) 
		{
			listener.Write(message, CreateMessage());
		}

		public override void Write(object obj) 
		{
			listener.Write(obj, CreateMessage());
		}

		public override void Write(string message, string category) 
		{
			listener.Write(message, CreateMessage(category));
		}

		public override void Write(object obj, string category) 
		{
			listener.Write(obj, CreateMessage(category));
		}

		public override void WriteLine(string message) 
		{
			listener.WriteLine(message, CreateMessage());
		}

		public override void WriteLine(object obj) 
		{
			listener.WriteLine(obj, CreateMessage());
		}

		public override void WriteLine(string message, string category) 
		{
			listener.WriteLine(message, CreateMessage(category));
		}

		public override void WriteLine(object obj, string category) 
		{
			listener.WriteLine(obj, CreateMessage(category));
		}

		public override void Flush()
		{
			base.Flush();
			listener.Flush();
		}

		public override void Close()
		{
			base.Close();
			listener.Close();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing) listener.Dispose();
		}
	}
}
