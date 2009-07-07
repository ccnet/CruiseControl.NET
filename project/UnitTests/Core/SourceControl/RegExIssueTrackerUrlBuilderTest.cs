using System;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
    [TestFixture]
    public class RegExIssueTrackerUrlBuilderTest
    {
        const string ExpectedUrl = "http://jira.public.thoughtworks.org/browse/CCNET-5000";
        const string issueTrackerUrl = "http://jira.public.thoughtworks.org";


        //Assert position at the beginning of the string «^»
        //Match any single character that is not a line break character «.*»
        //   Between zero and unlimited times, as many times as possible, giving back as needed (greedy) «*»
        //Match the regular expression below and capture its match into backreference number 1 «(CCNET-\d*)»
        //   Match the characters “CCNET-” literally «CCNET-»
        //   Match a single digit 0..9 «\d*»
        //      Between zero and unlimited times, as many times as possible, giving back as needed (greedy) «*»
        //Match any single character that is not a line break character «.*»
        //   Between zero and unlimited times, as many times as possible, giving back as needed (greedy) «*»
        //Assert position at the end of the string (or before the line break at the end of the string, if any) «$»
        private string searchString = @"^.*(CCNET-\d*).*$";


        //replace with /browse/backreference1  «$1»
        private string replacementString = string.Format("{0}/browse/$1", issueTrackerUrl);


        private string CreateSourceControlXml()
        {
            return string.Format("<issueUrlBuilder type=\"regexIssueTracker\"><find>{0}</find><replace>{1}</replace></issueUrlBuilder>", searchString, replacementString);
        }


        private RegExIssueTrackerUrlBuilder CreateBuilder()
        {
            RegExIssueTrackerUrlBuilder regexIssue = new RegExIssueTrackerUrlBuilder();
            NetReflector.Read(CreateSourceControlXml(), regexIssue);
            return regexIssue;
        }

        [Test]
        public void ValuePopulation()
        {
            RegExIssueTrackerUrlBuilder regexIssue = CreateBuilder();

            Assert.AreEqual(searchString, regexIssue.Find);
            Assert.AreEqual(replacementString, regexIssue.Replace);
        }




        [Test]
        public void CommentWithProjectPrefixAndIssueNumberAndText()
        {
            Modification[] mods = new Modification[1];
            mods[0] = new Modification();
            mods[0].FolderName = "/trunk";
            mods[0].FileName = "nant.bat";
            mods[0].ChangeNumber = 3;
            mods[0].Comment = "CCNET-5000 blablabla";

            RegExIssueTrackerUrlBuilder regexIssue = CreateBuilder();
            regexIssue.SetupModification(mods);

            Assert.AreEqual(ExpectedUrl, mods[0].IssueUrl);
        }


        [Test]
        public void CommentWithProjectPrefixAndIssueNumber()
        {
            Modification[] mods = new Modification[1];
            mods[0] = new Modification();
            mods[0].FolderName = "/trunk";
            mods[0].FileName = "nant.bat";
            mods[0].ChangeNumber = 3;
            mods[0].Comment = "CCNET-5000";

            RegExIssueTrackerUrlBuilder regexIssue = CreateBuilder();
            regexIssue.SetupModification(mods);

            Assert.AreEqual(ExpectedUrl, mods[0].IssueUrl);
        }



        [Test]
        public void CommentWithPrefixAndProjectPrefixAndIssueNumber()
        {
            Modification[] mods = new Modification[1];
            mods[0] = new Modification();
            mods[0].FolderName = "/trunk";
            mods[0].FileName = "nant.bat";
            mods[0].ChangeNumber = 3;
            mods[0].Comment = "some random text CCNET-5000 and the issue description";

            RegExIssueTrackerUrlBuilder regexIssue = CreateBuilder();
            regexIssue.SetupModification(mods);

            Assert.AreEqual(ExpectedUrl, mods[0].IssueUrl);
        }


        [Test]
        public void CommentWithIssueNumberAndText()
        {
            Modification[] mods = new Modification[1];
            mods[0] = new Modification();
            mods[0].FolderName = "/trunk";
            mods[0].FileName = "nant.bat";
            mods[0].ChangeNumber = 3;
            mods[0].Comment = "5000 blablabla";

            RegExIssueTrackerUrlBuilder regexIssue = CreateBuilder();
            regexIssue.SetupModification(mods);

            Assert.IsNull(mods[0].IssueUrl);
        }

        [Test]
        public void CommentWithIssueNumberOnly()
        {
            Modification[] mods = new Modification[1];
            mods[0] = new Modification();
            mods[0].FolderName = "/trunk";
            mods[0].FileName = "nant.bat";
            mods[0].ChangeNumber = 3;
            mods[0].Comment = "5000";

            RegExIssueTrackerUrlBuilder regexIssue = CreateBuilder();
            regexIssue.SetupModification(mods);

            Assert.IsNull(mods[0].IssueUrl);
        }


        [Test]
        public void CommentWithTextOnly()
        {
            Modification[] mods = new Modification[1];
            mods[0] = new Modification();
            mods[0].FolderName = "/trunk";
            mods[0].FileName = "nant.bat";
            mods[0].ChangeNumber = 3;
            mods[0].Comment = "bla blabla bla bla";

            RegExIssueTrackerUrlBuilder regexIssue = CreateBuilder();
            regexIssue.SetupModification(mods);

            Assert.IsNull(mods[0].IssueUrl);
        }


        [Test]
        public void NoCommentAtAll()
        {
            Modification[] mods = new Modification[1];
            mods[0] = new Modification();
            mods[0].FolderName = "/trunk";
            mods[0].FileName = "nant.bat";
            mods[0].ChangeNumber = 3;
            mods[0].Comment = "";

            RegExIssueTrackerUrlBuilder regexIssue = CreateBuilder();
            regexIssue.SetupModification(mods);

            Assert.IsNull(mods[0].IssueUrl);
        }



    }
}
