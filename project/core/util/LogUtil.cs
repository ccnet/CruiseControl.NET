#define TRACE
using System;
using System.Diagnostics;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class LogUtil
	{
		/// <summary>
		/// Utility type, not intended for instantiation.
		/// </summary>
		private LogUtil() {}

		public static void Log(IProject project, string message)
		{
			Trace.WriteLine(string.Format("[project: {0}] {1}", project.Name, message)); 
		}

		public static void Log(IProject project, string message, CruiseControlException ex) 
		{
			if (ex.InnerException!=null)
				LogUtil.Log(project, string.Format("{0}: {1}\n\tInner exception: {2}", message, ex.Message, ex.InnerException));
			else
				LogUtil.Log(project, string.Format("{0}: {1}", message, ex.Message));
		}

		public static void Log(string topic, string message)
		{
			Trace.WriteLine(string.Format("[{0}] {1}", topic, message)); 
		}

		public static void Log(string message)
		{
			Trace.WriteLine(message); 
		}
	}
}
