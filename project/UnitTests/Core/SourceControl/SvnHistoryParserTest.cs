using System;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class SvnHistoryParserTest : CustomAssertion
	{
		string emptyLogXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<log>\n</log>";
		string fullLogXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" + "<log>\n" + "<logentry\n" + "   revision=\"3\">\n" + "<author>mbr</author>\n" + "<date>2003-12-12T17:09:44.559203Z</date>\n" + "<paths>\n" + "<path\n" + "   action=\"M\">/myfile.txt</path>\n" + "<path\n" + "   action=\"D\">/foo/foofile.txt</path>\n" + "<path\n" + "   action=\"A\">/foo/barfile.txt</path>\n" + "</paths>\n" + "<msg>Other Mike made some changes</msg>\n" + "</logentry>\n" + "<logentry\n" + "   revision=\"2\">\n" + "<author>mgm</author>\n" + "<date>2003-12-12T16:50:44.000000Z</date>\n" + "<paths>\n" + "<path\n" + "   action=\"A\">/bar/mgmfile.txt</path>\n" + "<path\n" + "   action=\"M\">/myfile.txt</path>\n" + "<path\n" + "   action=\"A\">/bar</path>\n" + "</paths>\n" + "<msg>mgm made some changes</msg>\n" + "</logentry>\n" + "<logentry\n" + "   revision=\"1\">\n" + "<date>2003-12-12T16:48:51.000000Z</date>\n" + "<paths>\n" + "<path\n" + "   action=\"A\">/foo</path>\n" + "<path\n" + "   action=\"A\">/myfile.txt</path>\n" + "<path\n" + "   action=\"A\">/foo/foofile.txt</path>\n" + "</paths>\n" + "<msg>added some stuff with anon user</msg>\n" + "</logentry>\n" + "</log>	\n";
		string oneEntryLogXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" + "<log>\n<logentry revision=\"4\"><date>2003-12-12T16:48:51Z</date>\n" + "<paths><path action=\"A\">/foo/addedfile.txt</path></paths><msg>i added a file</msg></logentry></log>";

		DateTime oldestEntry = DateTime.Parse("2003-12-12T16:48:50Z");
		DateTime newestEntry = DateTime.Parse("2003-12-12T17:09:45Z");

		private SvnHistoryParser svn = new SvnHistoryParser();

		[Test]
		public void ParsingEmptyLogProducesNoModifications()
		{
			Modification[] modifications = svn.Parse(new StringReader(emptyLogXml), oldestEntry, newestEntry);
			Assert.AreEqual(0, modifications.Length);
		}

		[Test]
		public void ParsingSingleLogMessageProducesOneModification()
		{
			Modification[] modifications = svn.Parse(new StringReader(oneEntryLogXml), oldestEntry, newestEntry);

			Assert.AreEqual(1, modifications.Length);

			Modification expected = new Modification();
			expected.Type = "Added";
			expected.FileName = "addedfile.txt";
			expected.FolderName = "/foo";
			expected.ModifiedTime = CreateDate("2003-12-12T16:48:51Z");
			expected.ChangeNumber = 4;
			expected.UserName = "";
			expected.Comment = "i added a file";

			Assert.AreEqual(expected, modifications[0]);
		}

		[Test]
		public void ParsingLotsOfEntries()
		{
			Modification[] modifications = svn.Parse(new StringReader(fullLogXml), oldestEntry, newestEntry);

			Assert.AreEqual(9, modifications.Length);

			Modification mbrMod1 = new Modification();
			mbrMod1.Type = "Modified";
			mbrMod1.FileName = "myfile.txt";
			mbrMod1.FolderName = "";
			mbrMod1.ModifiedTime = CreateDate("2003-12-12T17:09:44.559203Z");
			mbrMod1.ChangeNumber = 3;
			mbrMod1.UserName = "mbr";
			mbrMod1.Comment = "Other Mike made some changes";

			Assert.AreEqual(mbrMod1, modifications[0]);

			mbrMod1.Type = "Deleted";
			mbrMod1.FolderName = "/foo";
			mbrMod1.FileName = "foofile.txt";

			Assert.AreEqual(mbrMod1, modifications[1]);
		}

		[Test]
		public void EntriesOutsideOfRequestedTimeRangeAreIgnored()
		{
			DateTime newest = DateTime.Parse("2003-12-12T17:09:40Z");
			DateTime oldest = DateTime.Parse("2003-12-12T16:48:52Z");

			Modification[] modifications = svn.Parse(new StringReader(fullLogXml), oldest, newest);
			Assert.AreEqual(3, modifications.Length);
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void HandleInvalidXml()
		{
			svn.Parse(new StringReader("<foo/><bar/>"), DateTime.Now, DateTime.Now);
		}

		[Test]
		public void ParseModificationWithReplaceAction()
		{
			string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><log><logentry revision=\"4\"><date>2003-12-12T16:48:51Z</date><paths><path action=\"R\">/foo/addedfile.txt</path></paths><msg>i added a file</msg></logentry></log>";
			Modification[] mods = svn.Parse(new StringReader(xml), oldestEntry, newestEntry);
			Assert.AreEqual("Replaced", mods[0].Type);
		}

		[Test]
		public void ParseModificationWithLongDate()
		{
			string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><log><logentry revision=\"4\"><date>2007-02-02T23:17:08.718100Z</date><paths><path action=\"R\">/foo/addedfile.txt</path></paths><msg>i added a file</msg></logentry></log>";
			Modification[] mods = svn.Parse(new StringReader(xml), new DateTime(2007, 2, 1, 23, 17, 8), new DateTime(2007, 2, 3, 23, 17, 9));
			Assert.AreEqual("Replaced", mods[0].Type);
		}

		private DateTime CreateDate(string dateString)
		{
			return DateTime.Parse(dateString);
		}
	}
}