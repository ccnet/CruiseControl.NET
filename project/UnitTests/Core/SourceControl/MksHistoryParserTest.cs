using System;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class MksHistoryParserTest
	{
		#region Test data

		public const string REVISION_TEST_DATA = @"
Revision changed: myproject.build was 1.28 changed to 1.29
Revision changed: Primary Functional Tests/Summary Page.html was 1.8 changed to 1.9
Revision changed: src/Common/MyProject.Common.DTO/Page/PageItem.cs was 1.5 changed to 1.6
Subproject changed: e:/MyProject/mySubProject.pj was 1.3 changed to working project
  Revision changed: mySubProject/extensions.js was 1.2 changed to 1.5	   
";

		public const string ADDED_TEST_DATA = @"
Added member: lib/Oracle.DataAccess.dll now at 1.1
";

		public const string DELETED_TEST_DATA = @"
Deleted member: src/AuthenticationService/AuthenticationService.asmx was at 1.3
";

		public const string MEMBER_INFO = @"
Member Name: c:\MyProject\myproject.build
Sandbox Name: c:\MyProject\myproject.pj
Development Branch: 1
Member Revision: 1.29
    Created By: ccnetuser on Aug 26, 2005 - 5:32 AM
    State: Exp
    Revision Description:
        just added a comment
		continued comment in another line
    Labels:
        Build - MyProject_3
Attributes: none
";

		#endregion

		[Test]
		public void ParseOnlyRevisions()
		{
			MksHistoryParser parser = new MksHistoryParser();
			Modification[] modifications = parser.Parse(new StringReader(REVISION_TEST_DATA), DateTime.Now, DateTime.Now);
			Assert.AreEqual(3, modifications.Length);
			Assert.AreEqual("myproject.build", modifications[0].FileName);
			Assert.AreEqual("src/Common/MyProject.Common.DTO/Page", modifications[2].FolderName);

			//Tests for checking the file and folder names with blank spaces
			Assert.AreEqual("Summary Page.html", modifications[1].FileName);
			Assert.AreEqual("Primary Functional Tests", modifications[1].FolderName);

			Assert.AreEqual("1.29", modifications[0].Version);
			Assert.AreEqual("Modified", modifications[0].Type);
		}

		[Test]
		public void ParseOnlyAdded()
		{
			MksHistoryParser parser = new MksHistoryParser();
			Modification[] modifications = parser.Parse(new StringReader(ADDED_TEST_DATA), DateTime.Now, DateTime.Now);
			Assert.AreEqual(1, modifications.Length);
			Assert.AreEqual("Oracle.DataAccess.dll", modifications[0].FileName);
			Assert.AreEqual("lib", modifications[0].FolderName);
			Assert.AreEqual("1.1", modifications[0].Version);
			Assert.AreEqual("Added", modifications[0].Type);
		}

		[Test]
		public void ParseOnlyDeleted()
		{
			MksHistoryParser parser = new MksHistoryParser();
			Modification[] modifications = parser.Parse(new StringReader(DELETED_TEST_DATA), DateTime.Now, DateTime.Now);
			Assert.AreEqual(1, modifications.Length);
			Assert.AreEqual("AuthenticationService.asmx", modifications[0].FileName);
			Assert.AreEqual("src/AuthenticationService", modifications[0].FolderName);
			Assert.AreEqual("1.3", modifications[0].Version);
			Assert.AreEqual("Deleted", modifications[0].Type);
		}

		[Test]
		public void ParseMemberInfo()
		{
			Modification modification = new Modification();
			MksHistoryParser parser = new MksHistoryParser();
			parser.ParseMemberInfoAndAddToModification(modification, new StringReader(MEMBER_INFO));
			Assert.AreEqual("ccnetuser", modification.UserName);
			Assert.AreEqual(new DateTime(2005, 8, 26, 5, 32, 0), modification.ModifiedTime);
			Assert.AreEqual("just added a comment\r\n\t\tcontinued comment in another line", modification.Comment);
		}
	}
}