using System.Collections;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC.View
{
	[TestFixture]
	public class VelocityViewGeneratorWithTransformerTest
	{
		private Mock<IVelocityTransformer> velocityTransformerMock;
		private VelocityViewGeneratorWithTransformer viewGenerator;

		[SetUp]
		public void Setup()
		{
			velocityTransformerMock = new Mock<IVelocityTransformer>();
			viewGenerator = new VelocityViewGeneratorWithTransformer((IVelocityTransformer) velocityTransformerMock.Object);
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

			velocityTransformerMock.Setup(transformer => transformer.Transform("myTemplate", It.Is<Hashtable>(t => t.Count == 1 && (string)t["foo"] == "bar"))).Returns("transformed").Verifiable();

			// Execute
			HtmlFragmentResponse response = viewGenerator.GenerateView("myTemplate", context);

			// Verify
			Assert.AreEqual("transformed", ((HtmlFragmentResponse) response).ResponseFragment);
			VerifyAll();
		}
	}
}
