using System;
using System.Diagnostics;

namespace tw.ccnet.core.util
{
	public class LogUtil
	{
		public static void Log(IProject project, string message)
		{
			Trace.WriteLine(String.Format("[project: {0}] {1}", project.Name, message)); 
		}

		public static void Log(IProject project, string message, CruiseControlException ex) 
		{
			LogUtil.Log(project, String.Format("{0}: {1}\n{2}", message, ex.Message, ex.InnerException));
		}

		public static void Log(string topic, string message)
		{
			Trace.WriteLine(String.Format("{0}] {1}", topic, message)); 
		}
	}
}
