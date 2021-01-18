using System;
using System.IO;
using System.Xml;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
    [TestFixture]
    public class ModificationHistoryPublisherTest
    {

        public static readonly string FULL_CONFIGURED_LOG_DIR = "FullConfiguredLogDir";
        public static readonly string FULL_CONFIGURED_LOG_DIR_PATH = Path.GetFullPath(TempFileUtil.GetTempPath(FULL_CONFIGURED_LOG_DIR));

        public static readonly string ARTIFACTS_DIR = "Artifacts";
        public static readonly string ARTIFACTS_DIR_PATH = Path.GetFullPath(TempFileUtil.GetTempPath(ARTIFACTS_DIR));

        private ModificationHistoryPublisher publisher;

        [SetUp]
        public void SetUp()
        {
            TempFileUtil.DeleteTempDir(FULL_CONFIGURED_LOG_DIR);
            TempFileUtil.DeleteTempDir(ARTIFACTS_DIR);
            TempFileUtil.CreateTempDir(ARTIFACTS_DIR);

            publisher = new ModificationHistoryPublisher();
        }

        [TearDown]
        public void TearDown()
        {
            TempFileUtil.DeleteTempDir(FULL_CONFIGURED_LOG_DIR);
            TempFileUtil.DeleteTempDir(ARTIFACTS_DIR);
        }

        [Test]
        public void BuildWithoutModificationsShouldPublishNoModifications()
        {
            // Setup
            IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, false);
            result.ArtifactDirectory = ARTIFACTS_DIR_PATH;
            string PublishedModifications;
            string ExpectedLoggedModifications = string.Format(System.Globalization.CultureInfo.CurrentCulture,"<History><Build BuildDate=\"{0}\" Success=\"True\" Label=\"{1}\" />" + Environment.NewLine + "</History>",
                                                    DateUtil.FormatDate(result.StartTime), result.Label);

            // Execute
            publisher.Run(result);            

            //Verify
            PublishedModifications = ModificationHistoryPublisher.LoadHistory(ARTIFACTS_DIR_PATH);

            Assert.AreEqual(ExpectedLoggedModifications, PublishedModifications, "Differences in log Detected");
        }

        [Test]
        public void BuildWithoutModificationsShouldNotLogWhenOnlyLogWhenChangesFound()
        {
            // Setup
            publisher.OnlyLogWhenChangesFound = true;

            IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, false);
            result.ArtifactDirectory = ARTIFACTS_DIR_PATH;
            string PublishedModifications;
            string ExpectedLoggedModifications = string.Empty;

            // Execute
            publisher.Run(result);

            //Verify
            PublishedModifications = ModificationHistoryPublisher.LoadHistory(ARTIFACTS_DIR_PATH);

            Assert.AreEqual(ExpectedLoggedModifications, PublishedModifications, "Differences in log Detected");
        }

        [Test]
        public void BuildWithModificationsShouldPublishModifications()
        {
            // Setup
            IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success,true);
            result.ArtifactDirectory = ARTIFACTS_DIR_PATH;
            string PublishedModifications;
            
            System.Text.StringBuilder ExpectedLoggedModifications = new System.Text.StringBuilder();
            ExpectedLoggedModifications.Append("<History>");
            ExpectedLoggedModifications.Append( GetExpectedMods(result));
            ExpectedLoggedModifications.AppendLine();
            ExpectedLoggedModifications.Append("</History>");

            // Execute
            publisher.Run(result);

            //Verify
            PublishedModifications = ModificationHistoryPublisher.LoadHistory(ARTIFACTS_DIR_PATH);

            Assert.AreEqual(ExpectedLoggedModifications.ToString(), PublishedModifications, "Differences in log Detected");
        }

        [Test]
        public void BuildWithModificationsShouldPublishModificationsWhenOnlyLogWhenChangesFound()
        {
            // Setup
            publisher.OnlyLogWhenChangesFound = true;

            IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, true);
            result.ArtifactDirectory = ARTIFACTS_DIR_PATH;
            string PublishedModifications;

            System.Text.StringBuilder ExpectedLoggedModifications = new System.Text.StringBuilder();
            ExpectedLoggedModifications.Append("<History>");
            ExpectedLoggedModifications.Append(GetExpectedMods(result));
            ExpectedLoggedModifications.AppendLine();
            ExpectedLoggedModifications.Append("</History>");

            // Execute
            publisher.Run(result);

            //Verify
            PublishedModifications = ModificationHistoryPublisher.LoadHistory(ARTIFACTS_DIR_PATH);

            Assert.AreEqual(ExpectedLoggedModifications.ToString(), PublishedModifications, "Differences in log Detected");
        }

        private string GetExpectedMods(IntegrationResult result)
        {
            System.IO.StringWriter sw = new System.IO.StringWriter();
            System.Xml.XmlTextWriter cbiw = new System.Xml.XmlTextWriter(sw);
            cbiw.Formatting = System.Xml.Formatting.Indented;

            cbiw.WriteStartElement("Build");
            cbiw.WriteStartAttribute("BuildDate");
            cbiw.WriteValue(DateUtil.FormatDate(result.EndTime));

            cbiw.WriteStartAttribute("Success");
            cbiw.WriteValue(result.Succeeded.ToString());

            cbiw.WriteStartAttribute("Label");
            cbiw.WriteValue(result.Label);

            if (result.Modifications.Length > 0)
            {
                cbiw.WriteStartElement("modifications");

                for (int i = 0; i < result.Modifications.Length; i++)
                {
                    result.Modifications[i].ToXml(cbiw);
                }

                cbiw.WriteEndElement();
            }

            cbiw.WriteEndElement();

            return sw.ToString();
        }

        private IntegrationResult CreateIntegrationResult(IntegrationStatus status, bool addModifications)
        {
            IntegrationResult result = IntegrationResultMother.Create(status, new DateTime(1980, 1, 1));
            result.ProjectName = "proj";
            result.StartTime = new DateTime(1980, 1, 1);
            result.Label = "1";
            result.Status = status;
            if (addModifications)
            {
                Modification[] modifications = new Modification[1];
                modifications[0] = new Modification();
                modifications[0].ModifiedTime = new DateTime(2002, 2, 3);
                modifications[0].ChangeNumber = "2";
                result.Modifications = modifications;
            }
            return result;
        }

    }
}
