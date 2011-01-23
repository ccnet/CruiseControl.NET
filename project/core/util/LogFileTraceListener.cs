
using System;
using System.Diagnostics;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// 	
    /// </summary>
	public class LogFileTraceListener : TraceListener
	{
		private TextWriterTraceListener listener;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogFileTraceListener" /> class.	
        /// </summary>
        /// <param name="logfile">The logfile.</param>
        /// <remarks></remarks>
		public LogFileTraceListener(string logfile) : base(logfile) 
		{ 
			listener = new TextWriterTraceListener(logfile);
		}

		private string CreateMessage()
		{
			return string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0}", DateTime.Now.ToString(CultureInfo.CurrentCulture));
		}

		private string CreateMessage(string category)
		{
			return string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0}: {1}", DateTime.Now.ToString(CultureInfo.CurrentCulture), category);
		}

        /// <summary>
        /// Writes the specified message.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
		public override void Write(string message) 
		{
			listener.Write(message, CreateMessage());
		}

        /// <summary>
        /// Writes the specified obj.	
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <remarks></remarks>
		public override void Write(object obj) 
		{
			listener.Write(obj, CreateMessage());
		}

        /// <summary>
        /// Writes the specified message.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="category">The category.</param>
        /// <remarks></remarks>
		public override void Write(string message, string category) 
		{
			listener.Write(message, CreateMessage(category));
		}

        /// <summary>
        /// Writes the specified obj.	
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="category">The category.</param>
        /// <remarks></remarks>
		public override void Write(object obj, string category) 
		{
			listener.Write(obj, CreateMessage(category));
		}

        /// <summary>
        /// Writes the line.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
		public override void WriteLine(string message) 
		{
			listener.WriteLine(message, CreateMessage());
		}

        /// <summary>
        /// Writes the line.	
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <remarks></remarks>
		public override void WriteLine(object obj) 
		{
			listener.WriteLine(obj, CreateMessage());
		}

        /// <summary>
        /// Writes the line.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="category">The category.</param>
        /// <remarks></remarks>
		public override void WriteLine(string message, string category) 
		{
			listener.WriteLine(message, CreateMessage(category));
		}

        /// <summary>
        /// Writes the line.	
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="category">The category.</param>
        /// <remarks></remarks>
		public override void WriteLine(object obj, string category) 
		{
			listener.WriteLine(obj, CreateMessage(category));
		}

        /// <summary>
        /// Flushes this instance.	
        /// </summary>
        /// <remarks></remarks>
		public override void Flush()
		{
			base.Flush();
			listener.Flush();
		}

        /// <summary>
        /// Closes this instance.	
        /// </summary>
        /// <remarks></remarks>
		public override void Close()
		{
			base.Close();
			listener.Close();
		}

        /// <summary>
        /// Disposes the specified disposing.	
        /// </summary>
        /// <param name="disposing">The disposing.</param>
        /// <remarks></remarks>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing) listener.Dispose();
		}
	}
}
