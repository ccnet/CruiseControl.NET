using System;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	[TestFixture]
	public class ProjectExceptionHandlingTest
	{
		[Test]
		public void ShouldHandleIncrementingLabelAfterInitialBuildFailsWithException()
		{
			IMock mockSourceControl = new DynamicMock(typeof (ISourceControl));
			mockSourceControl.ExpectAndThrow("GetModifications", new Exception("doh!"), new IsAnything(), new IsAnything());
			mockSourceControl.ExpectAndReturn("GetModifications", new Modification[] {new Modification()}, new IsAnything(), new IsAnything());

			Project project = new Project();
			project.Name = "test";
			project.SourceControl = (ISourceControl) mockSourceControl.MockInstance;
			project.StateManager = new StateManagerStub();
			try { project.Integrate(new IntegrationRequest(BuildCondition.ForceBuild, "test"));}
			catch (Exception) { }

			project.Integrate(new IntegrationRequest(BuildCondition.ForceBuild, "test"));
			Assert.AreEqual(IntegrationStatus.Success, project.LatestBuildStatus);
			Assert.AreEqual("1", project.LastIntegrationResult.Label);
		}

		[Test]
		public void ShouldNotResetLabelIfGetModificationsThrowsException()
		{
			IMock mockSourceControl = new DynamicMock(typeof (ISourceControl));
			mockSourceControl.ExpectAndThrow("GetModifications", new Exception("doh!"), new IsAnything(), new IsAnything());
			mockSourceControl.ExpectAndReturn("GetModifications", new Modification[] {new Modification()}, new IsAnything(), new IsAnything());

			StateManagerStub stateManagerStub = new StateManagerStub();
			stateManagerStub.SaveState(IntegrationResultMother.CreateSuccessful("10"));
			
			Project project = new Project();
			project.Name = "test";
			project.SourceControl = (ISourceControl) mockSourceControl.MockInstance;
			project.StateManager = stateManagerStub;
			try { project.Integrate(new IntegrationRequest(BuildCondition.ForceBuild, "test"));}
			catch (Exception) { }

			project.Integrate(new IntegrationRequest(BuildCondition.ForceBuild, "test"));
			Assert.AreEqual(IntegrationStatus.Success, project.LatestBuildStatus);
			Assert.AreEqual("11", project.LastIntegrationResult.Label);			
		}
	}

	internal class StateManagerStub : IStateManager
	{
		private IIntegrationResult savedResult = IntegrationResult.CreateInitialIntegrationResult("test", @"c:\temp");

		public IIntegrationResult LoadState(string project)
		{
			return savedResult;
		}

		public void SaveState(IIntegrationResult result)
		{
			savedResult = result;
		}
	}
}