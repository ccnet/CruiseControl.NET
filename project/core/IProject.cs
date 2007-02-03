using System.Xml;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	/// <summary>
	/// Interface to which all projects must adhere, and via which all application
	/// code should interact with projects.
	/// </summary>
	public interface IProject : IIntegratable
	{
		/// <summary>
		/// The name of this project.
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// An optional category that groups the project
		/// </summary>
		string Category
		{
			get;
		}

		/// <summary>
		/// A component to trigger integrations for this project.
		/// TODO: remove
		/// </summary>
		ITrigger Triggers 
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
		/// Gets the p
		///roject's working directory, where the primary build and checkout happens
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
		/// This method is called when the project is being deleted from the server. It allows resources to be cleaned up, SCM clients to be unregistered, etc.
		/// </summary>
		void Purge(bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment);

		ExternalLink[] ExternalLinks { get; }

		XmlDocument Statistics { get; }
		
		void Initialize();
		
		ProjectStatus CreateProjectStatus(IProjectIntegrator integrator);
		
		void AddMessage(Message message);
	}
}
