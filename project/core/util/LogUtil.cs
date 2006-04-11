using System;
using System.Text;
using System.Threading;
using log4net;
using log4net.Config;

// This attribute tells log4net to use the settings in the app.config file for configuration
[assembly: XmlConfigurator()]

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class Log
	{
		private static ILog logger = LogManager.GetLogger("CruiseControl.NET");

		/// <summary>
		/// Utility type, not intended for instantiation.
		/// </summary>
		private Log()
		{
		}

		static Log()
		{
			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("The trace level is currently set to debug.  "
				+ "This will cause CCNet to log at the most verbose level, which is useful for setting up or debugging the server.  "
				+ "Once your server is running smoothly, we recommend changing this setting in {0} to a lower level.",
					AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
			}
		}
		
		public static void Info(string message)
		{
			logger.Info(message);
		}

		public static void Debug(string message)
		{
			logger.Debug(message);
		}

		public static void Warning(string message)
		{
			logger.Warn(message);
		}

		public static void Warning(Exception ex)
		{
			logger.Warn(CreateExceptionMessage(ex));
		}

		public static void Error(string message)
		{
			logger.Error(message);
		}

		public static void Error(Exception ex)
		{
			logger.Error(CreateExceptionMessage(ex));
		}

		private static string CreateExceptionMessage(Exception ex)
		{
			if (ex is ThreadAbortException)
			{
				return "Thread aborted for Project: " + Thread.CurrentThread.Name;
			}

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
	}
}