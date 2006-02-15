using System;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class VaultHistoryParserTest : CustomAssertion
	{
		//Creating the date string this way will give us a string in the format of the builder's locale.
		//Can end up with DateTime parsing errors in the test otherwise...
		//e.g. US format date string "5/13/2003" gives format exception when parsed on UK locale system.
		private static string XML_COMMENT_DATE = new DateTime(2003, 5, 13, 22, 41, 30).ToString();
		private static readonly string XML = 
			@"<vault>
				<history>
					<item txid=""2"" date=""" + XML_COMMENT_DATE + @""" name=""$"" type=""70"" version=""1"" user=""admin"" comment=""creating repository"" />
				</history>
			  </vault>";
		private static readonly string NO_COMMENT_XML =
			@"<vault>
				<history>
					<item txid=""2"" date=""" + XML_COMMENT_DATE + @""" name=""$"" type=""70"" version=""1"" user=""admin"" />
				</history>
			  </vault>";
		private static readonly string XML_PADDED_WITH_EXTRA_CHARACTERS = @"_s ""Certificate Problem with
							accessing https://xxxx/VaultService/VaultService.asmx" + XML;

		private static readonly string ADD_AND_DELETE_FILES_XML = @"
			<vault>
				<history>
					<item txid=""13345"" date=""" + XML_COMMENT_DATE + @""" name=""$/1"" type=""80"" version=""319"" user=""jsmith"" comment=""temp file"" actionString=""Deleted 1.tmp"" />
					<item txid=""13344"" date=""" + XML_COMMENT_DATE + @""" name=""$/2"" type=""10"" version=""318"" user=""jsmith"" comment=""temp file"" actionString=""Added 2.tmp"" />
				</history>
			</vault>";

		private StringReader GetReader(string xml)
		{
			return new StringReader(xml);
		}

		[Test]
		public void NumberOfModifications()
		{
			VaultHistoryParser parser = new VaultHistoryParser();
			StringReader reader = GetReader(XML);
			Modification[] modifications = parser.Parse(reader, new DateTime(2003, 5, 12), DateTime.Now);
			reader.Close();
			Assert.AreEqual(1, modifications.Length);
		}

		[Test]
		public void NumberOfModificationsWithInvalidDate()
		{
			VaultHistoryParser parser = new VaultHistoryParser();
			StringReader reader = GetReader(XML);
			Modification[] modifications = parser.Parse(reader, DateTime.Now.AddMinutes(-1), DateTime.Now);
			reader.Close();
			Assert.AreEqual(0, modifications.Length);
		}

		[Test]
		public void ModificationData()
		{
			VaultHistoryParser parser = new VaultHistoryParser();
			StringReader reader = GetReader(XML);
			Modification[] modifications = parser.Parse(reader, new DateTime(2003, 5, 12), DateTime.Now);
			reader.Close();
			Modification mod = modifications[0];
			Assert.AreEqual("$", mod.FolderName);
			Assert.AreEqual(null, mod.FileName);
			Assert.AreEqual(new DateTime(2003, 5, 13, 22, 41, 30), mod.ModifiedTime);
			Assert.AreEqual("Created", mod.Type);
			Assert.AreEqual("admin", mod.UserName);
			Assert.AreEqual(2, mod.ChangeNumber);
		}

		[Test]
		public void ShouldStripCharactersOutsideOfVaultElement()
		{
			VaultHistoryParser parser = new VaultHistoryParser();
			StringReader reader = GetReader(XML_PADDED_WITH_EXTRA_CHARACTERS);
			Modification[] modifications = parser.Parse(reader, new DateTime(2003, 5, 12), DateTime.Now);			
			Assert.AreEqual(1, modifications.Length);
		}

		/// <summary>
		/// Tests a history entry with no comments (bug in first release of these classes).
		/// </summary>
		/// <remarks>
		/// Apparently the "comments" attribute is not always in the XML.
		/// </remarks>
		[Test]
		public void NoComments()
		{
			VaultHistoryParser parser = new VaultHistoryParser();
			StringReader reader = GetReader(NO_COMMENT_XML);
			parser.Parse(reader, new DateTime(2003, 5, 12), DateTime.Now);
			reader.Close();
		}

		[Test]
		public void ShouldFindFileAndFolderNamesForAddsAndDeletes()
		{
			VaultHistoryParser parser = new VaultHistoryParser();
			StringReader reader = GetReader(ADD_AND_DELETE_FILES_XML);
			Modification[] modifications = parser.Parse(reader, new DateTime(2003, 5, 12), DateTime.Now);			
			Assert.AreEqual(2, modifications.Length);
			Assert.AreEqual("1.tmp", modifications[0].FileName);
			Assert.AreEqual("$/1", modifications[0].FolderName);
			Assert.AreEqual("2.tmp", modifications[1].FileName);
			Assert.AreEqual("$/2", modifications[1].FolderName);
		}
	}
}
