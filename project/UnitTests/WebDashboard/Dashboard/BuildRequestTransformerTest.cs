using System.Web.UI;
using System.Web.UI.HtmlControls;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class BuildRequestTransformerTest
	{
		[Test]
		public void ShouldGetBuildLogAndReturnResultOfDelegateTransformer()
		{
			DynamicMock buildRetrieverMock = new DynamicMock(typeof(IBuildRetriever));
			DynamicMock delegateTransformerMock = new DynamicMock(typeof(IMultiTransformer));

			BuildRequestTransformer requestTransformer = new BuildRequestTransformer((IBuildRetriever) buildRetrieverMock.MockInstance, (IMultiTransformer) delegateTransformerMock.MockInstance);

			DefaultBuildSpecifier buildSpecifier = new DefaultBuildSpecifier(new DefaultProjectSpecifier(new DefaultServerSpecifier("myServer"), "myProject"), "myBuild");

			Build build = new Build(buildSpecifier, "logContents", null);

			buildRetrieverMock.ExpectAndReturn("GetBuild", build, buildSpecifier);

			string[] fileNames = new string[] { "file1", "file2" };

			delegateTransformerMock.ExpectAndReturn("Transform", "transformed", "logContents", fileNames);

			Control control = requestTransformer.Transform(buildSpecifier, fileNames);
			Assert.AreEqual("transformed", ((HtmlGenericControl) control).InnerHtml);

			buildRetrieverMock.Verify();
			delegateTransformerMock.Verify();
		}
	}
}
