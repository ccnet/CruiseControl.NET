using System;

using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.Shared.Services.Commands.Reporting.Test
{
	[TestFixture]
	public class GetProjectLogCommandTest
	{
		[Test]
		public void CanRequestLatestLogForDefaultProject()
		{
			GetProjectLogCommand command = new GetProjectLogCommand();

			Assert.AreEqual(GetProjectLogCommandLogType.Latest, command.LogType);
		}

		[Test]
		public void CanRequestDatedLogForDefaultProject()
		{
			DateTime dateRequested = DateTime.Now;
			GetProjectLogCommand command = new GetProjectLogCommand(dateRequested);

			Assert.AreEqual(GetProjectLogCommandLogType.Dated, command.LogType);
			Assert.AreEqual(dateRequested, command.LogDate);
		}

		[Test]
		public void CanRequestLatestLogForSpecifiedProject()
		{
			GetProjectLogCommand command = new GetProjectLogCommand("myProject");

			Assert.AreEqual(GetProjectLogCommandLogType.Latest, command.LogType);
			Assert.AreEqual("myProject", command.ProjectName);
		}

		[Test]
		public void CanRequestDatedLogForSpecifiedProject()
		{
			DateTime dateRequested = DateTime.Now;
			GetProjectLogCommand command = new GetProjectLogCommand("myProject", dateRequested);

			Assert.AreEqual(GetProjectLogCommandLogType.Dated, command.LogType);
			Assert.AreEqual(dateRequested, command.LogDate);
			Assert.AreEqual("myProject", command.ProjectName);
		}

		[Test]
		public void ResultIsAGetProjectLogResult()
		{
			GetProjectLogCommand command = new GetProjectLogCommand();
			GetProjectLogResult result = new GetProjectLogResult("");
			command.Result = result;

			Assert.AreEqual(result, command.Result);
		}
	}
}
