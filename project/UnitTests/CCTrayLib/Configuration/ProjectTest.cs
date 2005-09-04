using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Configuration
{
	[TestFixture]
	public class ProjectTest
	{
		[Test]
		public void CanBuildADisplayNameFromAServerUrl()
		{
			Project project = new Project("tcp://server:21234/blahblahblah.rem", "projectName");
			Assert.AreEqual("server", project.ServerDisplayName);			
		}

		[Test]
		public void WhenThePortNumberIsNonDefaultThePortNumberIsDisplayed()
		{
			Project project = new Project("tcp://server:123/blahblahblah.rem", "projectName");
			Assert.AreEqual("server:123", project.ServerDisplayName);			
		}
		
		[Test]
		public void CanParseADisplayNameWithAPort()
		{
			Project project = new Project();	
			project.SetServerUrlFromDisplayName("server:123");
			Assert.AreEqual("tcp://server:123/CruiseManager.rem", project.ServerUrl);
		}

		[Test]
		public void CanParseADisplayNameWithoutAPort()
		{
			Project project = new Project();	
			project.SetServerUrlFromDisplayName("server");
			Assert.AreEqual("tcp://server:21234/CruiseManager.rem", project.ServerUrl);
		}
		
		[Test]
		[ExpectedException(typeof(ApplicationException), "Expected string in format server[:port]")]
		public void ThrowsWhenParsingAStringThatContainsMoreThanOneColon()
		{
			Project project = new Project();	
			project.SetServerUrlFromDisplayName("tcp://server:123/blah.rem");
		}
		
		[Test]
		[ExpectedException(typeof(ApplicationException), "Port number must be an integer")]
		public void ThrowsWhenParsingAStringWithNonNumericPortNumber()
		{
			Project project = new Project();	
			project.SetServerUrlFromDisplayName("server:xxx");
		}

	}
}
