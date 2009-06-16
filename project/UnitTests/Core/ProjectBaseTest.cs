using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using System;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	[TestFixture]
	public class ProjectBaseTest
	{
		private ProjectBase project;
		private class ConcreteProject : ProjectBase { }

		[SetUp]
		public void Setup()
		{
			project = new ConcreteProject();
		}

		[Test]
		public void ShouldReturnConfiguredWorkingDirectoryIfOneIsSet()
		{
			string workingDir = Path.GetFullPath(Path.Combine(".", "workingdir"));

			// Setup
			project.ConfiguredWorkingDirectory = workingDir;

			// Execute & Verify
			Assert.AreEqual(workingDir, project.WorkingDirectory);
		}

		[Test]
		public void ShouldReturnCalculatedWorkingDirectoryIfOneIsNotSet()
		{
			// Setup
			project.Name = "myProject";

			// Execute & Verify
			Assert.AreEqual(
                Path.Combine(PathUtils.DefaultProgramDataFolder,
                    Path.Combine("myProject", "WorkingDirectory")), 
                project.WorkingDirectory);
		}

		[Test]
		public void ShouldReturnConfiguredArtifactDirectoryIfOneIsSet()
		{
			string artifactDir = Path.GetFullPath(Path.Combine(".", "artifacts"));

			// Setup
			project.ConfiguredArtifactDirectory = artifactDir;

			// Execute & Verify
			Assert.AreEqual(artifactDir, project.ArtifactDirectory);
		}

		[Test]
		public void ShouldReturnCalculatedArtifactDirectoryIfOneIsNotSet()
		{
			// Setup
			project.Name = "myProject";

			// Execute & Verify
			Assert.AreEqual(
                Path.Combine(PathUtils.DefaultProgramDataFolder, 
                    Path.Combine("myProject", "Artifacts")), 
                project.ArtifactDirectory);
		}




	}
}