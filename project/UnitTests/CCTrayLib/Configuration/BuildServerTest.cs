using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Configuration
{
	[TestFixture]
	public class BuildServerTest
	{
		[Test]
		public void CanBuildADisplayNameFromAServerUrl()
		{
			BuildServer server = new BuildServer("tcp://server:21234/blahblahblah.rem");
			Assert.AreEqual("server", server.DisplayName);			
		}

		[Test]
		public void ForHttpUrlsDisplayNameDisplaysTheEntireUrl()
		{
			BuildServer server = new BuildServer("http://one");
			Assert.AreEqual("http://one", server.DisplayName);
		}

		[Test]
		public void WhenThePortNumberIsNonDefaultThePortNumberIsDisplayed()
		{
			BuildServer server = new BuildServer("tcp://server:123/blahblahblah.rem");
			Assert.AreEqual("server:123", server.DisplayName);			
		}
		
		[Test]
		public void CanParseADisplayNameWithAPort()
		{
			BuildServer server = BuildServer.BuildFromRemotingDisplayName("server:123");
			Assert.AreEqual("tcp://server:123/CruiseManager.rem", server.Url);
		}

		[Test]
		public void CanParseADisplayNameWithoutAPort()
		{
			BuildServer server = BuildServer.BuildFromRemotingDisplayName("server");
			Assert.AreEqual("tcp://server:21234/CruiseManager.rem", server.Url);
		}

        [Test(Description = "Expected string in format server[:port]")]
        public void ThrowsWhenParsingAStringThatContainsMoreThanOneColon()
        {
            Assert.That(delegate { BuildServer.BuildFromRemotingDisplayName("tcp://server:123/blah.rem"); },
                        Throws.TypeOf<ApplicationException>());
        }

        [Test(Description = "Port number must be an integer")]
        public void ThrowsWhenParsingAStringWithNonNumericPortNumber()
        {
            Assert.That(delegate { BuildServer.BuildFromRemotingDisplayName("server:xxx"); },
                        Throws.TypeOf<ApplicationException>());
        }

        [Test(Description = "Extension transport must always define an extension name")]
        public void ThrowsWhenMissingExtension()
        {
            Assert.That(delegate { new BuildServer("http://test", BuildServerTransport.Extension, null, null); },
                        Throws.TypeOf<CCTrayLibException>());
        }

        [Test]
        public void SetExtensionName()
        {
            BuildServer newServer = new BuildServer();
            newServer.ExtensionName = "new extension";
            Assert.AreEqual("new extension", newServer.ExtensionName);
        }

        [Test]
        public void SetExtensionSettings()
        {
            BuildServer newServer = new BuildServer();
            newServer.ExtensionSettings = "new extension";
            Assert.AreEqual("new extension", newServer.ExtensionSettings);
        }

        [Test]
        public void SetTransport()
        {
            BuildServer newServer = new BuildServer();
            newServer.Transport = BuildServerTransport.Extension;
            Assert.AreEqual(BuildServerTransport.Extension, newServer.Transport);
        }
		
		[Test]
		public void TwoBuildServersAreEqualIfTheirUrlsAreTheSame()
		{
			BuildServer one = new BuildServer("http://one");
			BuildServer anotherOne = new BuildServer("http://one");
			BuildServer two = new BuildServer("http://two");
			
			Assert.AreEqual(one, one);
			Assert.AreEqual(one, anotherOne);
			Assert.AreEqual(anotherOne, one);
			Assert.IsFalse(one.Equals(two));
			Assert.IsFalse(two.Equals(one));
		}

	}
}