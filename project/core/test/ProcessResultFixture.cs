using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Test
{
	public class ProcessResultFixture
	{
		public static ProcessResult CreateSuccessfulResult()
		{
			return new ProcessResult("success", "", ProcessResult.SUCCESSFUL_EXIT_CODE, false);
		}
	}
}
