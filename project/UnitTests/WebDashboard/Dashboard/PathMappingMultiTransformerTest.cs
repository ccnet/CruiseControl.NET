using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class PathMappingMultiTransformerTest
	{
		[Test]
		public void ShouldCallDelegateTransformerWithCorrectFileNames()
		{
			DynamicMock delegateMock = new DynamicMock(typeof(IMultiTransformer));
			DynamicMock pathMapperMock = new DynamicMock(typeof(IPathMapper));

			PathMappingMultiTransformer transformer = new PathMappingMultiTransformer((IPathMapper) pathMapperMock.MockInstance, (IMultiTransformer) delegateMock.MockInstance);

			pathMapperMock.ExpectAndReturn("GetLocalPathFromURLPath", "pathed1", "xslFile1");
			pathMapperMock.ExpectAndReturn("GetLocalPathFromURLPath", "pathed2", "xslFile2");

			delegateMock.ExpectAndReturn("Transform", "output", "myInput", new string[] { "pathed1", "pathed2" });

			Assert.AreEqual("output", transformer.Transform("myInput", new string[] { "xslFile1", "xslFile2"} ));
			pathMapperMock.Verify();
			delegateMock.Verify();
		}
	}
}
