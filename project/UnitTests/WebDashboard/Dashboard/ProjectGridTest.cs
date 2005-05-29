using System;
using System.Drawing;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class ProjectGridTest
	{
		private ProjectGrid projectGrid;
		private DynamicMock urlBuilderMock;
		private DynamicMock linkFactoryMock;
		private IAbsoluteLink projectLink;
		private IServerSpecifier serverSpecifier;
		private IProjectSpecifier projectSpecifier;

		[SetUp]
		public void Setup()
		{
			urlBuilderMock = new DynamicMock(typeof(IUrlBuilder));
			linkFactoryMock = new DynamicMock(typeof(ILinkFactory));
			projectGrid = new ProjectGrid((IUrlBuilder) urlBuilderMock.MockInstance,
				(ILinkFactory) linkFactoryMock.MockInstance);

			serverSpecifier = new DefaultServerSpecifier("server");
			projectSpecifier = new DefaultProjectSpecifier(serverSpecifier, "my project");

			projectLink = new GeneralAbsoluteLink("myLinkText", "myLinkUrl");
		}

		private void VerifyAll()
		{
			urlBuilderMock.Verify();
			linkFactoryMock.Verify();
		}

		private void SetupProjectLinkExpectation()
		{
			SetupProjectLinkExpectation(projectSpecifier);
		}

		private void SetupProjectLinkExpectation(IProjectSpecifier projectSpecifierForLink)
		{
			linkFactoryMock.ExpectAndReturn("CreateProjectLink", projectLink, projectSpecifierForLink, ProjectReportProjectPlugin.ACTION_NAME);
		}

		[Test]
		public void ShouldReturnEmptyListOfRowsWhenNoProjectStatusesAvailable()
		{
			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[0];

			Assert.AreEqual(0, projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true).Length);

			VerifyAll();
		}

		[Test]
		public void ShouldCopyProjectNameToProjectRow()
		{
			// Setup
			ProjectStatus projectStatus1 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Success, ProjectActivity.Sleeping, projectSpecifier.ProjectName, "url", DateTime.Today, "1", DateTime.Today);
			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, serverSpecifier)
				};

			// Execute
			SetupProjectLinkExpectation();
			ProjectGridRow[] rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual(1, rows.Length);
			Assert.AreEqual(projectSpecifier.ProjectName, rows[0].Name);
			VerifyAll();
		}

		[Test]
		public void ShouldHandleResultsWithNoBuildLabel()
		{
			// Setup
			ProjectStatus projectStatus1 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Success, ProjectActivity.Sleeping, projectSpecifier.ProjectName, "url", DateTime.Today, null, DateTime.Today);
			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, serverSpecifier)
				};

			// Execute
			SetupProjectLinkExpectation();
			ProjectGridRow[] rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual(1, rows.Length);
			Assert.AreEqual("no build available", rows[0].LastBuildLabel);
			VerifyAll();
		}

		[Test]
		public void ShouldCopyBuildStatusToProjectRow()
		{
			// Setup
			ProjectStatus projectStatus1 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Success, ProjectActivity.Sleeping, projectSpecifier.ProjectName, "url", DateTime.Today, "1", DateTime.Today);
			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, serverSpecifier)
				};

			SetupProjectLinkExpectation();
			// Execute
			ProjectGridRow[] rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual("Success", rows[0].BuildStatus);
			Assert.AreEqual(Color.Green.Name, rows[0].BuildStatusHtmlColor);

			// Setup
			projectStatus1 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Failure, ProjectActivity.Sleeping, projectSpecifier.ProjectName, "url", DateTime.Today, "1", DateTime.Today);
			statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, serverSpecifier)
				};
			SetupProjectLinkExpectation();

			// Execute
			rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual("Failure", rows[0].BuildStatus);
			Assert.AreEqual(Color.Red.Name, rows[0].BuildStatusHtmlColor);

			// Setup
			projectStatus1 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Unknown, ProjectActivity.Sleeping, projectSpecifier.ProjectName, "url", DateTime.Today, "1", DateTime.Today);
			statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, serverSpecifier)
				};
			SetupProjectLinkExpectation();

			// Execute
			rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual("Unknown", rows[0].BuildStatus);
			Assert.AreEqual(Color.Yellow.Name, rows[0].BuildStatusHtmlColor);

			// Setup
			projectStatus1 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Exception, ProjectActivity.Sleeping, projectSpecifier.ProjectName, "url", DateTime.Today, "1", DateTime.Today);
			statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, serverSpecifier)
				};
			SetupProjectLinkExpectation();

			// Execute
			rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual("Exception", rows[0].BuildStatus);
			Assert.AreEqual(Color.Red.Name, rows[0].BuildStatusHtmlColor);

			VerifyAll();
		}

		[Test]
		public void ShouldCopyLastBuildDateToProjectRow()
		{
			// Setup
			DateTime date = DateTime.Today;
			ProjectStatus projectStatus1 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Success, ProjectActivity.Sleeping, projectSpecifier.ProjectName, "url", date, "1", DateTime.Today);
			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, serverSpecifier)
				};
			SetupProjectLinkExpectation();

			// Execute
			ProjectGridRow[] rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual(date, rows[0].LastBuildDate);
			VerifyAll();
		}

		[Test]
		public void ShouldCopyProjectStatusToProjectRow()
		{
			// Setup
			ProjectStatus projectStatus1 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Success, ProjectActivity.Sleeping, projectSpecifier.ProjectName, "url", DateTime.Today, "my label", DateTime.Today);
			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, serverSpecifier)
				};
			SetupProjectLinkExpectation();

			// Execute
			ProjectGridRow[] rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual("Running", rows[0].Status);
			VerifyAll();

			// Setup
			projectStatus1 = new ProjectStatus(ProjectIntegratorState.Stopped, 
				IntegrationStatus.Success, ProjectActivity.Sleeping, projectSpecifier.ProjectName, "url", DateTime.Today, "my label", DateTime.Today);
			statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, serverSpecifier)
				};
			SetupProjectLinkExpectation();

			// Execute
			rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual("Stopped", rows[0].Status);
			VerifyAll();
		}

		[Test]
		public void ShouldCopyProjectActivityToProjectRow()
		{
			// Setup
			ProjectStatus projectStatus1 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Success, ProjectActivity.Sleeping, projectSpecifier.ProjectName, "url", DateTime.Today, "my label", DateTime.Today);
			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, serverSpecifier)
				};
			SetupProjectLinkExpectation();

			// Execute
			ProjectGridRow[] rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual("Sleeping", rows[0].Activity);
			VerifyAll();

			// Setup
			projectStatus1 = new ProjectStatus(ProjectIntegratorState.Stopped, 
				IntegrationStatus.Success, ProjectActivity.CheckingModifications, projectSpecifier.ProjectName, "url", DateTime.Today, "my label", DateTime.Today);
			statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, new DefaultServerSpecifier("server"))
				};
			SetupProjectLinkExpectation();

			// Execute
			rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual("CheckingModifications", rows[0].Activity);
			VerifyAll();
		}

		[Test]
		public void ShouldCopyLastBuildLabelToProjectRow()
		{
			// Setup
			DateTime date = DateTime.Today;
			ProjectStatus projectStatus1 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Success, ProjectActivity.Sleeping, projectSpecifier.ProjectName, "url", date, "my label", DateTime.Today);
			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, serverSpecifier)
				};
			SetupProjectLinkExpectation();

			// Execute
			ProjectGridRow[] rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual("my label", rows[0].LastBuildLabel);
			VerifyAll();
		}

		[Test]
		public void ShouldCreateLinkToProjectReport()
		{
			// Setup
			ProjectStatus projectStatus1 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Success, ProjectActivity.Sleeping, projectSpecifier.ProjectName, "url", DateTime.Today, "1", DateTime.Today);
			
			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, serverSpecifier)
				};
			SetupProjectLinkExpectation();

			// Execute
			ProjectGridRow[] rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual("myLinkUrl", rows[0].Url);
			VerifyAll();
		}

		[Test]
		public void ShouldCreateCorrectForceBuildUrl()
		{
			// Setup
			ProjectStatus projectStatus1 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Success, ProjectActivity.Sleeping, projectSpecifier.ProjectName, "url", DateTime.Today, "1", DateTime.Today);
			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, serverSpecifier)
				};
			SetupProjectLinkExpectation();

			urlBuilderMock.ExpectAndReturn("BuildFormName", "myForceButton", "myAction");

			// Execute
			ProjectGridRow[] rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual("myForceButton", rows[0].ForceBuildButtonName);
			VerifyAll();
		}

		[Test]
		public void ShouldReturnProjectsSortedByNameIfNameColumnSpecifiedAsSortSeed()
		{
			// Setup
			IProjectSpecifier projectA = new DefaultProjectSpecifier(serverSpecifier, "a");
			IProjectSpecifier projectB = new DefaultProjectSpecifier(serverSpecifier, "b");

			ProjectStatus projectStatus1 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Success, ProjectActivity.Sleeping, "a", "url", DateTime.Today, "1", DateTime.Today);
			ProjectStatus projectStatus2 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Success, ProjectActivity.Sleeping, "b", "url", DateTime.Today, "1", DateTime.Today);
			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, serverSpecifier),
					new ProjectStatusOnServer(projectStatus2, serverSpecifier)
				};
			SetupProjectLinkExpectation(projectA);
			SetupProjectLinkExpectation(projectB);

			// Execute
			ProjectGridRow[] rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual(2, rows.Length);
			Assert.AreEqual("a", rows[0].Name);
			Assert.AreEqual("b", rows[1].Name);

			// Setup
			SetupProjectLinkExpectation(projectA);
			SetupProjectLinkExpectation(projectB);

			// Execute
			rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, false);

			// Verify
			Assert.AreEqual(2, rows.Length);
			Assert.AreEqual("b", rows[0].Name);
			Assert.AreEqual("a", rows[1].Name);

			VerifyAll();
		}

		[Test]
		public void ShouldReturnProjectsSortedByLastBuildDateIfLastBuildDateColumnSpecifiedAsSortSeed()
		{
			// Setup
			IProjectSpecifier projectA = new DefaultProjectSpecifier(serverSpecifier, "a");
			IProjectSpecifier projectB = new DefaultProjectSpecifier(serverSpecifier, "b");

			ProjectStatus projectStatus1 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Success, ProjectActivity.Sleeping, "b", "url", DateTime.Today, "1", DateTime.Today);
			ProjectStatus projectStatus2 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Success, ProjectActivity.Sleeping, "a", "url", DateTime.Today.AddHours(1), "1", DateTime.Today);
			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, serverSpecifier),
					new ProjectStatusOnServer(projectStatus2, serverSpecifier)
				};
			SetupProjectLinkExpectation(projectB);
			SetupProjectLinkExpectation(projectA);

			// Execute
			ProjectGridRow[] rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.LastBuildDate, true);

			// Verify
			Assert.AreEqual(2, rows.Length);
			Assert.AreEqual("b", rows[0].Name);
			Assert.AreEqual("a", rows[1].Name);

			// Setup
			SetupProjectLinkExpectation(projectB);
			SetupProjectLinkExpectation(projectA);

			// Execute
			rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.LastBuildDate, false);

			// Verify
			Assert.AreEqual(2, rows.Length);
			Assert.AreEqual("a", rows[0].Name);
			Assert.AreEqual("b", rows[1].Name);

			VerifyAll();
		}

		[Test]
		public void ShouldReturnProjectsSortedByBuildStatusIfBuildStatusColumnSpecifiedAsSortSeed()
		{
			// Setup
			IProjectSpecifier projectA = new DefaultProjectSpecifier(serverSpecifier, "a");
			IProjectSpecifier projectB = new DefaultProjectSpecifier(serverSpecifier, "b");
			ProjectStatus projectStatus1 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Success, ProjectActivity.Sleeping, "a", "url", DateTime.Today, "1", DateTime.Today);
			ProjectStatus projectStatus2 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Failure, ProjectActivity.Sleeping, "b", "url", DateTime.Today.AddHours(1), "1", DateTime.Today);
			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, serverSpecifier),
					new ProjectStatusOnServer(projectStatus2, serverSpecifier)
				};
			SetupProjectLinkExpectation(projectA);
			SetupProjectLinkExpectation(projectB);

			// Execute
			ProjectGridRow[] rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.BuildStatus, true);

			// Verify
			Assert.AreEqual(2, rows.Length);
			Assert.AreEqual("b", rows[0].Name);
			Assert.AreEqual("a", rows[1].Name);

			// Setup
			SetupProjectLinkExpectation(projectA);
			SetupProjectLinkExpectation(projectB);

			// Execute
			rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.BuildStatus, false);

			// Verify
			Assert.AreEqual(2, rows.Length);
			Assert.AreEqual("a", rows[0].Name);
			Assert.AreEqual("b", rows[1].Name);

			VerifyAll();
		}
	}
}
