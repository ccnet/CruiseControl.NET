using System;

namespace tw.ccnet.core
{
	/// <summary>
	/// Defines a task that may be run.
	/// </summary>
	public interface ITask
	{
		/// <summary>
		/// Runs the task, given the specified <see cref="IntegrationResult"/>.
		/// </summary>
		/// <param name="result"></param>
		void Run(IntegrationResult result);
	}
}
