using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib;
using ThoughtWorks.CruiseControl.UnitTests.CCTrayLib;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib
{
	[TestFixture]
	public class FileIconStoreTest : AbstractIconStoreTestCase
	{
		private StatusIcons _statusicons;
		private IconFileFixture _fileStoreFixture;

	    protected override IIconStore CreateIconStore()
	    {
	        return new FileIconStore(_statusicons);
	    }

	    protected override void Validate(StatusIcon expectedIcon, StatusIcon actualIcon)
	    {
			Assert.AreEqual(expectedIcon.Icon.Size, actualIcon.Icon.Size);
	    }

	    [SetUp]
		public override void Init()
		{
			_fileStoreFixture = new IconFileFixture();
			_fileStoreFixture.Init();

			_statusicons = new StatusIcons();
			_statusicons.UseDefaultIcons = false;
			_statusicons.NowBuilding = _fileStoreFixture.FileName;
			_statusicons.BuildSuccessful =_fileStoreFixture.FileName;
			_statusicons.BuildFailed=_fileStoreFixture.FileName;
			_statusicons.Unknown =_fileStoreFixture.FileName;
			_statusicons.Error =_fileStoreFixture.FileName;

			base.Init();
		}

		[Test]
		[ExpectedException(typeof(IconNotFoundException))]
		public void ValidatesIfFilesExist()
		{
			_statusicons.BuildFailed = "unknownFile.ico";
			CreateIconStore();    
		}
		[TearDown]
		public void DeleteFile()
		{
			_fileStoreFixture.DeleteFile();    
		}
	}
}
