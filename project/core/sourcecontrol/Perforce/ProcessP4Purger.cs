using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce
{
    /// <summary>
    /// 	
    /// </summary>
	public class ProcessP4Purger : IP4Purger
	{
		private readonly IP4ProcessInfoCreator infoCreator;
		private readonly ProcessExecutor executor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessP4Purger" /> class.	
        /// </summary>
        /// <param name="executor">The executor.</param>
        /// <param name="infoCreator">The info creator.</param>
        /// <remarks></remarks>
		public ProcessP4Purger(ProcessExecutor executor, IP4ProcessInfoCreator infoCreator)
		{
			this.executor = executor;
			this.infoCreator = infoCreator;
		}

        /// <summary>
        /// Purges the specified p4.	
        /// </summary>
        /// <param name="p4">The p4.</param>
        /// <param name="workingDirectory">The working directory.</param>
        /// <remarks></remarks>
		public void Purge(P4 p4, string workingDirectory)
		{
			if (p4.Client != null && p4.Client != string.Empty)
			{
				Log.Info(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Attempting to Delete Perforce Client Spec [{0}]", p4.Client));
				DeleteClientSpec(p4);
			}
			Log.Info(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Attempting to Delete Working Directory [{0}]", workingDirectory));
			new IoService().DeleteIncludingReadOnlyObjects(workingDirectory);
		}

		private void DeleteClientSpec(P4 p4)
		{
			ProcessResult result = executor.Execute(infoCreator.CreateProcessInfo(p4, "client -d " + p4.Client));
			if (result.ExitCode != ProcessResult.SUCCESSFUL_EXIT_CODE)
			{
				throw new CruiseControlException(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Failed to Initialize client (exit code was {0}).\r\nStandard output was: {1}\r\nStandard error was {2}", result.ExitCode, result.StandardOutput, result.StandardError));
			}
		}
	}
}
