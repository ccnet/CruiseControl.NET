using Moq;
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
			var delegateMock = new Mock<IMultiTransformer>();
			var pathProviderStub = new Mock<IPhysicalApplicationPathProvider>();

			PathMappingMultiTransformer transformer = new PathMappingMultiTransformer((IPhysicalApplicationPathProvider) pathProviderStub.Object, (IMultiTransformer) delegateMock.Object);

            pathProviderStub.Setup(provider => provider.GetFullPathFor("xslFile1")).Returns(@"c:\myAppPath\xslFile1").Verifiable();
            pathProviderStub.Setup(provider => provider.GetFullPathFor("xslFile2")).Returns(@"c:\myAppPath\xslFile2").Verifiable();

			delegateMock.Setup(t => t.Transform("myInput", new string[] { @"c:\myAppPath\xslFile1", @"c:\myAppPath\xslFile2" }, null)).Returns("output").Verifiable();

			Assert.AreEqual("output", transformer.Transform("myInput", new string[] { "xslFile1", "xslFile2"}, null));
			pathProviderStub.Verify();
			delegateMock.Verify();
		}
	}
}
