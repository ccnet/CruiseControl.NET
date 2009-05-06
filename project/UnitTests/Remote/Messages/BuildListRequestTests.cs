using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote.Messages;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Messages
{
    [TestFixture]
    public class BuildListRequestTests
    {
        [Test]
        public void GetSetAllPropertiesWorks()
        {
            BuildListRequest request = new BuildListRequest();
            request.NumberOfBuilds = 6;
            Assert.AreEqual(6, request.NumberOfBuilds, "NumberOfBuilds fails the get/set test");
        }

        [Test]
        public void InitialiseRequestWithSessionSetsTheCorrectValues()
        {
            string sessionToken = "the session";
            DateTime now = DateTime.Now;
            BuildListRequest request = new BuildListRequest(sessionToken);
            Assert.IsFalse(string.IsNullOrEmpty(request.Identifier), "Identifier was not set");
            Assert.AreEqual(Environment.MachineName, request.SourceName, "Source name doesn't match the machine name");
            Assert.AreEqual(sessionToken, request.SessionToken, "SessionToken doesn't match the input token");
            Assert.IsTrue((now <= request.Timestamp), "Timestamp was not set");
        }

        [Test]
        public void InitialiseRequestWithSessionAndProjectSetsTheCorrectValues()
        {
            string sessionToken = "the session";
            string projectName = "the project";
            DateTime now = DateTime.Now;
            BuildListRequest request = new BuildListRequest(sessionToken, projectName);
            Assert.IsFalse(string.IsNullOrEmpty(request.Identifier), "Identifier was not set");
            Assert.AreEqual(Environment.MachineName, request.SourceName, "Source name doesn't match the machine name");
            Assert.AreEqual(sessionToken, request.SessionToken, "SessionToken doesn't match the input token");
            Assert.AreEqual(projectName, request.ProjectName, "ProjectName doesn't match the input project name");
            Assert.IsTrue((now <= request.Timestamp), "Timestamp was not set");
        }

        [Test]
        public void ToStringSerialisesDefaultValues()
        {
            BuildListRequest request = new BuildListRequest();
            string actual = request.ToString();
            string expected = string.Format("<buildListMessage xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "identifier=\"{0}\" source=\"{1}\" timestamp=\"{2:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" number=\"0\" />",
                request.Identifier,
                request.SourceName,
                request.Timestamp);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToStringSerialisesAllValues()
        {
            BuildListRequest request = new BuildListRequest();
            request.Identifier = "identifier";
            request.ServerName = "serverName";
            request.SessionToken = "sessionToken";
            request.SourceName = "sourceName";
            request.Timestamp = DateTime.Now;
            request.NumberOfBuilds = 6;
            string actual = request.ToString();
            string expected = string.Format("<buildListMessage xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "identifier=\"{0}\" server=\"{1}\" source=\"{2}\" session=\"{3}\" timestamp=\"{4:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" number=\"6\" />",
                request.Identifier,
                request.ServerName,
                request.SourceName,
                request.SessionToken,
                request.Timestamp);
            Assert.AreEqual(expected, actual);
        }
    }
}
