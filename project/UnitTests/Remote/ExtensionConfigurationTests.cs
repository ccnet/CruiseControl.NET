using System.Xml;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    [TestFixture]
    public class ExtensionConfigurationTests
    {
        #region Test methods
        #region Properties
        [Test]
        public void ItemsGetSetTest()
        {
            XmlDocument document = new XmlDocument();
            XmlElement element = document.CreateElement("test");
            ExtensionConfiguration config = new ExtensionConfiguration();
            config.Items = new XmlElement[] {
                element
            };
            Assert.AreEqual(1, config.Items.Length);
            Assert.AreEqual(element, config.Items[0]);
        }
        #endregion
        #endregion
    }
}
