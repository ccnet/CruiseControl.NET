using System;
using System.Diagnostics;
using System.Threading;

using NMock;

using NUnit.Framework;

using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Test
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
		public void GetSetConfiguration() 
		{
			//			string fileName = CreateTestingCruiseControlConfigurationFile();
			//			CruiseManager manager = new CruiseManager(new CruiseServer(new ConfigurationLoader(fileName)));
			//            
			//			AssertEquals("<cruisecontrol></cruisecontrol>", manager.Configuration);
			//			string xml = SimpleBuildFile.Document.OuterXml;
			//			manager.Configuration = xml;
			//			AssertEquals(xml, manager.Configuration);
		}

		[Test]
		public void ForceBuild()
		{
			string testProjectName = "TestProjectName";
			MockProject mockProject = new MockProject(testProjectName, null);

			Mock mockCC = new DynamicMock(typeof(ICruiseControl));
			mockCC.Expect("ForceBuild", testProjectName);

			CruiseManager manager = new CruiseManager((ICruiseControl)mockCC.MockInstance);
			manager.ForceBuild(testProjectName);

			mockCC.Verify();
		}

		#region Helper methods

		private string CreateTestingCruiseControlConfigurationFile()
		{
			string xml = "<cruisecontrol></cruisecontrol>";
			return TempFileUtil.CreateTempXmlFile(TempFileUtil.CreateTempDir(this), "loadernet.config", xml);
		}

		#endregion
	}
}
