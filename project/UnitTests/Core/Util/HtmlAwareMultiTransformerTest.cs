using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
	[TestFixture]
	public class HtmlAwareMultiTransformerTest
	{
		[Test]
		public void ShouldDelegateForEachFileAndSeparateWithLineBreaks()
		{
			var delegateMock = new Mock<ITransformer>();
			HtmlAwareMultiTransformer transformer = new HtmlAwareMultiTransformer((ITransformer) delegateMock.Object);

			string input = "myinput";

			delegateMock.Setup(t => t.Transform(input, "xslFile1", null)).Returns(@"<p>MyFirstOutput<p>").Verifiable();
			delegateMock.Setup(t => t.Transform(input, "xslFile2", null)).Returns(@"<p>MySecondOutput<p>").Verifiable();

			Assert.AreEqual(@"<p>MyFirstOutput<p><p>MySecondOutput<p>", transformer.Transform(input, new string[] { "xslFile1", "xslFile2" }, null));
			delegateMock.Verify();
		}
	}
}
