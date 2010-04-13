namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Messages
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote.Messages;

    [TestFixture]
    public class DiagnoseSecurityRequestTests
    {
        [Test]
        public void GetSetAllPropertiesWorks()
        {
            DiagnoseSecurityRequest request = new DiagnoseSecurityRequest();
            request.UserName = "new user";
            Assert.AreEqual("new user", request.UserName, "UserName fails the get/set test");
            var projects = new List<string>();
            request.Projects = projects;
            Assert.AreSame(projects, request.Projects);
        }

        [Test]
        public void ToStringSerialisesDefaultValues()
        {
            DiagnoseSecurityRequest request = new DiagnoseSecurityRequest();
            string actual = request.ToString();
            string expected = string.Format("<diagnoseSecurityMessage xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "timestamp=\"{2:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" identifier=\"{0}\" source=\"{1}\" />",
                request.Identifier,
                request.SourceName,
                request.Timestamp);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToStringSerialisesAllValues()
        {
            DiagnoseSecurityRequest request = new DiagnoseSecurityRequest();
            request.Identifier = "identifier";
            request.UserName = "user name";
            request.ServerName = "serverName";
            request.SessionToken = "sessionToken";
            request.SourceName = "sourceName";
            request.Timestamp = DateTime.Now;
            request.Projects.Add("test project");
            string actual = request.ToString();
            string expected = string.Format("<diagnoseSecurityMessage xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "timestamp=\"{4:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" identifier=\"{0}\" server=\"{1}\" source=\"{2}\" session=\"{3}\" userName=\"{5}\">" +
                "<project>test project</project>" +
                "</diagnoseSecurityMessage>",
                request.Identifier,
                request.ServerName,
                request.SourceName,
                request.SessionToken,
                request.Timestamp,
                request.UserName);
            Assert.AreEqual(expected, actual);
        }
    }
}
