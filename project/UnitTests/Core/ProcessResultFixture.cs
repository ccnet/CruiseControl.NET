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
			return new ProcessResult(stdOut,string.Empty, ProcessResult.SUCCESSFUL_EXIT_CODE, false);
		}

		public static ProcessResult CreateTimedOutResult()
		{
			return new ProcessResult("timed out", string.Empty, ProcessResult.SUCCESSFUL_EXIT_CODE, true);
		}

		public static ProcessResult CreateNonZeroExitCodeResult()
		{
            return CreateNonZeroExitCodeResult("failed",string.Empty);
		}

        public static ProcessResult CreateNonZeroExitCodeResult(string stdOut)
        {
            return CreateNonZeroExitCodeResult(stdOut,string.Empty);
        }

		public static ProcessResult CreateNonZeroExitCodeResult(string stdOut, string stdErr)
		{
            return new ProcessResult(stdOut, stdErr, -2, false);		    
		}
	}
}