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
			Assert(cvs.ShouldRun(new IntegrationResult(), project));
			Assert(cvs.ShouldRun(IntegrationResultMother.CreateSuccessful(), project));
			AssertFalse(cvs.ShouldRun(IntegrationResultMother.CreateFailed(), project));
			AssertFalse(cvs.ShouldRun(IntegrationResultMother.CreateExceptioned(), project));
		}
	}
}
