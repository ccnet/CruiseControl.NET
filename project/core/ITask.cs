using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.Core
{
	/// <summary>
	/// Defines a task that may be run.
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ITask
	{
		/// <summary>
		/// Runs the task, given the specified <see cref="IntegrationResult"/>, in the specified <see cref="IProject"/>.
		/// </summary>
		/// <param name="result"></param>
		void Run(IIntegrationResult result);

		/// <summary>
		/// Evaluates if the task should be run, given the specified <see cref="IntegrationResult"/>, in the specified <see cref="IProject"/>.
		/// </summary>
		/// <param name="result"></param>
		bool ShouldRun(IIntegrationResult result);
	}
}
