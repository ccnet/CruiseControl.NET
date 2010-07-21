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
    public class SimpleScenarioTests
    {
        System.Collections.Generic.Dictionary<string, bool> IntegrationCompleted = new System.Collections.Generic.Dictionary<string, bool>();

        [TestFixtureSetUp]
        public void fixLog4Net()
        {
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("test.config"));
        }

        [Test]
        public void ForceBuildOf1ProjectAndCheckBasicPropertiesOfProjectStatus()
        {
            const string ProjectName1 = "test01";
            const string ProjectName2 = "test02";

            string IntegrationFolder = System.IO.Path.Combine("scenarioTests", ProjectName1);
            string CCNetConfigFile = System.IO.Path.Combine("IntegrationScenarios", "Simple.xml");
            string ProjectStateFile = new System.IO.FileInfo(ProjectName1 + ".state").FullName;

            IntegrationCompleted.Add(ProjectName1, false);
            IntegrationCompleted.Add(ProjectName2, false);

            Log("Clear existing state file, to simulate first run : " + ProjectStateFile);
            System.IO.File.Delete(ProjectStateFile);

            Log("Clear integration folder to simulate first run");
            if (System.IO.Directory.Exists(IntegrationFolder)) System.IO.Directory.Delete(IntegrationFolder, true);


            CCNet.Remote.Messages.ProjectStatusResponse psr;
            CCNet.Remote.Messages.ProjectRequest pr1 = new CCNet.Remote.Messages.ProjectRequest(null, ProjectName1);
            CCNet.Remote.Messages.ProjectRequest pr2 = new CCNet.Remote.Messages.ProjectRequest(null, ProjectName2);


            Log("Making CruiseServerFactory");
            CCNet.Core.CruiseServerFactory csf = new CCNet.Core.CruiseServerFactory();

            Log("Making cruiseServer with config from :" + CCNetConfigFile);
            using (var cruiseServer = csf.Create(true, CCNetConfigFile))
            {

                // subscribe to integration complete to be able to wait for completion of a build
                cruiseServer.IntegrationCompleted += new EventHandler<ThoughtWorks.CruiseControl.Remote.Events.IntegrationCompletedEventArgs>(cruiseServer_IntegrationCompleted);

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
                cruiseServer.IntegrationCompleted -= new EventHandler<ThoughtWorks.CruiseControl.Remote.Events.IntegrationCompletedEventArgs>(cruiseServer_IntegrationCompleted);

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
            Assert.AreEqual(2, psr.Projects.Count, "Amount of projects in configfile is not correct." + CCNetConfigFile);

            CCNet.Remote.ProjectStatus ps = null;

            Log("checking data of project " + ProjectName1);
            foreach (var p in psr.Projects)
            {
                if (p.Name == ProjectName1) ps = p;
            }

            Assert.AreEqual(ProjectName1, ps.Name);
            Assert.AreEqual(CCNet.Remote.IntegrationStatus.Success, ps.BuildStatus);
            Assert.IsTrue(ps.Activity.IsSleeping(), "Activity should be sleeping after an integration");
            Assert.AreEqual(ps.Category, "cat1");
            Assert.AreEqual(string.Empty, ps.CurrentMessage, "message should be empty after an ok build");
            Assert.AreEqual("first testing project", ps.Description);
            Assert.AreEqual("1", ps.LastBuildLabel);
            Assert.AreEqual("1", ps.LastSuccessfulBuildLabel);
            Assert.AreEqual(0, ps.Messages.Length);
            Assert.AreEqual("Q1", ps.Queue);
            Assert.AreEqual(1, ps.QueuePriority);
            Assert.AreEqual(System.Environment.MachineName, ps.ServerName);
            Assert.AreEqual(CCNet.Remote.ProjectIntegratorState.Running, ps.Status);
            Assert.AreEqual("http://confluence.public.thoughtworks.org", ps.WebURL);


            Log("checking data of project " + ProjectName2);
            foreach (var p in psr.Projects)
            {
                if (p.Name == ProjectName2) ps = p;
            }

            Assert.IsFalse(IntegrationCompleted[ProjectName2], "integration not done, event may not be fired");
            Assert.AreEqual(ProjectName2, ps.Name);
            Assert.AreEqual(CCNet.Remote.IntegrationStatus.Unknown, ps.BuildStatus);
            Assert.IsTrue(ps.Activity.IsSleeping(), "Activity should be still sleeping");
            Assert.AreEqual(ps.Category, "cat2");
            Assert.AreEqual(string.Empty, ps.CurrentMessage, "message should still be empty");
            Assert.AreEqual("second testing project", ps.Description);
            Assert.AreEqual("UNKNOWN", ps.LastBuildLabel);
            Assert.AreEqual("UNKNOWN", ps.LastSuccessfulBuildLabel);
            Assert.AreEqual(0, ps.Messages.Length);
            Assert.AreEqual("Q1", ps.Queue);
            Assert.AreEqual(2, ps.QueuePriority);
            Assert.AreEqual(System.Environment.MachineName, ps.ServerName);
            Assert.AreEqual(CCNet.Remote.ProjectIntegratorState.Unknown, ps.Status);
            Assert.AreEqual("http://" + System.Environment.MachineName + "/ccnet", ps.WebURL, "Default url not correct");



        }

        void cruiseServer_IntegrationCompleted(object sender, CCNet.Remote.Events.IntegrationCompletedEventArgs e)
        {
            Log(string.Format("Integration complete. Project {0} ", e.ProjectName));
            IntegrationCompleted[e.ProjectName] = true;
        }

        private void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("--> {0} {1}", DateTime.Now.ToUniversalTime(), message));
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
