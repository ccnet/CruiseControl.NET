using System;

using NUnit.Framework;
using NMock;

using ThoughtWorks.CruiseControl.Shared.Services.Commands.Reporting;

namespace ThoughtWorks.CruiseControl.UnitTests.Shared.Services.Commands.Reporting
{
	[TestFixture]
	public class GetProjectLogResultTest : Assertion
	{
		[Test]
		public void HasALogAttached()
		{
			GetProjectLogResult result = new GetProjectLogResult("some stuff in a log");

			AssertEquals("some stuff in a log", result.Log);
		}
	}
}
