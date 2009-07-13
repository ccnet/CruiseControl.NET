using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote.Messages;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Messages
{
    [TestFixture]
    public class BuildIntegrationRequestTests
    {
        [Test]
        public void GetSetAllPropertiesWorks()
        {
            BuildIntegrationRequest request = new BuildIntegrationRequest();
            request.BuildCondition = BuildCondition.IfModificationExists;
            Assert.AreEqual(BuildCondition.IfModificationExists, request.BuildCondition, "BuildCondition fails the get/set test");
        }

        [Test]
        public void AddBuildValueAddsToUnderlyingList()
        {
            BuildIntegrationRequest request = new BuildIntegrationRequest();
            request.AddBuildValue("value1", "actual value");
            string actual = NameValuePair.FindNamedValue(request.BuildValues, "value1");
            Assert.AreEqual("actual value", actual);
        }

        [Test]
        public void InitialiseRequestWithSessionSetsTheCorrectValues()
        {
            string sessionToken = "the session";
            DateTime now = DateTime.Now;
            BuildIntegrationRequest request = new BuildIntegrationRequest(sessionToken);
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
            BuildIntegrationRequest request = new BuildIntegrationRequest(sessionToken, projectName);
            Assert.IsFalse(string.IsNullOrEmpty(request.Identifier), "Identifier was not set");
            Assert.AreEqual(Environment.MachineName, request.SourceName, "Source name doesn't match the machine name");
            Assert.AreEqual(sessionToken, request.SessionToken, "SessionToken doesn't match the input token");
            Assert.AreEqual(projectName, request.ProjectName, "ProjectName doesn't match the input project name");
            Assert.IsTrue((now <= request.Timestamp), "Timestamp was not set");
        }

        [Test]
        public void ToStringSerialisesDefaultValues()
        {
            BuildIntegrationRequest request = new BuildIntegrationRequest();
            string actual = request.ToString();
            string expected = string.Format("<integrationMessage xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "timestamp=\"{2:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" identifier=\"{0}\" source=\"{1}\" condition=\"{3}\" />",
                request.Identifier,
                request.SourceName,
                request.Timestamp,
                request.BuildCondition);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToStringSerialisesAllValues()
        {
            BuildIntegrationRequest request = new BuildIntegrationRequest();
            request.Identifier = "identifier";
            request.ProjectName = "projectName";
            request.ServerName = "serverName";
            request.SessionToken = "sessionToken";
            request.SourceName = "sourceName";
            request.Timestamp = DateTime.Now;
            request.BuildCondition = BuildCondition.IfModificationExists;
            request.AddBuildValue("value1", "actual value");
            string actual = request.ToString();
            string expected = string.Format("<integrationMessage xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "timestamp=\"{4:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" identifier=\"{0}\" server=\"{1}\" source=\"{2}\" session=\"{3}\" project=\"{5}\" condition=\"{6}\">" + 
                "<buildValue name=\"value1\" value=\"actual value\" />" + 
                "</integrationMessage>",
                request.Identifier,
                request.ServerName,
                request.SourceName,
                request.SessionToken,
                request.Timestamp,
                request.ProjectName,
                request.BuildCondition);
            Assert.AreEqual(expected, actual);
        }
    }
}
