using System;
using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <summary>
	/// Interface of all integration schedules used by CruiseControl.NET.
	/// A schedule applies to a particular project.
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ISchedule
	{
		/// <summary>
		/// Gets a value indicating whether integration should happen
		/// at the time of the method call (i.e. Now!).  This method may
		/// be called repeatedly until conditions change such that integration
		/// should occur.  As this method will be called very regularly (many
		/// times a second), its internal processing should be kept concise.
		/// </summary>
		/// <returns>An item from the <see cref="BuildCondition"/> enumeration,
		/// indicating whether to build.</returns>
		BuildCondition ShouldRunIntegration();

		/// <summary>
		/// Gets a value indicating whether continuous integration should
		/// cease for this project.
		/// </summary>
		/// <returns>True if integration should be stopped, else false.</returns>
		bool ShouldStopIntegration();

		/// <summary>
		/// Notifies the schedule that an integration has completed.
		/// </summary>
		void IntegrationCompleted();

		/// <summary>
		/// Tells the scheduler that an integration should occur the next
		/// time <see cref="ShouldRunIntegration"/> is called.
		/// </summary>
		void ForceBuild();
	}
}
