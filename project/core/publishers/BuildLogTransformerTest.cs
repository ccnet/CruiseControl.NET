using System;
using NUnit.Framework;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	[TestFixture]
	public class BuildLogTransformerTest: Assertion
	{

		[Test]
			public void TransformingDocumentWithEmptyXSLFilesReturnsEmptyString()
		{
			BuildLogTransformer xformer = new BuildLogTransformer();
			string result = xformer.TransformResults(null, new XmlDocument());
			AssertEquals("",result);
		}
	}
}
