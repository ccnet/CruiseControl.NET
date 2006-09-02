using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	public class ProjectStatusFixture
	{
		public const string DefaultProject = "project";
		public const string DefaultCategory = "category";
		public static readonly ProjectActivity DefaultActivity = ProjectActivity.CheckingModifications;
		public static readonly DateTime DefaultLastBuildDate = DateTime.Now;
		public const ProjectIntegratorState DefaultIntegratorState = ProjectIntegratorState.Running;
		private const IntegrationStatus DefaultIntegrationStatus = IntegrationStatus.Success;
		public const string DefaultUrl = "http://localhost";
		public const string DefaultLastLabel = "1.0.0.0";
		public const string DefaultLabel = "1.0.0.1";

		public static ProjectStatus New(IntegrationStatus integrationStatus, DateTime lastBuildDate)
		{
			return New(DefaultProject, DefaultActivity, integrationStatus, DefaultIntegratorState, DefaultLabel, lastBuildDate);
		}

		public static ProjectStatus New(IntegrationStatus integrationStatus, ProjectActivity activity)
		{
			return New(DefaultProject, activity, integrationStatus, DefaultIntegratorState, DefaultLabel, DefaultLastBuildDate);
		}

		public static ProjectStatus New(string projectName)
		{
			return New(projectName, DefaultLabel);
		}

		public static ProjectStatus New(string projectName, string label)
		{
			return New(projectName, ProjectActivity.Building, DefaultIntegrationStatus, DefaultIntegratorState, label, DefaultLastBuildDate);
		}

		public static ProjectStatus New(string name, IntegrationStatus integrationStatus)
		{
			return New(name, integrationStatus, DefaultLastBuildDate);
		}

		public static ProjectStatus New(string name, IntegrationStatus integrationStatus, DateTime dateTime)
		{
			return New(name, ProjectActivity.Building, integrationStatus, DefaultIntegratorState, DefaultLabel, dateTime);
		}

		public static ProjectStatus New(ProjectActivity activity, string label)
		{
			return New(DefaultProject, activity, DefaultIntegrationStatus, DefaultIntegratorState, label, DefaultLastBuildDate);
		}

		public static ProjectStatus New(string project, ProjectActivity activity, IntegrationStatus integrationStatus, ProjectIntegratorState integratorState, string label, DateTime lastBuildDate)
		{
			return new ProjectStatus(project, DefaultCategory, activity, integrationStatus, integratorState, DefaultUrl, lastBuildDate, label, label, DefaultLastBuildDate);
		}

		public static ProjectStatus New(string project, string category, ProjectActivity activity, IntegrationStatus integrationStatus, ProjectIntegratorState integratorState, string label, DateTime lastBuildDate)
		{
			return new ProjectStatus(project, category, activity, integrationStatus, integratorState, DefaultUrl, lastBuildDate, label, label, DefaultLastBuildDate);
		}
	}
}