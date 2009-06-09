using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Mercurial;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol.Mercurial
{
    [TestFixture]
    public class MercurialModificationTest
    {
        [Test]
        public void CanReadXml()
        {
            MercurialModification mod = (MercurialModification) NetReflector.Read(Format("8b67211d9140", "ndbecker2", "Wed, 16 Apr 2008 15:26:37 -0700", "mergetools.hgrc patch", 6559, "asdf@asdf.com", "file1 file2"));
            Assert.AreEqual("8b67211d9140", mod.Version);
            Assert.AreEqual("ndbecker2", mod.UserName);
            Assert.AreEqual("Wed, 16 Apr 2008 15:26:37 -0700", mod.ModificationTime);
            Assert.AreEqual("mergetools.hgrc patch", mod.Comment);
            Assert.AreEqual(6559, mod.ChangeNumber);
        }

        [Test]
        public void CanReadMultipleEntries()
        {
            MercurialModificationCollection collection;
            collection = new MercurialModificationCollection();
            collection.modifications = new MercurialModification[2] {new MercurialModification(), new MercurialModification()};
            collection = (MercurialModificationCollection) NetReflector.Read("<modifications><array>" + Format("8b67211d9140", "ndbecker2", "Wed, 16 Apr 2008 15:26:37 -0700", "mergetools.hgrc patch", 6559, "asdf@asdf.com", "file1 file2") + Format("8b67211d9140", "ndbecker2", "Wed, 16 Apr 2008 15:26:37 -0700", "mergetools.hgrc patch", 6559, "asdf@asdf.com", "file1 file2") + "</array></modifications>");
            Assert.AreEqual(2, collection.modifications.Length);
        }

        private string Format(string node, string author, string date, string desc, int rev, string email, string files) { return string.Format(@"
<modification>
  <node>{0}</node>
  <author>{1}</author>
  <date>{2}</date>
  <desc>{3}</desc>
  <rev>{4}</rev>
  <email>{5}</email>
  <files>{6}</files>
</modification>", node, author, date, desc, rev, email, files); }
    }
}
