using System;
using System.Threading;

using NUnit.Framework;

using tw.ccnet.remote;

namespace tw.ccnet.controlpanel.test
{
    [TestFixture]
	public class ControlPanelTest : Assertion
	{
        private MockCruiseManager manager;
        private ControlPanel panel;
        
        [SetUp]
        public void SetUp() 
        {
            manager = new MockCruiseManager();
            panel = new ControlPanel(manager);
            panel.notifyIcon.Visible = false; // this is annoying in tests...
            panel.Show();
        }

        [TearDown]
        public void TearDown() 
        {
            panel.Close();
        }

        [Test]
        public void TestButtonState() 
        {
            panel.OnTimerTick(null, EventArgs.Empty);
            
            AssertEquals(false, panel.startButton.Enabled);
            AssertEquals(true, panel.stopButton.Enabled);
            AssertEquals(true, panel.stopNowButton.Enabled);
            AssertEquals("Service is running.", panel.statusBar.Text);

            manager.Status = CruiseControlStatus.Stopped;
            panel.OnTimerTick(null, EventArgs.Empty);

            AssertEquals(true, panel.startButton.Enabled);
            AssertEquals(false, panel.stopButton.Enabled);
            AssertEquals(false, panel.stopNowButton.Enabled);
            AssertEquals("Service is stopped.", panel.statusBar.Text);

            manager.Status = CruiseControlStatus.WillBeStopped;
            panel.OnTimerTick(null, EventArgs.Empty);

            AssertEquals(false, panel.startButton.Enabled);
            AssertEquals(false, panel.stopButton.Enabled);
            AssertEquals(false, panel.stopNowButton.Enabled);
            AssertEquals("Service is stopping...", panel.statusBar.Text);
        }

        [Test]
        public void TestStartButton() 
        {
            manager.Status = CruiseControlStatus.Stopped;
            panel.startButton.PerformClick();
            AssertEquals(CruiseControlStatus.Running, manager.Status);
        }

        [Test]
        public void TestStopButton() 
        {
            panel.stopButton.PerformClick();
            AssertEquals(CruiseControlStatus.WillBeStopped, manager.Status);
        }

        [Test]
        public void TestStopNowButton() 
        {
            panel.stopNowButton.PerformClick();
            AssertEquals(CruiseControlStatus.Stopped, manager.Status);
        }

        [Test, Ignore("can't get this to work with remoting")]
        public void TestConsoleOutputSink() 
        {
//            manager.Sink.Write("foo");
//            manager.Sink.Write("bar");

            AssertEquals("foobar", panel.consoleOutputTextBox.Text);
        }

        [Test]
        public void TestConfiguration() 
        {
            panel.tabControl.SelectedTab = panel.configurationPage;
            manager.Configuration = "<ccnet><project.../></ccnet>";
            AssertEquals("", panel.configurationTextBox.Text);

            panel.loadConfigurationButton.PerformClick();
            AssertEquals("<ccnet><project.../></ccnet>", panel.configurationTextBox.Text);

            panel.configurationTextBox.Text = "something else";
            panel.saveConfigurationButton.PerformClick();
            AssertEquals("something else", manager.Configuration);
        }

        public class MockCruiseManager : ICruiseManager
        {
            public CruiseControlStatus Status = CruiseControlStatus.Running;
            private string configuration;

            public string Configuration 
            {
                get { return configuration; }
                set { configuration = value; }
            }

            public void StopCruiseControl()
            {
                Status = CruiseControlStatus.WillBeStopped;
            }

            public ProjectStatus GetProjectStatus()
            {
                return new ProjectStatus ();
            }

            public void StartCruiseControl()
            {
                Status = CruiseControlStatus.Running;
            }

            public void Run(string project, ISchedule schedule)
            {
            }

            public CruiseControlStatus GetStatus()
            {
                return Status;
            }

            public void StopCruiseControlNow()
            {
                Status = CruiseControlStatus.Stopped;
            }
        }
	}
}
