using System;

using NUnit.Framework;
using NMock;

namespace ThoughtWorks.CruiseControl.Shared.Services.Commands.Reporting.Test
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
