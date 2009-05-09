using System.Text;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce
{
	public class P4ConfigProcessInfoCreator : IP4ProcessInfoCreator
	{
		public ProcessInfo CreateProcessInfo(P4 p4, string extraArguments)
		{
			ProcessInfo processInfo = new ProcessInfo(p4.Executable, BuildCommonArguments(p4) + extraArguments);
			processInfo.TimeOut = 0; // Don't time out - this should be configurable
			return processInfo;
		}

		private string BuildCommonArguments(P4 p4) 
		{
			StringBuilder args = new StringBuilder();
			args.Append("-s "); // for "scripting" mode
            if (!string.IsNullOrEmpty(p4.Client)) 
			{
				args.Append("-c " + p4.Client + " ");
			}
            if (!string.IsNullOrEmpty(p4.Port)) 
			{
				args.Append("-p " + p4.Port + " ");
			}
            if (!string.IsNullOrEmpty(p4.User))
			{
				args.Append("-u " + p4.User + " ");
			}
            if (!string.IsNullOrEmpty(p4.Password))
			{
				args.Append("-P " + p4.Password + " ");
			}
			return args.ToString();
		}
	}
}