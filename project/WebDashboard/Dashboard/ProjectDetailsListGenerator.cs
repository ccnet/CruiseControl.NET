using System.Collections;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.config;
using ThoughtWorks.CruiseControl.WebDashboard.Config;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class ProjectDetailsListGenerator
	{
		private readonly IConfigurationGetter configurationGetter;
		private readonly LocalCruiseManagerAggregator cruiseManager;

		public ProjectDetailsListGenerator(LocalCruiseManagerAggregator cruiseManager, IConfigurationGetter configurationGetter)
		{
			this.cruiseManager = cruiseManager;
			this.configurationGetter = configurationGetter;
		}

		public ProjectStatus[] ProjectDetailsList
		{
			get
			{
				ArrayList detailsList = cruiseManager.ProjectDetails;
				ProjectSpecification[] projectSpecifications = new ConfigurationProjectSpecificationRetriever(configurationGetter).ProjectSpecifications;

				foreach (ProjectStatus status in detailsList)
				{
					foreach (ProjectSpecification specification in projectSpecifications)
					{
						if (status.Name.Trim().ToLower() == specification.name.Trim().ToLower())
						{
							// ToDo, I know, hardcoded
							// ToDo - we need a URL generator
							status.WebURL = string.Format("projectreport.aspx?{0}={1}", LogFileUtil.ProjectQueryString, specification.name);
						}
					}
				}

				return (ProjectStatus[]) detailsList.ToArray(typeof (ProjectStatus));
			}
		}
	}
}
