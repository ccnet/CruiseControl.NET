using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	[TestFixture]
	public class HtmlDetailsMessageBuilderTest : CustomAssertion
	{
	    [Test]
		public void ShouldCreateStyleElementsInTheMailMessage()
	    {
	    	HtmlDetailsMessageBuilder builder = new HtmlDetailsMessageBuilder();
	        string message = builder.BuildMessage(IntegrationResultMother.CreateSuccessful());
	        int styleBegin = message.IndexOf("<style>");
	        int styleEnd = message.IndexOf("</style>");

	        Assert.IsTrue(styleBegin != -1);
			Assert.IsTrue(styleEnd != -1);
			Assert.IsTrue(styleEnd - styleBegin > 8, "There must be some styles from the loaded file");
	    }
	}
}
