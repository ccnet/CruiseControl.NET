using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC
{
	[TestFixture]
	public class HtmlViewTest
	{
		[Test]
		public void ShouldGiveHtmlFragmentIfStringConstructorUsed()
		{
			HtmlFragmentResponse responseFragment = new HtmlFragmentResponse("Some HTML");

			Assert.AreEqual("Some HTML", responseFragment.ResponseFragment );
		}
	}
}
