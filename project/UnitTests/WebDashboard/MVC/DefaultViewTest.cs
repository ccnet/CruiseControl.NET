using System.Web.UI;
using System.Web.UI.HtmlControls;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC
{
	[TestFixture]
	public class DefaultViewTest
	{
		[Test]
		public void ShouldGiveADivControlContainingOriginalHtmlContentIfStringConstructorUsed()
		{
			DefaultView view = new DefaultView("Some HTML");

			Control returnedControl = view.Control;

			Assert.AreEqual(typeof(HtmlGenericControl), returnedControl.GetType());
			Assert.AreEqual("Some HTML", ((HtmlGenericControl) returnedControl).InnerHtml );
		}

		[Test]
		public void ShouldGiveOriginalControlIfControlConstructorUsed()
		{
			Control embeddedControl = new HtmlGenericControl("<p>");
			DefaultView view = new DefaultView(embeddedControl);
			Assert.AreEqual(embeddedControl, view.Control);
		}
	}
}
