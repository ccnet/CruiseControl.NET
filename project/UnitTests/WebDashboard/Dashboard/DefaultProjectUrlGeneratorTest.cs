using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class DefaultProjectUrlGeneratorTest : Assertion
	{
		[Test]
		public void CombinesBaseUrlAndServerAndProjectToCreateCorrectUrl()
		{
			string returnedUrl = new DefaultProjectUrlGenerator().GenerateUrl("foo.aspx", "myserver", "myproject");
			AssertEquals("foo.aspx?server=myserver&amp;project=myproject", returnedUrl);
		}
	}
}
