using System;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Web.Test
{
	[TestFixture]
	public class PageTransformerTest : CustomAssertion
	{
		private IMock _transformer;
		[SetUp]
		public void Init()
		{
			_transformer = new DynamicMock(typeof(ITransformer));					    
		}

		[Test]
		public void PerformsTransformationsAndRuturnsTransformedData()
		{
		    string val = "<foo><bar/></foo>";
		    _transformer.ExpectAndReturn("Transform",val, null);
		    PageTransformer pageTransformer = new PageTransformer((ITransformer) _transformer.MockInstance);
			string data = pageTransformer.LoadPageContent();
			AssertEquals(val,data);
			_transformer.Verify();
		}

		[Test]
		public void IfTransformationFailsWithExceptionReturnsTheExceptionMessage()
		{

		    string val = "Transformation Failed";
		    _transformer.ExpectAndThrow("Transform",new CruiseControlException(val),null);
		    PageTransformer pageTransformer = new PageTransformer((ITransformer) _transformer.MockInstance);
			string data = pageTransformer.LoadPageContent();
			AssertContains(val,data);
			_transformer.Verify();
		}

			
	}
}
