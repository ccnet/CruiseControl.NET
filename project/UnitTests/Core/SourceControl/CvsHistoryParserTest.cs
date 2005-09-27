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
	public class CvsHistoryParserTest : CustomAssertion
	{
		private CvsHistoryParser cvs = new CvsHistoryParser();

		[Test]
		public void ParseStream() 
		{
			TextReader input = new StringReader(CvsMother.CVS_LOGFILE_CONTENT);
			Modification[] modifications = cvs.Parse(input, CvsMother.OLDEST_ENTRY, CvsMother.NEWEST_ENTRY);
			
			Assert.AreEqual(6, modifications.Length);

			Modification mod1 = new Modification();
			mod1.Type = "modified";
			mod1.FileName = "log4j.properties";
			mod1.FolderName = "foobar";
			mod1.ModifiedTime = CreateDate("2002/03/13 13:45:50 -6");
			mod1.UserName = "alden";
			mod1.Comment = "Shortening ConversionPattern so we don't use up all of the available screen space.";

			Modification mod2 = new Modification();
			mod2.Type = "modified";
			mod2.FileName = "build.xml";
			mod2.FolderName = "main";
			mod2.ModifiedTime = CreateDate("2002/03/13 19:56:34 -6");
			mod2.UserName = "alden";
			mod2.Comment = "Added target to clean up test results.";

			Modification mod3 = new Modification();
			mod3.Type = "modified";
			mod3.FileName = "build.xml";
			mod3.FolderName = "main";
			mod3.ModifiedTime = CreateDate("2002/03/15 13:20:28 -6");
			mod3.UserName = "alden";
			mod3.Comment = "enabled debug info when compiling tests.";

			Modification mod4 = new Modification();
			mod4.Type = "deleted";
			mod4.FileName = "kungfu.xml";
			mod4.FolderName = "main";
			mod4.ModifiedTime = CreateDate("2002/03/13 13:45:42 -6");
			mod4.UserName = "alden";
			mod4.Comment = "Hey, look, a deleted file.";

			Modification mod5 = new Modification();
			mod5.Type = "deleted";
			mod5.FileName = "stuff.xml";
			mod5.FolderName = "main";
			mod5.ModifiedTime = CreateDate("2002/03/13 13:38:42 -6");
			mod5.UserName = "alden";
			mod5.Comment = "Hey, look, another deleted file.";
		
			Modification mod6 = new Modification();
			mod6.Type = "modified";
			mod6.FileName = "File.cs";
			mod6.FolderName = "Project";
			mod6.ModifiedTime = CreateDate("2005/09/12 15:01:10 +0");
			mod6.UserName = "szko";
			mod6.Comment = "Fixed some bugs.";

			ArrayList.Adapter(modifications).Sort();
			Assert.AreEqual(mod5, modifications[0]);
			Assert.AreEqual(mod4, modifications[1]);
			Assert.AreEqual(mod1, modifications[2]);
			Assert.AreEqual(mod2, modifications[3]);
			Assert.AreEqual(mod3, modifications[4]);
			Assert.AreEqual(mod6, modifications[5]);
		}

		[Test]
		public void ParseCvs112Examples() 
		{
			TextReader input = new StringReader(CvsMother.Cvs112Examples());
			cvs.Parse(input, CvsMother.OLDEST_ENTRY, CvsMother.NEWEST_ENTRY);
		}

		private DateTime CreateDate(string dateString) 
		{
			DateTime date = DateTime.ParseExact(dateString,"yyyy/MM/dd HH:mm:ss z",DateTimeFormatInfo.GetInstance(CultureInfo.InvariantCulture));
			return date;
		}
	}
}

