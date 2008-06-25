using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	[TestFixture]
	public class RSSPublisherTest : IntegrationFixture
	{
		IIntegrationResult result;
		RssPublisher publisher;

		[SetUp]
		public void Setup()
		{
			publisher = new RssPublisher();
		}


        [Test]
        public void HeaderContainsCorrectDetails()
        {
            result = IntegrationResultMother.CreateSuccessful("99");
            result.ProjectUrl = "http://somewhere/someplace.html";

            /// Execute
            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            System.Xml.XmlNode SelectedNode = null;
            document.LoadXml(publisher.GenerateDocument(result));

            /// Verify
            SelectedNode = document.SelectSingleNode("//rss");
            Assert.IsNotNull(SelectedNode, "No rss data found");

            SelectedNode = document.SelectSingleNode("//rss/@version");
            Assert.IsNotNull(SelectedNode, "No rss version data found");
            Assert.AreEqual("2.0", SelectedNode.Value, "Wrong version number");
        }


		[Test]
		public void ChannelContainsCorrectDetails()
		{
            result = IntegrationResultMother.CreateSuccessful("99");
            result.ProjectUrl = "http://somewhere/someplace.html";
            

            /// Execute
            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            System.Xml.XmlNode SelectedNode = null;
            document.LoadXml( publisher.GenerateDocument(result));
           
            /// Verify
            SelectedNode = document.SelectSingleNode("//rss/channel");
            Assert.IsNotNull(SelectedNode, "No channel data found");

            
            SelectedNode = document.SelectSingleNode("//rss/channel/title");
            Assert.IsNotNull(SelectedNode, "No title in channel element found");
            Assert.AreEqual("CruiseControl.NET - " + result.ProjectName, SelectedNode.InnerText, "Wrong title");

            
            SelectedNode = document.SelectSingleNode("//rss/channel/link");
            Assert.IsNotNull(SelectedNode, "No link in channel element found");
            Assert.AreEqual(result.ProjectUrl, SelectedNode.InnerText, "Wrong link");


            SelectedNode = document.SelectSingleNode("//rss/channel/description");
            Assert.IsNotNull(SelectedNode, "No description in channel element found");
            Assert.AreEqual("The latest build results for " + result.ProjectName, SelectedNode.InnerText, "Wrong description");


            SelectedNode = document.SelectSingleNode("//rss/channel/language");
            Assert.IsNotNull(SelectedNode, "No language in channel element found");
            Assert.AreEqual("en" , SelectedNode.InnerText, "Wrong language");


            SelectedNode = document.SelectSingleNode("//rss/channel/ttl");
            Assert.IsNotNull(SelectedNode, "No ttl in channel element found");
            Assert.AreEqual("5", SelectedNode.InnerText, "Wrong ttl");

            
            SelectedNode = document.SelectSingleNode("//rss/channel/item");
            Assert.IsNotNull(SelectedNode, "No item in channel element found");

		}


        [Test]
        public void TitleForOKBuild_NoModifications()
        {
            result = IntegrationResultMother.CreateSuccessful("99");
            string ExpectedXMLItemData = "Build 99 : Success  No changed files found in build  ";

            /// Execute
            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            System.Xml.XmlNode SelectedNode = null;
            document.LoadXml(publisher.GenerateDocument(result));

            /// Verify
            SelectedNode = document.SelectSingleNode("//rss/channel/item/title");
            Assert.AreEqual(ExpectedXMLItemData, SelectedNode.InnerXml);
           

        }

        [Test]
        public void TitleForNotOKBuild_1Modification()
        {
            Modification[] mods = OneModification();

            result = IntegrationResultMother.CreateFailed(mods);
            string ExpectedXMLItemData = "Build 2.0 : Failure  1 changed file found in build  First Comment : some comment";

            /// Execute
            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            System.Xml.XmlNode SelectedNode = null;
            document.LoadXml(publisher.GenerateDocument(result));

            /// Verify
            SelectedNode = document.SelectSingleNode("//rss/channel/item/title");
            Assert.AreEqual(ExpectedXMLItemData, SelectedNode.InnerXml);


        }



        [Test]
        public void TitleForOKBuild_1Modification()
        {
            Modification[] mods = OneModification();

            result = IntegrationResultMother.CreateSuccessful(mods);
            string ExpectedXMLItemData = "Build 2.0 : Success  1 changed file found in build  First Comment : some comment";

            /// Execute
            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            System.Xml.XmlNode SelectedNode = null;
            document.LoadXml(publisher.GenerateDocument(result));

            /// Verify
            SelectedNode = document.SelectSingleNode("//rss/channel/item/title");
            Assert.AreEqual(ExpectedXMLItemData, SelectedNode.InnerXml);


        }


		[Test]
		public void ItemDataForOKBuildNoModifications()
		{
            result = IntegrationResultMother.CreateSuccessful("99");
            result.ProjectUrl = "http://somewhere/someplace.html";
            string ExpectedXMLItemData = "<title>Build 99 : Success  No changed files found in build  </title><description>No changed files found in build</description>";

            /// Execute
            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            System.Xml.XmlNode SelectedNode = null;
            document.LoadXml(publisher.GenerateDocument(result));

            /// Verify
            SelectedNode = document.SelectSingleNode("//rss/channel/item");
            Assert.AreEqual(ExpectedXMLItemData, SelectedNode.InnerXml);
           
		}

        [Test]
        public void ItemDataForOKBuild1Modification()
        {

            Modification[] mods = OneModification();           
            result = IntegrationResultMother.CreateSuccessful(mods);
            result.ProjectUrl = "http://somewhere/someplace.html";
            string ExpectedXMLItemData = "<title>Build 2.0 : Success  1 changed file found in build  First Comment : some comment</title><description>1 changed file found in build</description><content:encoded xmlns:content=\"http://purl.org/rss/1.0/modules/content/\"><![CDATA[\r\n<h4>Modifications in build :</h4>\r\n<table cellpadding=\"5\">\r\n<tr><td>Coder</td><td>some comment</td></tr>\r\n</table>\r\n<h4>Detailed information of the modifications in the build :</h4>\r\n<table cellpadding=\"5\">\r\n<tr><td><b>Coder</b></td><td>some comment</td></tr>\r\n<tr><td><font size=2>Modified</font></td><td><font size=2>$/SomeMainFolder/TheFolder/thecode.cs</font></td></tr>\r\n</table>\r\n]]></content:encoded>";


            /// Execute
            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            System.Xml.XmlNode SelectedNode = null;
            document.LoadXml(publisher.GenerateDocument(result));

            /// Verify
            SelectedNode = document.SelectSingleNode("//rss/channel/item");

            Assert.AreEqual(ExpectedXMLItemData, SelectedNode.InnerXml);

        }


        [Test]
        public void ItemDataForOKBuild1Modification1UserMultipleFiles()
        {

            Modification[] mods = new Modification[2];
            mods[0] = new Modification();
            mods[0].Comment = "some comment";
            mods[0].FileName = "thecode.cs";
            mods[0].FolderName = "$/SomeMainFolder/TheFolder";
            mods[0].Type = "Modified";
            mods[0].UserName = "Coder";

            mods[1] = new Modification();
            mods[1].Comment = "some comment";
            mods[1].FileName = "anothercode.cs";
            mods[1].FolderName = "$/SomeMainFolder/TheFolder";
            mods[1].Type = "Modified";
            mods[1].UserName = "Coder";



            result = IntegrationResultMother.CreateSuccessful(mods);
            result.ProjectUrl = "http://somewhere/someplace.html";
            string ExpectedXMLItemData = "<title>Build 2.0 : Success  2 changed files found in build  First Comment : some comment</title><description>2 changed files found in build</description><content:encoded xmlns:content=\"http://purl.org/rss/1.0/modules/content/\"><![CDATA[\r\n<h4>Modifications in build :</h4>\r\n<table cellpadding=\"5\">\r\n<tr><td>Coder</td><td>some comment</td></tr>\r\n</table>\r\n<h4>Detailed information of the modifications in the build :</h4>\r\n<table cellpadding=\"5\">\r\n<tr><td><b>Coder</b></td><td>some comment</td></tr>\r\n<tr><td><font size=2>Modified</font></td><td><font size=2>$/SomeMainFolder/TheFolder/thecode.cs</font></td></tr>\r\n<tr><td><font size=2>Modified</font></td><td><font size=2>$/SomeMainFolder/TheFolder/anothercode.cs</font></td></tr>\r\n</table>\r\n]]></content:encoded>";


            /// Execute
            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            System.Xml.XmlNode SelectedNode = null;
            document.LoadXml(publisher.GenerateDocument(result));

            /// Verify
            SelectedNode = document.SelectSingleNode("//rss/channel/item");

            Assert.AreEqual(ExpectedXMLItemData, SelectedNode.InnerXml);

        }

        [Test]
        public void ItemDataForOKBuild2Modifications2UsersMultipleFiles()
        {

            Modification[] mods = new Modification[4];
            mods[0] = new Modification();
            mods[0].Comment = "some comment";
            mods[0].FileName = "thecode.cs";
            mods[0].FolderName = "$/SomeMainFolder/TheFolder";
            mods[0].Type = "Modified";
            mods[0].UserName = "Coder";

            mods[1] = new Modification();
            mods[1].Comment = "some comment";
            mods[1].FileName = "anothercode.cs";
            mods[1].FolderName = "$/SomeMainFolder/TheFolder";
            mods[1].Type = "Modified";
            mods[1].UserName = "Coder";


            mods[2] = new Modification();
            mods[2].Comment = "some change";
            mods[2].FileName = "myfile.cs";
            mods[2].FolderName = "$/SomeMainFolder/TheFolder/subdivision";
            mods[2].Type = "Modified";
            mods[2].UserName = "GreatCoder";


            mods[3] = new Modification();
            mods[3].Comment = "some change";
            mods[3].FileName = "myfileFactory.cs";
            mods[3].FolderName = "$/SomeMainFolder/TheFolder/subdivision";
            mods[3].Type = "Modified";
            mods[3].UserName = "GreatCoder";



            result = IntegrationResultMother.CreateSuccessful(mods);
            result.ProjectUrl = "http://somewhere/someplace.html";
            string ExpectedXMLItemData = "<title>Build 2.0 : Success  4 changed files found in build  First Comment : some comment</title><description>4 changed files found in build</description><content:encoded xmlns:content=\"http://purl.org/rss/1.0/modules/content/\"><![CDATA[\r\n<h4>Modifications in build :</h4>\r\n<table cellpadding=\"5\">\r\n<tr><td>Coder</td><td>some comment</td></tr>\r\n<tr><td>GreatCoder</td><td>some change</td></tr>\r\n</table>\r\n<h4>Detailed information of the modifications in the build :</h4>\r\n<table cellpadding=\"5\">\r\n<tr><td><b>Coder</b></td><td>some comment</td></tr>\r\n<tr><td><font size=2>Modified</font></td><td><font size=2>$/SomeMainFolder/TheFolder/thecode.cs</font></td></tr>\r\n<tr><td><font size=2>Modified</font></td><td><font size=2>$/SomeMainFolder/TheFolder/anothercode.cs</font></td></tr>\r\n<tr><td><b>GreatCoder</b></td><td>some change</td></tr>\r\n<tr><td><font size=2>Modified</font></td><td><font size=2>$/SomeMainFolder/TheFolder/subdivision/myfile.cs</font></td></tr>\r\n<tr><td><font size=2>Modified</font></td><td><font size=2>$/SomeMainFolder/TheFolder/subdivision/myfileFactory.cs</font></td></tr>\r\n</table>\r\n]]></content:encoded>";


            /// Execute
            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            System.Xml.XmlNode SelectedNode = null;
            document.LoadXml(publisher.GenerateDocument(result));

            /// Verify
            SelectedNode = document.SelectSingleNode("//rss/channel/item");

            Assert.AreEqual(ExpectedXMLItemData, SelectedNode.InnerXml);

        }



        private static Modification[] OneModification()
        {
            Modification[] mods = new Modification[1];
            mods[0] = new Modification();
            mods[0].Comment = "some comment";
            mods[0].FileName = "thecode.cs";
            mods[0].FolderName = "$/SomeMainFolder/TheFolder";
            mods[0].Type = "Modified";
            mods[0].UserName = "Coder";

            return mods;
        }


    }
}
