using System;
using System.IO;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using NMock;


namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
	[TestFixture]
	public class FinalBuilderTaskTest : ProcessExecutorTestFixtureBase
	{		
		private IIntegrationResult _result;
		private FinalBuilderTask _task;
		private IMock _mockRegistry;
		private string fbExecutable;

		[SetUp]
		protected void SetUp()
		{
			fbExecutable = Path.Combine(DefaultWorkingDirectory, "FBCmd.exe");

			_mockRegistry = new DynamicMock(typeof (IRegistry));
			CreateProcessExecutorMock(fbExecutable);
			_result = IntegrationResult();
			_result.Label = "1.0";
			_result.ArtifactDirectory = Path.GetTempPath();
			_task = new FinalBuilderTask((IRegistry) _mockRegistry.MockInstance, (ProcessExecutor) mockProcessExecutor.MockInstance);
		}

		[TearDown]
		protected void TearDown()
		{
			Verify();
			_mockRegistry.Verify();
		}

		[Test]
		public void BuildCommandLine()
		{
			string expectedArgs = @"/B /S /Vvar1=value1;var2=""value 2"" /P" + StringUtil.AutoDoubleQuoteString(Path.Combine(DefaultWorkingDirectory, "TestProject.fbz5"));
			ExpectToExecuteArguments(expectedArgs);

			_mockRegistry.ExpectAndReturn("GetLocalMachineSubKeyValue", Path.Combine(DefaultWorkingDirectory, "FinalBuilder5.exe"), @"SOFTWARE\VSoft\FinalBuilder\5.0", "Location");

			_task.FBVariables = new FBVariable[2];
			_task.FBVariables[0] = new FBVariable("var1", "value1");
			_task.FBVariables[1] = new FBVariable("var2", "value 2");
			_task.ProjectFile = Path.Combine(DefaultWorkingDirectory, "TestProject.fbz5");
			_task.ShowBanner = false;
			_task.DontWriteToLog = true;
			_task.Timeout = 600;
			_task.Run(_result);

			Assert.AreEqual(1, _result.TaskResults.Count);
			Assert.AreEqual(IntegrationStatus.Success, _result.Status);
			Assert.AreEqual(ProcessResultOutput, _result.TaskOutput);
		}

		[Test]
		public void DoubleQuoteSpacesinPaths()
		{
			string expectedArgs = string.Concat("/P", StringUtil.AutoDoubleQuoteString(Path.Combine(DefaultWorkingDirectoryWithSpaces, "TestProject.fbz5")));
			ExpectToExecuteArguments(expectedArgs, DefaultWorkingDirectoryWithSpaces);

			_mockRegistry.ExpectAndReturn("GetLocalMachineSubKeyValue", Path.Combine(DefaultWorkingDirectory, "FinalBuilder5.exe"), @"SOFTWARE\VSoft\FinalBuilder\5.0", "Location");
							
			_task.ShowBanner = true;
			_task.ProjectFile = Path.Combine(DefaultWorkingDirectoryWithSpaces, "TestProject.fbz5");
			_task.Timeout = 600;
			_task.Run(_result);			

			Assert.AreEqual(1, _result.TaskResults.Count);
			Assert.AreEqual(IntegrationStatus.Success, _result.Status);
			Assert.AreEqual(ProcessResultOutput, _result.TaskOutput);		
		}

		[Test]
		public void PopulateFromCompleteConfiguration()
		{
			const string xmlConfig = @"<FinalBuilder>
				<ProjectFile>C:\Dummy\Project.fbz3</ProjectFile>
 				<ShowBanner>false</ShowBanner>
 				<FBVariables>
 					<FBVariable name=""MyVariable"" value=""SomeValue"" />
 				</FBVariables>
				<FBVersion>3</FBVersion>
 				<FBCMDPath>C:\Program Files\MyFinalBuilderPath\FBCMD.EXE</FBCMDPath>
 				<DontWriteToLog>true</DontWriteToLog>
 				<Timeout>100</Timeout> 
				</FinalBuilder>";
			
			NetReflector.Read(xmlConfig, _task);
			Assert.AreEqual(@"C:\Dummy\Project.fbz3", _task.ProjectFile);
			Assert.AreEqual(false, _task.ShowBanner);
			Assert.AreEqual(1, _task.FBVariables.Length);
			Assert.AreEqual("MyVariable", _task.FBVariables[0].Name);
			Assert.AreEqual("SomeValue", _task.FBVariables[0].Value);
			Assert.AreEqual(3, _task.FBVersion);
			Assert.AreEqual(@"C:\Program Files\MyFinalBuilderPath\FBCMD.EXE", _task.FBCMDPath);
			Assert.AreEqual(100, _task.Timeout);		
		}

		[Test]
		public void PopulateFromMinimalConfiguration()
		{	
			const string xmlConfig = @"<FinalBuilder>
								<ProjectFile>C:\Dummy\Project.fbz5</ProjectFile>
								</FinalBuilder>";

			// Get the FB5 path from the FB registry not the XML configuration.
			_mockRegistry.ExpectAndReturn("GetLocalMachineSubKeyValue", Path.Combine(DefaultWorkingDirectory, "FinalBuilder5.exe"), @"SOFTWARE\VSoft\FinalBuilder\5.0", "Location");
					
			NetReflector.Read(xmlConfig, _task);

			Assert.AreEqual(@"C:\Dummy\Project.fbz5", _task.ProjectFile);
			Assert.AreEqual(5, _task.FBVersion);
			Assert.AreEqual(fbExecutable, _task.FBCMDPath);
		}

		[Test]
		public void AutodetectFB5Path()
		{
			_mockRegistry.ExpectAndReturn("GetLocalMachineSubKeyValue", Path.Combine(DefaultWorkingDirectory, "FinalBuilder5.exe"), @"SOFTWARE\VSoft\FinalBuilder\5.0", "Location");
			_task.ProjectFile = @"C:\Dummy\Project.fbz5";
			Assert.AreEqual(5, _task.FBVersion);
			Assert.AreEqual(fbExecutable, _task.FBCMDPath);
		}

		[Test]
		public void AutodetectFB4Path()
		{
			_mockRegistry.ExpectAndReturn("GetLocalMachineSubKeyValue", Path.Combine(DefaultWorkingDirectory, "FinalBuilder4.exe"), @"SOFTWARE\VSoft\FinalBuilder\4.0", "Location");
			_task.ProjectFile = @"C:\Dummy\Project.fbz4";
			Assert.AreEqual(4, _task.FBVersion);
			Assert.AreEqual(fbExecutable, _task.FBCMDPath);
		}

		[Test]
		public void AutodetectFB3Path()
		{
			_mockRegistry.ExpectAndReturn("GetLocalMachineSubKeyValue", Path.Combine(DefaultWorkingDirectory, "FinalBuilder4.exe"), @"SOFTWARE\VSoft\FinalBuilder\3.0", "Location");
			_task.ProjectFile = @"C:\Dummy\Project.fbz3";
			Assert.AreEqual(3, _task.FBVersion);
			Assert.AreEqual(Path.Combine(DefaultWorkingDirectory, "FB3Cmd.exe"), _task.FBCMDPath);
		}

        [Test]
        public void InvalidProjectFileName()
        {
            ExpectThatExecuteWillNotBeCalled();
            _mockRegistry.ExpectNoCall("GetLocalMachineSubKeyValue", new Type[] { typeof(string), typeof(string) });
            NetReflector.Read(@"<FinalBuilder>
				<ProjectFile>C:\Dummy\Project.txt</ProjectFile>
			</FinalBuilder>", _task);
            Assert.That(delegate { _task.Run(_result); },
                        Throws.TypeOf<BuilderException>().With.Message.EqualTo("Finalbuilder version could not be autodetected from project file name."));
        }

        [Test]
        public void FinalBuilderIsNotInstalled()
        {
            ExpectThatExecuteWillNotBeCalled();
            _mockRegistry.ExpectAndReturn("GetLocalMachineSubKeyValue", null, @"SOFTWARE\VSoft\FinalBuilder\5.0", "Location");
            _task.ProjectFile = @"C:\Dummy\Project.fbz5";
            Assert.That(delegate { _task.Run(_result); },
                        Throws.TypeOf<BuilderException>().With.Message.EqualTo("Path to Finalbuilder 5 command line executable could not be found."));
        }

		[Test]
		public void RequiredPropertiesNotProvided()
		{
            Assert.That(delegate { NetReflector.Read(@"<FinalBuilder />", _task); },
                        Throws.TypeOf<NetReflectorException>());
		}

        [Test]
        public void TemporaryLogFile()
        {
			string expectedArgs = @"/B /TL /P" + StringUtil.AutoDoubleQuoteString(Path.Combine(DefaultWorkingDirectory, "TestProject.fbz5")); 
            ExpectToExecuteArguments(expectedArgs);

			_mockRegistry.ExpectAndReturn("GetLocalMachineSubKeyValue", Path.Combine(DefaultWorkingDirectory, "FinalBuilder5.exe"), @"SOFTWARE\VSoft\FinalBuilder\5.0", "Location");

			_task.ProjectFile = Path.Combine(DefaultWorkingDirectory, "TestProject.fbz5");
            _task.UseTemporaryLogFile = true;
            _task.Timeout = 600;
            _task.Run(_result);

            Assert.AreEqual(1, _result.TaskResults.Count);
            Assert.AreEqual(IntegrationStatus.Success, _result.Status);
            Assert.AreEqual(ProcessResultOutput, _result.TaskOutput);
        }

        [Test]
        public void TemporaryLogFileOverridesDontLogToOutput()
        {
			string expectedArgs = @"/B /TL /P" + StringUtil.AutoDoubleQuoteString(Path.Combine(DefaultWorkingDirectory, "TestProject.fbz5"));
            ExpectToExecuteArguments(expectedArgs);

			_mockRegistry.ExpectAndReturn("GetLocalMachineSubKeyValue", Path.Combine(DefaultWorkingDirectory, "FinalBuilder5.exe"), @"SOFTWARE\VSoft\FinalBuilder\5.0", "Location");

			_task.ProjectFile = Path.Combine(DefaultWorkingDirectory, "TestProject.fbz5");
            _task.UseTemporaryLogFile = true;
            _task.DontWriteToLog = true;
            _task.Timeout = 600;
            _task.Run(_result);

            Assert.AreEqual(1, _result.TaskResults.Count);
            Assert.AreEqual(IntegrationStatus.Success, _result.Status);
            Assert.AreEqual(ProcessResultOutput, _result.TaskOutput);
        }

	}
}
