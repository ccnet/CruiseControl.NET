using Exortech.NetReflector;
using NUnit.Framework;
using System;
using System.Threading;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Schedules;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace integration
{
	[TestFixture]
	public class StartLocalServer : Assertion
	{
		private AutoResetEvent wait;
		private int callCount;

		[SetUp]
		protected void SetUp()
		{
			TempFileUtil.CreateTempDir("StartLocalServer"); 
			wait = new AutoResetEvent(false);
			callCount = 0;
		}

		[TearDown]
		protected void TearDown()
		{
			TempFileUtil.DeleteTempDir("StartLocalServer"); 
		}

		[Test]
		public void StartCCServer()
		{
			string xml = CreateBasicProjectXml();
			string configFile = TempFileUtil.CreateTempXmlFile("StartLocalServer", "ccnet.config", xml);
			IConfigurationContainer config = new ConfigurationContainer(configFile);
			ICruiseServer server = new CruiseServer(config);
			server.Start();

			Project project = (Project)config.Projects["foo"];
			project.IntegrationCompleted += new IntegrationCompletedEventHandler(this.WaitForBuildToComplete);
			wait.WaitOne();
			server.Stop();
		}

		private void WaitForBuildToComplete(object sender, IntegrationCompletedEventArgs e)
		{
			callCount++;
			Console.WriteLine("yo: " + callCount);
			if (callCount > 5)
			{
				wait.Set();
			}
		}

		private string CreateBasicProjectXml()
		{
			Project project = new Project();
			project.Name = "foo";
			project.SourceControl = new ThoughtWorks.CruiseControl.Core.Sourcecontrol.DefaultSourceControl();
			project.Builder = new MockBuilder();
			((Schedule)project.Schedule).SleepSeconds = 1;
			((Schedule)project.Schedule).TotalIterations = 5;

			string xml = string.Format("<cruisecontrol>{0}</cruisecontrol>", NetReflector.Write(project));
			Console.WriteLine(xml);
			return xml;
		}
	}
}
