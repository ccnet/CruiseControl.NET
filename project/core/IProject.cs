using System;
using System.Collections;
using tw.ccnet.remote;

namespace tw.ccnet.core
{
	/// <summary>
	/// Interface to which all projects must adhere, and via which all application
	/// code should interact with projects.
	/// </summary>
	public interface IProject
	{
		/// <summary>
		/// Raised upon completion of an integration for this project.
		/// </summary>
		event IntegrationCompletedEventHandler IntegrationCompleted;

		/// <summary>
		/// The name of this project.
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Schedule information for this project's build.
		/// </summary>
		ISchedule Schedule 
		{
			get;
		}

		/// <summary>
		/// Gets this project's current activity, such as Building, Sleeping, etc...
		/// </summary>
		ProjectActivity CurrentActivity 
		{
			get;
		}

		/// <summary>
		/// Runs an integration of this project.
		/// </summary>
		/// <param name="buildCondition"></param>
		/// <returns>The result of the integration, or null if no integration took place.</returns>
		IntegrationResult RunIntegration(BuildCondition buildCondition);

		/// <summary>
		/// Returns the most recent build status.
		/// </summary>
		/// <returns>The most recent build status</returns>
		IntegrationStatus GetLatestBuildStatus();
	}
}
