using System.Collections;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class ProjectDetailsListGenerator
	{
		private readonly LocalCruiseManagerAggregator cruiseManager;

		public ProjectDetailsListGenerator(LocalCruiseManagerAggregator cruiseManager)
		{
			this.cruiseManager = cruiseManager;
		}

		public ProjectStatus[] ProjectDetailsList
		{
			get
			{
				ArrayList detailsList = cruiseManager.ProjectDetails;
				// ToDo - generate URL, don't use the passed in one.
				return (ProjectStatus[]) detailsList.ToArray(typeof (ProjectStatus));
			}
		}
	}
}
