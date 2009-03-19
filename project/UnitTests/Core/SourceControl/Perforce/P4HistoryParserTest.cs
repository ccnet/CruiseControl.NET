using System;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.SourceControl.Perforce
{
	[TestFixture]
	public class P4HistoryParserTest
	{
		private P4HistoryParser parser = new P4HistoryParser();

		[Test]
		public void ParseWithMultipleModifications()
		{
			Modification[] mods = parser.Parse(P4Mother.ContentReader, P4Mother.OLDEST_ENTRY, P4Mother.NEWEST_ENTRY);
			Assert.AreEqual(7, mods.Length);
			AssertModification(mods[0], 3328, "3", "SpoonCrusher.cs", "//depot/myproject/something", new DateTime(2002, 10, 31, 18, 20, 59), "edit", "someone@somewhere", "someone", "Something important\r\nso there!");
			AssertModification(mods[1], 3328, "1", "AppleEater.cs", "//depot/myproject/something", new DateTime(2002, 10, 31, 18, 20, 59), "add", "someone@somewhere", "someone", "Something important\r\nso there!");
			AssertModification(mods[2], 3328, "811", "MonkeyToucher.cs", "//depot/myproject/something", new DateTime(2002, 10, 31, 18, 20, 59), "edit", "someone@somewhere", "someone", "Something important\r\nso there!");
			AssertModification(mods[3], 3327, "3", "IPerson.cs", "//depot/myproject/foo", new DateTime(2002, 10, 31, 14, 20, 59), "edit", "someone@somewhere", "someone", "One line\r\n\r\nAnother line");
			AssertModification(mods[4], 3327, "1", "MiniMike.cs", "//depot/myproject/foo", new DateTime(2002, 10, 31, 14, 20, 59), "add", "someone@somewhere", "someone", "One line\r\n\r\nAnother line");
			AssertModification(mods[5], 3327, "1", "JoeJoeJoe.cs", "//depot/myproject/foo", new DateTime(2002, 10, 31, 14, 20, 59), "add", "someone@somewhere", "someone", "One line\r\n\r\nAnother line");
			AssertModification(mods[6], 332, "3", "Fish.cs", "//depot/myproject/tank", new DateTime(2002, 10, 31, 11, 20, 59), "add", "bob@nowhere", "bob", "thingy\r\n(evil below)\r\nAffected files ...\r\nChange 123 by someone@somewhere on 2002/10/31 11:20:59\r\n(end of evil)");
			Assert.AreEqual(7, mods.Length);
		}

		[Test]
		public void ParseWithSingleModification()
		{
			// NOTE!  on the comment line, there's a tab at position 7 (not spaces) (immediately before 'a test comment...')
			StringReader input = new StringReader(
				"text: Change 23680 by guox01@BP1HEMAP048 on 2003/09/01 16:30:53\r\n" +
					"text: \r\n" +
					"text: \ta test comment\r\n" +
					"text: \r\n" +
					"text: Affected files ...\r\n" +
					"text: \r\n" +
					"info1: //shipping/readme#10 edit\r\n" +
					"text: \r\n" +
					"exit: 0\r\n");
			DateTime entryDate = DateTime.Parse("2003/09/01 16:30:53");
			Modification[] mods = parser.Parse(input, entryDate, entryDate);
			Assert.AreEqual(1, mods.Length);
			Assert.AreEqual("guox01", mods[0].UserName);
			Assert.AreEqual("guox01@BP1HEMAP048", mods[0].EmailAddress);
			Assert.AreEqual("readme", mods[0].FileName);
			Assert.AreEqual("//shipping", mods[0].FolderName);
			Assert.AreEqual("edit", mods[0].Type);
			Assert.AreEqual("a test comment", mods[0].Comment);
			Assert.AreEqual(23680, mods[0].ChangeNumber);
			Assert.AreEqual("10", mods[0].Version);
		}

		private void AssertModification(Modification mod, int changeNumber, string revision, string file, string folder, DateTime modifiedTime, string type, string email, string username, string comment)
		{
			Assert.AreEqual(changeNumber, mod.ChangeNumber);
			Assert.AreEqual(file, mod.FileName);
			Assert.AreEqual(folder, mod.FolderName);
			Assert.AreEqual(modifiedTime, mod.ModifiedTime);
			Assert.AreEqual(type, mod.Type);
			Assert.AreEqual(email, mod.EmailAddress);
			Assert.AreEqual(username, mod.UserName);
			Assert.AreEqual(comment, mod.Comment);
			Assert.AreEqual(revision, mod.Version);
		}

		[Test]
		public void ParseChanges()
		{
			string changes =
				@"
info: Change 3328 on 2002/10/31 by someone@somewhere 'Something important '
info: Change 3327 on 2002/10/31 by someone@somewhere 'Joe's test '
info: Change 332 on 2002/10/31 by someone@somewhere 'thingy'
exit: 0
";
			Assert.AreEqual("3328 3327 332", new P4HistoryParser().ParseChanges(changes));
		}

		[Test]
		public void ParseEmptyChangeList()
		{
			Assert.AreEqual("", new P4HistoryParser().ParseChanges("exit: 0"));
		}

		[Test]
		public void ParseChangeListWithExitOne()
		{
			string changes =
				@"
info: blah
exit: 1
";			Assert.AreEqual("", new P4HistoryParser().ParseChanges(changes));
		}
	}
}