using System;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class SurroundHistoryParserTest : CustomAssertion
	{
		private SurroundHistoryParser parser;

		private readonly DateTime OLDEST_ENTRY = DateTime.Parse("2005-Sep-26 10:27:13");
		private readonly DateTime NEWEST_ENTRY = DateTime.Parse("2005-Sep-30 15:29:08");

		[SetUp]
		public void SetUp()
		{
			parser = new SurroundHistoryParser();
		}

		[Test]
		public void VerifyAllModificationsAreParsedSuccessfully()
		{
			Modification[] actual = parser.Parse(SurroundHistoryParserTest.ContentReader, OLDEST_ENTRY, NEWEST_ENTRY);
			Assert.AreEqual(8, actual.Length);
			AssertEqualArrays(ExpectedModifications(), actual);
		}

		public static String SampleSscmCCOutput
		{
			get { return @"total-8
<m20040908/scctt3><App.ico><1><Add><20050926102713><new icon><artist><artist@example.com>
<m20040908/scctt3><AssemblyInfo.cs><7><Check in><20050930152820><changes for defect #1234><admin><admin@example.com>
<m20040908/scctt3><Form1.cs><14><Check in><20050930152838><fixed build error><build><build@example.com>
<m20040908/scctt3><Form1.resx><1><Add><20050926102713><><admin><admin@example.com>
<m20040908/scctt3><scctt3.csproj><3><Check in><20050930152854><new icon><artist><artist@example.com>
<m20040908/scctt3><scctt3.csproj.vspscc><1><Add><20050926102713><><admin><admin@example.com>
<m20040908/scctt3><scctt3.sln><3><Check in><20050930152908><new icon><artist><artist@example.com>
<m20040908/scctt3><scctt3.vssscc><1><Add><20050926102713><><admin><admin@example.com>
"; }
		}

		public static TextReader ContentReader
		{
			get { return new StringReader(SampleSscmCCOutput); }
		}

		private Modification[] ExpectedModifications()
		{
			Modification[] mod = new Modification[8];

			// <m20040908/scctt3><App.ico><1><Add><20050926102713><new icon><artist><artist@example.com>
			mod[0] = new Modification();
			mod[0].FolderName = @"m20040908/scctt3";
			mod[0].FileName = @"App.ico";
			mod[0].ChangeNumber = 1;
			mod[0].Type = @"Add";
			mod[0].ModifiedTime = DateTime.Parse("2005-Sep-26 10:27:13");
			mod[0].Comment = @"new icon";
			mod[0].UserName = @"artist";
			mod[0].EmailAddress = @"artist@example.com";

			// <m20040908/scctt3><AssemblyInfo.cs><7><Check in><20050930152820><changes for defect #1234><admin><admin@example.com>
			mod[1] = new Modification();
			mod[1].FolderName = @"m20040908/scctt3";
			mod[1].FileName = @"AssemblyInfo.cs";
			mod[1].ChangeNumber = 7;
			mod[1].Type = @"Check in";
			mod[1].ModifiedTime = DateTime.Parse("2005-Sep-30 15:28:20");
			mod[1].Comment = @"changes for defect #1234";
			mod[1].UserName = @"admin";
			mod[1].EmailAddress = @"admin@example.com";

			// <m20040908/scctt3><Form1.cs><14><Check in><20050930152838><fixed build error><build><build@example.com>
			mod[2] = new Modification();
			mod[2].FolderName = @"m20040908/scctt3";
			mod[2].FileName = @"Form1.cs";
			mod[2].ChangeNumber = 14;
			mod[2].Type = @"Check in";
			mod[2].ModifiedTime = DateTime.Parse("2005-Sep-30 15:28:38");
			mod[2].Comment = @"fixed build error";
			mod[2].UserName = @"build";
			mod[2].EmailAddress = @"build@example.com";

			// <m20040908/scctt3><Form1.resx><1><Add><20050926102713><><admin><admin@example.com>
			mod[3] = new Modification();
			mod[3].FolderName = @"m20040908/scctt3";
			mod[3].FileName = @"Form1.resx";
			mod[3].ChangeNumber = 1;
			mod[3].Type = @"Add";
			mod[3].ModifiedTime = DateTime.Parse("2005-Sep-26 10:27:13");
			mod[3].Comment = @"";
			mod[3].UserName = @"admin";
			mod[3].EmailAddress = @"admin@example.com";

			// <m20040908/scctt3><scctt3.csproj><3><Check in><20050930152854><new icon><artist><artist@example.com>
			mod[4] = new Modification();
			mod[4].FolderName = @"m20040908/scctt3";
			mod[4].FileName = @"scctt3.csproj";
			mod[4].ChangeNumber = 3;
			mod[4].Type = @"Check in";
			mod[4].ModifiedTime = DateTime.Parse("2005-Sep-30 15:28:54");
			mod[4].Comment = @"new icon";
			mod[4].UserName = @"artist";
			mod[4].EmailAddress = @"artist@example.com";

			// <m20040908/scctt3><scctt3.csproj.vspscc><1><Add><20050926102713><><admin><admin@example.com>
			mod[5] = new Modification();
			mod[5].FolderName = @"m20040908/scctt3";
			mod[5].FileName = @"scctt3.csproj.vspscc";
			mod[5].ChangeNumber = 1;
			mod[5].Type = @"Add";
			mod[5].ModifiedTime = DateTime.Parse("2005-Sep-26 10:27:13");
			mod[5].Comment = @"";
			mod[5].UserName = @"admin";
			mod[5].EmailAddress = @"admin@example.com";

			// <m20040908/scctt3><scctt3.sln><3><Check in><20050930152908><new icon><artist><artist@example.com>
			mod[6] = new Modification();
			mod[6].FolderName = @"m20040908/scctt3";
			mod[6].FileName = @"scctt3.sln";
			mod[6].ChangeNumber = 3;
			mod[6].Type = @"Check in";
			mod[6].ModifiedTime = DateTime.Parse("2005-Sep-30 15:29:08");
			mod[6].Comment = @"new icon";
			mod[6].UserName = @"artist";
			mod[6].EmailAddress = @"artist@example.com";

			// <m20040908/scctt3><scctt3.vssscc><1><Add><20050926102713><><admin><admin@example.com>
			mod[7] = new Modification();
			mod[7].FolderName = @"m20040908/scctt3";
			mod[7].FileName = @"scctt3.vssscc";
			mod[7].ChangeNumber = 1;
			mod[7].Type = @"Add";
			mod[7].ModifiedTime = DateTime.Parse("2005-Sep-26 10:27:13");
			mod[7].Comment = @"";
			mod[7].UserName = @"admin";
			mod[7].EmailAddress = @"admin@example.com";

			return mod;
		}
	}
}