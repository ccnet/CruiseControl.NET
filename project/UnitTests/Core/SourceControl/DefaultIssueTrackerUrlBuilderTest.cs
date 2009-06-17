using System;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;


namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
    [TestFixture]
    public class DefaultIssueTrackerUrlBuilderTest
    {
        const string UrlFromConfigFile = "http://jira.public.thoughtworks.org/browse/CCNET-{0}";


        private string CreateSourceControlXml()
        {
            return string.Format("<issueUrlBuilder type=\"defaultIssueTracker\"><url>{0}</url></issueUrlBuilder>", UrlFromConfigFile);
        }


        private DefaultIssueTrackerUrlBuilder CreateBuilder()
        {
            DefaultIssueTrackerUrlBuilder defaultIssue = new DefaultIssueTrackerUrlBuilder();
            NetReflector.Read(CreateSourceControlXml(), defaultIssue);
            return defaultIssue;
        }

        [Test]
        public void ValuePopulation()
        {
            DefaultIssueTrackerUrlBuilder defaultIssue = CreateBuilder();

            Assert.AreEqual(UrlFromConfigFile, defaultIssue.Url);
        }


        [Test]
        public void CommentWithProjectPrefixAndIssueNumberAndText()
        {
            Modification[] mods = new Modification[1];
            mods[0] = new Modification();
            mods[0].FolderName = "/trunk";
            mods[0].FileName = "nant.bat";
            mods[0].ChangeNumber = "3";
            mods[0].Comment= "CCNET-5000 blablabla";

            DefaultIssueTrackerUrlBuilder defaultIssue = CreateBuilder();
            defaultIssue.SetupModification(mods);

            string url = String.Format(UrlFromConfigFile, 5000);
            Assert.AreEqual(url, mods[0].IssueUrl);
        }


        [Test]
        public void CommentWithProjectPrefixAndIssueNumber()
        {
            Modification[] mods = new Modification[1];
            mods[0] = new Modification();
            mods[0].FolderName = "/trunk";
            mods[0].FileName = "nant.bat";
            mods[0].ChangeNumber = "3";
            mods[0].Comment = "CCNET-5000";

            DefaultIssueTrackerUrlBuilder defaultIssue = CreateBuilder();
            defaultIssue.SetupModification(mods);

            string url = String.Format(UrlFromConfigFile, 5000);
            Assert.AreEqual(url, mods[0].IssueUrl);
        }


        [Test]
        public void CommentWithIssueNumberAndText()
        {
            Modification[] mods = new Modification[1];
            mods[0] = new Modification();
            mods[0].FolderName = "/trunk";
            mods[0].FileName = "nant.bat";
            mods[0].ChangeNumber = "3";
            mods[0].Comment = "5000 blablabla";

            DefaultIssueTrackerUrlBuilder defaultIssue = CreateBuilder();
            defaultIssue.SetupModification(mods);

            string url = String.Format(UrlFromConfigFile, 5000);
            Assert.AreEqual(url, mods[0].IssueUrl);
        }

        [Test]
        public void CommentWithIssueNumberOnly()
        {
            Modification[] mods = new Modification[1];
            mods[0] = new Modification();
            mods[0].FolderName = "/trunk";
            mods[0].FileName = "nant.bat";
            mods[0].ChangeNumber = "3";
            mods[0].Comment = "5000";

            DefaultIssueTrackerUrlBuilder defaultIssue = CreateBuilder();
            defaultIssue.SetupModification(mods);

            string url = String.Format(UrlFromConfigFile, 5000);
            Assert.AreEqual(url, mods[0].IssueUrl);
        }


        [Test]
        public void CommentWithTextOnly()
        {
            Modification[] mods = new Modification[1];
            mods[0] = new Modification();
            mods[0].FolderName = "/trunk";
            mods[0].FileName = "nant.bat";
            mods[0].ChangeNumber = "3";
            mods[0].Comment = "bla blabla bla bla";

            DefaultIssueTrackerUrlBuilder defaultIssue = CreateBuilder();
            defaultIssue.SetupModification(mods);

            Assert.IsNull( mods[0].IssueUrl);
        }


        [Test]
        public void NoCommentAtAll()
        {
            Modification[] mods = new Modification[1];
            mods[0] = new Modification();
            mods[0].FolderName = "/trunk";
            mods[0].FileName = "nant.bat";
            mods[0].ChangeNumber = "3";
            mods[0].Comment =string.Empty;

            DefaultIssueTrackerUrlBuilder defaultIssue = CreateBuilder();
            defaultIssue.SetupModification(mods);

            Assert.IsNull(mods[0].IssueUrl);
        }


        [Test]
        public void JustASpace()
        {
            Modification[] mods = new Modification[1];
            mods[0] = new Modification();
            mods[0].FolderName = "/trunk";
            mods[0].FileName = "nant.bat";
            mods[0].ChangeNumber = "3";
            mods[0].Comment = " ";

            DefaultIssueTrackerUrlBuilder defaultIssue = CreateBuilder();
            defaultIssue.SetupModification(mods);

            Assert.IsNull(mods[0].IssueUrl);
        }

    }
}
