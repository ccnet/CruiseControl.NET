#define TRACE
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class Log
	{
		/// <summary>
		/// Utility type, not intended for instantiation.
		/// </summary>
		private Log() {}

		public static void Info(string message)
		{
			WriteLine("Info", message, CruiseControlSwitch.TraceInfo);
		}

		public static void Debug(string message)
		{
			WriteLine("Debug", message, CruiseControlSwitch.TraceVerbose);
		}

		public static void Warning(string message)
		{
			WriteLine("Warning", message, CruiseControlSwitch.TraceWarning);
		}

		public static void Warning(Exception ex)
		{
			WriteLine("Warning", ex, CruiseControlSwitch.TraceWarning);
		}

		public static void Error(string message)
		{
			WriteLine("Error", message, CruiseControlSwitch.TraceError);
		}

		public static void Error(Exception ex)
		{
			WriteLine("Error", ex, CruiseControlSwitch.TraceError);
		}

		private static string CreateExceptionMessage(Exception ex)
		{
			StringBuilder buffer = new StringBuilder();
			buffer.Append(GetExceptionAlertMessage(ex));
			buffer.Append(ex.Message).Append(Environment.NewLine);
			buffer.Append("----------").Append(Environment.NewLine);
			buffer.Append(ex.ToString()).Append(Environment.NewLine);
			buffer.Append("----------").Append(Environment.NewLine);
			return buffer.ToString();
		}

		private static string GetExceptionAlertMessage(Exception ex)
		{
			return (ex is CruiseControlException) ? "Exception: " : "INTERNAL ERROR: ";
		}

		private static string GetContextName()
		{
			return (Thread.CurrentThread.Name == null) ? "CruiseControl Server" : Thread.CurrentThread.Name;
		}

		private static void WriteLine(string level, Exception ex, bool traceSwitch)
		{
			WriteLine(level, CreateExceptionMessage(ex), traceSwitch);
		}

		private static void WriteLine(string level, string message, bool traceSwitch)
		{
			if (traceSwitch)
			{
				WriteLine(level, message);
			}
		}

		private static void WriteLine(string level, string message)
		{
			string category = string.Format("[{0}:{1}]", GetContextName(), level);
			Trace.WriteLine(message, category); 
		}

		private static TraceSwitch CruiseControlSwitch = new CruiseControlTraceSwitch();

		private class CruiseControlTraceSwitch : TraceSwitch
		{
			public CruiseControlTraceSwitch() : base("CruiseControlSwitch", "TraceSwitch used for instrumenting CruiseControl.NET")
			{
				if (Level == TraceLevel.Off)
				{
					Level = TraceLevel.Error;
				}
			}
		}
	}
}
