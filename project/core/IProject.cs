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
		/// The minimum period of time for which the build process should wait
		/// after one build, before starting the next (unless a build is forced).
		/// </summary>
		int MinimumSleepTime 
		{
			get; 
		}

		/// <summary>
		/// Gets a list of IPublisher instances, used by this project.
		/// TODO make this an IPublisher[] (need to check whether NetReflector works with arrays).
		/// </summary>
		ArrayList Publishers
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
		/// Attempts to run an integration.  If <see cref="forceBuild"/> is not set, then a build
		/// should occur only if the elapsed sleeping period has occurred.  If <see cref="forceBuild"/>
		/// if true, then a build should occur regardless of sleeping time.
		/// </summary>
		/// <param name="forceBuild"></param>
		void RunIntegration(bool forceBuild);

		/// <summary>
		/// Returns the most recent build status.
		/// </summary>
		/// <returns>The most recent build status</returns>
		IntegrationStatus GetLatestBuildStatus();
	}
}
