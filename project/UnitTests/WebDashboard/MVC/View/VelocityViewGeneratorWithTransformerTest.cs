using System.Collections;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC.View
{
	[TestFixture]
	public class VelocityViewGeneratorWithTransformerTest
	{
		private DynamicMock velocityTransformerMock;
		private VelocityViewGeneratorWithTransformer viewGenerator;

		[SetUp]
		public void Setup()
		{
			velocityTransformerMock = new DynamicMock(typeof(IVelocityTransformer));
			viewGenerator = new VelocityViewGeneratorWithTransformer((IVelocityTransformer) velocityTransformerMock.MockInstance);
		}

		private void VerifyAll()
		{
			velocityTransformerMock.Verify();
		}

		[Test]
		public void ShouldReturnResultOfTransformerWrappedInAnHtmlView()
		{
			Hashtable context = new Hashtable();
			context["foo"] = "bar";

			velocityTransformerMock.ExpectAndReturn("Transform", "transformed", "myTemplate", new HashtableConstraint(context));

			// Execute
			HtmlFragmentResponse response = viewGenerator.GenerateView("myTemplate", context);

			// Verify
			Assert.AreEqual("transformed", ((HtmlFragmentResponse) response).ResponseFragment);
			VerifyAll();
		}
	}
}
