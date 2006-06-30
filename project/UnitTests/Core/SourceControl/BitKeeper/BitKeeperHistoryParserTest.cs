using System;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.BitKeeper;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol.Bitkeeper
{
	[TestFixture]
	public class BitKeeperHistoryParserTest
	{
		[Test]
		public void ParseChangeSets()
		{
			BitKeeperHistoryParser parser = new BitKeeperHistoryParser();
			Modification[] mods = parser.Parse(new StringReader(BitKeeperTestData.ChangeSetsBitKeeperOutput()), DateTime.Now, DateTime.Now);
			Assert.AreEqual(2, mods.Length);

			// Check specifics of the ChangeSet mod
			Assert.AreEqual("user@host.(none)", mods[0].UserName);
			Assert.AreEqual("  Remove file in subdir.", mods[0].Comment);
			Assert.AreEqual("ChangeSet", mods[0].Type);
			Assert.AreEqual("ChangeSet", mods[0].FileName);
			Assert.AreEqual("", mods[0].FolderName);
		}

		[Test]
		public void ParseRenamed()
		{
			BitKeeperHistoryParser parser = new BitKeeperHistoryParser();
			Modification[] mods = parser.Parse(new StringReader(BitKeeperTestData.RenamedBitKeeperOutput()), DateTime.Now, DateTime.Now);
			Assert.AreEqual(2, mods.Length);

			// Check specifics of the Renamed mod
			Assert.AreEqual("user@host.(none)", mods[1].UserName);
			Assert.AreEqual("    Rename: asubdir/baz.txt -> asubdir2/bazzz.txt", mods[1].Comment);
			Assert.AreEqual("Renamed", mods[1].Type);
			Assert.AreEqual("bazzz.txt", mods[1].FileName);
			Assert.AreEqual("asubdir2", mods[1].FolderName);
		}

		[Test]
		public void ParseDeletions()
		{
			BitKeeperHistoryParser parser = new BitKeeperHistoryParser();
			Modification[] mods = parser.Parse(new StringReader(BitKeeperTestData.DeletionsBitKeeperOutput()), DateTime.Now, DateTime.Now);
			Assert.AreEqual(2, mods.Length);

			// Check specifics of the Deleted mod
			Assert.AreEqual("user@host.(none)", mods[1].UserName);
			Assert.AreEqual("    Delete: asubdir/baz.txt", mods[1].Comment);
			Assert.AreEqual("Deleted", mods[1].Type);
			Assert.AreEqual("baz.txt", mods[1].FileName);
			Assert.AreEqual("asubdir", mods[1].FolderName);
		}

		[Test]
		public void ParseAdditions()
		{
			BitKeeperHistoryParser parser = new BitKeeperHistoryParser();
			Modification[] mods = parser.Parse(new StringReader(BitKeeperTestData.AdditionsBitKeeperOutput()), DateTime.Now, DateTime.Now);
			Assert.AreEqual(2, mods.Length);

			// Check specifics of the Added mod
			Assert.AreEqual("user@host.(none)", mods[1].UserName);
			Assert.AreEqual("    BitKeeper file /var/lib/bk/demo/dev-1.0/asubdir/baz.txt", mods[1].Comment);
			Assert.AreEqual("Added", mods[1].Type);
			Assert.AreEqual("baz.txt", mods[1].FileName);
			Assert.AreEqual("asubdir", mods[1].FolderName);
		}

		[Test]
		public void ParseVerboseModifications()
		{
			BitKeeperHistoryParser parser = new BitKeeperHistoryParser();
			Modification[] mods = parser.Parse(new StringReader(BitKeeperTestData.VerboseBitKeeperOutput()), DateTime.Now, DateTime.Now);
			Assert.AreEqual(21, mods.Length);
			foreach (Modification mod in mods)
			{
				Assert.AreEqual("user@host.(none)", mod.UserName);
				Assert.IsNotNull("filename should not be null", mod.FileName);
			}
		}

		[Test]
		public void ParseNonVerboseModifications()
		{
			BitKeeperHistoryParser parser = new BitKeeperHistoryParser();
			Modification[] mods = parser.Parse(new StringReader(BitKeeperTestData.NonVerboseBitKeeperOutput()), DateTime.Now, DateTime.Now);
			Assert.AreEqual(7, mods.Length);
			foreach (Modification mod in mods)
			{
				Assert.AreEqual("user@host.(none)", mod.UserName);
				Assert.IsNotNull("filename should not be null", mod.FileName);
				Assert.AreEqual("ChangeSet", mod.Type);
			}
		}

		private class BitKeeperTestData
		{
			public static string ChangeSetsBitKeeperOutput()
			{
				return @"ChangeSet
  1.6 05/10/06 12:58:40 user@host.(none) +1 -0
  Remove file in subdir.

  BitKeeper/deleted/.del-baz.txt~7545e8a43744d49
    1.2 05/10/06 12:58:27 user@host.(none) +0 -0
    Delete: asubdir/baz.txt
";
			}

			public static string RenamedBitKeeperOutput()
			{
				return @"ChangeSet
  1.7 05/10/06 12:59:40 user@host.(none) +1 -0
  Rename file in subdir.

  asubdir2/bazzz.txt
    1.2 05/10/06 12:59:27 user@host.(none) +0 -0
    Rename: asubdir/baz.txt -> asubdir2/bazzz.txt
";
			}

			public static string DeletionsBitKeeperOutput()
			{
				return @"ChangeSet
  1.6 05/10/06 12:58:40 user@host.(none) +1 -0
  Remove file in subdir.

  BitKeeper/deleted/.del-baz.txt~7545e8a43744d49
    1.2 05/10/06 12:58:27 user@host.(none) +0 -0
    Delete: asubdir/baz.txt
";
			}

			public static string AdditionsBitKeeperOutput()
			{
				return @"ChangeSet
  1.5 05/10/06 12:58:11 user@host.(none) +1 -0
  Another new one.

  asubdir/baz.txt
    1.0 05/10/06 12:58:11 user@host.(none) +0 -0
    BitKeeper file /var/lib/bk/demo/dev-1.0/asubdir/baz.txt
";
			}

			public static string VerboseBitKeeperOutput()
			{
				return @"ChangeSet
  1.7 05/10/06 12:59:40 user@host.(none) +1 -0
  Rename file in subdir.

  asubdir2/bazzz.txt
    1.2 05/10/06 12:59:27 user@host.(none) +0 -0
    Rename: asubdir/baz.txt -> asubdir2/bazzz.txt

ChangeSet
  1.6 05/10/06 12:58:40 user@host.(none) +1 -0
  Remove file in subdir.

  BitKeeper/deleted/.del-baz.txt~7545e8a43744d49
    1.2 05/10/06 12:58:27 user@host.(none) +0 -0
    Delete: asubdir/baz.txt

  asubdir/baz.txt
    1.1 05/10/06 12:58:11 user@host.(none) +1 -0
    Another new one.

ChangeSet
  1.5 05/10/06 12:58:11 user@host.(none) +1 -0
  Another new one.

  asubdir/baz.txt
    1.0 05/10/06 12:58:11 user@host.(none) +0 -0
    BitKeeper file /var/lib/bk/demo/dev-1.0/asubdir/baz.txt

ChangeSet
  1.4 05/10/06 12:55:38 user@host.(none) +1 -0
  Remove foo.txt.

  BitKeeper/deleted/.del-foo.txt~8259385fddc312ca
    1.2 05/10/06 12:55:24 user@host.(none) +0 -0
    Delete: foo.txt

  bar.txt
    1.1 05/10/06 12:55:07 user@host.(none) +1 -0
    Add another file.

ChangeSet
  1.3 05/10/06 12:55:07 user@host.(none) +1 -0
  Add another file.

  bar.txt
    1.0 05/10/06 12:55:07 user@host.(none) +0 -0
    BitKeeper file /var/lib/bk/demo/dev-1.0/bar.txt

ChangeSet
  1.2 05/10/06 12:53:09 user@host.(none) +1 -0
  Add sample file.

  foo.txt
    1.1 05/10/06 12:52:55 user@host.(none) +1 -0

  foo.txt
    1.0 05/10/06 12:52:55 user@host.(none) +0 -0
    BitKeeper file /var/lib/bk/demo/dev-1.0/foo.txt

ChangeSet
  1.1 05/10/06 12:52:28 user@host.(none) +2 -0
  Initial repository create

  BitKeeper/etc/ignore
    1.1 05/10/06 12:52:28 user@host.(none) +2 -0

  BitKeeper/etc/config
    1.1 05/10/06 12:52:28 user@host.(none) +22 -0

ChangeSet
  1.0 05/10/06 12:52:28 user@host.(none) +0 -0
  BitKeeper file /var/lib/bk/demo/dev-1.0/ChangeSet

  BitKeeper/etc/ignore
    1.0 05/10/06 12:52:28 user@host.(none) +0 -0
    BitKeeper file /var/lib/bk/demo/dev-1.0/BitKeeper/etc/ignore

  BitKeeper/etc/config
    1.0 05/10/06 12:52:28 user@host.(none) +0 -0
    BitKeeper file /var/lib/bk/demo/dev-1.0/BitKeeper/etc/config
";
			}

			public static string NonVerboseBitKeeperOutput()
			{
				return @"ChangeSet@1.6, 2005-10-06 12:58:40-07:00, user@host.(none)
  Remove file in subdir.

ChangeSet@1.5, 2005-10-06 12:58:11-07:00, user@host.(none)
  Another new one.

ChangeSet@1.4, 2005-10-06 12:55:38-07:00, user@host.(none)
  Remove foo.txt.

ChangeSet@1.3, 2005-10-06 12:55:07-07:00, user@host.(none)
  Add another file.

ChangeSet@1.2, 2005-10-06 12:53:09-07:00, user@host.(none)
  Add sample file.

ChangeSet@1.1, 2005-10-06 12:52:28-07:00, user@host.(none)
  Initial repository create

ChangeSet@1.0, 2005-10-06 12:52:28-07:00, user@host.(none)
  BitKeeper file /var/lib/bk/demo/dev-1.0/ChangeSet ";
			}
		}
	}
}
