using System;
using System.Globalization;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class StarTeamHistoryParserTest : CustomAssertion 
	{
		StarTeamHistoryParser _parser;
		DateTimeFormatInfo _dfi;

		private readonly DateTime OLDEST_ENTRY = DateTime.Parse("1996-Sep-13 13:34:52");
		private readonly DateTime NEWEST_ENTRY = DateTime.Parse("2005-Sep-13 13:34:52");

		[SetUp]
		public void SetUp()
		{
			_parser = new StarTeamHistoryParser(new StarTeam());
			_dfi = new DateTimeFormatInfo();
			_dfi.AMDesignator = "AM";
			_dfi.PMDesignator = "PM";
			_dfi.MonthDayPattern = @"M/d/yy h:mm:ss tt";
		}

		[Test]
		public void TestModificationCount()
		{
			Modification [] mod = _parser.Parse(StarTeamHistoryParserTest.ContentReader, OLDEST_ENTRY, NEWEST_ENTRY);
			Assert.AreEqual(4, mod.Length);
		}

		[Test]
		public void TestModificationContent()
		{
			_parser.Culture = new CultureInfo("en-US");
			Modification[] actual = _parser.Parse(StarTeamHistoryParserTest.ContentReader, OLDEST_ENTRY, NEWEST_ENTRY);
			Modification[] expected = getExpectedModifications();

			AssertEqualArrays(actual, expected);
		}

		#region Test data: Test log content

		public static String StarTeamLogContent
		{
			get
			{
				return @"Folder: CC.NET  (working dir: D:\Projects\CC.NET)
History for: test01.txt
Description: test file 1: initial check-in
Locked by:
Status: Missing
----------------------------
Revision: 1 View: .NET Lab Branch Revision: 1.0
Author: Ahsanul Zaki Date: 12/9/02 10:33:20 AM PST
=============================================================================

History for: test02.txt
Description: test file 2: initial check-in
Locked by:
Status: Out of Date
----------------------------
Revision: 2 View: .NET Lab Branch Revision: 1.1
Author: Ahsanul Zaki Date: 12/9/02 10:49:44 AM PST
added line 1

----------------------------
Revision: 1 View: .NET Lab Branch Revision: 1.0
Author: Ahsanul Zaki Date: 12/9/02 10:33:36 AM PST
added line 2
=============================================================================

Folder: ccnet  (working dir: D:\Projects\CC.NET\ccnet)
Folder: ccnet-config  (working dir: D:\Projects\CC.NET\ccnet\ccnet-config)
Folder: doc  (working dir: D:\Projects\CC.NET\ccnet\doc)
Folder: images  (working dir: D:\Projects\CC.NET\ccnet\doc\images)
Folder: etc  (working dir: D:\Projects\CC.NET\ccnet\etc)
Folder: pvcs  (working dir: D:\Projects\CC.NET\ccnet\etc\pvcs)
Folder: install  (working dir: D:\Projects\CC.NET\ccnet\install)
Folder: lib  (working dir: D:\Projects\CC.NET\ccnet\lib)
Folder: project  (working dir: D:\Projects\CC.NET\ccnet\project)
Folder: acceptance  (working dir: D:\Projects\CC.NET\ccnet\project\acceptance)
Folder: bin  (working dir: D:\Projects\CC.NET\ccnet\project\acceptance\bin)
Folder: Debug  (working dir: D:\Projects\CC.NET\ccnet\project\acceptance\bin\Debug)
Folder: core  (working dir: D:\Projects\CC.NET\ccnet\project\acceptance\core)
Folder: obj  (working dir: D:\Projects\CC.NET\ccnet\project\acceptance\obj)
Folder: Debug  (working dir: D:\Projects\CC.NET\ccnet\project\acceptance\obj\Debug)
Folder: temp  (working dir: D:\Projects\CC.NET\ccnet\project\acceptance\obj\Debug\temp)
Folder: TempPE  (working dir: D:\Projects\CC.NET\ccnet\project\acceptance\obj\Debug\TempPE)
Folder: web  (working dir: D:\Projects\CC.NET\ccnet\project\acceptance\web)
Folder: console  (working dir: D:\Projects\CC.NET\ccnet\project\console)
Folder: bin  (working dir: D:\Projects\CC.NET\ccnet\project\console\bin)
Folder: Debug  (working dir: D:\Projects\CC.NET\ccnet\project\console\bin\Debug)

Folder: obj  (working dir: D:\Projects\CC.NET\ccnet\project\console\obj)
Folder: Debug  (working dir: D:\Projects\CC.NET\ccnet\project\console\obj\Debug)

Folder: DC.NET  (working dir: D:\Projects\DC.NET)
History for: akz01.gif
Description: fake file
Locked by:
Status: Missing
----------------------------
Revision: 1 View: .NET Lab Branch Revision: 1.0
Author: Ahsanul Zaki Date: 12/9/02 10:33:20 AM PST
=============================================================================

Folder: temp  (working dir: D:\Projects\CC.NET\ccnet\project\console\obj\Debug\temp)
Folder: TempPE  (working dir: D:\Projects\CC.NET\ccnet\project\console\obj\Debug\TempPE)
Folder: test  (working dir: D:\Projects\CC.NET\ccnet\project\console\test)
Folder: core  (working dir: D:\Projects\CC.NET\ccnet\project\core)
Folder: bin  (working dir: D:\Projects\CC.NET\ccnet\project\core\bin)
Folder: Debug  (working dir: D:\Projects\CC.NET\ccnet\project\core\bin\Debug)
Folder: Release  (working dir: D:\Projects\CC.NET\ccnet\project\core\bin\Release)
Folder: obj  (working dir: D:\Projects\CC.NET\ccnet\project\core\obj)
Folder: Debug  (working dir: D:\Projects\CC.NET\ccnet\project\core\obj\Debug)
Folder: temp  (working dir: D:\Projects\CC.NET\ccnet\project\core\obj\Debug\temp)
Folder: TempPE  (working dir: D:\Projects\CC.NET\ccnet\project\core\obj\Debug\TempPE)
Folder: Release  (working dir: D:\Projects\CC.NET\ccnet\project\core\obj\Release)
Folder: temp  (working dir: D:\Projects\CC.NET\ccnet\project\core\obj\Release\temp)
Folder: TempPE  (working dir: D:\Projects\CC.NET\ccnet\project\core\obj\Release\TempPE)
Folder: publishers  (working dir: D:\Projects\CC.NET\ccnet\project\core\publishers)
Folder: test  (working dir: D:\Projects\CC.NET\ccnet\project\core\publishers\test)
Folder: sourcecontrol  (working dir: D:\Projects\CC.NET\ccnet\project\core\sourcecontrol)
Folder: test  (working dir: D:\Projects\CC.NET\ccnet\project\core\sourcecontrol\test)
Folder: summary  (working dir: D:\Projects\CC.NET\ccnet\project\core\summary)
Folder: test  (working dir: D:\Projects\CC.NET\ccnet\project\core\summary\test)
Folder: test  (working dir: D:\Projects\CC.NET\ccnet\project\core\test)
Folder: util  (working dir: D:\Projects\CC.NET\ccnet\project\core\util)
Folder: test  (working dir: D:\Projects\CC.NET\ccnet\project\core\util\test)
Folder: install.server  (working dir: D:\Projects\CC.NET\ccnet\project\install.server)
Folder: Debug  (working dir: D:\Projects\CC.NET\ccnet\project\install.server\Debug)
Folder: Release  (working dir: D:\Projects\CC.NET\ccnet\project\install.server\Release)
Folder: install.web  (working dir: D:\Projects\CC.NET\ccnet\project\install.web)

Folder: Debug  (working dir: D:\Projects\CC.NET\ccnet\project\install.web\Debug)

Folder: Release  (working dir: D:\Projects\CC.NET\ccnet\project\install.web\Release)
Folder: web  (working dir: D:\Projects\CC.NET\ccnet\project\web)
Folder: bin  (working dir: D:\Projects\CC.NET\ccnet\project\web\bin)
Folder: Debug  (working dir: D:\Projects\CC.NET\ccnet\project\web\bin\Debug)
Folder: Release  (working dir: D:\Projects\CC.NET\ccnet\project\web\bin\Release)

Folder: images  (working dir: D:\Projects\CC.NET\ccnet\project\web\images)
Folder: log  (working dir: D:\Projects\CC.NET\ccnet\project\web\log)
Folder: obj  (working dir: D:\Projects\CC.NET\ccnet\project\web\obj)
Folder: Debug  (working dir: D:\Projects\CC.NET\ccnet\project\web\obj\Debug)
Folder: temp  (working dir: D:\Projects\CC.NET\ccnet\project\web\obj\Debug\temp)

Folder: TempPE  (working dir: D:\Projects\CC.NET\ccnet\project\web\obj\Debug\TempPE)
Folder: Release  (working dir: D:\Projects\CC.NET\ccnet\project\web\obj\Release)

Folder: temp  (working dir: D:\Projects\CC.NET\ccnet\project\web\obj\Release\temp)
Folder: TempPE  (working dir: D:\Projects\CC.NET\ccnet\project\web\obj\Release\TempPE)
Folder: test  (working dir: D:\Projects\CC.NET\ccnet\project\web\test)
Folder: xsl  (working dir: D:\Projects\CC.NET\ccnet\project\web\xsl)
Folder: tools  (working dir: D:\Projects\CC.NET\ccnet\tools)
Folder: nant  (working dir: D:\Projects\CC.NET\ccnet\tools\nant)
Folder: NUnitAsp  (working dir: D:\Projects\CC.NET\ccnet\tools\NUnitAsp)
Folder: dtd  (working dir: D:\Projects\CC.NET\ccnet\tools\NUnitAsp\dtd)

Folder: DD.NET  (working dir: D:\Projects\DD.NET)
History for: akz last.gif
Description: last fake file
Locked by: nobody
Status: Missing
----------------------------
Revision: 1 View: .NET Lab Branch Revision: 1.0
Author: Ahsanul Zaki Date: 12/9/02 10:33:20 AM PST
fake test file
for Star team
=============================================================================
";
			}
		}

		public static TextReader ContentReader
		{
			get
			{
				return new StringReader(StarTeamLogContent);
			}
		}

		private Modification[] getExpectedModifications()
		{
			Modification [] mod = new Modification[4];
			mod[0] = new Modification();
			mod[0].Comment = @"";
			mod[0].EmailAddress = @"N/A";
			mod[0].FileName = @"test01.txt";
			mod[0].FolderName = @"D:\Projects\CC.NET";
			mod[0].ModifiedTime = DateTime.Parse("12/9/02 10:33:20 AM",	_dfi);
			mod[0].Type = " Missing";
			mod[0].UserName = "Ahsanul Zaki";

			mod[1] = new Modification();
			mod[1].Comment = @"added line 1

";
			mod[1].EmailAddress = @"N/A";
			mod[1].FileName = @"test02.txt";
			mod[1].FolderName = @"D:\Projects\CC.NET";
			mod[1].ModifiedTime = DateTime.Parse("12/9/02 10:49:44 AM",	_dfi);
			mod[1].Type = " Out of Date";
			mod[1].UserName = "Ahsanul Zaki";

			mod[2] = new Modification();
			mod[2].Comment = @"";
			mod[2].EmailAddress = @"N/A";
			mod[2].FileName = @"akz01.gif";
			mod[2].FolderName = @"D:\Projects\DC.NET";
			mod[2].ModifiedTime = DateTime.Parse("12/9/02 10:33:20 AM",	_dfi);
			mod[2].Type = " Missing";
			mod[2].UserName = "Ahsanul Zaki";

			mod[3] = new Modification();
			mod[3].Comment = "fake test file" + Environment.NewLine + "for Star team\n";
			mod[3].EmailAddress = @"N/A";
			mod[3].FileName = @"akz last.gif";
			mod[3].FolderName = @"D:\Projects\DD.NET";
			mod[3].ModifiedTime = DateTime.Parse("12/9/02 10:33:20 AM",	_dfi);
			mod[3].Type = " Missing";
			mod[3].UserName = "Ahsanul Zaki";
			return mod;
		}


		#endregion
	}
}

