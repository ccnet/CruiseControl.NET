using System;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Mercurial;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol.Mercurial
{
    [TestFixture]
    public class MercurialHistoryParserTest
    {
        private readonly string emptyLogXml = "";
        private readonly string fullLogXml = "<modification><node>48365ef6a3ea</node><author>bbarry</author><date>Thu, 24 Apr 2008 22:14:59 -0600</date><desc>asdf</desc><rev>3</rev><email>&#98;&#98;&#97;&#114;&#114;&#121;&#64;&#115;&#116;&#101;&#108;&#108;&#97;&#114;&#102;&#105;&#110;&#97;&#110;&#99;&#105;&#97;&#108;&#46;&#99;&#111;&#109;</email><files>New Text Document.txt</files></modification><modification><node>f5e50e14bf09</node><author>bbarry</author><date>Thu, 24 Apr 2008 11:30:48 -0600</date><desc>asdf</desc><rev>2</rev><email>&#98;&#98;&#97;&#114;&#114;&#121;&#64;&#115;&#116;&#101;&#108;&#108;&#97;&#114;&#102;&#105;&#110;&#97;&#110;&#99;&#105;&#97;&#108;&#46;&#99;&#111;&#109;</email><files>in_branch.txt</files></modification><modification><node>a903fa9d43f4</node><author>bbarry</author><date>Wed, 23 Apr 2008 22:12:48 -0600</date><desc>adding a file</desc><rev>1</rev><email>&#98;&#98;&#97;&#114;&#114;&#121;&#64;&#115;&#116;&#101;&#108;&#108;&#97;&#114;&#102;&#105;&#110;&#97;&#110;&#99;&#105;&#97;&#108;&#46;&#99;&#111;&#109;</email><files>ReadMe.txt</files></modification><modification><node>030098cf5256</node><author>bbarry</author><date>Wed, 23 Apr 2008 16:28:15 -0600</date><desc>c# app 1</desc><rev>0</rev><email>&#98;&#98;&#97;&#114;&#114;&#121;&#64;&#78;&#66;&#68;&#101;&#118;&#45;&#48;&#49;&#46;&#103;&#97;&#116;&#101;&#119;&#97;&#121;&#46;&#50;&#119;&#105;&#114;&#101;&#46;&#110;&#101;&#116;</email><files>.hgignore hg.sln hg/Class1.cs hg/Properties/AssemblyInfo.cs hg/hg.csproj</files></modification>";
        private readonly string oneEntryLogXml = "<modification><node>48365ef6a3ea</node><author>bbarry</author><date>Thu, 24 Apr 2008 22:14:59 -0600</date><desc>asdf</desc><rev>3</rev><email>&#98;&#98;&#97;&#114;&#114;&#121;&#64;&#115;&#116;&#101;&#108;&#108;&#97;&#114;&#102;&#105;&#110;&#97;&#110;&#99;&#105;&#97;&#108;&#46;&#99;&#111;&#109;</email><files>New Text Document.txt</files></modification>";

        private readonly MercurialHistoryParser hg = new MercurialHistoryParser();

        [Test]
        public void ParsingEmptyLogProducesNoModifications()
        {
            Modification[] modifications = hg.Parse(new StringReader(emptyLogXml), DateTime.Now, DateTime.Now);
            Assert.AreEqual(0, modifications.Length);
        }

        [Test]
        public void ParsingSingleLogMessageProducesOneModification()
        {
            Modification[] modifications = hg.Parse(new StringReader(oneEntryLogXml), DateTime.Now, DateTime.Now);
            Assert.AreEqual(1, modifications.Length);
        }

        [Test]
        public void ParsingLotsOfEntries()
        {
            Modification[] modifications = hg.Parse(new StringReader(fullLogXml), DateTime.Now, DateTime.Now);
            Assert.AreEqual(4, modifications.Length);
        }

        [Test]
        public void HandleInvalidXml()
        {
            Assert.That(delegate { hg.Parse(new StringReader("<foo/><bar/>"), DateTime.Now, DateTime.Now); },
                        Throws.TypeOf<CruiseControlException>());
        }
    }
}
