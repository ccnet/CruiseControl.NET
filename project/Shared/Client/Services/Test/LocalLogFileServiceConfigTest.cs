using System;
using System.Collections;

using NUnit.Framework;
using NMock;

using ThoughtWorks.CruiseControl.Shared.Client.Services;
using ThoughtWorks.CruiseControl.Shared.Services.Commands.Reporting;

namespace ThoughtWorks.CruiseControl.Shared.Client.Services.Test
{
	[TestFixture]
	public class LocalLogFileServiceConfigTest
	{
		[Test]
		public void ReturnsCorrectServiceType()
		{
			LocalLogFileServiceConfig config = new LocalLogFileServiceConfig();
			Assert.AreEqual(typeof(LocalLogFileService), config.ServiceType);
		}

		// This test will be invalid when we support multi-project instances
		[Test]
		public void DefaultProjectLogDirectoryIsSpecifiedLogDirectory()
		{
			LocalLogFileServiceConfig config = new LocalLogFileServiceConfig();
			config.LogDirectoryName = "testdir";
			Assert.AreEqual("testdir", config.GetDefaultProjectLogDirectory());
		}
	}
}
