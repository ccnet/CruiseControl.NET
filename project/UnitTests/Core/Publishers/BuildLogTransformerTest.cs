using System.IO;
using System.Xml.XPath;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Publishers;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	[TestFixture]
	public class BuildLogTransformerTest
	{
		[Test]
		public void TransformingDocumentWithEmptyXSLFilesReturnsEmptyString()
		{
			BuildLogTransformer xformer = new BuildLogTransformer();
			string result = xformer.TransformResults(null, new XPathDocument(new StringReader("<foo />")));
			Assert.AreEqual("", result);
		}
	}
}