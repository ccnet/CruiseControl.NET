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
			if (CruiseControlSwitch.TraceInfo)
			{
				WriteLine("Info", message);
			}
		}

		public static void Debug(string message)
		{
			if (CruiseControlSwitch.TraceVerbose)
			{
				WriteLine("Debug", message);
			}
		}

		public static void Warning(string message)
		{
			if (CruiseControlSwitch.TraceWarning)
			{
				WriteLine("Warning", message);
			}
		}

		public static void Warning(Exception ex)
		{
			if (CruiseControlSwitch.TraceWarning)
			{
				WriteLine("Warning", CreateExceptionMessage(ex));
			}
		}

		public static void Error(Exception ex)
		{
			if (CruiseControlSwitch.TraceError)
			{
				WriteLine("Error", CreateExceptionMessage(ex));
			}
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
