using System;
using NUnit.Framework;
using NMock;
using Exortech.NetReflector;
using tw.ccnet.remote;
using tw.ccnet.core.util;

namespace tw.ccnet.core.test
{
	[TestFixture]
	public class GenericProjectTest : CustomAssertion
	{
		[Test]
		public void LoadConfiguration()
		{
			string xml = @"
<generic name=""foo"">
	<tasks>
		<mock/>
		<mock/>
	</tasks>
</generic>";
			XmlPopulator populator = new XmlPopulator();
			object obj = populator.Populate(XmlUtil.CreateDocumentElement(xml));
			AssertNotNull(obj);
			AssertEquals(typeof(GenericProject), obj.GetType());
			GenericProject project = (GenericProject)obj;
			AssertEquals("foo", project.Name);
			AssertEquals(2, project.Tasks.Count);
			Assert(project.Tasks[0] is ITask);
			Assert(project.Tasks[1] is ITask);
			//TODO: need NetReflector to enforce type of collection elements
		}

		[Test]
		public void Run()
		{
			IMock taskMock1 = new DynamicMock(typeof(ITask));
			taskMock1.ExpectAndReturn("ShouldRun", true, new NMock.Constraints.NotNull());
			taskMock1.Expect("Run", new NMock.Constraints.NotNull());

			IMock taskMock2 = new DynamicMock(typeof(ITask));
			taskMock2.ExpectAndReturn("ShouldRun", true, new NMock.Constraints.NotNull());
			taskMock2.Expect("Run", new NMock.Constraints.NotNull());

			IMock taskMock3 = new DynamicMock(typeof(ITask));
			taskMock3.ExpectAndReturn("ShouldRun", false, new NMock.Constraints.NotNull());
			taskMock3.Expect("Run", new NMock.Constraints.NotNull());

			GenericProject project = new GenericProject();
			project.Tasks.Add(taskMock1.MockInstance);
			project.Tasks.Add(taskMock2.MockInstance);
			project.RunIntegration(BuildCondition.IfModificationExists);

			AssertNotNull(project.CurrentIntegration);
			AssertNotNull(project.CurrentIntegration.StartTime);
			AssertNotNull(project.CurrentIntegration.EndTime);
			AssertNotNull(project.CurrentIntegration.EndTime > project.CurrentIntegration.StartTime);
			AssertEquals(project.CurrentIntegration.Status, project.GetLatestBuildStatus());
			taskMock1.Verify();
			taskMock2.Verify();
		}

		[Test]
		public void RunForceBuild()
		{
			IMock taskMock1 = new DynamicMock(typeof(ITask));
			taskMock1.Expect("Run", new NMock.Constraints.NotNull());

			IMock taskMock2 = new DynamicMock(typeof(ITask));
			taskMock2.Expect("Run", new NMock.Constraints.NotNull());

			GenericProject project = new GenericProject();
			project.Tasks.Add(taskMock1.MockInstance);
			project.Tasks.Add(taskMock2.MockInstance);

			project.RunIntegration(BuildCondition.ForceBuild);

			taskMock1.Verify();
			taskMock2.Verify();
		}

		[Test]
		public void RunWhereFirstTaskThrowsException()
		{
			Exception ex = new CruiseControlException("foo");
			IMock taskMock1 = new DynamicMock(typeof(ITask));
			taskMock1.ExpectAndReturn("ShouldRun", true, new NMock.Constraints.NotNull());
			taskMock1.ExpectAndThrow("Run", ex, new NMock.Constraints.NotNull());

			IMock taskMock2 = new DynamicMock(typeof(ITask));
			taskMock2.ExpectAndReturn("ShouldRun", true, new NMock.Constraints.NotNull());
			taskMock2.Expect("Run", new NMock.Constraints.NotNull());

			GenericProject project = new GenericProject();
			project.Tasks.Add(taskMock1.MockInstance);
			project.Tasks.Add(taskMock2.MockInstance);

			IntegrationResult result = project.RunIntegration(BuildCondition.IfModificationExists);

			taskMock1.Verify();
			taskMock2.Verify();
			AssertEquals(IntegrationStatus.Exception, result.Status);
			AssertEquals(ex, result.ExceptionResult);
		}

		// need some way to determine if task should run
		// maybe task is always run and task decides what it should do about it?
		// or have ShouldRun method to determine if task should run
		// or use condition to determine if task should run -- can be internal or external to task -- external is best
		// configurable TaskCondition property with default implementation
		// what about exceptions: handled where and how?  should publisher be a task?  or just another type of logging?
	}
}
