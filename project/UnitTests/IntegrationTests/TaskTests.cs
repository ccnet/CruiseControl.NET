using System;
using NUnit.Framework;
using CCNet = ThoughtWorks.CruiseControl;


namespace ThoughtWorks.CruiseControl.UnitTests.IntegrationTests
{
    /// <summary>
    /// These are integration tests, so we use the real classes of CCNet as much as possible
    /// Mocks should not be used as we want to test the real classes.
    /// (mocks like an e-mail server are ok)
    /// </summary>
    [TestFixture]
    [Category("Integration")]
    public class TaskTests
    {

        [TestFixtureSetUp]
        public void fixLog4Net()
        {
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("test.config"));
        }

        System.Collections.Generic.Dictionary<string, bool> IntegrationCompleted = new System.Collections.Generic.Dictionary<string, bool>();


        [Test]
        public void RunSomeTasksInParallel()
        {
            const string ProjectName1 = "Task01";

            string IntegrationFolder = System.IO.Path.Combine("scenarioTests", ProjectName1);
            string CCNetConfigFile = System.IO.Path.Combine("IntegrationScenarios", "ParallelTaskTest01.xml");
            string ProjectStateFile = new System.IO.FileInfo(ProjectName1 + ".state").FullName;

            IntegrationCompleted = new System.Collections.Generic.Dictionary<string, bool>();

            string workingDirectory = "TaskTest01";

            var ios = new CCNet.Core.Util.IoService();
            ios.DeleteIncludingReadOnlyObjects(workingDirectory);

            IntegrationCompleted.Add(ProjectName1, false);

            Log("Clear existing state file, to simulate first run : " + ProjectStateFile);
            System.IO.File.Delete(ProjectStateFile);

            Log("Clear integration folder to simulate first run");
            if (System.IO.Directory.Exists(IntegrationFolder)) System.IO.Directory.Delete(IntegrationFolder, true);


            CCNet.Remote.Messages.ProjectStatusResponse psr;
            CCNet.Remote.Messages.ProjectRequest pr1 = new CCNet.Remote.Messages.ProjectRequest(null, ProjectName1);


            Log("Making CruiseServerFactory");
            CCNet.Core.CruiseServerFactory csf = new CCNet.Core.CruiseServerFactory();

            Log("Making cruiseServer with config from :" + CCNetConfigFile);
            using (var cruiseServer = csf.Create(true, CCNetConfigFile))
            {

                // subscribe to integration complete to be able to wait for completion of a build
                cruiseServer.IntegrationCompleted += new EventHandler<ThoughtWorks.CruiseControl.Remote.Events.IntegrationCompletedEventArgs>(CruiseServerIntegrationCompleted);

                Log("Starting cruiseServer");
                cruiseServer.Start();

                System.Threading.Thread.Sleep(250); // give time to start

                Log("Forcing build");
                CheckResponse(cruiseServer.ForceBuild(pr1));

                System.Threading.Thread.Sleep(250); // give time to start the build

                Log("Waiting for integration to complete");
                while (!IntegrationCompleted[ProjectName1])
                {
                    for (int i = 1; i <= 4; i++) System.Threading.Thread.Sleep(250);
                    Log(" waiting ...");
                }

                // un-subscribe to integration complete 
                cruiseServer.IntegrationCompleted -= new EventHandler<ThoughtWorks.CruiseControl.Remote.Events.IntegrationCompletedEventArgs>(CruiseServerIntegrationCompleted);

                Log("getting project status");
                psr = cruiseServer.GetProjectStatus(pr1);
                CheckResponse(psr);

                Log("Stopping cruiseServer");
                cruiseServer.Stop();

                Log("waiting for cruiseServer to stop");
                cruiseServer.WaitForExit(pr1);
                Log("cruiseServer stopped");

            }

            Log("Checking the data");

        }

        [Test]
        public void RunNantTaskWithArgumentsOnSingleAndMultiLines()
        {
            const string ProjectName1 = "NantTest01";

            string IntegrationFolder = System.IO.Path.Combine("scenarioTests", ProjectName1);
            string CCNetConfigFile = System.IO.Path.Combine("IntegrationScenarios", "NantTest01.xml");
            string ProjectStateFile = new System.IO.FileInfo(ProjectName1 + ".state").FullName;

            IntegrationCompleted = new System.Collections.Generic.Dictionary<string, bool>();

            string workingDirectory = "Nant01";

            var ios = new CCNet.Core.Util.IoService();
            ios.DeleteIncludingReadOnlyObjects(workingDirectory);
            System.IO.Directory.CreateDirectory(workingDirectory);

            string NantBuildFile = @"IntegrationScenarios\Nant.Build";
            var NantExeLocation = "";

#if DEBUG
            NantExeLocation = @"..\..\..\..\Tools\Nant\nant.exe";
#else
            NantExeLocation = @"..\..\Tools\Nant\nant.exe";
#endif
            var configFileData = System.IO.File.ReadAllText(CCNetConfigFile);
            configFileData = configFileData.Replace("WillBeReplacedViaTheTest", NantExeLocation);
            System.IO.File.WriteAllText(CCNetConfigFile, configFileData);

            System.IO.File.Copy(NantBuildFile, System.IO.Path.Combine(workingDirectory, new System.IO.FileInfo(NantBuildFile).Name));



            IntegrationCompleted.Add(ProjectName1, false);

            Log("Clear existing state file, to simulate first run : " + ProjectStateFile);
            System.IO.File.Delete(ProjectStateFile);

            Log("Clear integration folder to simulate first run");
            if (System.IO.Directory.Exists(IntegrationFolder)) System.IO.Directory.Delete(IntegrationFolder, true);


            CCNet.Remote.Messages.ProjectStatusResponse psr;
            CCNet.Remote.Messages.ProjectRequest pr1 = new CCNet.Remote.Messages.ProjectRequest(null, ProjectName1);


            Log("Making CruiseServerFactory");
            CCNet.Core.CruiseServerFactory csf = new CCNet.Core.CruiseServerFactory();

            Log("Making cruiseServer with config from :" + CCNetConfigFile);
            using (var cruiseServer = csf.Create(true, CCNetConfigFile))
            {

                // subscribe to integration complete to be able to wait for completion of a build
                cruiseServer.IntegrationCompleted += new EventHandler<ThoughtWorks.CruiseControl.Remote.Events.IntegrationCompletedEventArgs>(CruiseServerIntegrationCompleted);

                Log("Starting cruiseServer");
                cruiseServer.Start();

                System.Threading.Thread.Sleep(250); // give time to start

                Log("Forcing build");
                CheckResponse(cruiseServer.ForceBuild(pr1));

                System.Threading.Thread.Sleep(250); // give time to start the build

                Log("Waiting for integration to complete");
                while (!IntegrationCompleted[ProjectName1])
                {
                    for (int i = 1; i <= 4; i++) System.Threading.Thread.Sleep(250);
                    Log(" waiting ...");
                }

                // un-subscribe to integration complete 
                cruiseServer.IntegrationCompleted -= new EventHandler<ThoughtWorks.CruiseControl.Remote.Events.IntegrationCompletedEventArgs>(CruiseServerIntegrationCompleted);

                Log("getting project status");
                psr = cruiseServer.GetProjectStatus(pr1);
                CheckResponse(psr);

                Log("Stopping cruiseServer");
                cruiseServer.Stop();

                Log("waiting for cruiseServer to stop");
                cruiseServer.WaitForExit(pr1);
                Log("cruiseServer stopped");

            }

            Log("Checking the data");
            CCNet.Remote.ProjectStatus ps = null;

            // checking data of project 1
            foreach (var p in psr.Projects)
            {
                if (p.Name == ProjectName1) ps = p;
            }

            Assert.AreEqual(ProjectName1, ps.Name);
            Assert.AreEqual(CCNet.Remote.IntegrationStatus.Success, ps.BuildStatus, "wrong build state for project " + ProjectName1);

        }






        void CruiseServerIntegrationCompleted(object sender, CCNet.Remote.Events.IntegrationCompletedEventArgs e)
        {
            Log(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Integration complete. Project {0} ", e.ProjectName));
            IntegrationCompleted[e.ProjectName] = true;
        }

        private void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture, "--> {0} {1}", DateTime.Now.ToUniversalTime(), message));
        }

        private void CheckResponse(ThoughtWorks.CruiseControl.Remote.Messages.Response value)
        {
            if (value.Result == ThoughtWorks.CruiseControl.Remote.Messages.ResponseResult.Failure)
            {
                string message = "Request has failed on the server:" + System.Environment.NewLine +
                    value.ConcatenateErrors();
                throw new CCNet.Core.CruiseControlException(message);
            }
        }

    }
}
