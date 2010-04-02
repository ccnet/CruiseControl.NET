using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.Core
{
	/// <summary>
	/// Defines a task that may be run.
	/// </summary>
    /// <title>Task And Publisher Blocks</title>
    [TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ITask
	{
		/// <summary>
		/// Runs the task, given the specified <see cref="IIntegrationResult"/>, in the specified <see cref="IProject"/>.
		/// </summary>
		/// <param name="result"></param>
		void Run(IIntegrationResult result);
	}
}