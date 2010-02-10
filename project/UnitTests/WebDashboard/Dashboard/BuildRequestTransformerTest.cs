using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
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

			Build build = new Build(buildSpecifier, "logContents");

			buildRetrieverMock.ExpectAndReturn("GetBuild", build, buildSpecifier, null);

			string[] fileNames = new string[] { "file1", "file2" };

			delegateTransformerMock.ExpectAndReturn("Transform", "transformed", "logContents", fileNames, null);

			Assert.AreEqual("transformed", requestTransformer.Transform(buildSpecifier, fileNames, null, null, null));

			buildRetrieverMock.Verify();
			delegateTransformerMock.Verify();
		}
	}
}
