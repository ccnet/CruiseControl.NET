using System.IO;
using System.Xml.XPath;
using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Test
{
	[TestFixture]
	public class BuildLogTransformerTest
	{
		[Test]
		public void TransformingDocumentWithEmptyXSLFilesReturnsEmptyString()
		{
			BuildLogTransformer xformer = new BuildLogTransformer();
			string result = xformer.TransformResults(null, new XPathDocument(new StringReader("<foo />")));
			Assert.AreEqual("",result);
		}
	}
}
