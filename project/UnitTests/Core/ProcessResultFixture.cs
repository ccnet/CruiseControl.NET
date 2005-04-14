using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	public class ProcessResultFixture
	{
		public static ProcessResult CreateSuccessfulResult()
		{
			return CreateSuccessfulResult("success");
		}

		public static ProcessResult CreateSuccessfulResult(string stdOut)
		{
			return new ProcessResult(stdOut, "", ProcessResult.SUCCESSFUL_EXIT_CODE, false);
		}

		public static ProcessResult CreateTimedOutResult()
		{
			return new ProcessResult("timed out", "", ProcessResult.TIMED_OUT_EXIT_CODE, true);
		}

		public static ProcessResult CreateNonZeroExitCodeResult()
		{
			return new ProcessResult("failed", "", -2, false);
		}
	}
}
