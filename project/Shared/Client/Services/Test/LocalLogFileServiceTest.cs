using System;
using System.Collections;
using System.IO;

using NUnit.Framework;
using NMock;

using ThoughtWorks.CruiseControl.Shared.Client.Services;
using ThoughtWorks.CruiseControl.Shared.Services.Commands.Reporting;
using ThoughtWorks.CruiseControl.Shared.Util;
using ThoughtWorks.CruiseControl.Shared.Entities.Logging;
using ThoughtWorks.CruiseControl.Shared.Services;

namespace ThoughtWorks.CruiseControl.Shared.Client.Services.Test
{
	[TestFixture]
	public class LocalLogFileServiceTest : Assertion
	{
		DynamicMock _configMock;
		LocalLogFileServiceConfig _config;

		DateTime _testDate;
		string _tempDirLocation;
		string _tempFileLocation;

		[SetUp]
		public void SetUp()
		{
			_configMock = new DynamicMock(typeof(LocalLogFileServiceConfig));
			_config = _configMock.MockInstance as LocalLogFileServiceConfig;
			_testDate = DateTime.Now;
			_tempDirLocation = TempFileUtil.CreateTempDir("projectdir");
			_tempFileLocation = TempFileUtil.CreateTempFile(_tempDirLocation, LogFileUtil.CreateFailedBuildLogFileName(_testDate),
				"test log data");
		}

		[TearDown]
		public void TearDown()
		{
			TempFileUtil.DeleteTempDir(_tempDirLocation);
		}

		[Test]
		public void ReturnsNoValidServiceFoudnResultIfInvalidCommandRequested()
		{
			LocalLogFileService service = new LocalLogFileService(_config);
			ICruiseCommand command = (ICruiseCommand) new DynamicMock(typeof(ICruiseCommand)).MockInstance;

			AssertEquals(typeof(NoValidServiceFoundResult), service.Run(command).GetType());
		}

		[Test]
		public void CanGetLatestLogForDefaultProject()
		{
			_configMock.SetupResult("GetDefaultProjectLogDirectory", _tempDirLocation);

			LocalLogFileService service = new LocalLogFileService(_config);
			GetProjectLogResult result = (GetProjectLogResult) service.Run(new GetProjectLogCommand());

			AssertEquals("test log data", result.Log);

			_configMock.Verify();
		}
	}
}
