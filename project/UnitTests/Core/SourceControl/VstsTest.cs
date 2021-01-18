using System;
using System.Globalization;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
    [TestFixture]
    public class VstsTest: ProcessExecutorTestFixtureBase
    {
        private IRegistry mockRegistry;
        private VstsHistoryParser historyParser;
        private Vsts vsts;
        private DateTime today;
        private DateTime yesterday;

        private const string fakeTfsPath = "http://faketfs:8080";
        private const string fakeProjectPath = "$/TeamProjectName/path";
        private const string fakeUsername = "username";
        private const string fakePassword = "password";

        private class RegistryStub: IRegistry
        {
            private const string DEFAULT_VS2013_EXE_PATH = @"Software\Wow6432Node\Microsoft\VisualStudio\12.0";

            public string GetLocalMachineSubKeyValue(string path, string name)
            {
                if(path == DEFAULT_VS2013_EXE_PATH && name == "InstallDir")
                {
                    return "mockPath";
                }
                throw new NotImplementedException();
            }

            public string GetExpectedLocalMachineSubKeyValue(string path, string name)
            {
                throw new NotImplementedException();
            }
        }

        [SetUp]
        public void SetUp()
        {

            CreateProcessExecutorMock(System.IO.Path.Combine("mockPath", "TF.exe"));
            mockRegistry = new RegistryStub();
            historyParser = new VstsHistoryParser();

            vsts = new Vsts((ProcessExecutor)mockProcessExecutor.Object, historyParser, mockRegistry);
            vsts.Username = fakeUsername;
            vsts.Password = fakePassword;
            vsts.WorkingDirectory = DefaultWorkingDirectory;
            vsts.Server = fakeTfsPath;
            vsts.ProjectPath = fakeProjectPath;

            today = DateTime.Now;
            yesterday = today.AddDays(-1);
        }

        [TearDown]
        public void TearDown()
        {
            Verify();
        }

        [Test]
        public void VerifyGetModificationsProcessInfoArguments()
        {
            IntegrationResult from = IntegrationResultMother.CreateSuccessful(yesterday);
            IntegrationResult to = IntegrationResultMother.CreateSuccessful(today);
  
            ExpectToExecuteArguments(string.Format(System.Globalization.CultureInfo.CurrentCulture,"dir /folders /server:{0} \"{1}\" /login:{2},{3}",
                                                   fakeTfsPath,
                                                   fakeProjectPath,
                                                   fakeUsername,
                                                   fakePassword));

            ExpectToExecuteArguments(string.Format(System.Globalization.CultureInfo.CurrentCulture,"history -noprompt -server:{0} \"{1}\" -version:D{2}~D{3} -recursive -format:detailed /login:{4},{5}",
                                                   fakeTfsPath,
                                                   fakeProjectPath,
                                                   FormatCommandDate(from.StartTime),
                                                   FormatCommandDate(to.StartTime),
                                                   fakeUsername,
                                                   fakePassword));

            vsts.GetModifications(from, to);
        }

        [Test]
        public void VerifyGetModificationsProcessInfoArgumentsWhenDomainIsSpecified()
        {
            IntegrationResult from = IntegrationResultMother.CreateSuccessful(yesterday);
            IntegrationResult to = IntegrationResultMother.CreateSuccessful(today);

            vsts.Domain = "testDomain";
            string expectedUsername = vsts.Domain + "\\" + fakeUsername;

            ExpectToExecuteArguments(string.Format(System.Globalization.CultureInfo.CurrentCulture,"dir /folders /server:{0} \"{1}\" /login:{2},{3}",
                                                   fakeTfsPath,
                                                   fakeProjectPath,
                                                   expectedUsername,
                                                   fakePassword));

            ExpectToExecuteArguments(string.Format(System.Globalization.CultureInfo.CurrentCulture,"history -noprompt -server:{0} \"{1}\" -version:D{2}~D{3} -recursive -format:detailed /login:{4},{5}",
                                                   fakeTfsPath,
                                                   fakeProjectPath,
                                                   FormatCommandDate(from.StartTime),
                                                   FormatCommandDate(to.StartTime),
                                                   expectedUsername,
                                                   fakePassword));

            vsts.GetModifications(from, to);
        }

        [Test]
        public void VerifyGetSourceProcessInfoArguments()
        {
            IntegrationResult result = new IntegrationResult("testProject", "testWorkingDir", "testArtifactDir",
                                                             new IntegrationRequest(BuildCondition.ForceBuild,"testSource", "testUsername"),
                                                             new IntegrationSummary(IntegrationStatus.Unknown,"testLabel", "testLastSuccessfulLabel", DateTime.Now));
            vsts.AutoGetSource = true;
            vsts.Domain = "testDomain";
            vsts.Force = true;
            string expectedUsername = vsts.Domain + "\\" + fakeUsername;

            ExpectToExecuteArguments(string.Format(System.Globalization.CultureInfo.CurrentCulture,"dir /folders /server:{0} \"{1}\" /login:{2},{3}",
                                                   fakeTfsPath,
                                                   fakeProjectPath,
                                                   expectedUsername,
                                                   fakePassword));

            ExpectToExecuteArguments(string.Format(System.Globalization.CultureInfo.CurrentCulture,"workspaces /computer:{1} -server:{0} /format:detailed \"CCNET\" /login:{2},{3}",
                                                   fakeTfsPath,
                                                   Environment.MachineName,
                                                   expectedUsername,
                                                   fakePassword));

            ExpectToExecuteArguments(string.Format(@"workfold /map ""{0}"" ""{1}"" /server:{2} /workspace:CCNET /login:{3},{4}",
                                                   fakeProjectPath,
                                                   DefaultWorkingDirectory,
                                                   fakeTfsPath,
                                                   expectedUsername,
                                                   fakePassword));

            string getCommand = string.Format(System.Globalization.CultureInfo.CurrentCulture,"get /recursive /noprompt /force \"{0}\" /login:{1},{2}",
                                                   DefaultWorkingDirectory,
                                                   expectedUsername,
                                                   fakePassword);

            ProcessInfo info = NewProcessInfo(getCommand, DefaultWorkingDirectory);
            info.TimeOut = 3600000;
            ExpectToExecute(info);

            
            vsts.GetSource(result);
        }

        [Test]
        public void VerifyGetSourceProcessInfoArgumentsWhenForceIsFalse()
        {
            IntegrationResult result = new IntegrationResult("testProject", "testWorkingDir", "testArtifactDir",
                                                             new IntegrationRequest(BuildCondition.ForceBuild,"testSource", "testUsername"),
                                                             new IntegrationSummary(IntegrationStatus.Unknown,"testLabel", "testLastSuccessfulLabel", DateTime.Now));
            vsts.AutoGetSource = true;
            vsts.Domain = "testDomain";
            string expectedUsername = vsts.Domain + "\\" + fakeUsername;

            ExpectToExecuteArguments(string.Format(System.Globalization.CultureInfo.CurrentCulture,"dir /folders /server:{0} \"{1}\" /login:{2},{3}",
                                                   fakeTfsPath,
                                                   fakeProjectPath,
                                                   expectedUsername,
                                                   fakePassword));

            ExpectToExecuteArguments(string.Format(System.Globalization.CultureInfo.CurrentCulture,"workspaces /computer:{1} -server:{0} /format:detailed \"CCNET\" /login:{2},{3}",
                                                   fakeTfsPath,
                                                   Environment.MachineName,
                                                   expectedUsername,
                                                   fakePassword));

            ExpectToExecuteArguments(string.Format(@"workfold /map ""{0}"" ""{1}"" /server:{2} /workspace:CCNET /login:{3},{4}",
                                                   fakeProjectPath,
                                                   DefaultWorkingDirectory,
                                                   fakeTfsPath,
                                                   expectedUsername,
                                                   fakePassword));

            string getCommand = string.Format(System.Globalization.CultureInfo.CurrentCulture,"get /recursive /noprompt \"{0}\" /login:{1},{2}",
                                                   DefaultWorkingDirectory,
                                                   expectedUsername,
                                                   fakePassword);

            ProcessInfo info = NewProcessInfo(getCommand, DefaultWorkingDirectory);
            info.TimeOut = 3600000;
            ExpectToExecute(info);

            
            vsts.GetSource(result);
        }

        [Test]
        public void VerifyGetSourceProcessInfoArgumentsWhenDomainIsSpecified()
        {
            IntegrationResult result = new IntegrationResult("testProject", "testWorkingDir", "testArtifactDir",
                                                             new IntegrationRequest(BuildCondition.ForceBuild,"testSource", "testUsername"),
                                                             new IntegrationSummary(IntegrationStatus.Unknown,"testLabel", "testLastSuccessfulLabel", DateTime.Now));
            vsts.AutoGetSource = true;
            vsts.Force = true;

            ExpectToExecuteArguments(string.Format(System.Globalization.CultureInfo.CurrentCulture,"dir /folders /server:{0} \"{1}\" /login:{2},{3}",
                                                   fakeTfsPath,
                                                   fakeProjectPath,
                                                   fakeUsername,
                                                   fakePassword));

            ExpectToExecuteArguments(string.Format(System.Globalization.CultureInfo.CurrentCulture,"workspaces /computer:{1} -server:{0} /format:detailed \"CCNET\" /login:{2},{3}",
                                                   fakeTfsPath,
                                                   Environment.MachineName,
                                                   fakeUsername,
                                                   fakePassword));

            ExpectToExecuteArguments(string.Format(@"workfold /map ""{0}"" ""{1}"" /server:{2} /workspace:CCNET /login:{3},{4}",
                                                   fakeProjectPath,
                                                   DefaultWorkingDirectory,
                                                   fakeTfsPath,
                                                   fakeUsername,
                                                   fakePassword));

            string getCommand = string.Format(System.Globalization.CultureInfo.CurrentCulture,"get /recursive /noprompt /force \"{0}\" /login:{1},{2}",
                                                   DefaultWorkingDirectory,
                                                   fakeUsername,
                                                   fakePassword);

            ProcessInfo info = NewProcessInfo(getCommand, DefaultWorkingDirectory);
            info.TimeOut = 3600000;
            ExpectToExecute(info);

            
            vsts.GetSource(result);
        }

        private static string FormatCommandDate(DateTime date)
        {
            return date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        }

    }
}
