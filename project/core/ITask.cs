using System;

namespace ThoughtWorks.CruiseControl.Core
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

		/// <summary>
		/// Evaluates if the task should be run, given the specified <see cref="IntegrationResult"/>.
		/// </summary>
		/// <param name="result"></param>
		bool ShouldRun(IntegrationResult result);
	}
}
