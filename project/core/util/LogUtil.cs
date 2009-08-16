using System;
using System.Text;
using System.Threading;
using log4net;
using log4net.Config;
using ThoughtWorks.CruiseControl.Core.Util.Log4NetTrace;

// This attribute tells log4net to use the settings in the app.config file for configuration
[assembly: XmlConfigurator()]

namespace ThoughtWorks.CruiseControl.Core.Util
{
    // TODO: Replace using this class with the ILogger interface and the IoC container.
	public static class Log
	{
		private static ITraceLog logger = TraceLogManager.GetLogger("CruiseControl.NET");
        
        private static bool loggingEnabled = true;

		static Log()
		{

			if (logger.IsDebugEnabled)
			{
				logger.DebugFormat("The trace level is currently set to debug.  "
				+ "This will cause CCNet to log at the most verbose level, which is useful for setting up or debugging the server.  "
				+ "Once your server is running smoothly, we recommend changing this setting in {0} to a lower level.",
					AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
			}
            
            if (logger.IsTraceEnabled)
            {
                logger.WarnFormat("! ! Tracing is enabled ! !"
                    + "It allows you to sent the developpers of CCNet very detailed information of the program flow. "
                    + "This setting should only be enabled if you want to report a bug with the extra information. "
                    + "When bug reporting is done, it is advised to set the trace setting off. "
                    + "Adjust the setting in {0}", AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            }
        
        }

        public static void DisableLogging()
        {
            loggingEnabled = false;
        }

        public static void EnableLogging()
        {
            loggingEnabled = true;
        }

        public static void Info(string message, params object[] args)
		{
			if (loggingEnabled) logger.Info(string.Format(message,args));
		}

        public static void Info(string message)
        {
            if (loggingEnabled) logger.Info(message);
        }


		public static void Debug(string message)
		{
            if (loggingEnabled) logger.Debug(message);
		}

        public static void Debug(string message, params object[] args)
        {
            if (loggingEnabled) logger.Debug(string.Format(message,args));
        }

		public static void Warning(string message)
		{
            if (loggingEnabled) logger.Warn(message);
		}

        public static void Warning(string message, params object[] args)
        {
            if (loggingEnabled) logger.Warn(string.Format(message,args));
        }


		public static void Warning(Exception ex)
		{
            if (loggingEnabled) logger.Warn(CreateExceptionMessage(ex));
		}

		public static void Error(string message)
		{
            logger.Error(message);
		}

        public static void Error(string message, params object[] args)
        {
            logger.Error(string.Format(message,args));
        }


		public static void Error(Exception ex)
		{
            logger.Error(CreateExceptionMessage(ex));

		}

        public static void Trace()
        {
            if (loggingEnabled && logger.IsTraceEnabled) logger.TraceFormat(string.Concat(GetCallingClassName(), "Entering"));
        }


        public static void Trace(string message)
        {
            if (loggingEnabled && logger.IsTraceEnabled)  logger.TraceFormat(string.Concat(GetCallingClassName() ,message));
        }


        public static void Trace(string message, params object[] args)
        {
            if (loggingEnabled && logger.IsTraceEnabled) logger.TraceFormat(string.Concat(GetCallingClassName(), message), args);
        }


        private static string GetCallingClassName()
        {

            System.Diagnostics.StackTrace Stack = default(System.Diagnostics.StackTrace);
            System.Diagnostics.StackFrame CurrentFrame = default(System.Diagnostics.StackFrame);
            string myAssemblyName = null;
            string myClassName = null;
            string myMethodName = null;

            try
            {
                Stack = new System.Diagnostics.StackTrace();
                CurrentFrame = Stack.GetFrame(2);
                myAssemblyName = CurrentFrame.GetMethod().ReflectedType.Assembly.FullName.Split(',')[0];
                myClassName = CurrentFrame.GetMethod().ReflectedType.ToString();
                myMethodName = CurrentFrame.GetMethod().Name;
            }
            catch
            {
                myClassName = "";
                myMethodName = "";
            }

            return string.Format("{0} - {1}.{2} : ", myAssemblyName, myClassName, myMethodName);
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