using System.IO;	
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
	[TestFixture]
	public class GendarmeTaskTest : ProcessExecutorTestFixtureBase
	{
		private string logfile;
		private IIntegrationResult result;
		private GendarmeTask task;

		[SetUp]
		protected void SetUp()
		{
			CreateProcessExecutorMock(GendarmeTask.defaultExecutable);
			result = IntegrationResult();
			result.Label = "1.0";
			result.ArtifactDirectory = Path.GetTempPath();
			logfile = Path.Combine(result.ArtifactDirectory, GendarmeTask.logFilename);
			TempFileUtil.DeleteTempFile(logfile);
			task = new GendarmeTask((ProcessExecutor)mockProcessExecutor.MockInstance);
		}

		[TearDown]
		protected void TearDown()
		{
			Verify();
		}

		protected static void AddDefaultAssemblyToCheck(GendarmeTask task)
		{
            AssemblyMatch match1 = new AssemblyMatch();
            match1.Expression = "*.dll";
			AssemblyMatch match2 = new AssemblyMatch();
			match2.Expression = "*.exe";
			task.Assemblies = new AssemblyMatch[] { match1, match2 };
		}

		[Test]
		public void PopulateFromReflector()
		{
			const string xml = @"
    <gendarme>
    	<executable>gendarme.exe</executable>
    	<baseDirectory>C:\</baseDirectory>
		<configFile>rules.xml</configFile>
		<ruleSet>*</ruleSet>
		<ignoreFile>C:\gendarme.ignore.list.txt</ignoreFile>
		<limit>200</limit>
		<severity>medium+</severity>
		<confidence>normal+</confidence>
		<quiet>FALSE</quiet>
		<verbose>TRUE</verbose>
		<failBuildOnFoundDefects>TRUE</failBuildOnFoundDefects>
		<verifyTimeoutSeconds>600</verifyTimeoutSeconds>
		<assemblies>
      		<assemblyMatch expr='*.dll' />
			<assemblyMatch expr='*.exe' />
    	</assemblies>
		<assemblyListFile>C:\gendarme.assembly.list.txt</assemblyListFile>
		<description>Test description</description>
    </gendarme>";

			NetReflector.Read(xml, task);
			Assert.AreEqual("gendarme.exe", task.Executable);
			Assert.AreEqual(@"C:\", task.ConfiguredBaseDirectory);
			Assert.AreEqual("rules.xml", task.ConfigFile);
			Assert.AreEqual("*", task.RuleSet);
			Assert.AreEqual(@"C:\gendarme.ignore.list.txt", task.IgnoreFile);
			Assert.AreEqual(200, task.Limit);
			Assert.AreEqual("medium+", task.Severity);
			Assert.AreEqual("normal+", task.Confidence);
			Assert.AreEqual(false, task.Quiet);
			Assert.AreEqual(true, task.Verbose);
			Assert.AreEqual(true, task.FailBuildOnFoundDefects);
			Assert.AreEqual(600, task.VerifyTimeoutSeconds);
			Assert.AreEqual("Test description", task.Description);

			Assert.AreEqual(2, task.Assemblies.Length);
			Assert.AreEqual("*.dll", task.Assemblies[0].Expression);
			Assert.AreEqual("*.exe", task.Assemblies[1].Expression);
			Assert.AreEqual(@"C:\gendarme.assembly.list.txt", task.AssemblyListFile);
		}

		[Test]
		public void PopulateFromConfigurationUsingOnlyRequiredElementsAndCheckDefaultValues()
		{
			const string xml = @"<gendarme />";

			NetReflector.Read(xml, task);
			Assert.AreEqual(GendarmeTask.defaultExecutable, task.Executable);
			Assert.AreEqual(string.Empty, task.ConfiguredBaseDirectory);
			Assert.AreEqual(string.Empty, task.ConfigFile);
			Assert.AreEqual(string.Empty, task.RuleSet);
			Assert.AreEqual(string.Empty, task.IgnoreFile);
			Assert.AreEqual(GendarmeTask.defaultLimit, task.Limit);
			Assert.AreEqual(string.Empty, task.Severity);
			Assert.AreEqual(string.Empty, task.Confidence);
			Assert.AreEqual(GendarmeTask.defaultQuiet, task.Quiet);
			Assert.AreEqual(GendarmeTask.defaultVerbose, task.Verbose);
			Assert.AreEqual(GendarmeTask.defaultFailBuildOnFoundDefects, task.FailBuildOnFoundDefects);
			Assert.AreEqual(GendarmeTask.defaultVerifyTimeout, task.VerifyTimeoutSeconds);
			Assert.AreEqual(null, task.Description);
			Assert.AreEqual(0, task.Assemblies.Length);
			Assert.AreEqual(string.Empty, task.AssemblyListFile);
		}

		[Test]
		public void ShouldEncloseDirectoriesInQuotesIfTheyContainSpaces()
		{
			result.ArtifactDirectory = DefaultWorkingDirectoryWithSpaces;
			result.WorkingDirectory = DefaultWorkingDirectoryWithSpaces;

			task.AssemblyListFile = Path.Combine(DefaultWorkingDirectoryWithSpaces, "gendarme assembly file.txt");
			task.ConfigFile = Path.Combine(DefaultWorkingDirectoryWithSpaces, "gendarme rules.xml");
			task.IgnoreFile = Path.Combine(DefaultWorkingDirectoryWithSpaces, "gendarme ignore file.txt");

			ExpectToExecuteArguments(@"--config " + StringUtil.AutoDoubleQuoteString(task.ConfigFile) + " --ignore " +
			                         StringUtil.AutoDoubleQuoteString(task.IgnoreFile) + " --xml " +
			                         StringUtil.AutoDoubleQuoteString(Path.Combine(result.ArtifactDirectory, "gendarme-results.xml")) + " @" +
									 StringUtil.AutoDoubleQuoteString(task.AssemblyListFile), DefaultWorkingDirectoryWithSpaces);

			task.ConfiguredBaseDirectory = DefaultWorkingDirectoryWithSpaces;
			task.VerifyTimeoutSeconds = 600;
			task.Run(result);
		}

		[Test, ExpectedException(typeof(ThoughtWorks.CruiseControl.Core.Config.ConfigurationException))]
		public void ShouldThrowConfigurationExceptionIfAssemblyListNotSet()
		{
			//DO NOT SET: AddDefaultAssemblyToCheck(task);
			task.Run(result);
		}

		[Test, ExpectedException(typeof(BuilderException))]
		public void ShouldThrowBuilderExceptionIfProcessThrowsException()
		{
			AddDefaultAssemblyToCheck(task);
			ExpectToExecuteAndThrow();
			task.Run(result);
		}

		[Test]
		public void RebaseFromWorkingDirectory()
		{
			AddDefaultAssemblyToCheck(task);
			ProcessInfo info =
				NewProcessInfo(
					string.Format("--xml {0} {1} {2}",
					              StringUtil.AutoDoubleQuoteString(Path.Combine(result.ArtifactDirectory, "gendarme-results.xml")),
					              StringUtil.AutoDoubleQuoteString("*.dll"), StringUtil.AutoDoubleQuoteString("*.exe")),
					Path.Combine(DefaultWorkingDirectory, "src"));

			info.WorkingDirectory = Path.Combine(DefaultWorkingDirectory, "src");
			ExpectToExecute(info);
			task.ConfiguredBaseDirectory = "src";
			task.VerifyTimeoutSeconds = 600;
			task.Run(result);
		}

		[Test]
		public void UseAssemblyCollectionAndAssemblyListFile()
		{
			AddDefaultAssemblyToCheck(task);
			task.AssemblyListFile = Path.Combine(DefaultWorkingDirectoryWithSpaces, "gendarme assembly file.txt");

			ProcessInfo info =
				NewProcessInfo(
					string.Format("--xml {0} @{1} {2} {3}",
					              StringUtil.AutoDoubleQuoteString(Path.Combine(result.ArtifactDirectory, "gendarme-results.xml")),
					              StringUtil.AutoDoubleQuoteString(task.AssemblyListFile),
					              StringUtil.AutoDoubleQuoteString("*.dll"), StringUtil.AutoDoubleQuoteString("*.exe")),
					Path.Combine(DefaultWorkingDirectory, "src"));

			info.WorkingDirectory = Path.Combine(DefaultWorkingDirectory, "src");
			ExpectToExecute(info);
			task.ConfiguredBaseDirectory = "src";
			task.VerifyTimeoutSeconds = 600;
			task.Run(result);
		}

		[Test]
		public void TimedOutExecutionShouldFailBuild()
		{
			try
			{
				AddDefaultAssemblyToCheck(task);
				ExpectToExecuteAndReturn(TimedOutProcessResult());
				task.Run(result);
			}
			catch (BuilderException)
			{
			}
			Assert.AreEqual(IntegrationStatus.Failure, result.Status);
		}

		[Test]
		[ExpectedException(typeof(BuilderException))]
		public void TimedOutExecutionShouldCauseBuilderException()
		{
			AddDefaultAssemblyToCheck(task);
			ExpectToExecuteAndReturn(TimedOutProcessResult());
			task.Run(result);
		}

		[Test]
		public void ShouldAutomaticallyMergeTheBuildOutputFile()
		{
			AddDefaultAssemblyToCheck(task);
			TempFileUtil.CreateTempXmlFile(logfile, "<output/>");
			ExpectToExecuteAndReturn(SuccessfulProcessResult());
			task.Run(result);
			Assert.AreEqual(2, result.TaskResults.Count);
			Assert.AreEqual("<output/>" + ProcessResultOutput, result.TaskOutput);
			Assert.IsTrue(result.Succeeded);
		}

		[Test]
		public void ShouldFailOnFailedProcessResult()
		{
			AddDefaultAssemblyToCheck(task);
			TempFileUtil.CreateTempXmlFile(logfile, "<output/>");
			ExpectToExecuteAndReturn(FailedProcessResult());
			task.Run(result);
			Assert.AreEqual(2, result.TaskResults.Count);
			Assert.AreEqual("<output/>" + ProcessResultOutput, result.TaskOutput);
			Assert.IsTrue(result.Failed);
		}
	}
}
