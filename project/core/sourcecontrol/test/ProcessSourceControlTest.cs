using NUnit.Framework;
using System;
using tw.ccnet.core.util;
using tw.ccnet.core.test;

namespace tw.ccnet.core.sourcecontrol.test
{
	[TestFixture]
	public class ProcessSourceControlTest : CustomAssertion
	{
		[Test]
		public void ShouldRun()
		{
			Cvs cvs = new Cvs();
			Assert(cvs.ShouldRun(new IntegrationResult()));
			Assert(cvs.ShouldRun(IntegrationResultMother.CreateSuccessful()));
			AssertFalse(cvs.ShouldRun(IntegrationResultMother.CreateFailed()));
			AssertFalse(cvs.ShouldRun(IntegrationResultMother.CreateExceptioned()));
		}
	}
}
