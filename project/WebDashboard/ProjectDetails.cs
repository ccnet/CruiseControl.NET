using System;

using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	/// <summary>
	/// This is the value object that is passed to the datagrid in the aspx page
	/// </summary>
	public class ProjectDetails : ProjectStatus
	{
		private readonly string forceBuildUrl;

		public ProjectDetails(ProjectStatus projectStatus,	string forceBuildURL)
			: base (projectStatus.Status, projectStatus.BuildStatus, projectStatus.Activity, projectStatus.Name, projectStatus.WebURL, 
			projectStatus.LastBuildDate, projectStatus.LastBuildLabel)
		{
			this.forceBuildUrl = forceBuildURL;
		}


		public string ForceBuildURL
		{
			get { return forceBuildUrl; }
		}
	}
}
