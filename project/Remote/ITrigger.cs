using System;
using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <summary>
	/// Interface of all integration trigger used by CruiseControl.NET.
	/// A trigger applies to a particular project.
	/// </summary>
	[TypeConverter(typeof (ExpandableObjectConverter))]
	public interface ITrigger
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
		/// Notifies the trigger that an integration has completed.
		/// </summary>
		void IntegrationCompleted();

		/// <summary>
		/// Returns the time of the next build.
		/// </summary>
		DateTime NextBuild { get; }
	}
}