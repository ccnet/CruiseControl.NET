using System;
using System.IO;

using NUnit.Framework;
using marathon.framework;

namespace ThoughtWorks.CruiseControl.ControlPanel.Test
{
	[TestFixture]
	public class MainPanelTest 
	{
		private DirectoryInfo _tmpDir;
		public const string _configFile = @"<cruisecontrol>
  <project name=""MyProject"">
  </project>
</cruisecontrol>";

		[SetUp]
		public void SetUp() 
		{
			_tmpDir = new DirectoryInfo("tmp");
			_tmpDir.Create();
		}

		[TearDown]
		public void TearDown()
		{
			_tmpDir.Delete(true);
		}

		[Test, Ignore("Jeremy's working on it")]
		public void OpeningExistingConfigFileShowsProjectName()
		{
			using (StreamWriter writer = new StreamWriter(_tmpDir + "/ccnet.config")) 
			{
				writer.Write(_configFile);
			}

			TestableMainPanel panel = new TestableMainPanel();
			panel.FileToOpen = _tmpDir + "/ccnet.config";
			using (new MarathonThread(panel)) 
			{
				using (MForm form = new MForm("Cruise Control - Control Panel")) 
				{
					System.Threading.Thread.Sleep(5000);
					form.Press("&File.&Open");

					// this is not working in marathon yet, so we hacked around it
//					using (MForm openFileDialog = new MForm("Open File")) 
//					{
//						openFileDialog.Enter("File Name:", _tmpDir + "/ccnet.config");
//						openFileDialog.Press("Open");
//					}

					form.Check("Project Name:", "MyProject");
				}
			}
		}

		private class TestableMainPanel : MainPanel 
		{
			public string FileToOpen;

			protected override string ChooseFileToOpen()
			{
				return FileToOpen;
			}
		}
	}
}
