using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core.Test;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
{
	[TestFixture]
	public class ProcessSourceControlTest : CustomAssertion
	{
		[Test]
		public void ShouldRun()
		{
			Cvs cvs = new Cvs();
			IProject project = (IProject) new DynamicMock(typeof(IProject)).MockInstance;
			Assert.IsTrue(cvs.ShouldRun(new IntegrationResult()));
			Assert.IsTrue(cvs.ShouldRun(IntegrationResultMother.CreateSuccessful()));
			AssertFalse(cvs.ShouldRun(IntegrationResultMother.CreateFailed()));
			AssertFalse(cvs.ShouldRun(IntegrationResultMother.CreateExceptioned()));
		}
	}
}
