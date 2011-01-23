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
			decoratedBuilderMock.ExpectAndReturn("BuildUrl", "myPath/myRelativeUrl3", actionName, "query", "myPath/");

			/// Execute & Verify
			Assert.AreEqual(baseUrl + "/myRelativeUrl", decorator.BuildUrl(actionName));
			Assert.AreEqual(baseUrl + "/myRelativeUrl2", decorator.BuildUrl(actionName, "query"));
			Assert.AreEqual(baseUrl + "/myPath/myRelativeUrl3", decorator.BuildUrl(actionName, "query", "myPath/"));

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
		public void ShouldDelegateExtensionToSubBuilder()
		{
			// Setup
			DynamicMock decoratedBuilderMock = new DynamicMock(typeof(IUrlBuilder));
			decoratedBuilderMock.ExpectAndReturn("Extension", "foo");

			// Execute
			AbsolutePathUrlBuilderDecorator decorator = new AbsolutePathUrlBuilderDecorator((IUrlBuilder) decoratedBuilderMock.MockInstance, null);
			decorator.Extension = "foo";

			// Verify
			decoratedBuilderMock.Verify();
		}
	}
}
