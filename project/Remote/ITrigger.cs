using System;
using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <summary>
	/// Interface of all integration trigger used by CruiseControl.NET.
	/// A trigger applies to a particular project.
	/// </summary>
    /// <title>Trigger Blocks</title>
	[TypeConverter(typeof (ExpandableObjectConverter))]
	public interface ITrigger
	{
		/// <summary>
		/// Notifies the trigger that an integration has completed.
		/// </summary>
		void IntegrationCompleted();

		/// <summary>
		/// Returns the time of the next build.
		/// </summary>
		DateTime NextBuild { get; }

        /// <summary>
        /// Fires this instance.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
		IntegrationRequest Fire();
	}
}