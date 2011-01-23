namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol.Mercurial
{
	using NUnit.Framework;
	using System;
	using System.IO;
	using System.Text;

	/// <summary>
	/// Coverage test for <see cref="StubFileSystem"/>.
	/// </summary>
	[TestFixture]
	public class StubFileSystemTest
	{

		#region Private Members

		private StubFileSystem sf;

		#endregion

		#region SetUp Method

		[SetUp]
		public void SetUp()
		{
			sf = new StubFileSystem();
		}

		#endregion

		#region Tests

		[Test]
		public void AtomicSaveCoverage()
		{
			sf.AtomicSave("asdf", "asdf");
		}

		[Test]
		public void AtomicSaveCoverage2()
		{
			sf.AtomicSave("asdf", "asdf", Encoding.Unicode);
		}

		[Test]
		public void CopyCoverage()
		{
			sf.Copy("asdf", "asdf");
		}

		[Test]
		public void CreateDirectoryCoverage()
		{
			sf.CreateDirectory("asdf");
		}

		[Test]
		public void DeleteDirectoryCoverage()
		{
			sf.DeleteDirectory("asdsf");
		}

		[Test]
		public void DeleteDirectoryCoverage2()
		{
			sf.DeleteDirectory("asdf", true);
		}

		[Test]
		public void DirectoryExistsCoverage()
		{
			sf.DirectoryExists("asdf");
		}

		[Test]
		public void DeleteFileCoverage()
		{
			sf.DeleteFile("asdf");
		}


		[Test]
		public void EnsureFolderExistsCoverage()
		{
			sf.EnsureFolderExists("asdf");
		}

		[Test]
		public void EnsureFileExistsShouldThrowException()
		{
			Assert.That(delegate { sf.EnsureFileExists("asdf"); },
			            Throws.TypeOf<NotImplementedException>());
		}

		[Test]
		public void FileExistsCoverage()
		{
			sf.FileExists("asdf");
		}

		[Test]
		public void GenerateTaskResultFromFileCoverage()
		{
			sf.GenerateTaskResultFromFile("asdf");
		}

		[Test]
		public void GenerateTaskResultFromFileCoverage2()
		{
			sf.GenerateTaskResultFromFile("asdf", true);
		}

		[Test]
		public void GetFilesInDirectoryCoverage()
		{
			sf.GetFilesInDirectory("asdf");
		}

		[Test]
		public void GetFilesInDirectoryCoverage2()
		{
			sf.GetFilesInDirectory("asdf", true);
		}

		[Test]
		public void GetFilesInDirectoryShouldThrowException()
		{
			Assert.That(delegate {sf.GetFilesInDirectory("asdf", "asdf", SearchOption.AllDirectories); },
			            Throws.TypeOf<NotImplementedException>());
		}

		[Test]
		public void GetFileLengthShouldThrowException()
		{
			Assert.That(delegate { sf.GetFileLength("asdf"); },
			            Throws.TypeOf<NotImplementedException>());
		}

		[Test]
		public void GetFileVersionShouldThrowException()
		{
			Assert.That(delegate {sf.GetFileVersion("asdf"); },
			            Throws.TypeOf<NotImplementedException>());
		}

		[Test]
		public void GetFreeDiskSpaceCoverage()
		{
			sf.GetFreeDiskSpace("asdf");
		}

		[Test]
		public void GetLastWriteTimeCoverage()
		{
			sf.GetLastWriteTime("asdf");
		}

		[Test]
		public void LoadCoverage()
		{
			sf.Load("asdf");
		}

		[Test]
		public void OpenInputStreamCoverage()
		{
			sf.OpenInputStream("asdf");
		}

		[Test]
		public void OpenOutputStreamCoverage()
		{
			sf.OpenOutputStream("asdf");
		}

		[Test]
		public void SaveCoverage()
		{
			sf.Save("asdf", "Asdf");
		}

		#endregion
	}
}
