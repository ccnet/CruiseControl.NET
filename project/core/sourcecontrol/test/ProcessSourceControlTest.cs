using NUnit.Framework;
using System;
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
			Assert(cvs.ShouldRun(new IntegrationResult()));
			Assert(cvs.ShouldRun(IntegrationResultMother.CreateSuccessful()));
			AssertFalse(cvs.ShouldRun(IntegrationResultMother.CreateFailed()));
			AssertFalse(cvs.ShouldRun(IntegrationResultMother.CreateExceptioned()));
		}
	}
}
