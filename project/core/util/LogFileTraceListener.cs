using System;
using System.Diagnostics;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class LogFileTraceListener : TraceListener
	{
		private TextWriterTraceListener _listener;

		public LogFileTraceListener(string logfile) : base(logfile) 
		{ 
			_listener = new TextWriterTraceListener(logfile);
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
			_listener.Write(message, CreateMessage());
		}

		public override void Write(object obj) 
		{
			_listener.Write(obj, CreateMessage());
		}

		public override void Write(string message, string category) 
		{
			_listener.Write(message, CreateMessage(category));
		}

		public override void Write(object obj, string category) 
		{
			_listener.Write(obj, CreateMessage(category));
		}

		public override void WriteLine(string message) 
		{
			_listener.WriteLine(message, CreateMessage());
		}

		public override void WriteLine(object obj) 
		{
			_listener.WriteLine(obj, CreateMessage());
		}

		public override void WriteLine(string message, string category) 
		{
			_listener.WriteLine(message, CreateMessage(category));
		}

		public override void WriteLine(object obj, string category) 
		{
			_listener.WriteLine(obj, CreateMessage(category));
		}

		public override void Flush()
		{
			base.Flush();
			_listener.Flush();
		}

		public override void Close()
		{
			base.Close();
			_listener.Close();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing) _listener.Dispose();
		}
	}
}
