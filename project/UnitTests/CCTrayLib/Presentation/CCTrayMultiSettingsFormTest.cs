using System;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation
{
	[TestFixture]
	public class CCTrayMultiSettingsFormTest
	{
		[Test]
        [Platform(Exclude = "Mono", Reason = "No X display available")]
		public void ShouldCloneConfigurationAndOnlyBindToTheClone()
		{
			var existingConfiguration = new Mock<ICCTrayMultiConfiguration>(MockBehavior.Strict);
			CCTrayMultiConfiguration clonedConfiguration = new CCTrayMultiConfiguration(null, null, null);
			existingConfiguration.Setup(_configuration => _configuration.Clone()).Returns(clonedConfiguration).Verifiable();
			
			NullReferenceException nullReference = null;
			try
			{
				new CCTrayMultiSettingsForm((ICCTrayMultiConfiguration)existingConfiguration.Object);
			}
			catch (NullReferenceException e)
			{
				nullReference = e;
			}

			// As we are using a Strict mock, incorrect calls to the existing configuration 
			// will be caught by the verify.
			existingConfiguration.Verify();
			Assert.IsNull(nullReference,
				"There was a null reference exception not related to using existing configuration:\n{0}",
				nullReference);
		}
	}
}
