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


//		[Test]
//		public void MainPanelOpensAndLooksGood() 
//		{
//			Assert.IsTrue(Form.Visible);
//			Assert.AreEqual("Cruise Control - Control Panel", Form.Text);
//		}

		[Test]
		public void OpeningExistingConfigFileShowsProjectName()
		{
			using (StreamWriter writer = new StreamWriter(_tmpDir + "/ccnet.config")) 
			{
				writer.Write(_configFile);
			}

			MainPanel panel = new MainPanel();
			using (new MarathonThread(panel)) 
			{
				using (MForm form = new MForm("Cruise Control - Control Panel")) 
				{
					form.Press("File");
					form.Press("Open");

					using (MForm openFileDialog = new MForm("Open File")) 
					{
						openFileDialog.Enter("File Name:", _tmpDir + "/ccnet.config");
						openFileDialog.Press("Open");
					}

					form.Check("Project Name:", "MyProject");
				}
			}
		}
	}
}
