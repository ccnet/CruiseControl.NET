using System;
using System.IO;
using System.Globalization;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test 
{
	[TestFixture]
	public class PvcsHistoryParserTest : CustomAssertion
	{
		private PvcsHistoryParser _pvcs = new PvcsHistoryParser();

		[Test]
		public void ParseStream()
		{
			TextReader input = new StringReader(PvcsMother.LOGFILE_CONTENT);
			Modification[] modifications = _pvcs.Parse(input, PvcsMother.OLDEST_ENTRY, PvcsMother.NEWEST_ENTRY);
			
			input.Close();

			AssertEquals("Should have returned 2 modifications.", 2, modifications.Length);

			Modification mod1 = new Modification();
			mod1.Type = PvcsHistoryParser.UNKNOWN;
			mod1.FileName = "foo.txt";
			mod1.FolderName = "piddy dee";
			mod1.ModifiedTime = CreateDate("0001/01/01 00:00:00");
			mod1.UserName = "dante";
			mod1.Comment = "made a first hello world comment";

			Modification mod2 = new Modification();
			mod2.Type = PvcsHistoryParser.UNKNOWN;
			mod2.FileName = "bar.txt";
			mod2.FolderName = "raise the";
			mod1.ModifiedTime = CreateDate("0001/01/01 00:00:00");
			mod2.UserName = "virgil";
			mod2.Comment = "made a second hello world comment";
		
			AssertEquals("1", mod1, modifications[0]);
			AssertEquals("2", mod2, modifications[1]);
		}
		
		[Test]
		public void ExtendedLogFileContent()
		{
			TextReader input = new StringReader(PvcsMother.EXTENDED_LOGFILE_CONTENT);
			Modification[] modifications = _pvcs.Parse(input, PvcsMother.OLDEST_ENTRY, PvcsMother.NEWEST_ENTRY);
			AssertEquals(21, modifications.Length);
			
			Modification second = modifications[1];
			AssertEquals("ChessRules.java", second.FileName);
			AssertEquals("kerstinb", second.UserName);
			AssertEquals("Enabled system printouts.", second.Comment);
			AssertEquals(CreateDate("2000/02/01 16:26:14"), second.ModifiedTime);
			AssertEquals(
				@"D:\root\PVCS\vm\common\SampleDB\archives\chess\client\ChessRules.java-arc",
				second.FolderName);

			Modification third = modifications[2];
			AssertEquals("chessviewer.html", third.FileName);
			AssertEquals(CreateDate("1998/05/18 04:46:38"), third.ModifiedTime);

		}

		private DateTime CreateDate(string dateString) 
		{
			DateTime date = DateTime.ParseExact(dateString,"yyyy/MM/dd HH:mm:ss",DateTimeFormatInfo.GetInstance(CultureInfo.InvariantCulture));
			return date;
		}
	}
}