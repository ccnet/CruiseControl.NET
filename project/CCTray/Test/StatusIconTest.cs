using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTray;

namespace ThoughtWorks.CruiseControl.CCTray.Test
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
	      StatusIcon iconFile = StatusIcon.LoadFromFile(_file);
	        Size size = iconFile.Icon.Size;
			Assert.AreEqual(_originalIcon.Size,size);
	    }

		[Test]
		[ExpectedException(typeof(IconNotFoundException))]
		public void ShouldThrowIconNotFoundExceptionIfFileDoesNotExist()
		{
			StatusIcon.LoadFromFile("./fileNotOnDisk.ico");
		}

		[TearDown]
		public override void DeleteFile()
		{
			base.DeleteFile();
		}
	}
}
