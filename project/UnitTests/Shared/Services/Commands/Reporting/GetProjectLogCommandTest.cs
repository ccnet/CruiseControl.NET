using System;

using NUnit.Framework;
using NMock;

using ThoughtWorks.CruiseControl.Shared.Services.Commands.Reporting;

namespace ThoughtWorks.CruiseControl.UnitTests.Shared.Services.Commands.Reporting
{
	[TestFixture]
	public class GetProjectLogCommandTest : Assertion
	{
		[Test]
		public void CanRequestLatestLogForDefaultProject()
		{
			GetProjectLogCommand command = new GetProjectLogCommand();

			AssertEquals(GetProjectLogCommandLogType.Latest, command.LogType);
		}

		[Test]
		public void CanRequestDatedLogForDefaultProject()
		{
			DateTime dateRequested = DateTime.Now;
			GetProjectLogCommand command = new GetProjectLogCommand(dateRequested);

			AssertEquals(GetProjectLogCommandLogType.Dated, command.LogType);
			AssertEquals(dateRequested, command.LogDate);
		}

		[Test]
		public void CanRequestLatestLogForSpecifiedProject()
		{
			GetProjectLogCommand command = new GetProjectLogCommand("myProject");

			AssertEquals(GetProjectLogCommandLogType.Latest, command.LogType);
			AssertEquals("myProject", command.ProjectName);
		}

		[Test]
		public void CanRequestDatedLogForSpecifiedProject()
		{
			DateTime dateRequested = DateTime.Now;
			GetProjectLogCommand command = new GetProjectLogCommand("myProject", dateRequested);

			AssertEquals(GetProjectLogCommandLogType.Dated, command.LogType);
			AssertEquals(dateRequested, command.LogDate);
			AssertEquals("myProject", command.ProjectName);
		}

		[Test]
		public void ResultIsAGetProjectLogResult()
		{
			GetProjectLogCommand command = new GetProjectLogCommand();
			GetProjectLogResult result = new GetProjectLogResult("");
			command.Result = result;

			AssertEquals(result, command.Result);
		}
	}
}
