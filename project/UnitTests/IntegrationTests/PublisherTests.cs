using System;
using System.Collections.Generic;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using CCNet = ThoughtWorks.CruiseControl;

namespace ThoughtWorks.CruiseControl.UnitTests.IntegrationTests
{
    public class PublisherTests
    {
        [OneTimeSetUp]
        public void fixLog4Net()
        {
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("test.config"));
        }

        private System.Collections.Generic.Dictionary<string, bool> IntegrationCompleted;


        [Test]
        public void PackagePublisherWithFlattenFiles()
        {
            const string ProjectName1 = "PackageTest01";

            string IntegrationFolder = System.IO.Path.Combine("scenarioTests", ProjectName1);
            string CCNetConfigFile = System.IO.Path.Combine("IntegrationScenarios", "PackagePublisherTest01" + (Platform.IsWindows ? "" : "_linux") + ".xml");
            string ProjectStateFile = new System.IO.FileInfo(ProjectName1 + ".state").FullName;

            IntegrationCompleted = new System.Collections.Generic.Dictionary<string, bool>();

            

            string workingDirectory = "Packaging01";
            string InfoFolder = "Info";
            string subInfoFolder = "Sub1";

            string FolderContainingFiles= string.Format("{0}{3}{1}{3}{2}",workingDirectory,InfoFolder,subInfoFolder,System.IO.Path.DirectorySeparatorChar);
            string f1 = string.Format("{0}{1}a.txt",FolderContainingFiles,System.IO.Path.DirectorySeparatorChar);
            string f2 = string.Format("{0}{1}b.txt",FolderContainingFiles,System.IO.Path.DirectorySeparatorChar);
            string f3 = string.Format("{0}{1}c.rtf", FolderContainingFiles, System.IO.Path.DirectorySeparatorChar);

            var ios = new CCNet.Core.Util.IoService();
            ios.DeleteIncludingReadOnlyObjects(FolderContainingFiles);

            System.IO.Directory.CreateDirectory(FolderContainingFiles);
            System.IO.File.WriteAllText(f1, "somedata");
            System.IO.File.WriteAllText(f2, "somedata");

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
            string ExpectedZipFile = string.Format("{0}{1}Artifacts{1}1{1}TestPackage.zip", ProjectName1, System.IO.Path.DirectorySeparatorChar);
            Assert.IsTrue(System.IO.File.Exists(ExpectedZipFile),"zip package not found at expected location");

            ICSharpCode.SharpZipLib.Zip.ZipFile zf = new ICSharpCode.SharpZipLib.Zip.ZipFile(ExpectedZipFile);
            List<string> actualFiles = new List<string>();

            foreach (ICSharpCode.SharpZipLib.Zip.ZipEntry ze in zf)
            {
                System.Diagnostics.Debug.WriteLine(ze.Name);
                actualFiles.Add(ze.Name);
            }
            actualFiles.Sort();

            Assert.AreEqual("a.txtb.txt", String.Join("", actualFiles));

        }


        [Test]
        public void PackagePublisherWithoutFlattenFiles()
        {
            const string ProjectName1 = "PackageTest02";

            string IntegrationFolder = System.IO.Path.Combine("scenarioTests", ProjectName1);
            string CCNetConfigFile = System.IO.Path.Combine("IntegrationScenarios", "PackagePublisherTest02" + (Platform.IsWindows ? "" : "_linux") + ".xml");
            string ProjectStateFile = new System.IO.FileInfo(ProjectName1 + ".state").FullName;

            IntegrationCompleted = new System.Collections.Generic.Dictionary<string, bool>();

            string workingDirectory = "Packaging02";
            string InfoFolder = "Info";
            string subInfoFolder = "Sub1";

            string FolderContainingFiles = string.Format("{0}{3}{1}{3}{2}", workingDirectory, InfoFolder, subInfoFolder, System.IO.Path.DirectorySeparatorChar);
            string f1 = string.Format("{0}{1}a.txt", FolderContainingFiles, System.IO.Path.DirectorySeparatorChar);
            string f2 = string.Format("{0}{1}b.txt", FolderContainingFiles, System.IO.Path.DirectorySeparatorChar);
            string f3 = string.Format("{0}{1}c.rtf", FolderContainingFiles, System.IO.Path.DirectorySeparatorChar);


            var ios = new CCNet.Core.Util.IoService();
            ios.DeleteIncludingReadOnlyObjects(FolderContainingFiles);

            System.IO.Directory.CreateDirectory(FolderContainingFiles);
            System.IO.File.WriteAllText(f1, "somedata");
            System.IO.File.WriteAllText(f2, "somedata");

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
            string ExpectedZipFile = string.Format("{0}{1}Artifacts{1}1{1}TestPackage.zip", ProjectName1, System.IO.Path.DirectorySeparatorChar);
            Assert.IsTrue(System.IO.File.Exists(ExpectedZipFile), "zip package not found at expected location");

            ICSharpCode.SharpZipLib.Zip.ZipFile zf = new ICSharpCode.SharpZipLib.Zip.ZipFile(ExpectedZipFile);
            List<string> actualFiles = new List<string>();

            foreach (ICSharpCode.SharpZipLib.Zip.ZipEntry ze in zf)
            {
                System.Diagnostics.Debug.WriteLine(ze.Name);
                actualFiles.Add(ze.Name);
            }
            actualFiles.Sort();

            Assert.AreEqual(@"Info/Sub1/a.txtInfo/Sub1/b.txt", String.Join("", actualFiles));

        }

        /// <summary>
        /// test for issue 39 : Package Publisher / PackageFolder doesn't respect hierarchy when targetFolder is specified 
        /// http://www.cruisecontrolnet.org/issues/39
        /// </summary>
        [Test]
        public void PackagePublisherPackageFolderMustRespectHierarchy()
        {
            const string ProjectName1 = "PackageTest03";

            string IntegrationFolder = System.IO.Path.Combine("scenarioTests", ProjectName1);
            string CCNetConfigFile = System.IO.Path.Combine("IntegrationScenarios", "PackagePublisherTest03" + (Platform.IsWindows ? "" : "_linux") + ".xml");
            string ProjectStateFile = new System.IO.FileInfo(ProjectName1 + ".state").FullName;

            IntegrationCompleted = new System.Collections.Generic.Dictionary<string, bool>();

            string workingDirectory = "Packaging03";

            string infoFolder = string.Format("{0}{1}some{1}directories{1}deeper{1}_PublishedWebsites{1}MyProject{1}", workingDirectory, System.IO.Path.DirectorySeparatorChar);
            string subInfoFolder = string.Format("{0}{1}AFolder{1}", infoFolder, System.IO.Path.DirectorySeparatorChar);
            string f1 = string.Format("{0}{1}a.txt", infoFolder, System.IO.Path.DirectorySeparatorChar);
            string f2 = string.Format("{0}{1}b.txt", subInfoFolder, System.IO.Path.DirectorySeparatorChar);


            var ios = new CCNet.Core.Util.IoService();
            ios.DeleteIncludingReadOnlyObjects(infoFolder);
            ios.DeleteIncludingReadOnlyObjects("TheMegaWebSite");


            System.IO.Directory.CreateDirectory(infoFolder);
            System.IO.Directory.CreateDirectory(subInfoFolder);


            System.IO.File.WriteAllText(f1, "somedata");
            System.IO.File.WriteAllText(f2, "somedata");

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
            string ExpectedZipFile = string.Format("{0}{1}Artifacts{1}1{1}Project-package.zip", ProjectName1, System.IO.Path.DirectorySeparatorChar);
            Assert.IsTrue(System.IO.File.Exists(ExpectedZipFile), "zip package not found at expected location");

            ICSharpCode.SharpZipLib.Zip.ZipFile zf = new ICSharpCode.SharpZipLib.Zip.ZipFile(ExpectedZipFile);
            string expectedFiles = string.Empty;

            foreach (ICSharpCode.SharpZipLib.Zip.ZipEntry ze in zf)
            {
                System.Diagnostics.Debug.WriteLine(ze.Name);
                expectedFiles += ze.Name;
            }

            Assert.AreEqual(@"MegaWebSite/AFolder/b.txtMegaWebSite/a.txt", expectedFiles);
        }


        private void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0} {1}", DateTime.Now.ToLongTimeString(), message));
        }

        void CruiseServerIntegrationCompleted(object sender, CCNet.Remote.Events.IntegrationCompletedEventArgs e)
        {
            Log(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Integration complete. Project {0} ", e.ProjectName));
            IntegrationCompleted[e.ProjectName] = true;
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
