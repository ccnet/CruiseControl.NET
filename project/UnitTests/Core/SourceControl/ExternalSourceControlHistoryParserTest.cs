using System;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.SourceControl
{
    [TestFixture]
    public class ExternalSourceControlHistoryParserTest
    {
        [Test]
        public void CanParse()
        {
            DateTime oldestModification;
            DateTime newestModification;
            Modification[] expectedModifications = new Modification[2];
            string modificationXml =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<ArrayOfModification xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
    <Modification>
        <ChangeNumber>12245</ChangeNumber>
        <Comment>New Project for testing stuff</Comment>
        <EmailAddress>JUser@Example.Com</EmailAddress>
        <FileName>AssemblyInfo.cs</FileName>
        <FolderName>Dev\Server\Interface\Properties\</FolderName>
        <ModifiedTime>2006-11-22T11:11:00</ModifiedTime>
        <Type>add</Type>
        <UserName>joe_user</UserName>
        <Url>http://www.example.com/index.html</Url>
        <Version>5</Version>
    </Modification>
    <Modification>
        <ChangeNumber>12244</ChangeNumber>
        <Comment>New Project for accessing web services</Comment>
        <EmailAddress>SSpade@Example.Com</EmailAddress>
        <FileName>Interface</FileName>
        <FolderName>Dev\Server\</FolderName>
        <ModifiedTime>2006-11-22T11:10:44</ModifiedTime>
        <Type>add</Type>
        <UserName>sam_spade</UserName>
        <Url>http://www.example.com/index.html</Url>
        <Version>4</Version>
    </Modification>
</ArrayOfModification>
";

            expectedModifications[0] = new Modification();
            expectedModifications[0].ChangeNumber = 12245;
            expectedModifications[0].Comment = "New Project for testing stuff";
            expectedModifications[0].EmailAddress = "JUser@Example.Com";
            expectedModifications[0].FileName = "AssemblyInfo.cs";
            expectedModifications[0].FolderName = @"Dev\Server\Interface\Properties\";
            expectedModifications[0].ModifiedTime = new DateTime(2006, 11, 22, 11, 11, 00);
            expectedModifications[0].Type = "add";
            expectedModifications[0].Url = "http://www.example.com/index.html";
            expectedModifications[0].UserName = "joe_user";
            expectedModifications[0].Version = "5";

            expectedModifications[1] = new Modification();
            expectedModifications[1].ChangeNumber = 12244;
            expectedModifications[1].Comment = "New Project for accessing web services";
            expectedModifications[1].EmailAddress = "SSpade@Example.Com";
            expectedModifications[1].FileName = "Interface";
            expectedModifications[1].FolderName = @"Dev\Server\";
            expectedModifications[1].ModifiedTime = new DateTime(2006, 11, 22, 11, 10, 44);
            expectedModifications[1].Type = "add";
            expectedModifications[1].Url = "http://www.example.com/index.html"; 
            expectedModifications[1].UserName = "sam_spade";
            expectedModifications[1].Version = "4";

            oldestModification = DateTime.Parse("2006/11/22 11:10:44");
            newestModification = DateTime.Parse("2006/11/22 11:11:00");

            TextReader historyReader = new StringReader(modificationXml);

            ExternalSourceControlHistoryParser parser = new ExternalSourceControlHistoryParser();
            Modification[] mods = parser.Parse(historyReader, oldestModification, newestModification);
            Assert.IsNotNull(mods, "mods should not be null");
            Assert.AreEqual(expectedModifications.Length, mods.Length);
            for (int i = 0; i < expectedModifications.Length; i++)
            {
                Assert.AreEqual(expectedModifications[i].ChangeNumber, mods[i].ChangeNumber);
                Assert.AreEqual(expectedModifications[i].Comment, mods[i].Comment);
                Assert.AreEqual(expectedModifications[i].EmailAddress, mods[i].EmailAddress);
                Assert.AreEqual(expectedModifications[i].FileName, mods[i].FileName);
                Assert.AreEqual(expectedModifications[i].FolderName, mods[i].FolderName);
                Assert.AreEqual(expectedModifications[i].ModifiedTime, mods[i].ModifiedTime);
                Assert.AreEqual(expectedModifications[i].Type, mods[i].Type);
                Assert.AreEqual(expectedModifications[i].Url, mods[i].Url);
                Assert.AreEqual(expectedModifications[i].UserName, mods[i].UserName);
                Assert.AreEqual(expectedModifications[i].Version, mods[i].Version);
            }
        }

        [Test]
        public void CanParseEmpty()
        {
            TextReader historyReader = new StringReader("");
            DateTime oldestModification = DateTime.Parse("2006/11/22 11:10:44");
            DateTime newestModification = DateTime.Parse("2006/11/22 11:11:00");

            ExternalSourceControlHistoryParser parser = new ExternalSourceControlHistoryParser();
            Modification[] mods = parser.Parse(historyReader, oldestModification, newestModification);
            Assert.IsNotNull(mods, "mods should not be null");
            Assert.AreEqual(0, mods.Length);
        }
    }
}
