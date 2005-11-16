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
			DynamicMock pathProviderStub = new DynamicMock(typeof(IPhysicalApplicationPathProvider));

			PathMappingMultiTransformer transformer = new PathMappingMultiTransformer((IPhysicalApplicationPathProvider) pathProviderStub.MockInstance, (IMultiTransformer) delegateMock.MockInstance);

			pathProviderStub.SetupResult("PhysicalApplicationPath", @"c:\myAppPath");

			delegateMock.ExpectAndReturn("Transform", "output", "myInput", new string[] { @"c:\myAppPath\xslFile1", @"c:\myAppPath\xslFile2" }, null);

			Assert.AreEqual("output", transformer.Transform("myInput", new string[] { "xslFile1", "xslFile2"}, null));
			pathProviderStub.Verify();
			delegateMock.Verify();
		}
	}
}
