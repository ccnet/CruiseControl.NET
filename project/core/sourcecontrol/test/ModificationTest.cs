using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using tw.ccnet.core.util;
using System.Collections;

namespace tw.ccnet.core.sourcecontrol.test
{
	[TestFixture]
	public class ModificationTest : CustomAssertion
	{
		public void TestCompareTo()
		{
			Modification alpha = new Modification();
			alpha.ModifiedTime = new DateTime(1975, 3, 3);

			Modification beta = new Modification();
			alpha.ModifiedTime = new DateTime(1961, 3, 3);

			Assert(string.Format("expected alpha greater than beta {0}", 
				alpha.CompareTo(beta)), alpha.CompareTo(beta) > 0);
			Assert(string.Format("expected alpha-beta equality {0}", 
				alpha.CompareTo(beta)), alpha.CompareTo(alpha) == 0);
			Assert(string.Format("expected alpha less than beta {0}", 
				alpha.CompareTo(beta)), beta.CompareTo(alpha) < 0);
		}

		public void TestToXml() 
		{
			DateTime modifiedTime = new DateTime();
			Modification mod = new Modification();
			mod.FileName = "File\"Name&";
			mod.FolderName = "Folder'Name";
			mod.ModifiedTime = modifiedTime;
			mod.UserName = "User<>Name";
			mod.Comment = "Comment";
			mod.EmailAddress = "foo.bar@quuuux.quuux.quux.qux";

			string expected = string.Format(
@"<modification type=""unknown"">
	<filename>File""Name&amp;</filename>
	<project>Folder'Name</project>
	<date>{0}</date>
	<user>User&lt;&gt;Name</user>
	<comment>Comment</comment>
	<email>foo.bar@quuuux.quuux.quux.qux</email>
</modification>", DateUtil.FormatDate(modifiedTime));

			AssertEquals(XmlUtil.GenerateOuterXml(expected), mod.ToXml());
		}

		public void TestToXml_CommentContainsXmlSymbols()
		{
			DateTime modifiedTime = new DateTime();
			Modification mod = new Modification();
			mod.FileName = "File\"Name&";
			mod.FolderName = "Folder'Name";
			mod.ModifiedTime = modifiedTime;
			mod.UserName = "User<>Name";
			mod.Comment = "math says 2 < 4 & XML CDATA ends with ]]>; don't nest <![CDATA in <![CDATA]]> ]]>";

			XmlDocument document = XmlUtil.CreateDocument(mod.ToXml());
			
			string xpath = "/modification/comment";
			string actual = XmlUtil.SelectRequiredValue(document, xpath);
			AssertEquals(mod.Comment, actual);
		}
	}
}