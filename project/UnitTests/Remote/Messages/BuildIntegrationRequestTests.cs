﻿namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Remote.Messages;

    [TestFixture]
    public class BuildIntegrationRequestTests
    {
        [Test]
        public void GetSetAllPropertiesWorks()
        {
            BuildIntegrationRequest request = new BuildIntegrationRequest();
            var values = new List<NameValuePair>();
            request.BuildCondition = BuildCondition.IfModificationExists;
            request.BuildValues = values;
            Assert.AreSame(values, request.BuildValues);
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

            using (new AssertionScope())
            {
                request.Identifier.Should().NotBeNullOrEmpty("Identifier should be set");
                request.SourceName.Should().BeEquivalentTo(Environment.MachineName, "SourceName should match the machine name");
                request.SessionToken.Should().BeEquivalentTo(sessionToken, "SessionToken should match the input token");
                request.ProjectName.Should().BeEquivalentTo(projectName, "ProjectName should match the input project name");
                request.Timestamp.Should().Be(now, "Timestamp must be set");
            }
        }

        [Test]
        public void ToStringSerialisesDefaultValues()
        {
            BuildIntegrationRequest request = new BuildIntegrationRequest();
            string actual = request.ToString();
            string expected = string.Format(System.Globalization.CultureInfo.CurrentCulture,"<integrationMessage xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "timestamp=\"{2:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" identifier=\"{0}\" source=\"{1}\" condition=\"{3}\" />",
                request.Identifier,
                request.SourceName,
                request.Timestamp,
                request.BuildCondition);

            XDocument.Parse(actual).Should().BeEquivalentTo(XDocument.Parse(expected));
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
            string expected = string.Format(System.Globalization.CultureInfo.CurrentCulture,"<integrationMessage xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
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

            XDocument.Parse(actual).Should().BeEquivalentTo(XDocument.Parse(expected));
        }
    }
}
