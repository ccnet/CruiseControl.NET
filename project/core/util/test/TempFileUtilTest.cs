using System;
using System.IO;
using System.Xml;
using NUnit.Framework;

namespace tw.ccnet.core.util.test
{
	[TestFixture]
	public class TempFileUtilTest
	{
		private static readonly string TempDir = "tempfileutiltest";

		[SetUp]
		public void SetUp()
		{
			TempFileUtil.DeleteTempDir(TempDir);
			Assertion.Assert("Temp folder exists before test!", ! Directory.Exists(TempFileUtil.GetTempPath(TempDir)));
		}

		[TearDown]
		public void TearDown()
		{
			TempFileUtil.DeleteTempDir(TempDir);
			Assertion.Assert("Temp folder exists after test!", ! Directory.Exists(TempFileUtil.GetTempPath(TempDir)));
		}

		public void TestCreateTempDir()
		{
			TempFileUtil.CreateTempDir(TempDir);
			Assertion.Assert("Temp folder does not exist after test!", Directory.Exists(TempFileUtil.GetTempPath(TempDir)));
			TempFileUtil.DeleteTempDir(TempDir);
		}

		public void TestCreateTempDirOverride()
		{
			TempFileUtil.CreateTempDir(TempDir);
			TempFileUtil.CreateTempFiles(TempDir, new String[]{"test.tmp"});
			Assertion.AssertEquals(1, Directory.GetFiles(TempFileUtil.GetTempPath(TempDir)).Length);
			TempFileUtil.CreateTempDir(TempDir);
			Assertion.AssertEquals(0, Directory.GetFiles(TempFileUtil.GetTempPath(TempDir)).Length);
		}

		public void TestCreateTempXmlDoc()
		{
			TempFileUtil.CreateTempDir(TempDir);
			string path = TempFileUtil.CreateTempXmlFile(TempDir, "foobar.xml", "<test />");
			Assertion.Assert("Xml file does not exist", File.Exists(path));
			XmlDocument doc = new XmlDocument();
			doc.Load(path);
		}

		public void TestCreateTempFile_withContent()
		{
			string expected = "hello my name is rosebud";
			string path = TempFileUtil.CreateTempFile(TempDir, "TestCreateTempFile_withContent.txt", expected);
			Assertion.Assert("expected file to exist: " + path, File.Exists(path));
			StreamReader stream = null;
			try
			{
				stream = File.OpenText(path);
				Assertion.AssertEquals(expected, stream.ReadToEnd());				
			}
			finally
			{
				stream.Close();
			}
		}
	}
}
