using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class MksHistoryParserTest
	{
	    private string TEST_DATA = String.Empty;
        private string MEMBER_INFO = String.Empty;
        private static string windowsPath = @"c:\Sandboxes\Personal2";
        private string path = Platform.IsWindows ? windowsPath : @"/Sandboxes/Personal2";

        [OneTimeSetUp]
        public void SetUp()
        {
            // Transform xml output
            try
            {
                Assembly execAssem = Assembly.GetExecutingAssembly();
                using (Stream s =
                    execAssem.GetManifestResourceStream(
                        "ThoughtWorks.CruiseControl.UnitTests.Core.SourceControl.MksHistoryParserTestData.xml"))
                {

                    if (s != null)
                    {
                        StreamReader rdr = new StreamReader(s);
                        TEST_DATA = rdr.ReadToEnd().Replace(windowsPath.Replace('\\', '/'), path);
                    }
                    else
                    {
                        throw new CruiseControlException("Exception encountered while retrieving MksHistoryParserTestData.xml");
                    }
                }

                using (Stream s =
                    execAssem.GetManifestResourceStream(
                        "ThoughtWorks.CruiseControl.UnitTests.Core.SourceControl.MksHistoryParserTestDataMemberInfo.xml"))
                {
                    if (s != null)
                    {
                        StreamReader rdr = new StreamReader(s);
                        MEMBER_INFO = rdr.ReadToEnd();
                    }
                    else
                    {
                        throw new CruiseControlException("Exception encountered while retrieving MksHistoryParserTestDataMemberInfo.xml");
                    }
                }
            }
            catch (Exception e)
            {
                throw new CruiseControlException("Exception retrieving MKS test data.", e);
            }
        }

		[Test]
		public void ParseOnlyRevisions()
		{
			MksHistoryParser parser = new MksHistoryParser();
			Modification[] modifications = parser.Parse(new StringReader(TEST_DATA), DateTime.Now, DateTime.Now);

            int changeCount = 0;

		    foreach (var modification in modifications)
		    {
		        if (modification.Type == "change")
		        {
		            changeCount++;
                    Assert.AreEqual("TestFile1.txt", modification.FileName);
                    Assert.AreEqual(path, modification.FolderName);
                    Assert.AreEqual("1.3", modification.Version);
		        }
		    }
            Assert.AreEqual(1, changeCount);
            Assert.AreEqual(3, modifications.Length);
		}

		[Test]
		public void ParseOnlyAdded()
		{
			MksHistoryParser parser = new MksHistoryParser();
			Modification[] modifications = parser.Parse(new StringReader(TEST_DATA), DateTime.Now, DateTime.Now);

            int changeCount = 0;

            foreach (var modification in modifications)
            {
                if (modification.Type == "add")
                {
                    changeCount++;
                    Assert.AreEqual("TestNew.txt", modification.FileName);
                    Assert.AreEqual(path, modification.FolderName);
                    Assert.AreEqual("1.1", modification.Version);
                }
            }

            Assert.AreEqual(1, changeCount);
			Assert.AreEqual(3, modifications.Length);
		}

		[Test]
		public void ParseOnlyDeleted()
		{
			MksHistoryParser parser = new MksHistoryParser();
			Modification[] modifications = parser.Parse(new StringReader(TEST_DATA), DateTime.Now, DateTime.Now);

            int changeCount = 0;

            foreach (var modification in modifications)
            {
                if (modification.Type == "deleted")
                {
                    changeCount++;
                    Assert.AreEqual("TestFile2.txt", modification.FileName);
                    Assert.AreEqual(path, modification.FolderName);
                    Assert.AreEqual("NA", modification.Version);
                }
            }

            Assert.AreEqual(1, changeCount);
			Assert.AreEqual(3, modifications.Length);
		}

        [Test]
        public void ParseMemberInfo()
        {
            Modification modification = new Modification();
            MksHistoryParser parser = new MksHistoryParser();
            parser.ParseMemberInfoAndAddToModification(modification, new StringReader(MEMBER_INFO));

            DateTime modifiedTimeWithLocalTimeZone = DateTime.Parse("2009-10-16T18:07:08");
            DateTime modifiedTimeWithCorrectTimeZoneInformation = modification.ModifiedTime;
            TimeSpan actualOffsetAtModifiedTime = modifiedTimeWithCorrectTimeZoneInformation.Subtract(modifiedTimeWithLocalTimeZone);

            TimeSpan expectedOffsetAtModifiedTime = TimeZone.CurrentTimeZone.GetUtcOffset(modifiedTimeWithLocalTimeZone);

            Assert.AreEqual("Test", modification.UserName);
            Assert.AreEqual(expectedOffsetAtModifiedTime, actualOffsetAtModifiedTime, "Date was not parsed with correct time zone offset.");
            Assert.AreEqual("Test Comment", modification.Comment);
            Assert.AreEqual("1234", modification.ChangeNumber);
        }
	}
}
