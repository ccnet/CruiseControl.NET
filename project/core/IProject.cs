using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	/// <summary>
	/// Interface to which all projects must adhere, and via which all application
	/// code should interact with projects.
	/// </summary>
	public interface IProject
	{
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
		/// Where the results web page for this project can be found
		/// </summary>
		string WebURL 
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
		/// Gets the project's working directory, where the primary build and checkout happens
		/// </summary>
		string WorkingDirectory
		{
			get;
		}

		/// <summary>
		/// Gets the project's artifact directory, where build logs and distributables can be placed
		/// </summary>
		string ArtifactDirectory
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
		IntegrationStatus LatestBuildStatus { get; }

		/// <summary>
		/// This method is called when the project is being deleted from the server. It allows resources to be cleaned up, SCM clients to be unregistered, etc.
		/// </summary>
		void Purge(bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment);
	}
}
