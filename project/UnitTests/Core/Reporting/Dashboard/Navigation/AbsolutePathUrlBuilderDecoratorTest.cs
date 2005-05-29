using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Reporting.Dashboard.Navigation
{
	[TestFixture]
	public class AbsolutePathUrlBuilderDecoratorTest
	{
		[Test]
		public void ShouldDecorateUrlsToCreateAbsoluteURLs()
		{
			/// Setup
			DynamicMock decoratedBuilderMock = new DynamicMock(typeof(IUrlBuilder));
			string baseUrl = "https://myserver:8080/myvdir";

			AbsolutePathUrlBuilderDecorator decorator = new AbsolutePathUrlBuilderDecorator((IUrlBuilder) decoratedBuilderMock.MockInstance, baseUrl);
			string actionName = "myAction";
			decoratedBuilderMock.ExpectAndReturn("BuildUrl", "myRelativeUrl", actionName);
			decoratedBuilderMock.ExpectAndReturn("BuildUrl", "myRelativeUrl2", actionName, "query");
			decoratedBuilderMock.ExpectAndReturn("BuildUrl", "myRelativeUrl3", actionName, "query", "baseUrl");

			/// Execute & Verify
			Assert.AreEqual(baseUrl + "/" + "myRelativeUrl", decorator.BuildUrl(actionName));
			Assert.AreEqual(baseUrl + "/" + "myRelativeUrl2", decorator.BuildUrl(actionName, "query"));
			Assert.AreEqual(baseUrl + "/" + "myRelativeUrl3", decorator.BuildUrl(actionName, "query", "baseUrl"));

			decoratedBuilderMock.Verify();
		}

		[Test]
		public void ShouldHandleBaseURLsWithTrailingSlashes()
		{
			/// Setup
			DynamicMock decoratedBuilderMock = new DynamicMock(typeof(IUrlBuilder));
			string baseUrl = "https://myserver:8080/myvdir/";

			AbsolutePathUrlBuilderDecorator decorator = new AbsolutePathUrlBuilderDecorator((IUrlBuilder) decoratedBuilderMock.MockInstance, baseUrl);
			string actionName = "myAction";
			decoratedBuilderMock.ExpectAndReturn("BuildUrl", "myRelativeUrl", actionName);

			/// Execute & Verify
			Assert.AreEqual(baseUrl + "myRelativeUrl", decorator.BuildUrl(actionName));

			decoratedBuilderMock.Verify();
		}

		[Test]
		public void ShouldNotDecorateFormName()
		{
			/// Setup
			DynamicMock decoratedBuilderMock = new DynamicMock(typeof(IUrlBuilder));
			string baseUrl = "https://myserver:8080/myvdir";

			AbsolutePathUrlBuilderDecorator decorator = new AbsolutePathUrlBuilderDecorator((IUrlBuilder) decoratedBuilderMock.MockInstance, baseUrl);
			decoratedBuilderMock.ExpectAndReturn("BuildFormName", "formName", "myAction");

			/// Execute & Verify
			Assert.AreEqual("formName", decorator.BuildFormName("myAction"));

			decoratedBuilderMock.Verify();
		}
	}
}
