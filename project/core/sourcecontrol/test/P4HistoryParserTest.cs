using System;
using NUnit.Framework;

namespace tw.ccnet.core.sourcecontrol.test
{
	[TestFixture]
	public class P4HistoryParserTest
	{

		private IHistoryParser _parser = new P4HistoryParser();

		public void TestParseStream() 
		{
			Modification[] mods = _parser.Parse(P4Mother.ContentReader);
			assertModification(mods[0], "SpoonCrusher.cs", "//depot/myproject/something", new DateTime(2002, 10, 31, 18, 20, 59), "edit", "someone@somewhere", "someone", "Something important\r\nso there!");
			assertModification(mods[1], "AppleEater.cs", "//depot/myproject/something", new DateTime(2002, 10, 31, 18, 20, 59), "add", "someone@somewhere", "someone", "Something important\r\nso there!");
			assertModification(mods[2], "MonkeyToucher.cs", "//depot/myproject/something", new DateTime(2002, 10, 31, 18, 20, 59), "edit", "someone@somewhere", "someone", "Something important\r\nso there!");
			assertModification(mods[3], "IPerson.cs", "//depot/myproject/foo", new DateTime(2002, 10, 31, 14, 20, 59), "edit", "someone@somewhere", "someone", "One line\r\n\r\nAnother line");
			assertModification(mods[4], "MiniMike.cs", "//depot/myproject/foo", new DateTime(2002, 10, 31, 14, 20, 59), "add", "someone@somewhere", "someone", "One line\r\n\r\nAnother line");
			assertModification(mods[5], "JoeJoeJoe.cs", "//depot/myproject/foo", new DateTime(2002, 10, 31, 14, 20, 59), "add", "someone@somewhere", "someone", "One line\r\n\r\nAnother line");
			assertModification(mods[6], "Fish.cs", "//depot/myproject/tank", new DateTime(2002, 10, 31, 11, 20, 59), "add", "bob@nowhere", "bob", "thingy\r\n(evil below)\r\nAffected files ...\r\nChange 123 by someone@somewhere on 2002/10/31 11:20:59\r\n(end of evil)");
			Assertion.AssertEquals(7, mods.Length);
		}

		private void assertModification(Modification mod, string file, string folder, 
			DateTime modifiedTime, string type, string email, string username, string comment)
		{
			Assertion.AssertEquals(file, mod.FileName);
			Assertion.AssertEquals(folder, mod.FolderName);
			Assertion.AssertEquals(modifiedTime, mod.ModifiedTime);
			Assertion.AssertEquals(type, mod.Type);
			Assertion.AssertEquals(email, mod.EmailAddress);
			Assertion.AssertEquals(username, mod.UserName);
			Assertion.AssertEquals(comment, mod.Comment);
		}

		[Test]
		public void ParseChangeList()
		{
			string changes = 
				@"
info: Change 3328 on 2002/10/31 by someone@somewhere 'Something important '
info: Change 3327 on 2002/10/31 by someone@somewhere 'Joe's test '
info: Change 332 on 2002/10/31 by someone@somewhere 'thingy'
exit: 0
";
			string expected = "3328 3327 332";

			Assertion.AssertEquals(expected,new P4HistoryParser().ParseChanges(changes));
		}

		[Test]
		public void ParseEmptyChangeList()
		{
			string changes = "exit: 0";
			string expected = "";

			Assertion.AssertEquals(expected,new P4HistoryParser().ParseChanges(changes));
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
