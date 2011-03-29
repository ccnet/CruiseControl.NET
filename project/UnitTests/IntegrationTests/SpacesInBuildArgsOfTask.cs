using System;
using NUnit.Framework;
using CCNet = ThoughtWorks.CruiseControl;

namespace ThoughtWorks.CruiseControl.UnitTests.IntegrationTests
{
    [TestFixture]
    [Category("Integration")]
    public class SpacesInBuildArgsOfTask
    {
        [TestFixtureSetUp]
        public void fixLog4Net()
        {
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("test.config"));
        }

        [Ignore("Breaking change in 1.6 please fix")]
        [Test]
        public void ConfigShouldBeTheSameWithOrWithoutPreProcessor()
        {
            const string projectName1 = "SpaceCheck";
            const string projectName2 = "SpaceCheckPreProcessor";


            string IntegrationFolder = System.IO.Path.Combine("scenarioTests", projectName1);
            string CCNetConfigFile = System.IO.Path.Combine("IntegrationScenarios", "SpacesInBuildArgsOfTask.xml");

            string Project1StateFile = new System.IO.FileInfo(projectName1 + ".state").FullName;
            string Project2StateFile = new System.IO.FileInfo(projectName2 + ".state").FullName;


            Log("Clear existing state file, to simulate first run : " + Project1StateFile);
            System.IO.File.Delete(Project1StateFile);

            Log("Clear existing state file, to simulate first run : " + Project2StateFile);
            System.IO.File.Delete(Project2StateFile);


            Log("Clear integration folder to simulate first run");
            if (System.IO.Directory.Exists(IntegrationFolder)) System.IO.Directory.Delete(IntegrationFolder, true);


            CCNet.Remote.Messages.ProjectRequest pr1 = new CCNet.Remote.Messages.ProjectRequest(null, projectName1);
            CCNet.Remote.Messages.ProjectRequest pr2 = new CCNet.Remote.Messages.ProjectRequest(null, projectName2);


            CCNet.Remote.Messages.DataResponse dr1;
            CCNet.Remote.Messages.DataResponse dr2;


            Log("Making CruiseServerFactory");
            CCNet.Core.CruiseServerFactory csf = new CCNet.Core.CruiseServerFactory();
            Log("Making cruiseServer with config from :" + CCNetConfigFile);
            using (var cruiseServer = csf.Create(true, CCNetConfigFile))
            {
                Log("Starting cruiseServer");
                try
                {
                    cruiseServer.Start();

                }
                catch (Exception e)
                {
                    Log(e.ToString());
                }

                dr1 = cruiseServer.GetProject(pr1);
                dr2 = cruiseServer.GetProject(pr2);

                Log("Stopping cruiseServer");
                cruiseServer.Stop();

                System.Threading.Thread.Sleep(250); // give time to stop the build
            }

            var pr1config = dr1.Data;
            var pr2config = dr2.Data.Replace("<name>SpaceCheckPreProcessor</name>", "<name>SpaceCheck</name>"); // give project the same name


            Assert.AreEqual(pr1config, pr2config);

        }

        private void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture, "--> {0} {1}", DateTime.Now.ToLongTimeString(), message));
        }


    }
}
