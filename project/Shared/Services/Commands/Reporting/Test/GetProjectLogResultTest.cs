
using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.Shared.Services.Commands.Reporting.Test
{
	[TestFixture]
	public class GetProjectLogResultTest
	{
		[Test]
		public void HasALogAttached()
		{
			GetProjectLogResult result = new GetProjectLogResult("some stuff in a log");

			Assert.AreEqual("some stuff in a log", result.Log);
		}
	}
}
