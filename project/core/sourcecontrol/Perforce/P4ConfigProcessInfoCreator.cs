using System.Text;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce
{
	public class P4ConfigProcessInfoCreator : IP4ProcessInfoCreator
	{
		public ProcessInfo CreateProcessInfo(P4 p4, string extraArguments)
		{
			return new ProcessInfo(p4.Executable, BuildCommonArguments(p4) + extraArguments);
		}

		private string BuildCommonArguments(P4 p4) 
		{
			StringBuilder args = new StringBuilder();
			args.Append("-s "); // for "scripting" mode
			if (p4.Client!=null) 
			{
				args.Append("-c " + p4.Client + " ");
			}
			if (p4.Port!=null) 
			{
				args.Append("-p " + p4.Port + " ");
			}
			if (p4.User!=null)
			{
				args.Append("-u " + p4.User + " ");
			}
			return args.ToString();
		}
	}
}
