using System;
using System.Web.UI.HtmlControls;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.SiteTemplatePlugin;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.SiteTemplatePlugin
{
	[TestFixture]
	public class DefaultBuildListerTest : Assertion
	{
		private DefaultBuildLister defaultBuildLister;
		private DynamicMock cruiseManagerWrapperMock;

		[SetUp]
		public void Setup()
		{
			cruiseManagerWrapperMock = new DynamicMock(typeof(ICruiseManagerWrapper));
			defaultBuildLister = new DefaultBuildLister((ICruiseManagerWrapper) cruiseManagerWrapperMock.MockInstance);
		}

		private void VerifyMocks()
		{
			cruiseManagerWrapperMock.Verify();
		}

		[Test]
		public void ReturnsEmptyListIfProjectOrServerNotSpecifiedCorrectly()
		{
			cruiseManagerWrapperMock.ExpectNoCall("GetBuildNames", typeof(string), typeof(string));

			AssertEquals(0, defaultBuildLister.GetBuildLinks(null, null).Length);
			AssertEquals(0, defaultBuildLister.GetBuildLinks("", null).Length);
			AssertEquals(0, defaultBuildLister.GetBuildLinks(null, "").Length);
			AssertEquals(0, defaultBuildLister.GetBuildLinks("", "").Length);
			AssertEquals(0, defaultBuildLister.GetBuildLinks("myServer", "").Length);
			AssertEquals(0, defaultBuildLister.GetBuildLinks("", "myProject").Length);

			VerifyMocks();
		}

		[Test]
		public void GetsBuildNamesFromManagerWrapperAndRendersLinks()
		{
			string[] buildNames = {
									  "log20020830164057Lbuild.6.xml",
										 "log20020507042535.xml",
										 "log20020507023858.xml",
										 "log20020507010355.xml",
										 "log19750101120000.xml",
									     "log19741224120000.xml"
									 };

			cruiseManagerWrapperMock.ExpectAndReturn("GetBuildNames", buildNames, "myserver", "myproject");

			HtmlAnchor[] actualLinks = defaultBuildLister.GetBuildLinks("myserver", "myproject");
			AssertEquals(6, actualLinks.Length);

			// expected Date format: dd MMM yyyy HH:mm
			AssertEquals("BuildReport.aspx?server=myserver&amp;project=myproject&amp;build=log20020830164057Lbuild.6.xml", actualLinks[0].HRef);
			AssertEquals("BuildReport.aspx?server=myserver&amp;project=myproject&amp;build=log20020507042535.xml", actualLinks[1].HRef);
			AssertEquals("BuildReport.aspx?server=myserver&amp;project=myproject&amp;build=log20020507023858.xml", actualLinks[2].HRef);
			AssertEquals("BuildReport.aspx?server=myserver&amp;project=myproject&amp;build=log20020507010355.xml", actualLinks[3].HRef);
			AssertEquals("BuildReport.aspx?server=myserver&amp;project=myproject&amp;build=log19750101120000.xml", actualLinks[4].HRef);
			AssertEquals("BuildReport.aspx?server=myserver&amp;project=myproject&amp;build=log19741224120000.xml", actualLinks[5].HRef);

			AssertEquals("<nobr>30 Aug 2002 16:40 (6)</nobr>", actualLinks[0].InnerText);
			AssertEquals("<nobr>07 May 2002 04:25 (Failed)</nobr>", actualLinks[1].InnerHtml);
			AssertEquals("<nobr>07 May 2002 02:38 (Failed)</nobr>", actualLinks[2].InnerHtml);
			AssertEquals("<nobr>07 May 2002 01:03 (Failed)</nobr>", actualLinks[3].InnerHtml);
			AssertEquals("<nobr>01 Jan 1975 12:00 (Failed)</nobr>", actualLinks[4].InnerHtml);
			AssertEquals("<nobr>24 Dec 1974 12:00 (Failed)</nobr>", actualLinks[5].InnerHtml);
		}
	}
}
