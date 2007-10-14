using System;
using System.Drawing;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.Core;
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
			projectGrid = new ProjectGrid((ILinkFactory) linkFactoryMock.MockInstance);

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
			ProjectStatus projectStatus1 = ProjectStatusFixture.New(projectSpecifier.ProjectName);
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
			ProjectStatus projectStatus1 = ProjectStatusFixture.New(projectSpecifier.ProjectName, null);
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
			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(ProjectStatusFixture.New(projectSpecifier.ProjectName, IntegrationStatus.Success), serverSpecifier)
				};

			SetupProjectLinkExpectation();
			// Execute
			ProjectGridRow[] rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual("Success", rows[0].BuildStatus);
			Assert.AreEqual(Color.Green.Name, rows[0].BuildStatusHtmlColor);

			// Setup
			statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(ProjectStatusFixture.New(projectSpecifier.ProjectName, IntegrationStatus.Failure), serverSpecifier)
				};
			SetupProjectLinkExpectation();

			// Execute
			rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual("Failure", rows[0].BuildStatus);
			Assert.AreEqual(Color.Red.Name, rows[0].BuildStatusHtmlColor);

			// Setup
			statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(ProjectStatusFixture.New(projectSpecifier.ProjectName, IntegrationStatus.Unknown), serverSpecifier)
				};
			SetupProjectLinkExpectation();

			// Execute
			rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual("Unknown", rows[0].BuildStatus);
			Assert.AreEqual(Color.Yellow.Name, rows[0].BuildStatusHtmlColor);

			// Setup
			statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(ProjectStatusFixture.New(projectSpecifier.ProjectName, IntegrationStatus.Exception), serverSpecifier)
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
			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(ProjectStatusFixture.New(projectSpecifier.ProjectName, IntegrationStatus.Success, date), serverSpecifier)
				};
			SetupProjectLinkExpectation();

			// Execute
			ProjectGridRow[] rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual(DateUtil.FormatDate(date), rows[0].LastBuildDate);
			VerifyAll();
		}

		[Test]
		public void ShouldCopyProjectStatusToProjectRow()
		{
			// Setup
			ProjectStatus projectStatus1 = new ProjectStatus(projectSpecifier.ProjectName, "category",
			                                                 ProjectActivity.Sleeping, IntegrationStatus.Success, ProjectIntegratorState.Running, "url", DateTime.Today, "my label", null, DateTime.Today,"building");
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
			projectStatus1 = new ProjectStatus(projectSpecifier.ProjectName, "category",
			                                   ProjectActivity.Sleeping, IntegrationStatus.Success, ProjectIntegratorState.Stopped, "url", DateTime.Today, "my label", null, DateTime.Today,"");
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
			ProjectStatus projectStatus1 = new ProjectStatus(projectSpecifier.ProjectName, "category", 
			                                                 ProjectActivity.Sleeping, IntegrationStatus.Success, ProjectIntegratorState.Running, "url", DateTime.Today, "my label", null, DateTime.Today,"");
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
			projectStatus1 = new ProjectStatus(projectSpecifier.ProjectName, "category",
			                                   ProjectActivity.CheckingModifications, IntegrationStatus.Success, ProjectIntegratorState.Stopped, "url", DateTime.Today, "my label", null, DateTime.Today,"");
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
			ProjectStatus projectStatus1 = new ProjectStatus(projectSpecifier.ProjectName, "category",
			                                                 ProjectActivity.Sleeping, IntegrationStatus.Success, ProjectIntegratorState.Running, "url", date, "my label", null, DateTime.Today,"");
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
			ProjectStatus projectStatus1 = new ProjectStatus(projectSpecifier.ProjectName, "category",
			                                                 ProjectActivity.Sleeping, IntegrationStatus.Success, ProjectIntegratorState.Running, "url", DateTime.Today, "1", null, DateTime.Today,"");
			
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
		public void ShouldDisplayCurrentProjectMessagesInProjectGridRow()
		{
			// Setup
			ProjectStatus projectStatus1 = new ProjectStatus(projectSpecifier.ProjectName, "category",
				ProjectActivity.Sleeping, IntegrationStatus.Success, ProjectIntegratorState.Running, "url", DateTime.Today, "my label", null, DateTime.Today,"");

			projectStatus1.Messages = new Message[1] {new Message("Test Message")};

			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, serverSpecifier)
				};
			SetupProjectLinkExpectation();

			// Execute
			ProjectGridRow[] rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.IsNotNull(rows[0].CurrentMessage);
			Assert.AreEqual("Test Message", rows[0].CurrentMessage);
			VerifyAll();

			// Setup
			projectStatus1 = new ProjectStatus(projectSpecifier.ProjectName, "category",
				ProjectActivity.Sleeping, IntegrationStatus.Success, ProjectIntegratorState.Stopped, "url", DateTime.Today, "my label", null, DateTime.Today,"");

			projectStatus1.Messages = new Message[2] {new Message(string.Empty), new Message("Second Message")};
            
			statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, serverSpecifier)
				};
			SetupProjectLinkExpectation();

			// Execute
			rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.IsNotNull(rows[0].CurrentMessage);
			Assert.AreEqual("Second Message", rows[0].CurrentMessage);
			VerifyAll();
		}

		[Test]
		public void ShouldCopyProjectCategoryToProjectRow()
		{
			// Setup
			ProjectStatus projectStatus1 = new ProjectStatus(projectSpecifier.ProjectName, "category",
				ProjectActivity.Sleeping, IntegrationStatus.Success, ProjectIntegratorState.Running, "url", DateTime.Today, "my label", null, DateTime.Today,"");


			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, serverSpecifier)
				};
			SetupProjectLinkExpectation();

			// Execute
			ProjectGridRow[] rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual("category", rows[0].Category);
			VerifyAll();

			// Setup
			projectStatus1 = new ProjectStatus(projectSpecifier.ProjectName, "category1",
				ProjectActivity.Sleeping, IntegrationStatus.Success, ProjectIntegratorState.Stopped, "url", DateTime.Today, "my label", null, DateTime.Today,"");


			statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, serverSpecifier)
				};
			SetupProjectLinkExpectation();

			// Execute
			rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.Name, true);

			// Verify
			Assert.AreEqual("category1", rows[0].Category);
			VerifyAll();
		}

		[Test]
		public void ShouldReturnProjectsSortedByNameIfNameColumnSpecifiedAsSortSeed()
		{
			// Setup
			IProjectSpecifier projectA = new DefaultProjectSpecifier(serverSpecifier, "a");
			IProjectSpecifier projectB = new DefaultProjectSpecifier(serverSpecifier, "b");

			ProjectStatus projectStatus1 = new ProjectStatus("a", "category",
			                                                 ProjectActivity.Sleeping, IntegrationStatus.Success, ProjectIntegratorState.Running, "url", DateTime.Today, "1", null, DateTime.Today,"");
			ProjectStatus projectStatus2 = new ProjectStatus("b", "category",
			                                                 ProjectActivity.Sleeping, IntegrationStatus.Success, ProjectIntegratorState.Running, "url", DateTime.Today, "1", null, DateTime.Today,"");
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

			ProjectStatus projectStatus1 = new ProjectStatus("b", "category",
			                                                 ProjectActivity.Sleeping, IntegrationStatus.Success, ProjectIntegratorState.Running, "url", DateTime.Today, "1", null, DateTime.Today,"");
			ProjectStatus projectStatus2 = new ProjectStatus("a", "category",
			                                                 ProjectActivity.Sleeping, IntegrationStatus.Success, ProjectIntegratorState.Running, "url", DateTime.Today.AddHours(1), "1", null, DateTime.Today,"");
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
			ProjectStatus projectStatus1 = new ProjectStatus("a", "category",
			                                                 ProjectActivity.Sleeping, IntegrationStatus.Success, ProjectIntegratorState.Running, "url", DateTime.Today, "1", null, DateTime.Today,"");
			ProjectStatus projectStatus2 = new ProjectStatus("b", "category",
			                                                 ProjectActivity.Sleeping, IntegrationStatus.Failure, ProjectIntegratorState.Running, "url", DateTime.Today.AddHours(1), "1", null, DateTime.Today,"");
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

		[Test]
		public void ShouldReturnProjectsSortedByServerIfServerNameColumnSpecifiedAsSortSeed()
		{
			// Setup
			IServerSpecifier serverSpecifierA = new DefaultServerSpecifier("Aserver");
			IServerSpecifier serverSpecifierB = new DefaultServerSpecifier("Bserver");
			IProjectSpecifier projectA = new DefaultProjectSpecifier(serverSpecifierA, "a");
			IProjectSpecifier projectB = new DefaultProjectSpecifier(serverSpecifierB, "b");

			ProjectStatus projectStatus1 = new ProjectStatus("a", "category",
				ProjectActivity.Sleeping, IntegrationStatus.Success, ProjectIntegratorState.Running, "url", DateTime.Today, "1", null, DateTime.Today,"");
			ProjectStatus projectStatus2 = new ProjectStatus("b", "category",
				ProjectActivity.Sleeping, IntegrationStatus.Failure, ProjectIntegratorState.Running, "url", DateTime.Today.AddHours(1), "1", null, DateTime.Today,"");
			ProjectStatusOnServer[] statusses = new ProjectStatusOnServer[]
				{
					new ProjectStatusOnServer(projectStatus1, serverSpecifierA),
					new ProjectStatusOnServer(projectStatus2, serverSpecifierB)
				};
			SetupProjectLinkExpectation(projectA);
			SetupProjectLinkExpectation(projectB);

			// Execute
			ProjectGridRow[] rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.ServerName, true);

			// Verify
			Assert.AreEqual(2, rows.Length);
			Assert.AreEqual("a", rows[0].Name);
			Assert.AreEqual("b", rows[1].Name);

			// Setup
			SetupProjectLinkExpectation(projectA);
			SetupProjectLinkExpectation(projectB);

			// Execute
			rows = projectGrid.GenerateProjectGridRows(statusses, "myAction", ProjectGridSortColumn.ServerName, false);

			// Verify
			Assert.AreEqual(2, rows.Length);
			Assert.AreEqual("b", rows[0].Name);
			Assert.AreEqual("a", rows[1].Name);

			VerifyAll();
		}
	}
}
