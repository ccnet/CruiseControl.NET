using Exortech.NetReflector;
using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;
using System.Xml;
using tw.ccnet.core.util;
using tw.ccnet.remote;

namespace tw.ccnet.core.builder.test
{
	[TestFixture]
	public class NAntBuilderTest : CustomAssertion
	{
		public const string TEMP_DIR = "NAntBuilderTest";
		public static readonly string NANT_TEST_BASEDIR = ".";

		public static readonly string NANT_TEST_EXECUTABLE = @"..\tools\nant\nant.exe";

		public static readonly string NANT_TEST_BUILDFILE = TempFileUtil.GetTempFilePath("nant-build-temp", "test.build");
		public static readonly string NANT_TEST_TARGET = "success";

		private NAntBuilder _builder;

		[SetUp]
		public void SetUp()
		{
			_builder = new NAntBuilder();
		}

		[TearDown]
		public void TearDown()
		{
			TempFileUtil.DeleteTempDir(TEMP_DIR);
		}

		[Test]
		public void PopulateFromReflector()
		{
			string xml = string.Format(@"
    <build>
    	<executable>{0}</executable>
    	<baseDirectory>{1}</baseDirectory>
    	<buildFile>{2}</buildFile>
		<targetList>
      		<target>{3}</target>
    	</targetList>
    </build>", NANT_TEST_EXECUTABLE, NANT_TEST_BASEDIR, NANT_TEST_BUILDFILE, NANT_TEST_TARGET);

			XmlNode node = XmlUtil.CreateDocumentElement(xml);
			AssertNotNull("Node is null", node);
			XmlPopulator populator = new XmlPopulator();
			populator.Reflector.AddReflectorTypes(Assembly.GetExecutingAssembly());
			populator.Populate(node, _builder);
			AssertEquals(NANT_TEST_BASEDIR, _builder.BaseDirectory);
			AssertEquals(NANT_TEST_BUILDFILE, _builder.BuildFile);
			AssertEquals(NANT_TEST_EXECUTABLE, _builder.Executable);
			AssertEquals(1, _builder.Targets.Length);
			AssertEquals(NANT_TEST_TARGET, _builder.Targets[0]);
		}

		[Test]
		public void ExecuteCommand() 
		{
			string tempFile = TempFileUtil.CreateTempFile(TEMP_DIR, "testexecute.bat", "echo hello martin");
			_builder.Executable = tempFile;
			_builder.BaseDirectory = TempFileUtil.GetTempPath(TEMP_DIR);
			Assert(tempFile + " does not exist!", File.Exists(tempFile));
			string expected = "hello martin";

			IntegrationResult result = new IntegrationResult();
			_builder.Run(result);
			string errorMessage = string.Format("{0} not contained in {1}",expected, result.Output);
			Assert(errorMessage, StringUtil.StringContains(result.Output.ToString(), expected));
		}
		
		[Test, ExpectedException(typeof(BuilderException))]
		public void ExecuteCommandWithInvalidFile()
		{
			// simulate nant missing
			_builder.Executable = @"\nodir\invalidfile.bat";
			_builder.BuildArgs = "";
			_builder.BaseDirectory = TempFileUtil.GetTempPath(TEMP_DIR);
			Assert("invalidfile.bat should not exist!",! File.Exists(_builder.Executable));
			_builder.Run(new IntegrationResult());
		}
		
		[Test]
		public void BuildSucceed()
		{
			CreateTestBuildFile();
			_builder.Executable = NANT_TEST_EXECUTABLE;
			_builder.BuildFile = "test.build";
			_builder.BaseDirectory = TempFileUtil.GetTempPath(TEMP_DIR);
			_builder.Targets = new string[] {"success" };
			IntegrationResult result = new IntegrationResult();
			_builder.Run(result);
			
			Assert("test build should succeed", result.Succeeded);
			Assert(StringUtil.StringContains(result.Output.ToString(), "I am success itself"));
		}
		
		[Test]
		public void BuildFailed()
		{
			CreateTestBuildFile();
			_builder.Executable = NANT_TEST_EXECUTABLE;
			_builder.BuildFile = "test.build";
			_builder.BaseDirectory = TempFileUtil.GetTempPath(TEMP_DIR);
			_builder.Targets = new string[] {"fail" };
			IntegrationResult result = new IntegrationResult();
			_builder.Run(result);

			Assert("test build should fail", result.Failed);
			Assert(StringUtil.StringContains(result.Output.ToString(), "I am failure itself"));
		}

		[Test, ExpectedException(typeof(BuilderException))]
		public void BuildWithInvalidBuildFile()
		{
			// simulate missing build file
			_builder.Executable = NANT_TEST_EXECUTABLE;
			_builder.BaseDirectory = TempFileUtil.GetTempPath(TEMP_DIR);
			_builder.BuildArgs = "";
			_builder.BuildFile = "invalidbuildfile";
			IntegrationResult result = new IntegrationResult();
			_builder.Run(result);
			Fail("Build should fail when invoked with missing buildfile, but didn't!");
		}

		[Test]
		public void CreateBuildArgs()
		{
			_builder.BuildFile = "foo.xml";
			_builder.BuildArgs = "-bar";
			_builder.LabelToApply = "1234";
			_builder.Targets = new string[] {"a", "b"};
			AssertEquals("-buildfile:foo.xml -bar -D:label-to-apply=1234 a b", _builder.CreateArgs());
		}

		[Test]
		public void CreateBuildArgs_MissingArguments()
		{
			AssertEquals("-buildfile: -logger:NAnt.Core.XmlLogger -D:label-to-apply=NO-LABEL ", _builder.CreateArgs());
		}

		[Test]
		public void LabelGetsPassedThrough() 
		{
			CreateTestBuildFile();
			_builder.Executable = NANT_TEST_EXECUTABLE;
			_builder.BuildFile = "test.build";
			_builder.BaseDirectory = TempFileUtil.GetTempPath(TEMP_DIR);
			_builder.Targets = new string[] {"checkLabel" };
			IntegrationResult result = new IntegrationResult();
			result.Label = "ATestLabel";
			_builder.Run(result);
			
			Assert("test build should succeed", result.Succeeded);
			Assert(StringUtil.StringContains(result.Output.ToString(), "ATestLabel"));
		}

		[Test]
		public void ShouldRun()
		{
			AssertFalse(_builder.ShouldRun(new IntegrationResult()));
			Assert(_builder.ShouldRun(CreateIntegrationResultWithModifications(IntegrationStatus.Unknown)));
			Assert(_builder.ShouldRun(CreateIntegrationResultWithModifications(IntegrationStatus.Success)));
			AssertFalse(_builder.ShouldRun(CreateIntegrationResultWithModifications(IntegrationStatus.Failure)));
			AssertFalse(_builder.ShouldRun(CreateIntegrationResultWithModifications(IntegrationStatus.Exception)));
		}

		private IntegrationResult CreateIntegrationResultWithModifications(IntegrationStatus status)
		{
			IntegrationResult result = new IntegrationResult();
			result.Status = status;
			result.Modifications = new Modification[] { new Modification() };
			return result;
		}
		
		private string CreateTestBuildFile()
		{
			string contents =  
@"<project name=""ccnetlaunch"" default=""all"">

  <target name=""all"">
  	<echo message=""Build Succeeded""/>
  </target>

  <target name=""success"">
    <echo message=""I am success itself""/>
  </target>

  <target name=""fail"">
    <echo message=""I am failure itself""/>
    <fail message=""Intentional failure for test purposes, that is to say, purposes of testing, if you will""/>
  </target>

  <target name=""checkLabel"">
    <echo message=""${label-to-apply}"" />
  </target>

</project>";
			return TempFileUtil.CreateTempFile(TEMP_DIR, "test.build", contents);
		}
	}
}