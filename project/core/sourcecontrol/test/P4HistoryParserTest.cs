using System;
using System.IO;
using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
{
	[TestFixture]
	public class P4HistoryParserTest : Assertion
	{
		private IHistoryParser _parser = new P4HistoryParser();

		[Test]
		public void Parse_MultipleModifications() 
		{
			Modification[] mods = _parser.Parse(P4Mother.ContentReader, P4Mother.OLDEST_ENTRY, P4Mother.NEWEST_ENTRY);
			AssertEquals("number of modifications", 7, mods.Length);
			assertModification(mods[0], "SpoonCrusher.cs", "//depot/myproject/something", new DateTime(2002, 10, 31, 18, 20, 59), "edit", "someone@somewhere", "someone", "Something important\r\nso there!");
			assertModification(mods[1], "AppleEater.cs", "//depot/myproject/something", new DateTime(2002, 10, 31, 18, 20, 59), "add", "someone@somewhere", "someone", "Something important\r\nso there!");
			assertModification(mods[2], "MonkeyToucher.cs", "//depot/myproject/something", new DateTime(2002, 10, 31, 18, 20, 59), "edit", "someone@somewhere", "someone", "Something important\r\nso there!");
			assertModification(mods[3], "IPerson.cs", "//depot/myproject/foo", new DateTime(2002, 10, 31, 14, 20, 59), "edit", "someone@somewhere", "someone", "One line\r\n\r\nAnother line");
			assertModification(mods[4], "MiniMike.cs", "//depot/myproject/foo", new DateTime(2002, 10, 31, 14, 20, 59), "add", "someone@somewhere", "someone", "One line\r\n\r\nAnother line");
			assertModification(mods[5], "JoeJoeJoe.cs", "//depot/myproject/foo", new DateTime(2002, 10, 31, 14, 20, 59), "add", "someone@somewhere", "someone", "One line\r\n\r\nAnother line");
			assertModification(mods[6], "Fish.cs", "//depot/myproject/tank", new DateTime(2002, 10, 31, 11, 20, 59), "add", "bob@nowhere", "bob", "thingy\r\n(evil below)\r\nAffected files ...\r\nChange 123 by someone@somewhere on 2002/10/31 11:20:59\r\n(end of evil)");
			AssertEquals(7, mods.Length);
		}

		[Test]
		public void Parse_SingleModification()
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
			Modification[] mods = _parser.Parse(input, entryDate, entryDate);
			AssertEquals("number of mods", 1, mods.Length);
			AssertEquals("user", "guox01", mods[0].UserName);
			AssertEquals("email address", "guox01@BP1HEMAP048", mods[0].EmailAddress);
			AssertEquals("filename", "readme", mods[0].FileName);
			AssertEquals("folder name", "//shipping", mods[0].FolderName);
			AssertEquals("mod type", "edit", mods[0].Type);
			AssertEquals("comment", "a test comment", mods[0].Comment);
		}

		private void assertModification(Modification mod, string file, string folder, 
			DateTime modifiedTime, string type, string email, string username, string comment)
		{
			AssertEquals(file, mod.FileName);
			AssertEquals(folder, mod.FolderName);
			AssertEquals(modifiedTime, mod.ModifiedTime);
			AssertEquals(type, mod.Type);
			AssertEquals(email, mod.EmailAddress);
			AssertEquals(username, mod.UserName);
			AssertEquals(comment, mod.Comment);
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
			string expected = "3328 3327 332";

			AssertEquals(expected,new P4HistoryParser().ParseChanges(changes));
		}

		[Test]
		public void ParseEmptyChangeList()
		{
			string changes = "exit: 0";
			string expected = "";

			AssertEquals(expected,new P4HistoryParser().ParseChanges(changes));
		}

		[Test]
		[ExpectedException(typeof(CruiseControlException))]
		public void ParseChangeListWithExitOne()
		{
			string changes = 
				@"
info: blah
exit: 1
";
			new P4HistoryParser().ParseChanges(changes);
		}

	}
}
