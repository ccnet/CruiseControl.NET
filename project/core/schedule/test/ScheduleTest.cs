using System;
using System.Xml;
using System.Reflection;
using System.Threading;

using Exortech.NetReflector;
using NUnit.Framework;

using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Schedules.Test
{
	[TestFixture]
	public class ScheduleTest : CustomAssertion
	{
		[Test]
		public void PopulateFromReflector()
		{
			string xml = string.Format(@"<schedule sleepSeconds=""1"" iterations=""1""/>");
			Schedule schedule = (Schedule)NetReflector.Read(xml);
			AssertEquals(1, schedule.SleepSeconds);
			AssertEquals(1, schedule.TotalIterations);
		}

		[Test]
		public void ForceBuild()
		{
			Schedule schedule = new Schedule();
			schedule.SleepSeconds = 10; // longer than this test (ignore)

			AssertEquals(BuildCondition.IfModificationExists, schedule.ShouldRunIntegration());
			schedule.IntegrationCompleted();
			AssertEquals(BuildCondition.NoBuild, schedule.ShouldRunIntegration());
			
			schedule.ForceBuild();
			AssertEquals(BuildCondition.ForceBuild, schedule.ShouldRunIntegration());
			schedule.IntegrationCompleted();
			AssertEquals(BuildCondition.NoBuild, schedule.ShouldRunIntegration());
			
			schedule.ForceBuild();
			schedule.ForceBuild();
			AssertEquals(BuildCondition.ForceBuild, schedule.ShouldRunIntegration());
			schedule.IntegrationCompleted();
			AssertEquals(BuildCondition.NoBuild, schedule.ShouldRunIntegration());
		}

		[Test]
		public void ShouldRunIntegration_SleepTime()
		{
			Schedule schedule = new Schedule();
			schedule.SleepSeconds = 1;

			AssertEquals(BuildCondition.IfModificationExists, schedule.ShouldRunIntegration());
			schedule.IntegrationCompleted();
			AssertEquals(BuildCondition.NoBuild, schedule.ShouldRunIntegration());

			Thread.Sleep(750);

			// still before 1sec
			AssertEquals(BuildCondition.NoBuild, schedule.ShouldRunIntegration());
			
			// sleep beyond the 1sec mark
			Thread.Sleep(500);
			
			AssertEquals(BuildCondition.IfModificationExists, schedule.ShouldRunIntegration());
			schedule.IntegrationCompleted();
			AssertEquals(BuildCondition.NoBuild, schedule.ShouldRunIntegration());
		}

		[Test]
		public void ShouldRunIntegration_SleepsFromEndOfIntegration()
		{
			Schedule schedule = new Schedule();
			schedule.SleepSeconds = 0.5;

			AssertEquals(BuildCondition.IfModificationExists, schedule.ShouldRunIntegration());

			Thread.Sleep(550);

			AssertEquals(BuildCondition.IfModificationExists, schedule.ShouldRunIntegration());
			schedule.IntegrationCompleted();
			AssertEquals(BuildCondition.NoBuild, schedule.ShouldRunIntegration());

			Thread.Sleep(550);

			AssertEquals(BuildCondition.IfModificationExists, schedule.ShouldRunIntegration());
			schedule.IntegrationCompleted();
			AssertEquals(BuildCondition.NoBuild, schedule.ShouldRunIntegration());

			Thread.Sleep(550);

			AssertEquals(BuildCondition.IfModificationExists, schedule.ShouldRunIntegration());
		}

		[Test]
		public void ShouldStopIntegration()
		{
			Schedule schedule = new Schedule();
			schedule.TotalIterations = 2;

			Assert(!schedule.ShouldStopIntegration());
			schedule.IntegrationCompleted();
			Assert(!schedule.ShouldStopIntegration());
			schedule.IntegrationCompleted();
			Assert(schedule.ShouldStopIntegration());
			AssertEquals(BuildCondition.NoBuild, schedule.ShouldRunIntegration());
		}
	}
}
