using System;
using System.Collections;
using System.Globalization;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class RobocopyHistoryParserTest : CustomAssertion
	{
		public static DateTime OLDEST_ENTRY = DateTime.Parse("2002/03/13 19:38:42");
		public static DateTime NEWEST_ENTRY = DateTime.Parse("2002/03/15 19:20:28");

		private RobocopyHistoryParser parser = new RobocopyHistoryParser();

        private static string windowsPath = @"E:\copytest";
        private string path = Platform.IsWindows ? windowsPath : @"/copytest";

		[Test]
		public void ParseNoChange()
		{
			Modification[] modifications = parser.Parse(new StringReader(""), OLDEST_ENTRY, NEWEST_ENTRY);

			Assert.AreEqual(0, modifications.Length);
		}

		[Test]
		public void ParseChanges()
		{
			Modification[] modifications = parser.Parse(new StringReader(LOG_CONTENT), OLDEST_ENTRY, NEWEST_ENTRY);

			Assert.AreEqual(8, modifications.Length);

			Modification mod0 = new Modification();
			mod0.Type = "deleted";
			mod0.FolderName = Path.Combine(path, "dst", "dir2");

			Assert.AreEqual(modifications[0], mod0);

			Modification mod1 = new Modification();
			mod1.Type = "deleted";
			mod1.FileName = "deleted.txt";
			mod1.FolderName = Path.Combine(path, "dst", "dir2");

			Assert.AreEqual(modifications[1], mod1);

			Modification mod2 = new Modification();
			mod2.Type = "deleted";
			mod2.FileName = "delete.txt";
			mod2.FolderName = Path.Combine(path, "dst");

			Assert.AreEqual(modifications[2], mod2);

			Modification mod3 = new Modification();
			mod3.Type = "added";
			mod3.FileName = "file2.txt";
			mod3.FolderName = Path.Combine(path, "src");
			mod3.ModifiedTime = CreateDate("2008/02/06 09:16:49");

			Assert.AreEqual(modifications[3], mod3);

			Modification mod4 = new Modification();
			mod4.Type = "modified";
			mod4.FileName = "file3.txt";
			mod4.FolderName = Path.Combine(path, "src");
			mod4.ModifiedTime = CreateDate("2008/02/06 09:35:50");

			Assert.AreEqual(modifications[4], mod4);

			Modification mod5 = new Modification();
			mod5.Type = "added";
			mod5.FileName = "file";
			mod5.FolderName = Path.Combine(path, "src", "dir with a space");
			mod5.ModifiedTime = CreateDate("2008/02/07 07:55:32");

			Assert.AreEqual(modifications[5], mod5);

			Modification mod6 = new Modification();
			mod6.Type = "added";
			mod6.FileName = "file with a space.txt";
			mod6.FolderName = Path.Combine(path, "src", "dir with a space");
			mod6.ModifiedTime = CreateDate("2008/02/07 07:55:38");

			Assert.AreEqual(modifications[6], mod6);

			Modification mod7 = new Modification();
			mod7.Type = "added";
			mod7.FileName = "file1.txt";
			mod7.FolderName = Path.Combine(path, "src", "dir1");
			mod7.ModifiedTime = CreateDate("2008/02/06 09:31:26");

			Assert.AreEqual(modifications[7], mod7);
		}


		private DateTime CreateDate(string dateString)
		{
			DateTime date = DateTime.ParseExact(dateString, "yyyy/MM/dd HH:mm:ss", DateTimeFormatInfo.GetInstance(CultureInfo.InvariantCulture));
			return date;
		}

		// We require these switches...
		// /V /L /MIR /X /TS /FP /NDL /NP /NS /NJH /NJS

		public string LOG_CONTENT { get { return
@"
	*EXTRA Dir  	E:\copytest\dst\dir2\
	  *EXTRA File 		 2008/02/06 09:31:37	E:\copytest\dst\dir2\deleted.txt
	  *EXTRA File 		 2008/02/06 09:17:22	E:\copytest\dst\delete.txt
	          same		 2008/02/06 09:15:49	E:\copytest\src\file1.txt
	    New File  		 2008/02/06 09:16:49	E:\copytest\src\file2.txt
	    Newer     		 2008/02/06 09:35:50	E:\copytest\src\file3.txt
	    New File  		 2008/02/07 07:55:32	E:\copytest\src\dir with a space\file
	    New File  		 2008/02/07 07:55:38	E:\copytest\src\dir with a space\file with a space.txt
	    New File  		 2008/02/06 09:31:26	E:\copytest\src\dir1\file1.txt
".Replace(windowsPath, path).Replace('\\', Path.DirectorySeparatorChar);
            }
        }
	}
}

