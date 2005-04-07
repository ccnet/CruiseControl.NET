using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.UnitTests;

namespace ThoughtWorks.CruiseControl.Web.Test
{
	[TestFixture]
	public class PageTransformerTest : CustomAssertion
	{
		private IMock _transformer;
		[SetUp]
		public void Init()
		{
			_transformer = new DynamicMock(typeof(IFileTransformer));					    
		}

		[Test]
		public void PerformsTransformationsAndRuturnsTransformedData()
		{
		    string val = "<foo><bar/></foo>";
		    _transformer.ExpectAndReturn("Transform",val, "xmlFile", "xslFile");
		    PageTransformer pageTransformer = new PageTransformer((IFileTransformer) _transformer.MockInstance, "xmlFile", "xslFile");
			string data = pageTransformer.LoadPageContent();
			Assert.AreEqual(val,data);
			_transformer.Verify();
		}

		[Test]
		public void IfTransformationFailsWithExceptionReturnsTheExceptionMessage()
		{
		    string val = "Transformation Failed";
		    _transformer.ExpectAndThrow("Transform",new CruiseControlException(val), "xmlFile", "xslFile");
		    PageTransformer pageTransformer = new PageTransformer((IFileTransformer) _transformer.MockInstance, "xmlFile", "xslFile");
			string data = pageTransformer.LoadPageContent();
			AssertContains(val,data);
			_transformer.Verify();
		}
	}
}
