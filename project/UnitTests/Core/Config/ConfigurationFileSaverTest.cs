using System.IO;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Config
{
	[TestFixture]
	public class ConfigurationFileSaverTest
	{
		[TearDown]
		protected void TearDown() 
		{
			TempFileUtil.DeleteTempDir(this);
		}

		[Test]
		public void ShouldBeAbleToSaveProjectsThatALoaderCanLoad()
		{
			ExecutableTask builder = new ExecutableTask();
			builder.Executable = "foo";
			FileSourceControl sourceControl = new FileSourceControl();
			sourceControl.RepositoryRoot = "bar";
			// Setup
			Project project1 = new Project();
			project1.Name = "Project One";
			project1.SourceControl = sourceControl;
			Project project2 = new Project();
			project2.Name = "Project Two";
			project2.SourceControl = sourceControl;
			ProjectList projectList = new ProjectList();
			projectList.Add(project1);
			projectList.Add(project2);

			var mockConfiguration = new Mock<IConfiguration>();
			mockConfiguration.SetupGet(_configuration => _configuration.Projects).Returns(projectList).Verifiable();

			FileInfo configFile = new FileInfo(TempFileUtil.CreateTempFile(TempFileUtil.CreateTempDir(this), "loadernet.config"));

			// Execute
			DefaultConfigurationFileSaver saver = new DefaultConfigurationFileSaver(new NetReflectorProjectSerializer());
			saver.Save((IConfiguration) mockConfiguration.Object, configFile);

			DefaultConfigurationFileLoader loader = new DefaultConfigurationFileLoader();
			IConfiguration configuration2 = loader.Load(configFile);

			// Verify
			Assert.IsNotNull (configuration2.Projects["Project One"]);
			Assert.IsNotNull (configuration2.Projects["Project Two"]);
			mockConfiguration.Verify();
		}
	}
}
