using System;
using NUnit.Framework;
using CCNet = ThoughtWorks.CruiseControl;

namespace ThoughtWorks.CruiseControl.UnitTests.IntegrationTests
{
    /// <summary>
    ///These triggers doesn't allow to build project (build is not started):
    ///   <triggers>
    ///      <multiTrigger operator="And">
    ///        <triggers>
    ///          <scheduleTrigger time="23:00" buildCondition="ForceBuild" />
    ///          <projectTrigger project="CheckBuild" serverUri="tcp://buildhost:21234/CruiseManager.rem">
    ///            <triggerStatus>Success</triggerStatus>
    ///            <innerTrigger type="intervalTrigger" seconds="10" buildCondition="ForceBuild" />
    ///          </projectTrigger>
    ///        </triggers>
    ///      </multiTrigger>
    ///    </triggers>
    ///
    ///In CruiseControl.NET 1.4 it worked..
    ///We had some problems with multi triggers in 1.4 when some projectTriggers must be checked and we hoped to get fixes in 1.5, but now we have a problem even with one projectTrigger. Please, fix this bug.
    ///
    ///Note: If I make build by Force Build, the build is started twice.
    /// </summary>
    [TestFixture]
    [Category("Integration")]
    public class CCNet_1835
    {

        [TestFixtureSetUp]
        public void fixLog4Net()
        {
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("test.config"));
        }

        private System.Collections.Generic.Dictionary<string, bool> IntegrationCompleted;

        //report as failure if it takes longer as 120.000 milliseconds --> 2 minutes
        [Test]        
        [Timeout(120000)]
        [Ignore("Do not know if this setup is expected behaviour or not")]
        public void StartServer_ForceBuildDependendProject_Wait_CheckingProjectDoesNotGetTriggered()
        {
            const string projectName1 = "InnerTriggerTest_CCNet1835";
            const string projectName2 = "CheckBuild";

            IntegrationCompleted = new System.Collections.Generic.Dictionary<string, bool>();


            string IntegrationFolder = System.IO.Path.Combine("scenarioTests", projectName1);
            string CCNetConfigFile = System.IO.Path.Combine("IntegrationScenarios", "ProblemWithInnerTriggers_CCnet1835.xml");
            string Project1StateFile = new System.IO.FileInfo(projectName1 + ".state").FullName;
            string Project2StateFile = new System.IO.FileInfo(projectName2 + ".state").FullName;

            IntegrationCompleted.Add(projectName1, false);
            IntegrationCompleted.Add(projectName2, false);

            const Int32 SecondsToWaitFromNow = 60;
            // adjust triggertime of project 1 to now + 60 seconds (SecondsToWaitFromNow)
            // this will give the unittest time to create an ok build of project2
            // and let the schedule trigger work as normal : check if it is time to integrate and check on the status
            // 60 seconds should be ok, less time may give problems on slower machines 
            // keep in mind that cruise server is also starting, so this time must also be taken into account
            // also we want the cuise server to wait for 1 minute, otherwise it starts integrating project 1 immediately
            System.Xml.XmlDocument xdoc = new System.Xml.XmlDocument();
            xdoc.Load(CCNetConfigFile);
            string xslt = string.Format("/cruisecontrol/project[@name='{0}']/triggers/multiTrigger/triggers/scheduleTrigger", projectName1);
            var scheduleTrigger = xdoc.SelectSingleNode(xslt);

            if (scheduleTrigger == null)
            {
                throw new Exception(string.Format("Schedule trigger not found via xslt {0} in configfile {1}", xslt, CCNetConfigFile));
            }

            string newIntegrationTime = System.DateTime.Now.AddSeconds(SecondsToWaitFromNow).ToString("HH:mm");
            Log("--------------------------------------------------------------------------");
            Log(string.Format("{0} is scheduled to integrate at {1}", projectName1, newIntegrationTime));
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
            CCNet.Remote.Messages.ProjectRequest pr1 = new CCNet.Remote.Messages.ProjectRequest(null, projectName1);
            CCNet.Remote.Messages.ProjectRequest pr2 = new CCNet.Remote.Messages.ProjectRequest(null, projectName2);



            Log("Making CruiseServerFactory");
            CCNet.Core.CruiseServerFactory csf = new CCNet.Core.CruiseServerFactory();

            Log("Making cruiseServer with config from :" + CCNetConfigFile);
            using (var cruiseServer = csf.Create(true, CCNetConfigFile))
            {

                // subscribe to integration complete to be able to wait for completion of a build
                cruiseServer.IntegrationCompleted += new EventHandler<ThoughtWorks.CruiseControl.Remote.Events.IntegrationCompletedEventArgs>(CruiseServerIntegrationCompleted);

                Log("Starting cruiseServer");
                cruiseServer.Start();

                Log("Forcing build on project " + projectName2 + " so it has a ok build state");
                CheckResponse(cruiseServer.ForceBuild(pr2));

                System.Threading.Thread.Sleep(250); // give time to start the build

                Log("Waiting for integration to complete of : " + projectName2);
                while (!IntegrationCompleted[projectName2])
                {
                    for (int i = 1; i <= 4; i++) System.Threading.Thread.Sleep(250);
                    Log(" waiting ...");
                }


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
            Assert.AreEqual(2, psr.Projects.Count, "Amount of projects in configfile is not correct." + CCNetConfigFile);

            CCNet.Remote.ProjectStatus ps = null;

            // checking data of project 1
            foreach (var p in psr.Projects)
            {
                if (p.Name == projectName1) ps = p;
            }

            Assert.AreEqual(projectName1, ps.Name);
            Assert.AreEqual(CCNet.Remote.IntegrationStatus.Success, ps.BuildStatus,"wrong build state for project " + projectName1);


            // checking data of project 2
            foreach (var p in psr.Projects)
            {
                if (p.Name == projectName2) ps = p;
            }

            Assert.AreEqual(projectName2, ps.Name);
            Assert.AreEqual(CCNet.Remote.IntegrationStatus.Success, ps.BuildStatus,"wrong build state for project " + projectName2);


        }



        [Test]
        [Timeout(120000)]
        public void StartServer_ForceBuildDependendProjectTwice_Wait_CheckingProjectDoesGetTriggered()
        {
            const string projectName1 = "InnerTriggerTest_CCNet1835";
            const string projectName2 = "CheckBuild";

            IntegrationCompleted = new System.Collections.Generic.Dictionary<string, bool>();

            string IntegrationFolder = System.IO.Path.Combine("scenarioTests", projectName1);
            string CCNetConfigFile = System.IO.Path.Combine("IntegrationScenarios", "ProblemWithInnerTriggers_CCnet1835.xml");
            string Project1StateFile = new System.IO.FileInfo(projectName1 + ".state").FullName;
            string Project2StateFile = new System.IO.FileInfo(projectName2 + ".state").FullName;

            IntegrationCompleted.Add(projectName1, false);
            IntegrationCompleted.Add(projectName2, false);

            const Int32 SecondsToWaitFromNow = 60;
            // adjust triggertime of project 1 to now + 60 seconds (SecondsToWaitFromNow)
            // this will give the unittest time to create an ok build of project2
            // and let the schedule trigger work as normal : check if it is time to integrate and check on the status
            // 60 seconds should be ok, less time may give problems on slower machines 
            // keep in mind that cruise server is also starting, so this time must also be taken into account
            // also we want the cuise server to wait for 1 minute, otherwise it starts integrating project 1 immediately
            System.Xml.XmlDocument xdoc = new System.Xml.XmlDocument();
            xdoc.Load(CCNetConfigFile);
            string xslt = string.Format("/cruisecontrol/project[@name='{0}']/triggers/multiTrigger/triggers/scheduleTrigger", projectName1);
            var scheduleTrigger = xdoc.SelectSingleNode(xslt);

            if (scheduleTrigger == null)
            {
                throw new Exception(string.Format("Schedule trigger not found via xslt {0} in configfile {1}", xslt, CCNetConfigFile));
            }

            string newIntegrationTime = System.DateTime.Now.AddSeconds(SecondsToWaitFromNow).ToString("HH:mm");
            Log("--------------------------------------------------------------------------");
            Log(string.Format("{0} is scheduled to integrate at {1}", projectName1, newIntegrationTime));
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
            CCNet.Remote.Messages.ProjectRequest pr1 = new CCNet.Remote.Messages.ProjectRequest(null, projectName1);
            CCNet.Remote.Messages.ProjectRequest pr2 = new CCNet.Remote.Messages.ProjectRequest(null, projectName2);



            Log("Making CruiseServerFactory");
            CCNet.Core.CruiseServerFactory csf = new CCNet.Core.CruiseServerFactory();

            Log("Making cruiseServer with config from :" + CCNetConfigFile);
            using (var cruiseServer = csf.Create(true, CCNetConfigFile))
            {

                // subscribe to integration complete to be able to wait for completion of a build
                cruiseServer.IntegrationCompleted += new EventHandler<ThoughtWorks.CruiseControl.Remote.Events.IntegrationCompletedEventArgs>(CruiseServerIntegrationCompleted);

                Log("Starting cruiseServer");
                cruiseServer.Start();

                Log("Forcing build on project " + projectName2 + " so it has a ok build state");
                CheckResponse(cruiseServer.ForceBuild(pr2));

                System.Threading.Thread.Sleep(250); // give time to start the build

                Log("Waiting for integration to complete of : " + projectName2);
                while (!IntegrationCompleted[projectName2])
                {
                    for (int i = 1; i <= 4; i++) System.Threading.Thread.Sleep(250);
                    Log(" waiting ...");
                }

                /// extra build
                System.Threading.Thread.Sleep(250); // give some time 
                IntegrationCompleted[projectName2] = false;

                Log("Forcing 2nd build on project " + projectName2 + " so it has a ok build state");
                CheckResponse(cruiseServer.ForceBuild(pr2));

                System.Threading.Thread.Sleep(250); // give time to start the build

                Log("Waiting again for integration to complete of : " + projectName2);
                while (!IntegrationCompleted[projectName2])
                {
                    for (int i = 1; i <= 4; i++) System.Threading.Thread.Sleep(250);
                    Log(" waiting ...");
                }
                 ///extra build


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
            Assert.AreEqual(2, psr.Projects.Count, "Amount of projects in configfile is not correct." + CCNetConfigFile);

            CCNet.Remote.ProjectStatus ps = null;

            // checking data of project 1
            foreach (var p in psr.Projects)
            {
                if (p.Name == projectName1) ps = p;
            }

            Assert.AreEqual(projectName1, ps.Name);
            Assert.AreEqual(CCNet.Remote.IntegrationStatus.Success, ps.BuildStatus, "wrong build state for project " + projectName1);


            // checking data of project 2
            foreach (var p in psr.Projects)
            {
                if (p.Name == projectName2) ps = p;
            }

            Assert.AreEqual(projectName2, ps.Name);
            Assert.AreEqual(CCNet.Remote.IntegrationStatus.Success, ps.BuildStatus, "wrong build state for project " + projectName2);


        }


        [Test]
        [Timeout(120000)]
        public void StartServer_ForceBuildDependendProject_Wait_CheckingProjectDoesGetTriggeredIfTriggerFirstTimeIsTrue()
        {
            const string projectName1 = "InnerTriggerTest_CCNet1835";
            const string projectName2 = "CheckBuild";

            IntegrationCompleted = new System.Collections.Generic.Dictionary<string, bool>();

            string IntegrationFolder = System.IO.Path.Combine("scenarioTests", projectName1);
            string CCNetConfigFile = System.IO.Path.Combine("IntegrationScenarios", "ProblemWithInnerTriggers_CCnet1835.xml");
            string Project1StateFile = new System.IO.FileInfo(projectName1 + ".state").FullName;
            string Project2StateFile = new System.IO.FileInfo(projectName2 + ".state").FullName;

            IntegrationCompleted.Add(projectName1, false);
            IntegrationCompleted.Add(projectName2, false);

            const Int32 SecondsToWaitFromNow = 60;
            // adjust triggertime of project 1 to now + 60 seconds (SecondsToWaitFromNow)
            // this will give the unittest time to create an ok build of project2
            // and let the schedule trigger work as normal : check if it is time to integrate and check on the status
            // 60 seconds should be ok, less time may give problems on slower machines 
            // keep in mind that cruise server is also starting, so this time must also be taken into account
            // also we want the cuise server to wait for 1 minute, otherwise it starts integrating project 1 immediately
            System.Xml.XmlDocument xdoc = new System.Xml.XmlDocument();
            xdoc.Load(CCNetConfigFile);
            string xslt = string.Format("/cruisecontrol/project[@name='{0}']/triggers/multiTrigger/triggers/scheduleTrigger", projectName1);
            var scheduleTrigger = xdoc.SelectSingleNode(xslt);

            if (scheduleTrigger == null)
            {
                throw new Exception(string.Format("Schedule trigger not found via xslt {0} in configfile {1}", xslt, CCNetConfigFile));
            }

            string newIntegrationTime = System.DateTime.Now.AddSeconds(SecondsToWaitFromNow).ToString("HH:mm");
            scheduleTrigger.Attributes["time"].Value = newIntegrationTime;

            xslt = string.Format("/cruisecontrol/project[@name='{0}']/triggers/multiTrigger/triggers/projectTrigger", projectName1);
            var projectTrigger = xdoc.SelectSingleNode(xslt);
            if (projectTrigger == null)
            {
                throw new Exception(string.Format("projectTrigger trigger not found via xslt {0} in configfile {1}", xslt, CCNetConfigFile));
            }
            projectTrigger.Attributes["triggerFirstTime"].Value = "true";

            Log("--------------------------------------------------------------------------");
            Log(string.Format("{0} is scheduled to integrate at {1}", projectName1, newIntegrationTime));
            Log(string.Format("{0} projectTrigger has triggerFirstTime set to true", projectName1));
            Log("--------------------------------------------------------------------------");

            xdoc.Save(CCNetConfigFile);


            Log("Clear existing state file, to simulate first run : " + Project1StateFile);
            System.IO.File.Delete(Project1StateFile);

            Log("Clear existing state file, to simulate first run : " + Project2StateFile);
            System.IO.File.Delete(Project2StateFile);


            Log("Clear integration folder to simulate first run");
            if (System.IO.Directory.Exists(IntegrationFolder)) System.IO.Directory.Delete(IntegrationFolder, true);


            CCNet.Remote.Messages.ProjectStatusResponse psr;
            CCNet.Remote.Messages.ProjectRequest pr1 = new CCNet.Remote.Messages.ProjectRequest(null, projectName1);
            CCNet.Remote.Messages.ProjectRequest pr2 = new CCNet.Remote.Messages.ProjectRequest(null, projectName2);



            Log("Making CruiseServerFactory");
            CCNet.Core.CruiseServerFactory csf = new CCNet.Core.CruiseServerFactory();

            Log("Making cruiseServer with config from :" + CCNetConfigFile);
            using (var cruiseServer = csf.Create(true, CCNetConfigFile))
            {

                // subscribe to integration complete to be able to wait for completion of a build
                cruiseServer.IntegrationCompleted += new EventHandler<ThoughtWorks.CruiseControl.Remote.Events.IntegrationCompletedEventArgs>(CruiseServerIntegrationCompleted);

                Log("Starting cruiseServer");
                cruiseServer.Start();

                Log("Forcing build on project " + projectName2 + " so it has a ok build state");
                CheckResponse(cruiseServer.ForceBuild(pr2));

                System.Threading.Thread.Sleep(250); // give time to start the build

                Log("Waiting for integration to complete of : " + projectName2);
                while (!IntegrationCompleted[projectName2])
                {
                    for (int i = 1; i <= 4; i++) System.Threading.Thread.Sleep(250);
                    Log(" waiting ...");
                }


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
            Assert.AreEqual(2, psr.Projects.Count, "Amount of projects in configfile is not correct." + CCNetConfigFile);

            CCNet.Remote.ProjectStatus ps = null;

            // checking data of project 1
            foreach (var p in psr.Projects)
            {
                if (p.Name == projectName1) ps = p;
            }

            Assert.AreEqual(projectName1, ps.Name);
            Assert.AreEqual(CCNet.Remote.IntegrationStatus.Success, ps.BuildStatus, "wrong build state for project " + projectName1);


            // checking data of project 2
            foreach (var p in psr.Projects)
            {
                if (p.Name == projectName2) ps = p;
            }

            Assert.AreEqual(projectName2, ps.Name);
            Assert.AreEqual(CCNet.Remote.IntegrationStatus.Success, ps.BuildStatus, "wrong build state for project " + projectName2);


        }

        void CruiseServerIntegrationCompleted(object sender, CCNet.Remote.Events.IntegrationCompletedEventArgs e)
        {
            Log(string.Format("Integration complete. Project {0} ", e.ProjectName));
            IntegrationCompleted[e.ProjectName] = true;
        }

        private void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("{0} {1}", DateTime.Now.ToLongTimeString(), message));
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
