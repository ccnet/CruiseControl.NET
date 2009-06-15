using System;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class GitHistoryParserTest
	{
		const string oneEntryLog = "Commit:24024978a9823df37a23afe533ad1c81f62467ed\nTime:2009-06-13 10:37:42 +0000\nAuthor:cj_sutherland\nE-Mail:cj_sutherland@8a0e9b86-a613-0410-befa-817088176213\nMessage:Expanded the tests to also cover default values.\n\ngit-svn-id: https://ccnet.svn.sourceforge.net/svnroot/ccnet/trunk@5714 8a0e9b86-a613-0410-befa-817088176213\n\nChanges:\nM\tproject/UnitTests/Core/Tasks/ReplacementDynamicValueTests.cs\n\n";
		private readonly GitHistoryParser git = new GitHistoryParser();

		[Test]
		public void ParsingEmptyLogProducesNoModifications()
		{
			Modification[] modifications = git.Parse(new StringReader(string.Empty), DateTime.Now, DateTime.Now);
			Assert.AreEqual(0, modifications.Length, "#A1");
		}

		[Test]
		public void ParsingSingleLogMessageProducesOneModification()
		{
			Modification[] modifications = git.Parse(new StringReader(oneEntryLog), new DateTime(2009, 06, 13, 10, 00, 00, DateTimeKind.Utc), DateTime.Now);
			Assert.AreEqual(1, modifications.Length, "#B1");

			Modification mod = modifications[0];
			Assert.AreEqual("24024978a9823df37a23afe533ad1c81f62467ed", mod.ChangeNumber, "#B2");
			Assert.IsTrue(mod.Comment.StartsWith("Expanded the tests"), "#B2");
			Assert.IsTrue(mod.Comment.Contains("git-svn-id"), "#B3");
			Assert.AreEqual("cj_sutherland@8a0e9b86-a613-0410-befa-817088176213", mod.EmailAddress, "#B4");
			Assert.AreEqual("ReplacementDynamicValueTests.cs", mod.FileName, "#B5");
			Assert.AreEqual("project/UnitTests/Core/Tasks", mod.FolderName, "#B6");
			Assert.IsNull(mod.IssueUrl, "#B7");
			Assert.AreEqual(new DateTime(2009, 6, 13, 10, 37, 42, DateTimeKind.Utc), mod.ModifiedTime.ToUniversalTime(), "#B8");
			Assert.AreEqual("Modified", mod.Type, "#B9");
			Assert.IsNull(mod.Url, "#B9");
			Assert.AreEqual("cj_sutherland", mod.UserName, "#B10");
			Assert.IsEmpty(mod.Version, "#B11");
		}

		[Test]
		public void ParsingLotsOfEntriesOneModification()
		{
			Modification[] modifications = git.Parse(File.OpenText("CCNet.git.log.txt"), new DateTime(2009, 01, 01, 10, 00, 00, DateTimeKind.Utc), new DateTime(2009, 01, 31, 10, 00, 00, DateTimeKind.Utc));
			Assert.AreEqual(85, modifications.Length, "#C1");
			Assert.AreEqual("dec91e2a00ce14c091330f41ed5055627272022e", Modification.GetLastChangeNumber(modifications), "C2");

			Modification mod = modifications[0];
			Assert.AreEqual("dec91e2a00ce14c091330f41ed5055627272022e", mod.ChangeNumber, "#C3");
			Assert.IsTrue(mod.Comment.StartsWith("CCNet-748 Provide better trigger exception logging"), "#C4");
			Assert.AreEqual("willemsruben@8a0e9b86-a613-0410-befa-817088176213", mod.EmailAddress, "#C5");
			Assert.AreEqual("Project.cs", mod.FileName, "#C6");
			Assert.AreEqual("project/core", mod.FolderName, "#C7");
			Assert.IsNull(mod.IssueUrl, "#C8");
			Assert.AreEqual(new DateTime(2009, 1, 30, 13, 39, 57, DateTimeKind.Utc), mod.ModifiedTime.ToUniversalTime(), "#C9");
			Assert.AreEqual("Modified", mod.Type, "#C10");
			Assert.IsNull(mod.Url, "#C11");
			Assert.AreEqual("willemsruben", mod.UserName, "#C12");
			Assert.IsEmpty(mod.Version, "#C13");

			mod = modifications[55];
			Assert.AreEqual("f8f963749bb64d3867599eb660fafbfc18a8ea0a", mod.ChangeNumber, "#C14");
			Assert.AreEqual("Added the validator to the installer.\n\ngit-svn-id: https://ccnet.svn.sourceforge.net/svnroot/ccnet/trunk@4533 8a0e9b86-a613-0410-befa-817088176213\n", mod.Comment, "#C15");
			Assert.AreEqual("cj_sutherland@8a0e9b86-a613-0410-befa-817088176213", mod.EmailAddress, "#C16");
			Assert.AreEqual("ccnet.sln", mod.FileName, "#C17");
			Assert.AreEqual("project", mod.FolderName, "#C18");
			Assert.IsNull(mod.IssueUrl, "#C19");
			Assert.AreEqual(new DateTime(2009, 1, 8, 20, 52, 37, DateTimeKind.Utc), mod.ModifiedTime.ToUniversalTime(), "#C20");
			Assert.AreEqual("Modified", mod.Type, "#C21");
			Assert.IsNull(mod.Url, "#C22");
			Assert.AreEqual("cj_sutherland", mod.UserName, "#C23");
			Assert.IsEmpty(mod.Version, "#C24");
		}

		[Test]
		public void ParsingLargeGitLog()
		{
			Modification[] modifications = git.Parse(File.OpenText("CCNet.git.log.txt"), DateTime.MinValue, DateTime.MaxValue);
			Assert.AreEqual(22212, modifications.Length, "#D1");
			Assert.AreEqual("dcbc5553fbc7e18905c47ae0eb10d15072e55cae", Modification.GetLastChangeNumber(modifications), "D2");

			Modification mod = modifications[0];
			Assert.AreEqual("dcbc5553fbc7e18905c47ae0eb10d15072e55cae", mod.ChangeNumber, "#D3");
			Assert.IsTrue(mod.Comment.StartsWith("Updated PowerShellTaskTest and NDepend"), "#D4");
			Assert.AreEqual("cj_sutherland@8a0e9b86-a613-0410-befa-817088176213", mod.EmailAddress, "#D5");
			Assert.AreEqual("NDependTaskTests.cs", mod.FileName, "#D6");
			Assert.AreEqual("project/UnitTests/Core/Tasks", mod.FolderName, "#D7");
			Assert.IsNull(mod.IssueUrl, "#D8");
			Assert.AreEqual(new DateTime(2009, 6, 14, 11, 02, 31, DateTimeKind.Utc), mod.ModifiedTime.ToUniversalTime(), "#D9");
			Assert.AreEqual("Modified", mod.Type, "#D10");
			Assert.IsNull(mod.Url, "#D11");
			Assert.AreEqual("cj_sutherland", mod.UserName, "#D12");
			Assert.IsEmpty(mod.Version, "#D13");

			mod = modifications[22211];
			Assert.AreEqual("0653fe2541cde05dc6098fe8afac79d1b4e0a62f", mod.ChangeNumber, "#D14");
			Assert.AreEqual("first checkin\n\ngit-svn-id: https://ccnet.svn.sourceforge.net/svnroot/ccnet/trunk@2 8a0e9b86-a613-0410-befa-817088176213\n", mod.Comment, "#D15");
			Assert.AreEqual("mikeroberts@8a0e9b86-a613-0410-befa-817088176213", mod.EmailAddress, "#D16");
			Assert.AreEqual("updateConfig.bat", mod.FileName, "#D17");
			Assert.IsEmpty(mod.FolderName, "#D18");
			Assert.IsNull(mod.IssueUrl, "#D19");
			Assert.AreEqual(new DateTime(2003, 4, 22, 16, 49, 49, DateTimeKind.Utc), mod.ModifiedTime.ToUniversalTime(), "#D20");
			Assert.AreEqual("Added", mod.Type, "#D21");
			Assert.IsNull(mod.Url, "#D22");
			Assert.AreEqual("mikeroberts", mod.UserName, "#D23");
			Assert.IsEmpty(mod.Version, "#D24");
		}
	}
}
