using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Publishers;
using System.Xml;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
    [TestFixture]
    public class ManifestGeneratorTests
    {
        #region Test methods
        #region GenerateFullManifest()
        /// <summary>
        /// Test generating a full manifest, including both files and modifications.
        /// </summary>
        [Test]
        public void GenerateFullManifest()
        {
            ManifestGenerator generator = new ManifestGenerator();
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, "Somewhere");
            IntegrationSummary summary = new IntegrationSummary(IntegrationStatus.Success, "A Label", "Another Label", new DateTime(2009, 1, 1));
            IntegrationResult result = new IntegrationResult("Test project", "Working directory", "Artifact directory", request, summary);
            Modification modification1 = GenerateModification("first file", "Add");
            Modification modification2 = GenerateModification("second file", "Modify");
            result.Modifications = new Modification[] { modification1, modification2 };
            List<string> files = new List<string>();
            files.Add("first file");
            XmlDocument manifest = generator.Generate(result, files.ToArray());
            Assert.IsNotNull(manifest);
            string actualManifest = manifest.OuterXml;
            string expectedManifest = "<manifest>"  +
                    "<header project=\"Test project\" label=\"A Label\" build=\"ForceBuild\" status=\"Unknown\">" +
                        "<modification user=\"johnDoe\" changeNumber=\"1\" time=\"2009-01-01T00:00:00\">" +
                            "<comment>A comment</comment>" +
                            "<file name=\"first file\" type=\"Add\" />" +
                            "<file name=\"second file\" type=\"Modify\" />" +
                        "</modification>" +
                    "</header>" +
                    "<file name=\"first file\" />" +
                "</manifest>";
            Assert.AreEqual(expectedManifest, actualManifest);
        }
        #endregion
        #endregion

        #region Private methods
        #region GenerateModification()
        private Modification GenerateModification(string name, string type)
        {
            Modification modification = new Modification();
            modification.ChangeNumber = 1;
            modification.Comment = "A comment";
            modification.EmailAddress = "email@somewhere.com";
            modification.FileName = name;
            modification.ModifiedTime = new DateTime(2009, 1, 1);
            modification.Type = type;
            modification.UserName = "johnDoe";
            modification.Version = "1.1.1.1";
            return modification;
        }
        #endregion
        #endregion
    }
}
