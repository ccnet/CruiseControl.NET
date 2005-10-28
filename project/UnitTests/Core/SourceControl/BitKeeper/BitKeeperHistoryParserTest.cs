using System;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.BitKeeper;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol.Bitkeeper
{
	[TestFixture]
	public class BitKeeperHistoryParserTest
	{
		[Test]
		public void ParseVerboseModifications()
		{
			BitKeeperHistoryParser parser = new BitKeeperHistoryParser();
			Modification[] mods = parser.Parse(new StringReader(BitKeeperTestData.VerboseBitKeeperOutput()), DateTime.Now, DateTime.Now);
			Assert.AreEqual(19, mods.Length);
			foreach (Modification mod in mods)
			{
				Assert.AreEqual("hunth@survivor.(none)", mod.UserName);
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
				Assert.AreEqual("hunth@survivor.(none)", mod.UserName);
				Assert.IsNotNull("filename should not be null", mod.FileName);
			}
		}

		class BitKeeperTestData
		{
			public static string VerboseBitKeeperOutput()
			{
				return @"ChangeSet
  1.6 05/10/06 12:58:40 hunth@survivor.(none) +1 -0
  Remove file in subdir.

  BitKeeper/deleted/.del-baz.txt~7545e8a43744d49
    1.2 05/10/06 12:58:27 hunth@survivor.(none) +0 -0
    Delete: asubdir/baz.txt

  asubdir/baz.txt
    1.1 05/10/06 12:58:11 hunth@survivor.(none) +1 -0
    Another new one.

ChangeSet
  1.5 05/10/06 12:58:11 hunth@survivor.(none) +1 -0
  Another new one.

  asubdir/baz.txt
    1.0 05/10/06 12:58:11 hunth@survivor.(none) +0 -0
    BitKeeper file /var/lib/bk/demo/dev-1.0/asubdir/baz.txt

ChangeSet
  1.4 05/10/06 12:55:38 hunth@survivor.(none) +1 -0
  Remove foo.txt.

  BitKeeper/deleted/.del-foo.txt~8259385fddc312ca
    1.2 05/10/06 12:55:24 hunth@survivor.(none) +0 -0
    Delete: foo.txt

  bar.txt
    1.1 05/10/06 12:55:07 hunth@survivor.(none) +1 -0
    Add another file.

ChangeSet
  1.3 05/10/06 12:55:07 hunth@survivor.(none) +1 -0
  Add another file.

  bar.txt
    1.0 05/10/06 12:55:07 hunth@survivor.(none) +0 -0
    BitKeeper file /var/lib/bk/demo/dev-1.0/bar.txt

ChangeSet
  1.2 05/10/06 12:53:09 hunth@survivor.(none) +1 -0
  Add sample file.

  foo.txt
    1.1 05/10/06 12:52:55 hunth@survivor.(none) +1 -0

  foo.txt
    1.0 05/10/06 12:52:55 hunth@survivor.(none) +0 -0
    BitKeeper file /var/lib/bk/demo/dev-1.0/foo.txt

ChangeSet
  1.1 05/10/06 12:52:28 hunth@survivor.(none) +2 -0
  Initial repository create

  BitKeeper/etc/ignore
    1.1 05/10/06 12:52:28 hunth@survivor.(none) +2 -0

  BitKeeper/etc/config
    1.1 05/10/06 12:52:28 hunth@survivor.(none) +22 -0

ChangeSet
  1.0 05/10/06 12:52:28 hunth@survivor.(none) +0 -0
  BitKeeper file /var/lib/bk/demo/dev-1.0/ChangeSet

  BitKeeper/etc/ignore
    1.0 05/10/06 12:52:28 hunth@survivor.(none) +0 -0
    BitKeeper file /var/lib/bk/demo/dev-1.0/BitKeeper/etc/ignore

  BitKeeper/etc/config
    1.0 05/10/06 12:52:28 hunth@survivor.(none) +0 -0
    BitKeeper file /var/lib/bk/demo/dev-1.0/BitKeeper/etc/config
";
			}

			public static string NonVerboseBitKeeperOutput()
			{
				return @"ChangeSet@1.6, 2005-10-06 12:58:40-07:00, hunth@survivor.(none)
  Remove file in subdir.

ChangeSet@1.5, 2005-10-06 12:58:11-07:00, hunth@survivor.(none)
  Another new one.

ChangeSet@1.4, 2005-10-06 12:55:38-07:00, hunth@survivor.(none)
  Remove foo.txt.

ChangeSet@1.3, 2005-10-06 12:55:07-07:00, hunth@survivor.(none)
  Add another file.

ChangeSet@1.2, 2005-10-06 12:53:09-07:00, hunth@survivor.(none)
  Add sample file.

ChangeSet@1.1, 2005-10-06 12:52:28-07:00, hunth@survivor.(none)
  Initial repository create

ChangeSet@1.0, 2005-10-06 12:52:28-07:00, hunth@survivor.(none)
  BitKeeper file /var/lib/bk/demo/dev-1.0/ChangeSet ";
			}
		}
	}
}
