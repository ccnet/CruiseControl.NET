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
			HtmlView view = new HtmlView("Some HTML");

			Assert.AreEqual("Some HTML", view.HtmlFragment );
		}
	}
}
