using System;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
{
	[TestFixture]
	public class VaultHistoryParserTest : CustomAssertion
	{
		//Creating the date string this way will give us a string in the format of the builder's locale.
		//Can end up with DateTime parsing errors in the test otherwise...
		//e.g. US format date string "5/13/2003" gives format exception when parsed on UK locale system.
		private static string XML_COMMENT_DATE = (new DateTime(2003, 5, 13, 22, 41, 30)).ToString();
		private string XML = 
			@"<vault>
				<history>
					<item txid=""2"" date=""" + XML_COMMENT_DATE + @""" name=""$"" type=""70"" version=""1"" user=""admin"" comment=""creating repository"" />
				</history>
			  </vault>";
		private string NO_COMMENT_XML =
			@"<vault>
				<history>
					<item txid=""2"" date=""" + XML_COMMENT_DATE + @""" name=""$"" type=""70"" version=""1"" user=""admin"" />
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
			Modification[] modifications = parser.Parse(reader, new DateTime(2003, 5, 12), DateTime.Now);
			reader.Close();
		}
	}
}
