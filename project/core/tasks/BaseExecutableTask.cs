namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System;
    using System.Collections;
    using System.IO;
    using ThoughtWorks.CruiseControl.Core.Util;
    using System.Text;

    public abstract class BaseExecutableTask : TaskBase
	{
		protected ProcessExecutor executor;
		protected BuildProgressInformation buildProgressInformation;

		protected abstract string GetProcessFilename();
		protected abstract string GetProcessArguments(IIntegrationResult result);
		protected abstract string GetProcessBaseDirectory(IIntegrationResult result);
		protected abstract int GetProcessTimeout();

		protected virtual int[] GetProcessSuccessCodes()
		{
			return null;
		}

		protected virtual ProcessInfo CreateProcessInfo(IIntegrationResult result)
		{
			ProcessInfo info = new ProcessInfo(GetProcessFilename(), GetProcessArguments(result), GetProcessBaseDirectory(result), GetProcessSuccessCodes());
			info.TimeOut = GetProcessTimeout();

			IDictionary properties = result.IntegrationProperties;
			foreach (string key in properties.Keys)
			{
				info.EnvironmentVariables[key] = StringUtil.IntegrationPropertyToString(properties[key]);
			}

			return info;
		}

        [Obsolete("This method is no longer supported.")]
        protected ProcessResult TryToRun(ProcessInfo info, IIntegrationResult result)
        {
            throw new NotImplementedException("This method is no longer supported");
        }

        /// <summary>
        /// Tries to run an external executable.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="result">The result.</param>
        /// <param name="stdOut">The standard output stream.</param>
        /// <param name="stdErr">The standard error stream.</param>
        /// <returns>
        /// A <see cref="ProcessResult"/> containing the results.
        /// </returns>
        protected ProcessResult TryToRun(ProcessInfo info, IIntegrationResult result, Stream stdOut, Stream stdErr)
        {
            return this.TryToRun(info, result, stdOut, stdErr, false);
        }

        /// <summary>
        /// Tries to run an external executable.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="result">The result.</param>
        /// <param name="stdOut">The standard output stream.</param>
        /// <param name="stdErr">The standard error stream.</param>
        /// <param name="generateXml">If set to <c>true</c> XML will be generated.</param>
        /// <returns>
        /// A <see cref="ProcessResult"/> containing the results.
        /// </returns>
        protected ProcessResult TryToRun(ProcessInfo info, IIntegrationResult result, Stream stdOut, Stream stdErr, bool generateXml)
        {
            buildProgressInformation = result.BuildProgressInformation;

            ProcessResult processResult = null;
            try
            {
                // enable Stdout monitoring
                executor.ProcessOutput += ProcessExecutor_ProcessOutput;

                // Run the executable
                processResult = executor.Execute(info, stdOut, stdErr, generateXml);
                return processResult;
            }
            catch (IOException e)
            {
                throw new BuilderException(this, string.Format("Unable to execute: {0} {1}\n{2}", info.FileName, info.SafeArguments, e), e);
            }
            finally
            {
                // remove Stdout monitoring
                executor.ProcessOutput -= ProcessExecutor_ProcessOutput;
            }
        }

		private void ProcessExecutor_ProcessOutput(object sender, ProcessOutputEventArgs e)
		{
			if (buildProgressInformation == null)
				return;

			// ignore error output in the progress information
			if (e.OutputType == ProcessOutputType.ErrorOutput)
				return;

			buildProgressInformation.AddTaskInformation(e.Data);
		}
	}
}
