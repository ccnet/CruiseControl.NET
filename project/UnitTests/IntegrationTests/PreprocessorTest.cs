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
    public class PreprocessorTest
    {
        System.Collections.Generic.Dictionary<string, bool> IntegrationCompleted = new System.Collections.Generic.Dictionary<string, bool>();

        [Test]
        [Timeout(120000)]
        public void Simulate()
        {
            const string projectName1 = "Test01";

            var integrationFolder = System.IO.Path.Combine("scenarioTests", projectName1);
            var ccNetConfigFile = System.IO.Path.Combine("IntegrationScenarios", "CCNetConfigWithPreProcessor.xml");
            var project1StateFile = new System.IO.FileInfo(projectName1 + ".state").FullName;

            IntegrationCompleted.Add(projectName1, false);

            Log("Clear existing state file, to simulate first run : " + project1StateFile);
            System.IO.File.Delete(project1StateFile);

            Log("Clear integration folder to simulate first run");
            if (System.IO.Directory.Exists(integrationFolder)) System.IO.Directory.Delete(integrationFolder, true);


            CCNet.Remote.Messages.ProjectStatusResponse psr;
            var pr1 = new CCNet.Remote.Messages.ProjectRequest(null, projectName1);

            Log("Making CruiseServerFactory");
            var csf = new CCNet.Core.CruiseServerFactory();

            Log("Making cruiseServer with config from :" + ccNetConfigFile);
            using (var cruiseServer = csf.Create(true, ccNetConfigFile))
            {

                // subscribe to integration complete to be able to wait for completion of a build
                cruiseServer.IntegrationCompleted += new EventHandler<ThoughtWorks.CruiseControl.Remote.Events.IntegrationCompletedEventArgs>(CruiseServerIntegrationCompleted);

                Log("Starting cruiseServer");
                cruiseServer.Start();

                Log("Forcing build on project " + projectName1 + " to test the innertrigger");
                CheckResponse(cruiseServer.ForceBuild(pr1));

                System.Threading.Thread.Sleep(250); // give time to start the build

                Log("Waiting for integration to complete of : " + projectName1);
                while (!IntegrationCompleted[projectName1])
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
            Assert.AreEqual(1, psr.Projects.Count, "Amount of projects in configfile is not correct." + ccNetConfigFile);

            CCNet.Remote.ProjectStatus ps = null;

            // checking data of project 1
            foreach (var p in psr.Projects)
            {
                if (p.Name == projectName1) ps = p;
            }

            Assert.AreEqual(projectName1, ps.Name);
            Assert.AreEqual(CCNet.Remote.IntegrationStatus.Success, ps.BuildStatus, "wrong build state for project " + projectName1);

        }


        [Test]
        [Timeout(120000)]
        public void MustBeAbleToParse_1()
        {
            var ccNetConfigFile = System.IO.Path.Combine("IntegrationScenarios", "CCNetConfigWithPreProcessor_1.xml");

            Log("Making CruiseServerFactory");
            var csf = new CCNet.Core.CruiseServerFactory();

            Log("Making cruiseServer with config from :" + ccNetConfigFile);
            using (var cruiseServer = csf.Create(true, ccNetConfigFile))
            {
                Log("Starting cruiseServer");
                cruiseServer.Start();

                System.Threading.Thread.Sleep(250); 

                Log("Stopping cruiseServer");
                cruiseServer.Stop();

                Log("waiting for cruiseServer to stop");
                cruiseServer.WaitForExit();
                Log("cruiseServer stopped");
            }
        }


        [Test]
        [Timeout(120000)]
        public void MustBeAbleToParse_2()
        {
            var ccNetConfigFile = System.IO.Path.Combine("IntegrationScenarios", "CCNetConfigWithPreProcessor_2.xml");

            Log("Making CruiseServerFactory");
            var csf = new CCNet.Core.CruiseServerFactory();

            Log("Making cruiseServer with config from :" + ccNetConfigFile);
            using (var cruiseServer = csf.Create(true, ccNetConfigFile))
            {
                Log("Starting cruiseServer");
                cruiseServer.Start();

                System.Threading.Thread.Sleep(250);

                Log("Stopping cruiseServer");
                cruiseServer.Stop();

                Log("waiting for cruiseServer to stop");
                cruiseServer.WaitForExit();
                Log("cruiseServer stopped");
            }
        }

        [Test]
        [Timeout(120000)]
        public void MustBeAbleToParse_3()
        {
            var ccNetConfigFile = System.IO.Path.Combine("IntegrationScenarios", "CCNetConfigWithPreProcessor_3.xml");

            Log("Making CruiseServerFactory");
            var csf = new CCNet.Core.CruiseServerFactory();

            Log("Making cruiseServer with config from :" + ccNetConfigFile);
            using (var cruiseServer = csf.Create(true, ccNetConfigFile))
            {
                Log("Starting cruiseServer");
                cruiseServer.Start();

                System.Threading.Thread.Sleep(250);

                Log("Stopping cruiseServer");
                cruiseServer.Stop();

                Log("waiting for cruiseServer to stop");
                cruiseServer.WaitForExit();
                Log("cruiseServer stopped");
            }
        }


        [Test]
        [Timeout(120000)]
        public void MustBeAbleToParse_4()
        {
            var ccNetConfigFile = System.IO.Path.Combine("IntegrationScenarios", "CCNetConfigWithPreProcessor_4.xml");

            Log("Making CruiseServerFactory");
            var csf = new CCNet.Core.CruiseServerFactory();

            Log("Making cruiseServer with config from :" + ccNetConfigFile);
            using (var cruiseServer = csf.Create(true, ccNetConfigFile))
            {
                Log("Starting cruiseServer");
                cruiseServer.Start();

                System.Threading.Thread.Sleep(250);

                Log("Stopping cruiseServer");
                cruiseServer.Stop();

                Log("waiting for cruiseServer to stop");
                cruiseServer.WaitForExit();
                Log("cruiseServer stopped");
            }
        }



        [Test]
        [Timeout(120000)]
        public void MustBeAbleToParse_5()
        {
            var ccNetConfigFile = System.IO.Path.Combine("IntegrationScenarios", "CCNetConfigWithPreProcessor_5.xml");

            Log("Making CruiseServerFactory");
            var csf = new CCNet.Core.CruiseServerFactory();

            Log("Making cruiseServer with config from :" + ccNetConfigFile);
            using (var cruiseServer = csf.Create(true, ccNetConfigFile))
            {
                Log("Starting cruiseServer");
                cruiseServer.Start();

                System.Threading.Thread.Sleep(250);

                Log("Stopping cruiseServer");
                cruiseServer.Stop();

                Log("waiting for cruiseServer to stop");
                cruiseServer.WaitForExit();
                Log("cruiseServer stopped");
            }
        }



        void CruiseServerIntegrationCompleted(object sender, CCNet.Remote.Events.IntegrationCompletedEventArgs e)
        {
            Log(string.Format("Integration complete. Project {0} ", e.ProjectName));
            IntegrationCompleted[e.ProjectName] = true;
        }

        private void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("{0} {1}", DateTime.Now.ToUniversalTime(), message));
        }

        private void CheckResponse(ThoughtWorks.CruiseControl.Remote.Messages.Response value)
        {
            if (value.Result == ThoughtWorks.CruiseControl.Remote.Messages.ResponseResult.Failure)
            {
                var message = "Request has failed on the server:" + System.Environment.NewLine +
                    value.ConcatenateErrors();
                throw new CCNet.Core.CruiseControlException(message);
            }
        }

    }



}
