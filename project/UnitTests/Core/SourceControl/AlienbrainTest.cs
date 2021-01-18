using System;
using Exortech.NetReflector;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class AlienbrainTest
	{
		public const string EXECUTABLE = @"EXECUTABLE_DOES_NOT_EXIST\ab.exe";
		public const string INSTALLDIR = @"C:\Program Files\alienbrain";
		public const string SERVER = @"SERVER_DOES_NOT_EXIST";
		public const string DATABASE = @"DATABASE_DOES_NOT_EXIST";
		public const string USER = @"USER_DOES_NOT_EXIST";
		public const string PASSWORD = @"PASSWORD_DOES_NOT_EXIST";
		public const string PROJECT_PATH = @"ab://PATH_DOES_NOT_EXIST";
		public const string WORKDIR_PATH = @"C:\DOES_NOT_EXIST";
		public const string BRANCH_PATH = @"BRANCH_DOES_NOT_EXIST";
		public const string AUTO_GET_SOURCE = "false";
		public const string LABEL_ON_SUCCESS = "false";

		public static readonly string XML_STUB = @"<sourceControl type=""alienbrain"">
			<executable>{0}</executable>
			<server>{1}</server>
			<database>{2}</database>
			<username>{3}</username>
			<password>{4}</password>
			<project>{5}</project>
			<workingDirectory>{6}</workingDirectory>
			<branch>{7}</branch>
			<autoGetSource>{8}</autoGetSource>
			<labelOnSuccess>{9}</labelOnSuccess>
		</sourceControl>";

		public static readonly string XML_STUB_MINIMAL = @"<sourceControl type=""alienbrain"">
			<server>{0}</server>
			<database>{1}</database>
			<username>{2}</username>
			<password>{3}</password>
			<project>{4}</project>
		</sourceControl>";

		public static readonly string ALIENBRAIN_XML = string.Format(XML_STUB,
		                                                             EXECUTABLE,
		                                                             SERVER,
		                                                             DATABASE,
		                                                             USER,
		                                                             PASSWORD,
		                                                             PROJECT_PATH,
		                                                             WORKDIR_PATH,
		                                                             BRANCH_PATH,
		                                                             AUTO_GET_SOURCE,
		                                                             LABEL_ON_SUCCESS);

		public static readonly string ALIENBRAIN_XML_MINIMAL = string.Format(XML_STUB_MINIMAL,
		                                                                     SERVER,
		                                                                     DATABASE,
		                                                                     USER,
		                                                                     PASSWORD,
		                                                                     PROJECT_PATH);

		private Alienbrain alienbrain;
		private Mock<ProcessExecutor> executor;
		private Mock<IHistoryParser> parser;
		private Mock<IRegistry> registry;

		[SetUp]
		protected void Setup()
		{
			executor = new Mock<ProcessExecutor>();
			parser = new Mock<IHistoryParser>();
			registry = new Mock<IRegistry>();

			alienbrain = new Alienbrain((IHistoryParser) parser.Object, (ProcessExecutor) executor.Object, (IRegistry) registry.Object);
		}

// Process Creations

		[Test]
		public void CanCreateModificationProcess()
		{
			const string project = "ab://test_path";
			DateTime to = DateTime.Today;
			DateTime from = to.AddDays(-1);

			InitialiseAlienbrain();
			alienbrain.Project = project;
			ProcessInfo info = alienbrain.CreateModificationProcess(Alienbrain.MODIFICATIONS_COMMAND_TEMPLATE, from, to);
			Assert.AreEqual(EXECUTABLE + " " + String.Format(Alienbrain.MODIFICATIONS_COMMAND_TEMPLATE,
			                                                 project,
			                                                 SERVER,
			                                                 DATABASE,
			                                                 USER,
			                                                 PASSWORD,
			                                                 from.ToFileTime(), to.ToFileTime()),
			                info.FileName + " " + info.Arguments);
		}

		[Test]
		public void CanCreateLabelProcess()
		{
			const string name = "WORKING LABEL";
			const string project = "ab://test_path";

			InitialiseAlienbrain();
			alienbrain.Project = project;

			ProcessInfo info = alienbrain.CreateLabelProcess(Alienbrain.LABEL_COMMAND_TEMPLATE, IntegrationResultMother.CreateSuccessful(name));
			Assert.AreEqual(EXECUTABLE + " " + String.Format(Alienbrain.LABEL_COMMAND_TEMPLATE,
			                                                 project,
			                                                 SERVER,
			                                                 DATABASE,
			                                                 USER,
			                                                 PASSWORD,
			                                                 name),
			                info.FileName + " " + info.Arguments);
		}

		[Test]
		public void CanCreateGetProcess()
		{
			alienbrain.Executable = "ab.exe";
			alienbrain.Project = "ab://my project";
			alienbrain.Server = "s c m";
			alienbrain.Database = "d b";
			alienbrain.Username = "o r";
			alienbrain.Password = "p w";
			alienbrain.WorkingDirectory = "c:\\my source";
			ProcessInfo info = alienbrain.CreateGetProcess();
			Assert.AreEqual("ab.exe", info.FileName);
			Assert.AreEqual(
				@"getlatest ""ab://my project"" -s ""s c m"" -d ""d b"" -u ""o r"" -p ""p w"" -localpath ""c:\my source"" -overwritewritable replace -overwritecheckedout replace -response:GetLatest.PathInvalid y -response:GetLatest.Writable y -response:GetLatest.CheckedOut y", info.Arguments);
		}

		[Test]
		public void CanCreateGetProcessWithNoWorkingDirectory()
		{
			alienbrain.Executable = "ab.exe";
			alienbrain.Project = "ab://project";
			alienbrain.Server = "server";
			alienbrain.Database = "database";
			alienbrain.Username = "user";
			alienbrain.Password = "password";
			ProcessInfo info = alienbrain.CreateGetProcess();
			Assert.AreEqual("ab.exe", info.FileName);
			Assert.AreEqual(
				@"getlatest ab://project -s server -d database -u user -p password -overwritewritable replace -overwritecheckedout replace -response:GetLatest.PathInvalid y -response:GetLatest.Writable y -response:GetLatest.CheckedOut y", info.Arguments);
		}

		[Test]
		public void CanCreateBranchProcess()
		{
			InitialiseAlienbrain();
			const string name = "branch sample";
			alienbrain.Branch = name;

			ProcessInfo info = alienbrain.CreateBranchProcess(Alienbrain.BRANCH_COMMAND_TEMPLATE);
			Assert.AreEqual(EXECUTABLE + " " + String.Format(Alienbrain.BRANCH_COMMAND_TEMPLATE,
			                                                 name,
			                                                 SERVER,
			                                                 DATABASE,
			                                                 USER,
			                                                 PASSWORD),
			                info.FileName + " " + info.Arguments);
		}

// XML Tests
		[Test]
		public void ShouldPopulateCorrectlyFromXml()
		{
			NetReflector.Read(ALIENBRAIN_XML, alienbrain);
			Assert.AreEqual(EXECUTABLE, alienbrain.Executable);
			Assert.AreEqual(SERVER, alienbrain.Server);
			Assert.AreEqual(DATABASE, alienbrain.Database);
			Assert.AreEqual(USER, alienbrain.Username);
			Assert.AreEqual(PASSWORD, alienbrain.Password.PrivateValue);
			Assert.AreEqual(PROJECT_PATH, alienbrain.Project);
			Assert.AreEqual(WORKDIR_PATH, alienbrain.WorkingDirectory);
			Assert.AreEqual(BRANCH_PATH, alienbrain.Branch);
			Assert.AreEqual(Convert.ToBoolean(AUTO_GET_SOURCE), alienbrain.AutoGetSource);
			Assert.AreEqual(Convert.ToBoolean(LABEL_ON_SUCCESS), alienbrain.LabelOnSuccess);
		}

		[Test]
		public void ShouldPopulateCorrectlyFromMinimalXml()
		{
			NetReflector.Read(ALIENBRAIN_XML_MINIMAL, alienbrain);

			// Get Default Executable from registry
			registry.Setup(r => r.GetExpectedLocalMachineSubKeyValue(Alienbrain.AB_REGISTRY_PATH, Alienbrain.AB_REGISTRY_KEY)).Returns(INSTALLDIR).Verifiable();
			alienbrain.Executable = string.Empty;

			Assert.AreEqual(INSTALLDIR + System.IO.Path.DirectorySeparatorChar + Alienbrain.AB_COMMMAND_PATH + "\\" + Alienbrain.AB_EXE, alienbrain.Executable);
			Assert.AreEqual(SERVER, alienbrain.Server);
			Assert.AreEqual(DATABASE, alienbrain.Database);
			Assert.AreEqual(USER, alienbrain.Username);
			Assert.AreEqual(PASSWORD, alienbrain.Password.PrivateValue);
			Assert.AreEqual(PROJECT_PATH, alienbrain.Project);
			Assert.AreEqual(string.Empty, alienbrain.WorkingDirectory);
			Assert.AreEqual(string.Empty, alienbrain.Branch);
			Assert.AreEqual(Convert.ToBoolean(true), alienbrain.AutoGetSource);
			Assert.AreEqual(Convert.ToBoolean(false), alienbrain.LabelOnSuccess);
		}

		[Test]
		public void CanCatchInvalidGetSourceFlagConfiguration()
		{
			const string invalidXml = "<sourcecontrol type=\"alienbrain\"><autoGetSource>NOT_A_BOOLEAN</autoGetSource></sourcecontrol>";
			Assert.That(delegate { NetReflector.Read(invalidXml); },
                        Throws.TypeOf<NetReflectorException>());
		}

		[Test]
		public void CanCatchInvalidLabelOnSuccessConfiguration()
		{
			const string invalidXml = "<sourcecontrol type=\"alienbrain\"><labelOnSuccess>NOT_A_BOOLEAN</labelOnSuccess></sourcecontrol>";
            Assert.That(delegate { NetReflector.Read(invalidXml); },
                        Throws.TypeOf<NetReflectorException>());
		}

// Actions tests
		[Test]
		public void CanExecuteHasChanges()
		{
			InitialiseAlienbrain();
			ProcessInfo expectedProcessRequest = new ProcessInfo(EXECUTABLE, string.Format(Alienbrain.GET_COMMAND_TEMPLATE,
			                                                                               PROJECT_PATH,
			                                                                               SERVER,
			                                                                               DATABASE,
			                                                                               USER,
			                                                                               PASSWORD,
			                                                                               WORKDIR_PATH));
			expectedProcessRequest.TimeOut = Timeout.DefaultTimeout.Millis;

			executor.Setup(e => e.Execute(expectedProcessRequest)).Returns(new ProcessResult("foo", null, 0, false)).Verifiable();
			Assert.IsTrue(alienbrain.HasChanges(expectedProcessRequest));
			executor.Verify();
		}

		[Test]
		public void CanGetModifications()
		{
			DateTime todatetime = DateTime.Today;
			DateTime fromdatetime = todatetime.AddDays(-1);

			IIntegrationResult from = IntegrationResultMother.CreateSuccessful(fromdatetime);
			IIntegrationResult to = IntegrationResultMother.CreateSuccessful(todatetime);

			InitialiseAlienbrain();
			alienbrain.Branch = string.Empty;

			string args = string.Format(Alienbrain.MODIFICATIONS_COMMAND_TEMPLATE,
			                            PROJECT_PATH,
			                            SERVER,
			                            DATABASE,
			                            USER,
			                            PASSWORD,
			                            from.StartTime.ToFileTime(),
			                            to.StartTime.ToFileTime());
			ProcessInfo expectedProcessRequest = NewProcessInfo(args);

			executor.Setup(e => e.Execute(expectedProcessRequest)).Returns(new ProcessResult("foo", null, 0, false)).Verifiable();
			alienbrain.GetModifications(from, to);
			executor.Verify();
		}

		[Test]
		public void CanGetModificationsIfNoModsAreFound()
		{
			DateTime todatetime = DateTime.Today;
			DateTime fromdatetime = todatetime.AddDays(-1);

			IIntegrationResult from = IntegrationResultMother.CreateSuccessful(fromdatetime);
			IIntegrationResult to = IntegrationResultMother.CreateSuccessful(todatetime);

			InitialiseAlienbrain();
			alienbrain.Branch = string.Empty;

			string args = string.Format(Alienbrain.MODIFICATIONS_COMMAND_TEMPLATE,
			                            PROJECT_PATH,
			                            SERVER,
			                            DATABASE,
			                            USER,
			                            PASSWORD,
			                            from.StartTime.ToFileTime(),
			                            to.StartTime.ToFileTime());
			ProcessInfo expectedProcessRequest = NewProcessInfo(args);

			executor.Setup(e => e.Execute(expectedProcessRequest)).Returns(new ProcessResult(Alienbrain.NO_CHANGE, null, 1, false)).Verifiable();
			alienbrain.GetModifications(from, to);
			executor.Verify();
		}

		[Test]
		public void ShouldLabelSourceControlifLabelOnSuccessisTrueAndResultisSuccess()
		{
			string name = "VALID_LABEL_NAME";
			InitialiseAlienbrain();
			alienbrain.LabelOnSuccess = true;

			string args = string.Format(Alienbrain.LABEL_COMMAND_TEMPLATE,
			                            PROJECT_PATH,
			                            SERVER,
			                            DATABASE,
			                            USER,
			                            PASSWORD,
			                            name);
			ProcessInfo expectedProcessRequest = NewProcessInfo(args);

			executor.Setup(e => e.Execute(expectedProcessRequest)).Returns(new ProcessResult("foo", null, 0, false)).Verifiable();
			alienbrain.LabelSourceControl(IntegrationResultMother.CreateSuccessful(name));
			executor.Verify();
		}

		[Test]
		public void ShouldNotLabelSourceControlifLabelOnSuccessisTrueAndResultisFailed()
		{
			alienbrain.LabelOnSuccess = true;

			alienbrain.LabelSourceControl(IntegrationResultMother.CreateFailed());
			executor.Verify();
			executor.VerifyNoOtherCalls();
		}

		[Test]
		public void ShouldNotLabelSourceControlifLabelOnSuccessisFalseAndResultisSuccess()
		{
			alienbrain.LabelOnSuccess = false;

			alienbrain.LabelSourceControl(IntegrationResultMother.CreateSuccessful());
			executor.Verify();
			executor.VerifyNoOtherCalls();
		}

		[Test]
		public void ShouldGetSourceIfAutoGetSourceTrue()
		{
			InitialiseAlienbrain();
			alienbrain.AutoGetSource = true;

			string args =
				@"getlatest ab://PATH_DOES_NOT_EXIST -s SERVER_DOES_NOT_EXIST -d DATABASE_DOES_NOT_EXIST -u USER_DOES_NOT_EXIST -p PASSWORD_DOES_NOT_EXIST -localpath C:\DOES_NOT_EXIST -overwritewritable replace -overwritecheckedout replace -response:GetLatest.PathInvalid y -response:GetLatest.Writable y -response:GetLatest.CheckedOut y";
			ProcessInfo expectedProcessRequest = NewProcessInfo(args);

			executor.Setup(e => e.Execute(expectedProcessRequest)).Returns(new ProcessResult("foo", null, 0, false)).Verifiable();
			alienbrain.GetSource(new IntegrationResult());
			executor.Verify();
		}

		[Test]
		public void ShouldNotGetSourceIfAutoGetSourceFalse()
		{
			alienbrain.AutoGetSource = false;

			alienbrain.GetSource(new IntegrationResult());
			executor.Verify();
			executor.VerifyNoOtherCalls();
		}

		private void InitialiseAlienbrain()
		{
			alienbrain.Project = PROJECT_PATH;
			alienbrain.Executable = EXECUTABLE;
			alienbrain.Server = SERVER;
			alienbrain.Database = DATABASE;
			alienbrain.Username = USER;
			alienbrain.Password = PASSWORD;
			alienbrain.WorkingDirectory = WORKDIR_PATH;
		}

		private ProcessInfo NewProcessInfo(string args)
		{
			ProcessInfo expectedProcessRequest = new ProcessInfo(EXECUTABLE, args);
			expectedProcessRequest.TimeOut = Timeout.DefaultTimeout.Millis;
			return expectedProcessRequest;
		}
	}
}