using System;
using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.Core.State
{
	/// <summary>
	/// Manages the state of continuous integration for a single project.
	/// State must be persisted between shutdown/startup of the CruiseControl.NET
	/// server, as modification dates and label numbers must follow sequence.
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface IStateManager
	{
		/// <summary>
		/// Gets a value indicating whether the state file exists.
		/// </summary>
		/// <returns></returns>
		bool StateFileExists();

		/// <summary>
		/// Loads the state of the project.
		/// </summary>
		/// <returns></returns>
		IntegrationResult LoadState();

		/// <summary>
		/// Persists the state of the project.
		/// </summary>
		/// <param name="result"></param>
		void SaveState(IntegrationResult result);
	}
}
