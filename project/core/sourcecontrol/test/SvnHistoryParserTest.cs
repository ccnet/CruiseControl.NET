using System;
using System.IO;
using System.Collections;
using System.Globalization;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
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
			AssertEquals(0, modifications.Length);
		}

		[Test]
		public void ParsingSingleLogMessageProducesOneModification()
		{
			Modification[] modifications = svn.Parse(new StringReader(oneEntryLogXml), oldestEntry, newestEntry);

			AssertEquals(1, modifications.Length);

			Modification expected = new Modification();
			expected.Type = "Added";
			expected.FileName = "addedfile.txt";
			expected.FolderName = "/foo";
			expected.ModifiedTime = CreateDate("2003-12-12T16:48:51Z");
			expected.ChangeNumber = 4;
			expected.UserName = "";
			expected.Comment = "i added a file";

			AssertEquals(expected, modifications[0]);
		}

		[Test]
		public void ParsingLotsOfEntries()
		{
			Modification[] modifications = svn.Parse(new StringReader(fullLogXml), oldestEntry, newestEntry);

			AssertEquals(9, modifications.Length);

			Modification mbrMod1 = new Modification();
			mbrMod1.Type = "Modified";
			mbrMod1.FileName = "myfile.txt";
			mbrMod1.FolderName = "";
			mbrMod1.ModifiedTime = CreateDate("2003-12-12T17:09:44.559203Z");
			mbrMod1.ChangeNumber = 3;
			mbrMod1.UserName = "mbr";
			mbrMod1.Comment = "Other Mike made some changes";

			AssertEquals(mbrMod1, modifications[0]);

			mbrMod1.Type = "Deleted";
			mbrMod1.FolderName = "/foo";
			mbrMod1.FileName = "foofile.txt";

			AssertEquals(mbrMod1, modifications[1]);
		}

		[Test]
		public void EntriesOutsideOfRequestedTimeRangeAreIgnored()
		{
			DateTime newest = DateTime.Parse("2003-12-12T17:09:40Z");
			DateTime oldest = DateTime.Parse("2003-12-12T16:48:52Z");

			Modification[] modifications = svn.Parse(new StringReader(fullLogXml), oldest, newest);
			AssertEquals(3, modifications.Length);
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void HandleInvalidXml()
		{
			svn.Parse(new StringReader("<foo/><bar/>"), DateTime.Now, DateTime.Now);
		}

		private DateTime CreateDate(string dateString)
		{
			return DateTime.Parse(dateString);
		}
	}
}