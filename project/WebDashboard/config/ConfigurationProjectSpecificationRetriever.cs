using System;
using ThoughtWorks.CruiseControl.WebDashboard.Config;

namespace ThoughtWorks.CruiseControl.WebDashboard.config
{
	/// <summary>
	/// Summary description for ConfigurationProjectSpecificationRetriever.
	/// </summary>
	public class ConfigurationProjectSpecificationRetriever
	{
		private readonly IConfigurationGetter configurationGetter;

		public ConfigurationProjectSpecificationRetriever(IConfigurationGetter configurationGetter)
		{
			this.configurationGetter = configurationGetter;
		}

		public ProjectSpecification[] ProjectSpecifications
		{
			get
			{
				object projects = configurationGetter.GetConfig("CCNet/projects");
				if (projects == null)
				{
					throw new ApplicationException("<projects> section not configured correctly in web.config");
				}

				if (! (projects is ProjectSpecification[]))
				{
					throw new ApplicationException("Application not configured correctly - CCNet/projects is not bound to a Section Handler that returns a ProjectSpecification[]");
				}

				return (ProjectSpecification[]) projects;
			}
		}
	}
}
