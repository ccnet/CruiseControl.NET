using NMock;
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
			DynamicMock delegateMock = new DynamicMock(typeof(ITransformer));
			HtmlAwareMultiTransformer transformer = new HtmlAwareMultiTransformer((ITransformer) delegateMock.MockInstance);

			string input = "myinput";

			delegateMock.ExpectAndReturn("Transform", @"<p>MyFirstOutput<p>",  input, "xslFile1");
			delegateMock.ExpectAndReturn("Transform", @"<p>MySecondOutput<p>",  input, "xslFile2");

			Assert.AreEqual(@"<p>MyFirstOutput<p><br/><p>MySecondOutput<p><br/>", transformer.Transform(input, new string[] { "xslFile1", "xslFile2" }));
			delegateMock.Verify();
		}
	}
}
