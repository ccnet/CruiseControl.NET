using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
	[TestFixture]
	public class XslFileTransformerTest : CustomAssertion
	{
		private static readonly string TestFolder = "logTransformerTest";
		private XslFileTransformer xslFileTransformer;
		private DynamicMock subTransformerMock;

		[SetUp]
		public void Setup()
		{
			subTransformerMock = new DynamicMock(typeof(ITransformer));
			xslFileTransformer = new XslFileTransformer((ITransformer) subTransformerMock.MockInstance);
			TempFileUtil.CreateTempDir(TestFolder);
		}

		[TearDown]
		public void Teardown()
		{
			TempFileUtil.DeleteTempDir(TestFolder);
		}

		[Test]
		public void ShouldDelegateWithInputFileContents()
		{
			subTransformerMock.ExpectAndReturn("Transform", "<someOutput />", "myXmlFileContents", "myXslFile");
			string logfile = TempFileUtil.CreateTempXmlFile(TestFolder, "samplelog.xml", "myXmlFileContents");

			Assert.AreEqual("<someOutput />", xslFileTransformer.Transform(logfile, "myXslFile"));
			subTransformerMock.Verify();
		}

		[Test]
		public void TestTransform_LogfileMissing()
		{
			subTransformerMock.ExpectNoCall("Transform", typeof(string), typeof(string));
			string logfile = "nosuchlogfile";
			string xslfile = "myXsl";
			try
			{
				xslFileTransformer.Transform(logfile, xslfile);
				Assert.Fail("Should throw CC Exception since file missing");
			}
			catch (CruiseControlException) { }
			subTransformerMock.Verify();
		}
	}
}
