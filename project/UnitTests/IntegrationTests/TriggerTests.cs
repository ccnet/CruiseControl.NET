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
    public class TriggerTests
    {
        System.Collections.Generic.Dictionary<string, bool> IntegrationCompleted = new System.Collections.Generic.Dictionary<string, bool>();

        [Test]
        [Timeout(120000)]
        public void Simulate()
        {
            const string ProjectName1 = "triggerTest01";
            const string ProjectName2 = "triggerTest02";


            string IntegrationFolder = System.IO.Path.Combine("scenarioTests", ProjectName1);
            string CCNetConfigFile = System.IO.Path.Combine("IntegrationScenarios", "Triggers.xml");
            string Project1StateFile = new System.IO.FileInfo(ProjectName1 + ".state").FullName;
            string Project2StateFile = new System.IO.FileInfo(ProjectName2 + ".state").FullName;

            IntegrationCompleted.Add(ProjectName1, false);
            IntegrationCompleted.Add(ProjectName2, false);

            const Int32 SecondsToWaitFromNow = 120;
            // adjust triggertime of project 1 to now + 70 seconds (SecondsToWaitFromNow)
            // this will give the unittest time to create an ok build of project2
            // and let the schedule trigger work as normal : check if it is time to integrate and check on the status
            // 70 seconds should be ok, less time may give problems on slower machines 
            // keep in mind that cruise server is also starting, so this time must also be taken into account
            // also we want the cuise server to wait for 1 minute, otherwise it starts integrating project 1 immediately
            System.Xml.XmlDocument xdoc = new System.Xml.XmlDocument();
            xdoc.Load(CCNetConfigFile);
            string xslt = string.Format("/cruisecontrol/project[@name='{0}']/triggers/scheduleTrigger", ProjectName1);
            var scheduleTrigger = xdoc.SelectSingleNode(xslt);

            if (scheduleTrigger == null)
            {
                throw new Exception(string.Format("Schedule trigger not found,via xslt {0} in configfile {1}", xslt, CCNetConfigFile));
            }

            string newIntegrationTime = System.DateTime.Now.AddSeconds(SecondsToWaitFromNow).ToString("HH:mm");
            Log("--------------------------------------------------------------------------");
            Log(string.Format("{0} is scheduled to integrate at {1}", ProjectName1, newIntegrationTime));
            Log("--------------------------------------------------------------------------");

            scheduleTrigger.Attributes["time"].Value = newIntegrationTime;
            xdoc.Save(CCNetConfigFile);


            Log("Clear existing state file, to simulate first run : " + Project1StateFile);
            System.IO.File.Delete(Project1StateFile);

            Log("Clear existing state file, to simulate first run : " + Project2StateFile);
            System.IO.File.Delete(Project2StateFile);


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

                Log("Forcing build on project " + ProjectName1 + " to test the innertrigger");
                CheckResponse(cruiseServer.ForceBuild(pr1));

                System.Threading.Thread.Sleep(250); // give time to start the build

                Log("Waiting for integration to complete of : " + ProjectName1);
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

            // checking data of project 1
            foreach (var p in psr.Projects)
            {
                if (p.Name == ProjectName1) ps = p;
            }

            Assert.AreEqual(ProjectName1, ps.Name);
            Assert.AreEqual(CCNet.Remote.IntegrationStatus.Success, ps.BuildStatus, "wrong build state for project " + ProjectName1);


            // checking data of project 2
            foreach (var p in psr.Projects)
            {
                if (p.Name == ProjectName2) ps = p;
            }

            Assert.AreEqual(ProjectName2, ps.Name);
            Assert.AreEqual(CCNet.Remote.IntegrationStatus.Unknown , ps.BuildStatus, "wrong build state for project " + ProjectName2);


        }
 

        void cruiseServer_IntegrationCompleted(object sender, CCNet.Remote.Events.IntegrationCompletedEventArgs e)
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
                string message = "Request has failed on the server:" + System.Environment.NewLine +
                    value.ConcatenateErrors();
                throw new CCNet.Core.CruiseControlException(message);
            }
        }


    }
}
