using System;
using System.Drawing;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class ProjectGridTest
	{
		private ProjectGrid projectGrid;
		private DynamicMock urlBuilderMock;

		[SetUp]
		public void Setup()
		{
			urlBuilderMock = new DynamicMock(typeof(IUrlBuilder));
			projectGrid = new ProjectGrid((IUrlBuilder) urlBuilderMock.MockInstance);
		}

		private void VerifyAll()
		{
			urlBuilderMock.Verify();
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
				IntegrationStatus.Success, ProjectActivity.Sleeping, "my project", "url", DateTime.Today, "1", DateTime.Today);
			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, new DefaultServerSpecifier("server"))
				};

			// Execute
			ProjectGridRow[] rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual(1, rows.Length);
			Assert.AreEqual("my project", rows[0].Name);
			VerifyAll();
		}

		[Test]
		public void ShouldCopyBuildStatusToProjectRow()
		{
			// Setup
			ProjectStatus projectStatus1 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Success, ProjectActivity.Sleeping, "my project", "url", DateTime.Today, "1", DateTime.Today);
			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, new DefaultServerSpecifier("server"))
				};

			// Execute
			ProjectGridRow[] rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual("Success", rows[0].BuildStatus);
			Assert.AreEqual(Color.Green.Name, rows[0].BuildStatusHtmlColor);

			// Setup
			projectStatus1 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Failure, ProjectActivity.Sleeping, "my project", "url", DateTime.Today, "1", DateTime.Today);
			statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, new DefaultServerSpecifier("server"))
				};

			// Execute
			rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual("Failure", rows[0].BuildStatus);
			Assert.AreEqual(Color.Red.Name, rows[0].BuildStatusHtmlColor);

			// Setup
			projectStatus1 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Unknown, ProjectActivity.Sleeping, "my project", "url", DateTime.Today, "1", DateTime.Today);
			statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, new DefaultServerSpecifier("server"))
				};

			// Execute
			rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual("Unknown", rows[0].BuildStatus);
			Assert.AreEqual(Color.Yellow.Name, rows[0].BuildStatusHtmlColor);

			// Setup
			projectStatus1 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Exception, ProjectActivity.Sleeping, "my project", "url", DateTime.Today, "1", DateTime.Today);
			statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, new DefaultServerSpecifier("server"))
				};

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
				IntegrationStatus.Success, ProjectActivity.Sleeping, "my project", "url", date, "1", DateTime.Today);
			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, new DefaultServerSpecifier("server"))
				};

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
				IntegrationStatus.Success, ProjectActivity.Sleeping, "my project", "url", DateTime.Today, "my label", DateTime.Today);
			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, new DefaultServerSpecifier("server"))
				};

			// Execute
			ProjectGridRow[] rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual("Running", rows[0].Status);
			VerifyAll();

			// Setup
			projectStatus1 = new ProjectStatus(ProjectIntegratorState.Stopped, 
				IntegrationStatus.Success, ProjectActivity.Sleeping, "my project", "url", DateTime.Today, "my label", DateTime.Today);
			statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, new DefaultServerSpecifier("server"))
				};

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
				IntegrationStatus.Success, ProjectActivity.Sleeping, "my project", "url", DateTime.Today, "my label", DateTime.Today);
			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, new DefaultServerSpecifier("server"))
				};

			// Execute
			ProjectGridRow[] rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual("Sleeping", rows[0].Activity);
			VerifyAll();

			// Setup
			projectStatus1 = new ProjectStatus(ProjectIntegratorState.Stopped, 
				IntegrationStatus.Success, ProjectActivity.CheckingModifications, "my project", "url", DateTime.Today, "my label", DateTime.Today);
			statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, new DefaultServerSpecifier("server"))
				};

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
				IntegrationStatus.Success, ProjectActivity.Sleeping, "my project", "url", date, "my label", DateTime.Today);
			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, new DefaultServerSpecifier("server"))
				};

			// Execute
			ProjectGridRow[] rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual("my label", rows[0].LastBuildLabel);
			VerifyAll();
		}

		[Test]
		public void ShouldCopyUrlFromProjectStatusToProjectRow()
		{
			// Setup
			ProjectStatus projectStatus1 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Success, ProjectActivity.Sleeping, "my project", "url", DateTime.Today, "1", DateTime.Today);
			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, new DefaultServerSpecifier("server"))
				};

			// Execute
			ProjectGridRow[] rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual("url", rows[0].Url);
			VerifyAll();
		}

		[Test]
		public void ShouldCreateCorrectForceBuildUrl()
		{
			// Setup
			ProjectStatus projectStatus1 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Success, ProjectActivity.Sleeping, "myproject", "url", DateTime.Today, "1", DateTime.Today);
			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, new DefaultServerSpecifier("myServer"))
				};

			IActionSpecifier expectedActionSpecifier = new ActionSpecifierWithName("myAction");
			urlBuilderMock.ExpectAndReturn("BuildFormName", "myForceButton", expectedActionSpecifier, new string[] { "myServer", "myproject" });

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
			ProjectStatus projectStatus1 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Success, ProjectActivity.Sleeping, "a", "url", DateTime.Today, "1", DateTime.Today);
			ProjectStatus projectStatus2 = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Success, ProjectActivity.Sleeping, "b", "url", DateTime.Today, "1", DateTime.Today);
			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, new DefaultServerSpecifier("server")),
					new ProjectStatusOnServer(projectStatus2, new DefaultServerSpecifier("server"))
				};

			// Execute
			ProjectGridRow[] rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual(2, rows.Length);
			Assert.AreEqual("a", rows[0].Name);
			Assert.AreEqual("b", rows[1].Name);

			// Execute
			rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, false);

			// Verify
			Assert.AreEqual(2, rows.Length);
			Assert.AreEqual("b", rows[0].Name);
			Assert.AreEqual("a", rows[1].Name);

			VerifyAll();
		}
	}
}
