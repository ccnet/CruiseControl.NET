using System;
using System.IO;
using System.Xml;
using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.Core.Util.Test
{
	[TestFixture]
	public class TempFileUtilTest : CustomAssertion
	{
		private static readonly string TempDir = "tempfileutiltest";

		[SetUp]
		public void SetUp()
		{
			TempFileUtil.DeleteTempDir(TempDir);
			Assert.IsTrue(! Directory.Exists(TempFileUtil.GetTempPath(TempDir)), "Temp folder exists before test!");
		}

		[TearDown]
		public void TearDown()
		{
			TempFileUtil.DeleteTempDir(TempDir);
			Assert.IsTrue(! Directory.Exists(TempFileUtil.GetTempPath(TempDir)));
		}

		public void TestCreateTempDir()
		{
			TempFileUtil.CreateTempDir(TempDir);
			Assert.IsTrue(Directory.Exists(TempFileUtil.GetTempPath(TempDir)));
			TempFileUtil.DeleteTempDir(TempDir);
		}

		public void TestCreateTempDirOverride()
		{
			TempFileUtil.CreateTempDir(TempDir);
			TempFileUtil.CreateTempFiles(TempDir, new String[]{"test.tmp"});
			Assert.AreEqual(1, Directory.GetFiles(TempFileUtil.GetTempPath(TempDir)).Length);
			TempFileUtil.CreateTempDir(TempDir);
			Assert.AreEqual(0, Directory.GetFiles(TempFileUtil.GetTempPath(TempDir)).Length);
		}

		public void TestCreateTempXmlDoc()
		{
			TempFileUtil.CreateTempDir(TempDir);
			string path = TempFileUtil.CreateTempXmlFile(TempDir, "foobar.xml", "<test />");
			Assert.IsTrue(File.Exists(path));
			XmlDocument doc = new XmlDocument();
			doc.Load(path);
		}

		[Test]
		public void CreateTempFileWithContent()
		{
			string expected = "hello my name is rosebud";
			string path = TempFileUtil.CreateTempFile(TempDir, "TestCreateTempFile_withContent.txt", expected);
			Assert.IsTrue(File.Exists(path));
			using (StreamReader stream = File.OpenText(path))
			{
				Assert.AreEqual(expected, stream.ReadToEnd());				
			}
		}
	}
}
