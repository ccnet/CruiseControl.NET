using System.Web.UI;
using System.Web.UI.HtmlControls;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

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
			DynamicMock cruiseRequestMock = new DynamicMock(typeof(ICruiseRequest));

			BuildRequestTransformer requestTransformer = new BuildRequestTransformer((IBuildRetriever) buildRetrieverMock.MockInstance, (IMultiTransformer) delegateTransformerMock.MockInstance);

			cruiseRequestMock.ExpectAndReturn("ServerName", "myServer");
			cruiseRequestMock.ExpectAndReturn("ProjectName", "myProject");
			cruiseRequestMock.ExpectAndReturn("BuildName", "myBuild");

			Build build = new Build(null, "logContents", null, null, null);

			buildRetrieverMock.ExpectAndReturn("GetBuild", build, "myServer", "myProject", "myBuild");

			string[] fileNames = new string[] { "file1", "file2" };

			delegateTransformerMock.ExpectAndReturn("Transform", "transformed", "logContents", fileNames);

			Control control = requestTransformer.Transform((ICruiseRequest) cruiseRequestMock.MockInstance, fileNames);
			Assert.AreEqual("transformed", ((HtmlGenericControl) control).InnerHtml);

			buildRetrieverMock.Verify();
			delegateTransformerMock.Verify();
			cruiseRequestMock.Verify();
		}
	}
}
