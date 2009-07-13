using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote.Messages;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Messages
{
    [TestFixture]
    public class ReadAuditRequestTests
    {
        [Test]
        public void GetSetAllPropertiesWorks()
        {
            ReadAuditRequest request = new ReadAuditRequest();
            request.NumberOfRecords = 12;
            Assert.AreEqual(12, request.NumberOfRecords, "NumberOfRecords fails the get/set test");
            request.StartRecord = 15;
            Assert.AreEqual(15, request.StartRecord, "StartRecord fails the get/set test");
        }

        [Test]
        public void ToStringSerialisesDefaultValues()
        {
            ReadAuditRequest request = new ReadAuditRequest();
            string actual = request.ToString();
            string expected = string.Format("<readAuditMessage xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "timestamp=\"{2:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" identifier=\"{0}\" source=\"{1}\" />",
                request.Identifier,
                request.SourceName,
                request.Timestamp);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToStringSerialisesAllValues()
        {
            ReadAuditRequest request = new ReadAuditRequest();
            request.Identifier = "identifier";
            request.StartRecord = 10;
            request.NumberOfRecords = 20;
            request.Filter = AuditFilters.ByProject("testing");
            request.ServerName = "serverName";
            request.SessionToken = "sessionToken";
            request.SourceName = "sourceName";
            request.Timestamp = DateTime.Now;
            string actual = request.ToString();
            string expected = string.Format("<readAuditMessage xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "timestamp=\"{4:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" identifier=\"{0}\" server=\"{1}\" source=\"{2}\" session=\"{3}\" start=\"{5}\" number=\"{6}\">" +
                "<filter xsi:type=\"ProjectAuditFilter\" project=\"testing\" />" + 
                "</readAuditMessage>",
                request.Identifier,
                request.ServerName,
                request.SourceName,
                request.SessionToken,
                request.Timestamp,
                request.StartRecord,
                request.NumberOfRecords);
            Assert.AreEqual(expected, actual);
        }
    }
}
