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
			StringView view = new StringView("Some HTML");

			Assert.AreEqual("Some HTML", view.ResponseFragment );
		}
	}
}
