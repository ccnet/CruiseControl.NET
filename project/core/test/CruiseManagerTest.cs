using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using System.Threading;

using NUnit.Framework;

using tw.ccnet.core.util;
using tw.ccnet.remote;

namespace tw.ccnet.core.test
{
	[TestFixture]
	public class CruiseManagerTest : Assertion
	{
		[Test]
		[Ignore("takes too long, will move to acceptanceTests")]
		public void RemotingStopNowAndRestart() 
		{
			Process p = new Process();
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardInput = true;
			p.StartInfo.Arguments = " -remoting:on ..\\ccnet-config\\cclog\\ccnet.config";
			p.StartInfo.FileName = "ccnet.exe";
			p.StartInfo.WorkingDirectory = "D:\\cvsprojects\\ccnet\\build";
			
			try 
			{
				p.Start();
				Thread.Sleep(20000);
				CruiseManager _manager = (CruiseManager) Activator.GetObject(typeof(CruiseManager),string.Format("tcp://localhost:{0}/CruiseManager.soap", CruiseManager.TCP_PORT));
				CruiseControlStatus _status = _manager.GetStatus();

				Assertion.AssertEquals(CruiseControlStatus.Running, _status);

				_manager.StopCruiseControlNow();

				_status = _manager.GetStatus();

				Assertion.AssertEquals(CruiseControlStatus.Stopped, _status);

				_manager.StartCruiseControl();

				_status = _manager.GetStatus();

				Assertion.AssertEquals(CruiseControlStatus.Running, _status);
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
            string xml = "<cruisecontrol></cruisecontrol>";
            string file = TempFileUtil.CreateTempXmlFile(TempFileUtil.CreateTempDir(this), "loadernet.config", xml);
            CruiseManager manager = new CruiseManager(file);
            
            AssertEquals("<cruisecontrol></cruisecontrol>", manager.Configuration);
            manager.Configuration = SimpleBuildFile.Content;
            AssertEquals(SimpleBuildFile.Content, manager.Configuration);
        }
	}
}
