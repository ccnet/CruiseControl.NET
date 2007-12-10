// $HeadURL$

using System.IO;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
    [TestFixture]
    public class ProcessMonitorTest
    {
        private Project project1;
        private Project project2;
        private IMock mockSourceControl;
        private IMock mockStateManager;
        private IMock mockTrigger;
        private IMock mockLabeller;
        private IMock mockPublisher;
        private IMock mockTask;
        private string workingDirPath;
        private string artifactDirPath;
        private const string ProjectName1 = "test1";
        private Mockery mockery;


        [SetUp]
        public void SetUp()
        {
            workingDirPath = TempFileUtil.CreateTempDir("workingDirectory");
            artifactDirPath = TempFileUtil.CreateTempDir("artifactDirectory");
            Assert.IsTrue(Directory.Exists(workingDirPath));
            Assert.IsTrue(Directory.Exists(artifactDirPath));

            mockery = new Mockery();
            mockSourceControl = mockery.NewStrictMock(typeof (ISourceControl));
            mockStateManager = mockery.NewStrictMock(typeof (IStateManager));
            mockTrigger = mockery.NewStrictMock(typeof (ITrigger));
            mockLabeller = mockery.NewStrictMock(typeof (ILabeller));
            mockPublisher = mockery.NewStrictMock((typeof (ITask)));
            mockTask = mockery.NewStrictMock((typeof (ITask)));

            project1 = new Project();
            project2 = new Project();
            SetupProjects();
        }

        private void SetupProjects()
        {
            project1.Name = ProjectName1;
            project1.SourceControl = (ISourceControl) mockSourceControl.MockInstance;
            project1.StateManager = (IStateManager) mockStateManager.MockInstance;
            project1.Triggers = (ITrigger) mockTrigger.MockInstance;
            project1.Labeller = (ILabeller) mockLabeller.MockInstance;
            project1.Publishers = new ITask[] {(ITask) mockPublisher.MockInstance};
            project1.Tasks = new ITask[] {(ITask) mockTask.MockInstance};
            project1.ConfiguredWorkingDirectory = workingDirPath;
            project1.ConfiguredArtifactDirectory = artifactDirPath;

            project2.Name = ProjectName1;
            project2.SourceControl = (ISourceControl) mockSourceControl.MockInstance;
            project2.StateManager = (IStateManager) mockStateManager.MockInstance;
            project2.Triggers = (ITrigger) mockTrigger.MockInstance;
            project2.Labeller = (ILabeller) mockLabeller.MockInstance;
            project2.Publishers = new ITask[] {(ITask) mockPublisher.MockInstance};
            project2.Tasks = new ITask[] {(ITask) mockTask.MockInstance};
            project2.ConfiguredWorkingDirectory = workingDirPath;
            project2.ConfiguredArtifactDirectory = artifactDirPath;
        }

        [TearDown]
        public void TearDown()
        {
            mockery.Verify();
        }

        [Test]
        public void ShouldReturnAMonitorForTheProject()
        {
            ProcessMonitor monitor = project1.ProcessMonitor;
            Assert.AreEqual(ProcessMonitor.GetProcessMonitorByProject(project1.Name), monitor);
        }


        /*[Test]
		public void ShouldKillTheBuildProcess()
		{
			System.Diagnostics.Process p = new System.Diagnostics.Process();
			p.StartInfo = new ProcessStartInfo(@"C:\WINDOWS\system32\cmd.exe","");
			p.Start();
			project1.ProcessMonitor.monitorNewProcess(p);
			string result = project1.ProcessMonitor.KillProcess();
			Assert.AreEqual("success", result);
		}

		[Test]
		public void ShouldReturnAllreadyEndedMassageWhenTryingToKillTheBuildProcess()
		{
			System.Diagnostics.Process p = new System.Diagnostics.Process();
			p.StartInfo = new ProcessStartInfo(@"C:\WINDOWS\system32\cmd.exe","");
			project2.ProcessMonitor.monitorNewProcess(p);
			string result = project2.ProcessMonitor.KillProcess();
			Assert.AreEqual("The build process can't be terminated because it has allready ended.", result);
		}*/
    }
}
// $HeadURL$

using System.IO;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
    [TestFixture]
    public class ProcessMonitorTest
    {
        private Project project1;
        private Project project2;
        private IMock mockSourceControl;
        private IMock mockStateManager;
        private IMock mockTrigger;
        private IMock mockLabeller;
        private IMock mockPublisher;
        private IMock mockTask;
        private string workingDirPath;
        private string artifactDirPath;
        private const string ProjectName1 = "test1";
        private Mockery mockery;


        [SetUp]
        public void SetUp()
        {
            workingDirPath = TempFileUtil.CreateTempDir("workingDirectory");
            artifactDirPath = TempFileUtil.CreateTempDir("artifactDirectory");
            Assert.IsTrue(Directory.Exists(workingDirPath));
            Assert.IsTrue(Directory.Exists(artifactDirPath));

            mockery = new Mockery();
            mockSourceControl = mockery.NewStrictMock(typeof (ISourceControl));
            mockStateManager = mockery.NewStrictMock(typeof (IStateManager));
            mockTrigger = mockery.NewStrictMock(typeof (ITrigger));
            mockLabeller = mockery.NewStrictMock(typeof (ILabeller));
            mockPublisher = mockery.NewStrictMock((typeof (ITask)));
            mockTask = mockery.NewStrictMock((typeof (ITask)));

            project1 = new Project();
            project2 = new Project();
            SetupProjects();
        }

        private void SetupProjects()
        {
            project1.Name = ProjectName1;
            project1.SourceControl = (ISourceControl) mockSourceControl.MockInstance;
            project1.StateManager = (IStateManager) mockStateManager.MockInstance;
            project1.Triggers = (ITrigger) mockTrigger.MockInstance;
            project1.Labeller = (ILabeller) mockLabeller.MockInstance;
            project1.Publishers = new ITask[] {(ITask) mockPublisher.MockInstance};
            project1.Tasks = new ITask[] {(ITask) mockTask.MockInstance};
            project1.ConfiguredWorkingDirectory = workingDirPath;
            project1.ConfiguredArtifactDirectory = artifactDirPath;

            project2.Name = ProjectName1;
            project2.SourceControl = (ISourceControl) mockSourceControl.MockInstance;
            project2.StateManager = (IStateManager) mockStateManager.MockInstance;
            project2.Triggers = (ITrigger) mockTrigger.MockInstance;
            project2.Labeller = (ILabeller) mockLabeller.MockInstance;
            project2.Publishers = new ITask[] {(ITask) mockPublisher.MockInstance};
            project2.Tasks = new ITask[] {(ITask) mockTask.MockInstance};
            project2.ConfiguredWorkingDirectory = workingDirPath;
            project2.ConfiguredArtifactDirectory = artifactDirPath;
        }

        [TearDown]
        public void TearDown()
        {
            mockery.Verify();
        }

        [Test]
        public void ShouldReturnAMonitorForTheProject()
        {
            ProcessMonitor monitor = project1.ProcessMonitor;
            Assert.AreEqual(ProcessMonitor.GetProcessMonitorByProject(project1.Name), monitor);
        }


        /*[Test]
		public void ShouldKillTheBuildProcess()
		{
			System.Diagnostics.Process p = new System.Diagnostics.Process();
			p.StartInfo = new ProcessStartInfo(@"C:\WINDOWS\system32\cmd.exe","");
			p.Start();
			project1.ProcessMonitor.monitorNewProcess(p);
			string result = project1.ProcessMonitor.KillProcess();
			Assert.AreEqual("success", result);
		}

		[Test]
		public void ShouldReturnAllreadyEndedMassageWhenTryingToKillTheBuildProcess()
		{
			System.Diagnostics.Process p = new System.Diagnostics.Process();
			p.StartInfo = new ProcessStartInfo(@"C:\WINDOWS\system32\cmd.exe","");
			project2.ProcessMonitor.monitorNewProcess(p);
			string result = project2.ProcessMonitor.KillProcess();
			Assert.AreEqual("The build process can't be terminated because it has allready ended.", result);
		}*/
    }
}
// $HeadURL$

using System.IO;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
    [TestFixture]
    public class ProcessMonitorTest
    {
        private Project project1;
        private Project project2;
        private IMock mockSourceControl;
        private IMock mockStateManager;
        private IMock mockTrigger;
        private IMock mockLabeller;
        private IMock mockPublisher;
        private IMock mockTask;
        private string workingDirPath;
        private string artifactDirPath;
        private const string ProjectName1 = "test1";
        private Mockery mockery;


        [SetUp]
        public void SetUp()
        {
            workingDirPath = TempFileUtil.CreateTempDir("workingDirectory");
            artifactDirPath = TempFileUtil.CreateTempDir("artifactDirectory");
            Assert.IsTrue(Directory.Exists(workingDirPath));
            Assert.IsTrue(Directory.Exists(artifactDirPath));

            mockery = new Mockery();
            mockSourceControl = mockery.NewStrictMock(typeof (ISourceControl));
            mockStateManager = mockery.NewStrictMock(typeof (IStateManager));
            mockTrigger = mockery.NewStrictMock(typeof (ITrigger));
            mockLabeller = mockery.NewStrictMock(typeof (ILabeller));
            mockPublisher = mockery.NewStrictMock((typeof (ITask)));
            mockTask = mockery.NewStrictMock((typeof (ITask)));

            project1 = new Project();
            project2 = new Project();
            SetupProjects();
        }

        private void SetupProjects()
        {
            project1.Name = ProjectName1;
            project1.SourceControl = (ISourceControl) mockSourceControl.MockInstance;
            project1.StateManager = (IStateManager) mockStateManager.MockInstance;
            project1.Triggers = (ITrigger) mockTrigger.MockInstance;
            project1.Labeller = (ILabeller) mockLabeller.MockInstance;
            project1.Publishers = new ITask[] {(ITask) mockPublisher.MockInstance};
            project1.Tasks = new ITask[] {(ITask) mockTask.MockInstance};
            project1.ConfiguredWorkingDirectory = workingDirPath;
            project1.ConfiguredArtifactDirectory = artifactDirPath;

            project2.Name = ProjectName1;
            project2.SourceControl = (ISourceControl) mockSourceControl.MockInstance;
            project2.StateManager = (IStateManager) mockStateManager.MockInstance;
            project2.Triggers = (ITrigger) mockTrigger.MockInstance;
            project2.Labeller = (ILabeller) mockLabeller.MockInstance;
            project2.Publishers = new ITask[] {(ITask) mockPublisher.MockInstance};
            project2.Tasks = new ITask[] {(ITask) mockTask.MockInstance};
            project2.ConfiguredWorkingDirectory = workingDirPath;
            project2.ConfiguredArtifactDirectory = artifactDirPath;
        }

        [TearDown]
        public void TearDown()
        {
            mockery.Verify();
        }

        [Test]
        public void ShouldReturnAMonitorForTheProject()
        {
            ProcessMonitor monitor = project1.ProcessMonitor;
            Assert.AreEqual(ProcessMonitor.GetProcessMonitorByProject(project1.Name), monitor);
        }


        /*[Test]
		public void ShouldKillTheBuildProcess()
		{
			System.Diagnostics.Process p = new System.Diagnostics.Process();
			p.StartInfo = new ProcessStartInfo(@"C:\WINDOWS\system32\cmd.exe","");
			p.Start();
			project1.ProcessMonitor.monitorNewProcess(p);
			string result = project1.ProcessMonitor.KillProcess();
			Assert.AreEqual("success", result);
		}

		[Test]
		public void ShouldReturnAllreadyEndedMassageWhenTryingToKillTheBuildProcess()
		{
			System.Diagnostics.Process p = new System.Diagnostics.Process();
			p.StartInfo = new ProcessStartInfo(@"C:\WINDOWS\system32\cmd.exe","");
			project2.ProcessMonitor.monitorNewProcess(p);
			string result = project2.ProcessMonitor.KillProcess();
			Assert.AreEqual("The build process can't be terminated because it has allready ended.", result);
		}*/
    }
}
// $HeadURL$

using System.IO;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
    [TestFixture]
    public class ProcessMonitorTest
    {
        private Project project1;
        private Project project2;
        private IMock mockSourceControl;
        private IMock mockStateManager;
        private IMock mockTrigger;
        private IMock mockLabeller;
        private IMock mockPublisher;
        private IMock mockTask;
        private string workingDirPath;
        private string artifactDirPath;
        private const string ProjectName1 = "test1";
        private Mockery mockery;


        [SetUp]
        public void SetUp()
        {
            workingDirPath = TempFileUtil.CreateTempDir("workingDirectory");
            artifactDirPath = TempFileUtil.CreateTempDir("artifactDirectory");
            Assert.IsTrue(Directory.Exists(workingDirPath));
            Assert.IsTrue(Directory.Exists(artifactDirPath));

            mockery = new Mockery();
            mockSourceControl = mockery.NewStrictMock(typeof (ISourceControl));
            mockStateManager = mockery.NewStrictMock(typeof (IStateManager));
            mockTrigger = mockery.NewStrictMock(typeof (ITrigger));
            mockLabeller = mockery.NewStrictMock(typeof (ILabeller));
            mockPublisher = mockery.NewStrictMock((typeof (ITask)));
            mockTask = mockery.NewStrictMock((typeof (ITask)));

            project1 = new Project();
            project2 = new Project();
            SetupProjects();
        }

        private void SetupProjects()
        {
            project1.Name = ProjectName1;
            project1.SourceControl = (ISourceControl) mockSourceControl.MockInstance;
            project1.StateManager = (IStateManager) mockStateManager.MockInstance;
            project1.Triggers = (ITrigger) mockTrigger.MockInstance;
            project1.Labeller = (ILabeller) mockLabeller.MockInstance;
            project1.Publishers = new ITask[] {(ITask) mockPublisher.MockInstance};
            project1.Tasks = new ITask[] {(ITask) mockTask.MockInstance};
            project1.ConfiguredWorkingDirectory = workingDirPath;
            project1.ConfiguredArtifactDirectory = artifactDirPath;

            project2.Name = ProjectName1;
            project2.SourceControl = (ISourceControl) mockSourceControl.MockInstance;
            project2.StateManager = (IStateManager) mockStateManager.MockInstance;
            project2.Triggers = (ITrigger) mockTrigger.MockInstance;
            project2.Labeller = (ILabeller) mockLabeller.MockInstance;
            project2.Publishers = new ITask[] {(ITask) mockPublisher.MockInstance};
            project2.Tasks = new ITask[] {(ITask) mockTask.MockInstance};
            project2.ConfiguredWorkingDirectory = workingDirPath;
            project2.ConfiguredArtifactDirectory = artifactDirPath;
        }

        [TearDown]
        public void TearDown()
        {
            mockery.Verify();
        }

        [Test]
        public void ShouldReturnAMonitorForTheProject()
        {
            ProcessMonitor monitor = project1.ProcessMonitor;
            Assert.AreEqual(ProcessMonitor.GetProcessMonitorByProject(project1.Name), monitor);
        }


        /*[Test]
		public void ShouldKillTheBuildProcess()
		{
			System.Diagnostics.Process p = new System.Diagnostics.Process();
			p.StartInfo = new ProcessStartInfo(@"C:\WINDOWS\system32\cmd.exe","");
			p.Start();
			project1.ProcessMonitor.monitorNewProcess(p);
			string result = project1.ProcessMonitor.KillProcess();
			Assert.AreEqual("success", result);
		}

		[Test]
		public void ShouldReturnAllreadyEndedMassageWhenTryingToKillTheBuildProcess()
		{
			System.Diagnostics.Process p = new System.Diagnostics.Process();
			p.StartInfo = new ProcessStartInfo(@"C:\WINDOWS\system32\cmd.exe","");
			project2.ProcessMonitor.monitorNewProcess(p);
			string result = project2.ProcessMonitor.KillProcess();
			Assert.AreEqual("The build process can't be terminated because it has allready ended.", result);
		}*/
    }
}
