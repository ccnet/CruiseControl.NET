using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation
{
	[TestFixture]
	public class CCTrayMultiSettingsFormTest
	{
		[Test]
		public void ShouldCloneConfigurationAndOnlyBindToTheClone()
		{
			DynamicMock existingConfiguration = new DynamicMock(typeof(ICCTrayMultiConfiguration));
			existingConfiguration.Strict = true;
			CCTrayMultiConfiguration clonedConfiguration = new CCTrayMultiConfiguration(null, null, null);
			existingConfiguration.ExpectAndReturn("Clone", clonedConfiguration);
			
			NullReferenceException nullReference = null;
			try
			{
				new CCTrayMultiSettingsForm((ICCTrayMultiConfiguration)existingConfiguration.MockInstance);
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
