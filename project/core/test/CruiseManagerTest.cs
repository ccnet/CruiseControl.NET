using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using System.Threading;

using NUnit.Framework;

using tw.ccnet.core.util;
using tw.ccnet.remote;
using tw.ccnet.core.schedule;
using tw.ccnet.core.schedule.test;

namespace tw.ccnet.core.test
{
	[TestFixture]
	public class CruiseManagerTest : CustomAssertion
	{
		[Test]
		[Ignore("takes too long, will move to acceptanceTests")]
		public void RemotingStopNowAndRestart() 
		{
			Process p = new Process();
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardInput = true;
			p.StartInfo.Arguments = @" -remoting:on ..\ccnet-config\cclog\ccnet.config";
			p.StartInfo.FileName = "ccnet.exe";
			p.StartInfo.WorkingDirectory = @"D:\cvsprojects\ccnet\build";
			
			try 
			{
				p.Start();
				Thread.Sleep(20000);
				CruiseManager _manager = (CruiseManager) Activator.GetObject(typeof(CruiseManager),string.Format("tcp://localhost:{0}/CruiseManager.soap", CruiseManager.TCP_PORT));
				CruiseControlStatus _status = _manager.GetStatus();

				AssertEquals(CruiseControlStatus.Running, _status);

				_manager.StopCruiseControlNow();

				_status = _manager.GetStatus();

				AssertEquals(CruiseControlStatus.Stopped, _status);

				_manager.StartCruiseControl();

				_status = _manager.GetStatus();

				AssertEquals(CruiseControlStatus.Running, _status);
			} 
			finally 
			{
				p.StandardInput.WriteLine(" ");
				p.WaitForExit();
			}
		}

		[Test]
		public void TestStartStopCruise() 
		{
			// stellsmi - in progress
		}

		[Test]
		public void TestGetSetConfiguration() 
		{
			string fileName = CreateTestingCruiseControlConfigurationFile();
			CruiseManager manager = new CruiseManager(fileName);
            
			AssertEquals("<cruisecontrol></cruisecontrol>", manager.Configuration);
			manager.Configuration = SimpleBuildFile.Content;
			AssertEquals(SimpleBuildFile.Content, manager.Configuration);
		}
		/*
				[Test]
				public void ForceBuild()
				{
					string testProjectName = "TestProjectName";

					MockCruiseControl mockCC = new MockCruiseControl();
					CruiseManager manager = new CruiseManager(mockCC);
					MockProject mockProject = new MockProject(testProjectName, null);
					mockCC.GetProject_ReturnValue = mockProject;

					AssertEquals(0, mockCC.GetProject_CallCount);
					AssertEquals(0, mockProject.RunIntegration_CallCount);

					// server is sleeping, so build can occur
					mockProject.CurrentActivity = ProjectActivity.Sleeping;
			
					// we're testing this method
					manager.ForceBuild(testProjectName);

					AssertEquals(1, mockCC.GetProject_CallCount);
					AssertEquals(testProjectName, mockCC.GetProject_projectName);
					AssertEquals(1, mockProject.RunIntegration_CallCount);
					Assert(mockProject.RunIntegration_forceBuild);
				}
		*/

		[Test]
		public void ForceBuild_AlreadyBuilding()
		{
			string testProjectName = "TestProjectName";

			MockCruiseControl mockCC = new MockCruiseControl();
			CruiseManager manager = new CruiseManager(mockCC);
			MockSchedule schedule = new MockSchedule();
			MockProject mockProject = new MockProject(testProjectName, schedule);
			mockCC.GetProject_ReturnValue = mockProject;
			
			// already building
			mockProject.CurrentActivity = ProjectActivity.Building;

			AssertEquals(0, schedule.ForceBuild_CallCount);

			// we're testing this method
			manager.ForceBuild(testProjectName);

			AssertEquals(1, schedule.ForceBuild_CallCount);
		}

		// TODO what happens when someone forces a build, while a build is already running?  this situation would suck...

		#region Helper methods

		private string CreateTestingCruiseControlConfigurationFile()
		{
			string xml = "<cruisecontrol></cruisecontrol>";
			return TempFileUtil.CreateTempXmlFile(TempFileUtil.CreateTempDir(this), "loadernet.config", xml);
		}

		#endregion
	}
}
