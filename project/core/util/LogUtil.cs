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
			WriteLine(LogTraceLevel.Info, message, CruiseControlSwitch.TraceInfo);
		}

		public static void Debug(string message)
		{
			WriteLine(LogTraceLevel.Debug, message, CruiseControlSwitch.TraceVerbose);
		}

		public static void Warning(string message)
		{
			WriteLine(LogTraceLevel.Warning, message, CruiseControlSwitch.TraceWarning);
		}

		public static void Warning(Exception ex)
		{
			WriteLine(LogTraceLevel.Warning, ex, CruiseControlSwitch.TraceWarning);
		}

		public static void Error(string message)
		{
			WriteLine(LogTraceLevel.Error, message, CruiseControlSwitch.TraceError);
		}

		public static void Error(Exception ex)
		{
			WriteLine(LogTraceLevel.Error, ex, CruiseControlSwitch.TraceError);
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

		private static void WriteLine(LogTraceLevel level, Exception ex, bool traceSwitch)
		{
			WriteLine(level, CreateExceptionMessage(ex), traceSwitch);
		}

		private static void WriteLine(LogTraceLevel level, string message, bool traceSwitch)
		{
			if (traceSwitch)
			{
				WriteLine(level, message);
			}
		}

		private static void WriteLine(LogTraceLevel level, string message)
		{
			string category = string.Format("[{0}:{1}]", GetContextName(), level);
			Trace.WriteLine(message, category); 
		}

		enum LogTraceLevel
		{
			Error,
			Warning,
			Info,
			Debug
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

				if (Level == TraceLevel.Verbose) 
					WriteLine(LogTraceLevel.Debug, "The trace level is currently set to debug.  This will cause CCNet to log at the most verbose level, which is useful for setting up or debugging the server.  Once your server is running smoothly, we recommend changing this setting in your ccnet.config file to a lower level.");
			}
		}
	}
}
