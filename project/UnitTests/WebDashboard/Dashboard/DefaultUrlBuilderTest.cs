using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class DefaultUrlBuilderTest
	{
		private DefaultUrlBuilder urlBuilder;

		[SetUp]
		public void Setup()
		{
			urlBuilder = new DefaultUrlBuilder();
		}

		[Test]
		public void ShouldBuildUrlAddingCorrectlyFormattedAction()
		{
			Assert.AreEqual("default.aspx?_action_myAction=true", urlBuilder.BuildUrl("myAction"));
		}

		[Test]
		public void ShouldBuildUrlWithActionAndQueryString()
		{
			Assert.AreEqual("default.aspx?_action_myAction=true&myparam=myvalue", urlBuilder.BuildUrl("myAction", "myparam=myvalue"));
		}

		[Test]
		public void ShouldBuildUrlWithAlternativeBaseRelativeUrlIfGiven()
		{
			Assert.AreEqual("myBaseUrl?_action_myAction=true&myparam=myvalue", urlBuilder.BuildUrl("myAction", "myparam=myvalue", "myBaseUrl"));
		}

		[Test]
		public void ShouldBuildFormNameBasedOnActionName()
		{
			Assert.AreEqual("_action_myAction", urlBuilder.BuildFormName("myAction"));
		}
	}
}