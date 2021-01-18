using System;
using System.IO;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class FileSourceControlTest : IntegrationFixture
	{
		private FileSourceControl sc;
		private Mock<IFileSystem> fileSystemMock;
		private SystemPath tempRoot;
		private SystemPath tempSubRoot;

		[SetUp]
		public void SetUp()
		{
			fileSystemMock = new Mock<IFileSystem>();

			tempRoot = SystemPath.UniqueTempPath();
			tempSubRoot = tempRoot.Combine("subrepo");

			sc = new FileSourceControl((IFileSystem) fileSystemMock.Object);
			sc.RepositoryRoot = tempRoot.ToString();
		}

		[TearDown]
		public void TearDown()
		{
			tempRoot.DeleteDirectory();
		}

		[Test]
		public void MissingDirectoryThrowsException()
		{
			Assert.IsFalse(tempRoot.Exists(), "Temporary directory should not exist: " + tempRoot.ToString());
            Assert.That(delegate { sc.GetModifications(IntegrationResult(DateTime.MinValue), IntegrationResult(DateTime.MaxValue)); },
                        Throws.TypeOf<DirectoryNotFoundException>());
		}

		[Test]
		public void IgnoreMissingDirectoryReturnsZeroMods()
		{
			Assert.IsFalse(tempRoot.Exists(), "Temporary directory should not exist: " + tempRoot.ToString());
			sc.IgnoreMissingRoot = true;
			try
			{
				Modification[] mods = sc.GetModifications(IntegrationResult(DateTime.MinValue), IntegrationResult(DateTime.MaxValue));
				Assert.AreEqual(0, mods.Length, "Modifications found in a missing directory");
			}
			finally
			{
				sc.IgnoreMissingRoot = false;
			}
		}

		[Test]
		public void GetModifications_EmptyLocal()
		{
			tempRoot.CreateDirectory();
			tempSubRoot.CreateDirectory();
			string file1 = tempRoot.CreateTextFile("file1.txt", "foo").ToString();
			string file2 = tempRoot.CreateTextFile("file2.txt", "bar").ToString();
			string file3 = tempSubRoot.CreateTextFile("file3.txt", "bat").ToString();

			Modification[] mods = sc.GetModifications(IntegrationResult(DateTime.MinValue), IntegrationResult(DateTime.MaxValue));
            Array.Sort(
                mods,
                (left, right) => 
                {
                    int result = left.FolderName.CompareTo(right.FolderName);
                    if (result == 0) 
                        result = left.FileName.CompareTo(right.FileName);
                        
                    return result;
                }
            );

			using( new AssertionScope())
			{
				mods.Length.Should().Be(4);
				mods[0].FileName.Should().Be("file1.txt", "FileName[0]");
				mods[1].FileName.Should().Be("file2.txt", "FileName[1]");
				mods[2].FileName.Should().Be("file3.txt", "FileName[2]");
				mods[3].FileName.Should().Be(Path.GetFileName(tempSubRoot.ToString()), "FileName[3]");

				mods[0].FolderName.Should().Be(Path.GetDirectoryName(file1), "FolderName[0]");
				mods[1].FolderName.Should().Be(Path.GetDirectoryName(file2), "FolderName[1]");
				mods[2].FolderName.Should().Be(Path.GetDirectoryName(file3), "FolderName[2]");
				mods[3].FolderName.Should().Be(Path.GetFileName(tempSubRoot.ToString()), "FolderName[3]");

				new FileInfo(file1).LastWriteTime.Should().BeCloseTo(mods[0].ModifiedTime,  100, "LastWriteTime[0]");
				new FileInfo(file2).LastWriteTime.Should().BeCloseTo(mods[1].ModifiedTime, 100, "LastWriteTime[1]");
				new FileInfo(file3).LastWriteTime.Should().BeCloseTo(mods[2].ModifiedTime, 100, "LastWriteTime[2]");
				new FileInfo(tempSubRoot.ToString()).LastWriteTime.Should().BeCloseTo(mods[3].ModifiedTime, 100, "LastWriteTime[3]");
			}

            mods = sc.GetModifications(IntegrationResult(DateTime.Now.AddHours(1)), IntegrationResult(DateTime.MaxValue));

			mods.Length.Should().Be(0);
		}

		[Test]
		public void GetModificationsWhenRepositoryFolderIsEmpty()
		{
			tempRoot.CreateDirectory();
			Modification[] mods = sc.GetModifications(IntegrationResult(DateTime.MinValue), IntegrationResult(DateTime.MaxValue));
			Assert.IsNotNull(mods);
			Assert.AreEqual(0, mods.Length);
		}

		[Test]
		public void GetModificationsWhenRepositoryRootContainsOneUnmodifiedFile()
		{
			tempRoot.CreateDirectory();
			tempRoot.CreateTextFile("file1.txt", "foo");
			string file2 = tempRoot.CreateTextFile("file2.txt", "bar").ToString();
			new FileInfo(file2).LastWriteTime = DateTime.Now.AddHours(2);

			Modification[] mods = sc.GetModifications(IntegrationResult(DateTime.Now.AddHours(1)), IntegrationResult(DateTime.MaxValue));
			Assert.AreEqual(1, mods.Length);
			Assert.AreEqual("file2.txt", mods[0].FileName);
		}

		[Test]
		public void ShouldCopyRespositoryRootToWorkingDirectoryForGetSource()
		{
			IntegrationResult result = (IntegrationResult) Integration("foo", "myWorkingDirectory", "myArtifactDirectory");
			sc.AutoGetSource = true;
			fileSystemMock.Setup(fileSystem => fileSystem.Copy(tempRoot.ToString(), "myWorkingDirectory")).Verifiable();

			sc.GetSource(result);

			fileSystemMock.Verify();
		}

		[Test]
		public void ShouldNotCopySourceIfAutoGetSourceNotBeenSetToTrue()
		{
            IIntegrationResult result = Integration("foo", "myWorkingDirectory", "myArtifactDirectory");

			sc.GetSource(result);

			fileSystemMock.Verify();
			fileSystemMock.VerifyNoOtherCalls();
		}

		private IntegrationResult IntegrationResult(DateTime date)
		{
			return IntegrationResultMother.CreateSuccessful(date);
		}

        [Test]
        public void GetModifications_NonRecursive()
        {
            tempRoot.CreateDirectory();
            tempSubRoot.CreateDirectory();
            string file1 = tempRoot.CreateTextFile("file1.txt", "foo").ToString();
            string file2 = tempRoot.CreateTextFile("file2.txt", "bar").ToString();
            string file3 = tempSubRoot.CreateTextFile("file3.txt", "bat").ToString();

            sc.CheckRecursively = false;

            Modification[] mods = sc.GetModifications(IntegrationResult(DateTime.MinValue), IntegrationResult(DateTime.MaxValue));
            Array.Sort(
                mods,
                (left, right) => left.FileName.CompareTo(right.FileName)
            );

			using(new AssertionScope())
			{
				mods.Length.Should().Be(3);

				mods[0].FileName.Should().Be("file1.txt");
				mods[1].FileName.Should().Be("file2.txt");
				mods[2].FileName.Should().Be(Path.GetFileName(tempSubRoot.ToString()));

				mods[0].FolderName.Should().Be(Path.GetDirectoryName(file1));
				mods[1].FolderName.Should().Be(Path.GetDirectoryName(file2));
				mods[2].FolderName.Should().Be(Path.GetFileName(tempSubRoot.ToString()));

				new FileInfo(file1).LastWriteTime.Should().BeCloseTo(mods[0].ModifiedTime, 100);
				new FileInfo(file2).LastWriteTime.Should().BeCloseTo(mods[1].ModifiedTime, 100);
				new FileInfo(tempSubRoot.ToString()).LastWriteTime.Should().BeCloseTo(mods[2].ModifiedTime, 100);
			}

            mods = sc.GetModifications(IntegrationResult(DateTime.Now.AddHours(1)), IntegrationResult(DateTime.MaxValue));
			mods.Length.Should().Be(0);
        }
	}
}
