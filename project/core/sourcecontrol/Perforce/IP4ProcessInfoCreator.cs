using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce
{
    /// <summary>
    /// 	
    /// </summary>
	public interface IP4ProcessInfoCreator
	{
        /// <summary>
        /// Creates the process info.	
        /// </summary>
        /// <param name="p4">The p4.</param>
        /// <param name="extraArguments">The extra arguments.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		ProcessInfo CreateProcessInfo(P4 p4, string extraArguments);
	}
}
