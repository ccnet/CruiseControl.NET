using System;
using Exortech.NetReflector;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Triggers;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Triggers
{
	[TestFixture]
	public class ProjectTriggerTest
	{
		private IMock mockFactory;
		private IMock mockCruiseManager;
		private IMock mockInnerTrigger;
		private ProjectTrigger trigger;

		[SetUp]
		protected void SetUp()
		{
			mockCruiseManager = new DynamicMock(typeof (ICruiseManager));
			mockFactory = new DynamicMock(typeof (ICruiseManagerFactory));
			mockInnerTrigger = new DynamicMock(typeof (ITrigger));
			trigger = new ProjectTrigger((ICruiseManagerFactory) mockFactory.MockInstance);
			trigger.Project = "project";
			trigger.InnerTrigger = (ITrigger) mockInnerTrigger.MockInstance;
		}

		protected void Verify()
		{
			mockFactory.Verify();
			mockCruiseManager.Verify();
			mockInnerTrigger.Verify();
		}

		[Test]
		public void TriggerWhenDependentProjectBuildsSuccessfully()
		{
			mockInnerTrigger.ExpectAndReturn("ShouldRunIntegration", BuildCondition.IfModificationExists);
			mockInnerTrigger.Expect("IntegrationCompleted");
			mockFactory.ExpectAndReturn("GetCruiseManager", mockCruiseManager.MockInstance, ProjectTrigger.DefaultServerUri);
			mockCruiseManager.ExpectAndReturn("GetProjectStatus", new ProjectStatus[]
				{
					NewProjectStatus("wrong", IntegrationStatus.Failure), NewProjectStatus("project", IntegrationStatus.Success)
				});
			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.ShouldRunIntegration());
			Verify();
		}

		[Test]
		public void DoNotTriggerWhenInnerTriggerReturnsNoBuild()
		{
			mockInnerTrigger.ExpectAndReturn("ShouldRunIntegration", BuildCondition.NoBuild);
			mockFactory.ExpectNoCall("GetCruiseManager", typeof(string));
			mockCruiseManager.ExpectNoCall("GetProjectStatus");
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());
			Verify();
		}

		[Test]
		public void DoNotTriggerWhenDependentProjectBuildFails()
		{
			mockInnerTrigger.ExpectAndReturn("ShouldRunIntegration", BuildCondition.IfModificationExists);
			mockInnerTrigger.Expect("IntegrationCompleted");
			mockFactory.ExpectAndReturn("GetCruiseManager", mockCruiseManager.MockInstance, ProjectTrigger.DefaultServerUri);
			mockCruiseManager.ExpectAndReturn("GetProjectStatus", new ProjectStatus[] {NewProjectStatus("project", IntegrationStatus.Failure)});
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());
			Verify();
		}

		[Test]
		public void DoNotTriggerIfProjectHasNotBuiltSinceLastPoll()
		{
			ProjectStatus status = NewProjectStatus("project", IntegrationStatus.Success);
			mockInnerTrigger.ExpectAndReturn("ShouldRunIntegration", BuildCondition.IfModificationExists);
			mockInnerTrigger.Expect("IntegrationCompleted");
			mockFactory.ExpectAndReturn("GetCruiseManager", mockCruiseManager.MockInstance, ProjectTrigger.DefaultServerUri);
			mockCruiseManager.ExpectAndReturn("GetProjectStatus", new ProjectStatus[] {status});
			mockFactory.ExpectAndReturn("GetCruiseManager", mockCruiseManager.MockInstance, ProjectTrigger.DefaultServerUri);
			mockInnerTrigger.ExpectAndReturn("ShouldRunIntegration", BuildCondition.IfModificationExists);
			mockInnerTrigger.Expect("IntegrationCompleted");
			mockCruiseManager.ExpectAndReturn("GetProjectStatus", new ProjectStatus[] {status}); // same build as last time
			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.ShouldRunIntegration());
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());
			Verify();
		}

		[Test]
		public void IntegrationCompletedShouldDelegateToInnerTrigger()
		{
			mockInnerTrigger.Expect("IntegrationCompleted");
			trigger.IntegrationCompleted();
			Verify();
		}

		[Test]
		public void NextBuildShouldReturnInnerTriggerNextBuildIfUnknown()
		{

			Assert.AreEqual(DateTime.MinValue, trigger.NextBuild);
		}

		private ProjectStatus NewProjectStatus(string name, IntegrationStatus integrationStatus)
		{
			ProjectStatus status = new ProjectStatus();
			status.Name = name;
			status.BuildStatus = integrationStatus;
			status.LastBuildDate = DateTime.Now;
			return status;
		}

		[Test]
		public void PopulateFromConfiguration()
		{
			string xml = @"<projectTrigger>
	<serverUri>http://fooserver:12342/CruiseManager.rem</serverUri>
	<project>Foo</project>
	<triggerStatus>Failure</triggerStatus>
	<innerTrigger type=""intervalTrigger"">
		<buildCondition>ForceBuild</buildCondition>
		<seconds>10</seconds>
	</innerTrigger>
</projectTrigger>";
			trigger = (ProjectTrigger) NetReflector.Read(xml);
			Assert.AreEqual("http://fooserver:12342/CruiseManager.rem", trigger.ServerUri);
			Assert.AreEqual("Foo", trigger.Project);
			Assert.IsNotNull(trigger.InnerTrigger);
			Assert.AreEqual(IntegrationStatus.Failure, trigger.TriggerStatus);
		}

		[Test]
		public void PopulateFromMinimalConfiguration()
		{
			string xml = @"<projectTrigger><project>Foo</project></projectTrigger>";
			trigger = (ProjectTrigger) NetReflector.Read(xml);
			Assert.AreEqual(ProjectTrigger.DefaultServerUri, trigger.ServerUri);
			Assert.AreEqual("Foo", trigger.Project);
			Assert.IsNotNull(trigger.InnerTrigger);
			Assert.AreEqual(IntegrationStatus.Success, trigger.TriggerStatus);
		}

		[Test]
		public void HandleExceptionInProjectLocator()
		{}
	}
}