using System;
using System.Collections;

using NUnit.Framework;
using NMock;

using ThoughtWorks.CruiseControl.Shared.Client.Services;
using ThoughtWorks.CruiseControl.Shared.Services.Commands.Reporting;

namespace ThoughtWorks.CruiseControl.UnitTests.Shared.Client.Services
{
	[TestFixture]
	public class LocalLogFileServiceConfigTest : Assertion
	{
		[Test]
		public void ReturnsCorrectServiceType()
		{
			LocalLogFileServiceConfig config = new LocalLogFileServiceConfig();
			AssertEquals(typeof(LocalLogFileService), config.ServiceType);
		}

		// This test will be invalid when we support multi-project instances
		[Test]
		public void DefaultProjectLogDirectoryIsSpecifiedLogDirectory()
		{
			LocalLogFileServiceConfig config = new LocalLogFileServiceConfig();
			config.LogDirectoryName = "testdir";
			AssertEquals("testdir", config.GetDefaultProjectLogDirectory());
		}
	}
}
