using System.Collections;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    public abstract class BaseExecutableTask : TaskBase, ITask
	{
		protected ProcessExecutor executor;

		protected abstract string GetProcessFilename();
		protected abstract string GetProcessArguments(IIntegrationResult result);
		protected abstract string GetProcessBaseDirectory(IIntegrationResult result);
		protected abstract int GetProcessTimeout();

		protected virtual int[] GetProcessSuccessCodes()
		{
			return null;
		}

		public abstract void Run(IIntegrationResult result);

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

		protected ProcessResult TryToRun(ProcessInfo info)
		{
			try
			{
				return executor.Execute(info);
			}
			catch (IOException e)
			{
				throw new BuilderException(this, string.Format("Unable to execute: {0} {1}\n{2}", info.FileName, info.Arguments, e), e);
			}
		}		
	}
}
