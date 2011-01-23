using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Telelogic
{
	/// <summary>
	///     A subclass to escape ASCII bell characters (0x07) from the output
	///     of Synergy commands
	/// </summary>
	public class SynergyProcessExecutor : ProcessExecutor
	{
		/// <summary>
		///     Replaces all ASCII bell characters (0x07) (^G) with a space
		///     character.  Certain ccm.exe commands emit a bell, which cannot
		///     be disabled.  The CCNET XML parser disallows this reserved character.
		/// </summary>
		/// <param name="processInfo">The process to run.</param>
		/// <returns>
		///     Sanitized standard output and input.
		/// </returns>
		public override ProcessResult Execute(ProcessInfo processInfo)
		{
			char bell = (char) 0x07;
			char empty = ' ';

			ProcessResult retVal = base.Execute(processInfo);
			string standardOutput = retVal.StandardOutput.Replace(bell, empty);
			string standardError = retVal.StandardError.Replace(bell, empty);
			return new ProcessResult(standardOutput, standardError, retVal.ExitCode, retVal.TimedOut);
		}
	}
}