using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class ModificationTest : CustomAssertion
	{
		[Test]
		public void ModificationsAreComparedByModifiedDatetime()
		{
			Modification alpha = new Modification();
			alpha.ModifiedTime = new DateTime(1975, 3, 3);

			Modification beta = new Modification();
			alpha.ModifiedTime = new DateTime(1961, 3, 3);

			Assert.IsTrue(alpha.CompareTo(beta) > 0, string.Format("expected alpha greater than beta {0}", alpha.CompareTo(beta)));
			Assert.IsTrue(alpha.CompareTo(alpha) == 0, string.Format("expected alpha-beta equality {0}", alpha.CompareTo(beta)));
			Assert.IsTrue(beta.CompareTo(alpha) < 0, string.Format("expected alpha less than beta {0}", alpha.CompareTo(beta)));
		}

		[Test]
		public void OutputModificationToXml() 
		{
			Modification mod = CreateModification();

			string expected = string.Format(
@"<modification type=""unknown"">
	<filename>File""Name&amp;</filename>
	<project>Folder'Name</project>
	<date>{0}</date>
	<user>User&lt;&gt;Name</user>
	<comment>Comment</comment>
	<changeNumber>16</changeNumber>
	<url>http://localhost/viewcvs/</url>
	<email>foo.bar@quuuux.quuux.quux.qux</email>
</modification>", DateUtil.FormatDate(mod.ModifiedTime));

			Assert.AreEqual(XmlUtil.GenerateOuterXml(expected), mod.ToXml());
		}

		[Test]
		public void OutputToXmlWithSpecialCharactersInCommentField()
		{
			Modification mod = CreateModification();
			mod.Comment = "math says 2 < 4 & XML CDATA ends with ]]>; don't nest <![CDATA in <![CDATA]]> ]]>";

			string actual = XmlUtil.SelectRequiredValue(mod.ToXml(), "/modification/comment");
			Assert.AreEqual(mod.Comment, actual);
		}

		[Test]
		public void NullEmailAddressOrUrlShouldNotBeIncludedInXml()
		{
			Modification mod = CreateModification();
			mod.EmailAddress = null;
			mod.Url = null;

			Assert.IsNull(XmlUtil.SelectNode(mod.ToXml(), "/modification/email"));
			Assert.IsNull(XmlUtil.SelectNode(mod.ToXml(), "/modification/url"));
		}

        [Test]
        public void ShouldReturnTheMaximumChangeNumberFromAllModificationsAsLastChangeNumber()
        {
            Modification mod1 = new Modification();
            mod1.ChangeNumber = 10;

            Modification mod2 = new Modification();
            mod2.ChangeNumber = 20;

            Modification[] modifications = new Modification[] { mod1 };
            Assert.AreEqual(10, Modification.GetLastChangeNumber(modifications), "from Modification.GetLastChangeNumber({10})");
            modifications = new Modification[] { mod1, mod2 };
            Assert.AreEqual(20, Modification.GetLastChangeNumber(modifications), "from Modification.GetLastChangeNumber({10, 20})");
            modifications = new Modification[] { mod2, mod1 };
            Assert.AreEqual(20, Modification.GetLastChangeNumber(modifications), "from Modification.GetLastChangeNumber({20, 10})");
        }

        [Test]
        public void ShouldReturnZeroAsLastChangeNumberIfNoModifications()
        {
            Modification[] modifications = new Modification[0];
            Assert.AreEqual(0, Modification.GetLastChangeNumber(modifications), "LastChangeNumer({})");
        }

		private static Modification CreateModification()
		{
			Modification mod = new Modification();
			mod.FileName = "File\"Name&";
			mod.FolderName = "Folder'Name";
			mod.ModifiedTime = DateTime.Now;
			mod.UserName = "User<>Name";
			mod.Comment = "Comment";
			mod.ChangeNumber = 16;
			mod.EmailAddress = "foo.bar@quuuux.quuux.quux.qux";
			mod.Url = "http://localhost/viewcvs/";
			return mod;
		}
	}
}