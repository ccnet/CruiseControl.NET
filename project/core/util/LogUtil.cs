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
	}
}
