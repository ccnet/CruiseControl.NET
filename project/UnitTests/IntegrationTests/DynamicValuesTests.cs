namespace ThoughtWorks.CruiseControl.UnitTests.IntegrationTests
{
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote;

    public class DynamicValuesTests
    {
        #region Tests
        [Test]
        public void DynamicValuesHandlePrivateStringsWithPassword()
        {
            var document = EmbeddedData.LoadEmbeddedXml("DynamicValues.PrivateString.xml");
            var projectName = "Test Project";
            using (var harness = new CruiseServerHarness(document, projectName))
            {
                var status = IntegrationStatus.Unknown;
                harness.IntegrationCompleted += (o, e) =>
                {
                    status = e.Status;
                };
                harness.TriggerBuildAndWait(
                    projectName,
                    new NameValuePair("Password", "John's Password"));
                Assert.AreEqual(IntegrationStatus.Success, status);
            }
        }

        [Test]
        public void DynamicValuesHandlePrivateStringsWithoutPassword()
        {
            var document = EmbeddedData.LoadEmbeddedXml("DynamicValues.PrivateString.xml");
            var projectName = "Test Project";
            using (var harness = new CruiseServerHarness(document, projectName))
            {
                var status = IntegrationStatus.Unknown;
                harness.IntegrationCompleted += (o, e) =>
                {
                    status = e.Status;
                };
                harness.TriggerBuildAndWait(
                    projectName);
                Assert.AreEqual(IntegrationStatus.Success, status);
            }
        }
        #endregion
    }
}
