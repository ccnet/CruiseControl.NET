using System.Xml;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
    [TestFixture]
    public class CruiseXmlWriterTest
    {
        [Test]
        public void WriteSingleProject()
        {
            CruiseXmlWriter writer = new CruiseXmlWriter();
            ProjectStatus status = ProjectStatusFixture.New("test");
            string xml = writer.Write(status);
            XmlDocument document = XPathAssert.LoadAsDocument(xml);
            XPathAssert.Matches(document, "/CruiseControl/Projects/Project/@name", "test");
        }
    }
}