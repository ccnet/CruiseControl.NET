using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using tw.ccnet.core.util;
using Exortech.NetReflector;
using System.Reflection;

namespace tw.ccnet.core.builder.test
{
	[TestFixture]
	public class NAntBuilderTest
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
		public void TestPopulateFromReflector()
		{
			string xml = String.Format(@"
    <build>
    	<executable>{0}</executable>
    	<baseDirectory>{1}</baseDirectory>
    	<buildFile>{2}</buildFile>
		<targetList>
      		<target>{3}</target>
    	</targetList>
    </build>", NANT_TEST_EXECUTABLE, NANT_TEST_BASEDIR, NANT_TEST_BUILDFILE, NANT_TEST_TARGET);

			XmlNode node = XmlUtil.CreateDocumentElement(xml);
			Assertion.AssertNotNull("Node is null", node);
			XmlPopulator populator = new XmlPopulator();
			populator.Reflector.AddReflectorTypes(Assembly.GetExecutingAssembly());
			populator.Populate(node, _builder);
			Assertion.AssertEquals(NANT_TEST_BASEDIR, _builder.BaseDirectory);
			Assertion.AssertEquals(NANT_TEST_BUILDFILE, _builder.BuildFile);
			Assertion.AssertEquals(NANT_TEST_EXECUTABLE, _builder.Executable);
			Assertion.AssertEquals(1, _builder.Targets.Length);
			Assertion.AssertEquals(NANT_TEST_TARGET, _builder.Targets[0]);
		}

		public void TestExecuteCommand() 
		{
			string tempFile = TempFileUtil.CreateTempFile(TEMP_DIR, "testexecute.bat", "echo hello martin");
			_builder.Executable = tempFile;
			_builder.BaseDirectory = TempFileUtil.GetTempPath(TEMP_DIR);
			Assertion.Assert(tempFile + " does not exist!", File.Exists(tempFile));
			string expected = "hello martin";

			IntegrationResult result = new IntegrationResult();
			_builder.Build(result);
			string errorMessage = String.Format("{0} not contained in {1}",expected, result.Output);
			Assertion.Assert(errorMessage, StringUtil.StringContains(result.Output.ToString(), expected));
		}
		
		[ExpectedException(typeof(BuilderException))]
		public void TestExecuteCommandWithInvalidFile()
		{
			// simulate nant missing
			_builder.Executable = @"\nodir\invalidfile.bat";
			_builder.BuildArgs = "";
			_builder.BaseDirectory = TempFileUtil.GetTempPath(TEMP_DIR);
			Assertion.Assert("invalidfile.bat should not exist!",! File.Exists(_builder.Executable));
			_builder.Build(new IntegrationResult());
		}
		
		public void TestBuildSucceed()
		{
			CreateTestBuildFile();
			_builder.Executable = NANT_TEST_EXECUTABLE;
			_builder.BuildFile = "test.build";
			_builder.BaseDirectory = TempFileUtil.GetTempPath(TEMP_DIR);
			_builder.Targets = new string[] {"success" };
			IntegrationResult result = new IntegrationResult();
			_builder.Build(result);
			
			Assertion.Assert("test build should succeed", result.Succeeded);
			Assertion.Assert(StringUtil.StringContains(result.Output.ToString(), "I am success itself"));
		}
		
		public void TestBuildFailed()
		{
			CreateTestBuildFile();
			_builder.Executable = NANT_TEST_EXECUTABLE;
			_builder.BuildFile = "test.build";
			_builder.BaseDirectory = TempFileUtil.GetTempPath(TEMP_DIR);
			_builder.Targets = new string[] {"fail" };
			IntegrationResult result = new IntegrationResult();
			_builder.Build(result);

			Assertion.Assert("test build should fail", result.Failed);
			Assertion.Assert(StringUtil.StringContains(result.Output.ToString(), "I am failure itself"));
		}

		[ExpectedException(typeof(BuilderException))]
		public void TestBuildWithInvalidBuildFile()
		{
			// simulate missing build file
			_builder.Executable = NANT_TEST_EXECUTABLE;
			_builder.BaseDirectory = TempFileUtil.GetTempPath(TEMP_DIR);
			_builder.BuildArgs = "";
			_builder.BuildFile = "invalidbuildfile";
			IntegrationResult result = new IntegrationResult();
			_builder.Build(result);
			Assertion.Fail("Build should fail when invoked with missing buildfile, but didn't!");
		}

		public void TestCreateBuildArgs()
		{
			_builder.BuildFile = "foo.xml";
			_builder.BuildArgs = "-bar";
			_builder.Targets = new string[] {"a", "b"};
			Assertion.AssertEquals("-buildfile:foo.xml -bar a b", _builder.CreateArgs());
		}

		public void TestCreateBuildArgs_MissingArguments()
		{
			Assertion.AssertEquals("-buildfile: -logger:SourceForge.NAnt.XmlLogger ", _builder.CreateArgs());
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

</project>";
			return TempFileUtil.CreateTempFile(TEMP_DIR, "test.build", contents);
		}
	}
}