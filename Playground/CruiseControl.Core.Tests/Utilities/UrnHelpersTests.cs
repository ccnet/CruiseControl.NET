namespace CruiseControl.Core.Tests.Utilities
{
    using System;
    using Core.Utilities;
    using NUnit.Framework;

    [TestFixture]
    public class UrnHelpersTests
    {
        #region Tests
        [Test]
        public void IsCCNetUrnReturnsTrueForCCNetUrn()
        {
            Assert.IsTrue(UrnHelpers.IsCCNetUrn("urn:ccnet:local"));
        }

        [Test]
        public void IsCCNetUrnReturnsFalseWhenMissingCCNet()
        {
            Assert.IsFalse(UrnHelpers.IsCCNetUrn("urn:local"));
        }

        [Test]
        public void IsCCNetUrnReturnsFalseWhenNotAUrn()
        {
            Assert.IsFalse(UrnHelpers.IsCCNetUrn("http://local"));
        }

        [Test]
        public void IsCCNetUrnReturnsFalseWhenUrnMissing()
        {
            Assert.IsFalse(UrnHelpers.IsCCNetUrn(null));
        }

        [Test]
        public void GenerateProjectUrnDoesNothingIfProjectAlreadyAUrn()
        {
            var server = "urn:ccnet:local";
            var project = "urn:ccnet:local:project";
            var expected = "urn:ccnet:local:project";
            var actual = UrnHelpers.GenerateProjectUrn(server, project);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GenerateProjectUrnGeneratesUrnUsingServerUrn()
        {
            var server = "urn:ccnet:local";
            var project = "project";
            var expected = "urn:ccnet:local:project";
            var actual = UrnHelpers.GenerateProjectUrn(server, project);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GenerateProjectUrnGeneratesUrnUsingServerName()
        {
            var server = "local";
            var project = "project";
            var expected = "urn:ccnet:local:project";
            var actual = UrnHelpers.GenerateProjectUrn(server, project);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GenerateProjectUrnFailsIfServerMissing()
        {
            Assert.Throws<ArgumentNullException>(
                () => UrnHelpers.GenerateProjectUrn((Server)null, "project"));
        }

        [Test]
        public void GenerateProjectUrnGeneratesFromServer()
        {
            var server = new Server("thisServer");
            var project = "local";
            var expected = "urn:ccnet:thisServer:local";
            var actual = UrnHelpers.GenerateProjectUrn(server, project);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GenerateProjectUrnFailsIfServerNameBlank()
        {
            Assert.Throws<ArgumentNullException>(
                () => UrnHelpers.GenerateProjectUrn(string.Empty, "project"));
        }

        [Test]
        public void GenerateProjectUrnFailsIfProjectMissing()
        {
            Assert.Throws<ArgumentNullException>(
                () => UrnHelpers.GenerateProjectUrn("server", null));
        }

        [Test]
        public void ExtractServerUrnFailsIfServerMissing()
        {
            Assert.Throws<ArgumentNullException>(
                () => UrnHelpers.ExtractServerUrn(null));
        }

        [Test]
        public void ExtractServerUrnFailsIfNotCCNetUrn()
        {
            Assert.Throws<ArgumentException>(
                () => UrnHelpers.ExtractServerUrn("http://somewhere"));
        }

        [Test]
        public void ExtractServerUrnFailsIfUrnIncomplete()
        {
            Assert.Throws<ArgumentException>(
                () => UrnHelpers.ExtractServerUrn("urn:ccnet:"));
        }

        [Test]
        public void ExtractServerUrnReturnsUrnIfJustServer()
        {
            var urn = "urn:ccnet:local";
            var expected = "urn:ccnet:local";
            var actual = UrnHelpers.ExtractServerUrn(urn);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ExtractServerUrnReturnsJustServerUrn()
        {
            var urn = "urn:ccnet:local:project";
            var expected = "urn:ccnet:local";
            var actual = UrnHelpers.ExtractServerUrn(urn);
            Assert.AreEqual(expected, actual);
        }
        #endregion
    }
}
