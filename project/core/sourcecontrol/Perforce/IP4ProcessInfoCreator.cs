using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce
{
	public interface IP4ProcessInfoCreator
	{
		ProcessInfo CreateProcessInfo(P4 p4, string extraArguments);
	}
}
