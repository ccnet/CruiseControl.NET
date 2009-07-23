using System.Drawing;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib
{
	[TestFixture]
	public class StatusIconTest : IconFileFixture
	{
		[SetUp]
		public override void Init()
		{
			base.Init();
		}

		[Test]
		public void ShouldLoadIconFromFileWhenFileExists()
		{
			StatusIcon iconFile = StatusIcon.LoadFromFile(file);
			Size size = iconFile.Icon.Size;
			Assert.AreEqual(originalIcon.Size, size);
		}

		[Test]
		public void ShouldThrowIconNotFoundExceptionIfFileDoesNotExist()
		{
		    Assert.That(delegate { StatusIcon.LoadFromFile("./fileNotOnDisk.ico"); },
		                Throws.TypeOf<IconNotFoundException>());
		}

		[TearDown]
		public override void DeleteFile()
		{
			base.DeleteFile();
		}
	}
}