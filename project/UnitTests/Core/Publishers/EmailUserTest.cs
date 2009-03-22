using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Publishers;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
    [TestFixture]
    public class EmailUserTest
	{
        [Test, ExpectedException(typeof(NetReflectorException), "Missing Xml node (address) for required member (ThoughtWorks.CruiseControl.Core.Publishers.EmailUser.Address).\r\n" + 
            "Xml: <user name=\"username\" />")]
        public void ShouldFailToReadWithoutAddress()
        {
            NetReflector.Read(@"<user name=""username""/>");
        }

        [Test, ExpectedException(typeof(NetReflectorException), "Missing Xml node (name) for required member (ThoughtWorks.CruiseControl.Core.Publishers.EmailUser.Name).\r\n" +
            "Xml: <user address=\"UserName@example.com\" />")]
        public void ShouldFailToReadWithoutName()
        {
            NetReflector.Read(@"<user address=""UserName@example.com""/>");   
        }

        [Test]
        public void ShouldReadFromMinimalXml()
        {
            EmailUser user = (EmailUser) NetReflector.Read(@"<user name=""username"" address=""UserName@example.com""/>");
            Assert.AreEqual("username", user.Name);
            Assert.AreEqual("UserName@example.com", user.Address);
            Assert.AreEqual(null, user.Group);
        }

        [Test]
        public void ShouldReadFromMaximalSimpleXml()
        {
            EmailUser user = (EmailUser)NetReflector.Read(@"<user name=""username"" address=""UserName@example.com"" group=""group1""/>");
            Assert.AreEqual("username", user.Name);
            Assert.AreEqual("UserName@example.com", user.Address);
            Assert.AreEqual("group1", user.Group);
        }

        [Test]
        public void ShouldReadFromMaximalComplexXml()
        {
            EmailUser user = (EmailUser)NetReflector.Read(
@"<user>
    <name>username</name>
    <address>UserName@example.com</address>
    <group>group1</group>
</user>
");
            Assert.AreEqual("username", user.Name);
            Assert.AreEqual("UserName@example.com", user.Address);
            Assert.AreEqual("group1", user.Group);
        }

	}
}