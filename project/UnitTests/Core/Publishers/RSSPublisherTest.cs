using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Remote;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	[TestFixture]
	public class RSSPublisherTest
    {
        #region Private fields
        private string dataFile = Path.Combine(Path.GetTempPath(), "RSSData.xml");
        #endregion

        #region Test methods
        #region GenerateNewFeed()
        /// <summary>
        /// Generate a brand new feed.
        /// </summary>
        [Test]
        public void GenerateNewFeed()
        {
            if (File.Exists(dataFile)) File.Delete(dataFile);

            RssPublisher publisher = new RssPublisher();
            IntegrationResult result = GenerateResult(0);

            publisher.Run(result);
            Assert.IsTrue(File.Exists(dataFile));
            CheckAgainstExpected("NewFeed");
        }
        #endregion

        #region AppendToFeedWithinLimits()
        /// <summary>
        /// Test generating a full manifest, including both files and modifications.
        /// </summary>
        [Test]
        public void AppendToFeedWithinLimits()
        {
            if (File.Exists(dataFile)) File.Delete(dataFile);

            RssPublisher publisher = new RssPublisher();
            IntegrationResult result = GenerateResult(1);

            publisher.Run(result);
            publisher.Run(result);
            Assert.IsTrue(File.Exists(dataFile));
            CheckAgainstExpected("WithinLimits");
        }
        #endregion

        #region AppendToFeedBeyondLimits()
        /// <summary>
        /// Test generating a full manifest, including both files and modifications.
        /// </summary>
        [Test]
        public void AppendToFeedBeyondLimits()
        {
            if (File.Exists(dataFile)) File.Delete(dataFile);

            RssPublisher publisher = new RssPublisher();
            publisher.NumberOfItems = 1;
            IntegrationResult result = GenerateResult(2);
            result.ProjectUrl = "http://viewproject";

            publisher.Run(result);
            publisher.Run(result);
            Assert.IsTrue(File.Exists(dataFile));
            CheckAgainstExpected("BeyondLimits");
        }
        #endregion
        #endregion

        #region Private methods
        #region GenerateModification()
        private Modification GenerateModification(string name, string type)
        {
            Modification modification = new Modification();
            modification.ChangeNumber = "1";
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

        #region GenerateResult()
        private IntegrationResult GenerateResult(int numberOfModifications)
        {
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, "Somewhere");
            IntegrationSummary summary = new IntegrationSummary(IntegrationStatus.Success, "A Label", "Another Label", new DateTime(2009, 1, 1));
            IntegrationResult result = new IntegrationResult("Test project", "Working directory", "Artifact directory", request, summary);
            List<Modification> modifications = new List<Modification>();
            for (int loop = 0; loop < numberOfModifications; loop++)
            {
                modifications.Add(GenerateModification(string.Format("modification #{0}", loop + 1), "Add"));
            }
            result.Modifications = modifications.ToArray();
            result.ArtifactDirectory = Path.GetTempPath();
            return result;
        }
        #endregion

        #region CheckAgainstExpected()
        private void CheckAgainstExpected(string resourceName)
        {
            Stream resourceStream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(
                    string.Format("ThoughtWorks.CruiseControl.UnitTests.Core.Publishers.Rss.{0}.xml",
                        resourceName));
            string expected;
            using (StreamReader reader = new StreamReader(resourceStream))
            {
                expected = reader.ReadToEnd();
            }

            string actual = File.ReadAllText(dataFile);
            Assert.AreEqual(RemoveDynamicValues(expected), RemoveDynamicValues(actual));
        }
        #endregion

        #region RemoveDynamicValues()
        private string RemoveDynamicValues(string value)
        {
            Regex dynamicRegex = new Regex("<guid>[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}</guid>|" +
                "<pubDate>[A-Za-z]{3}, [0-9]{2} [A-Za-z]{3} [0-9]{4} [0-9]{2}:[0-9]{2}:[0-9]{2} GMT</pubDate>");
            string replaced = dynamicRegex.Replace(value, string.Empty);
            return replaced;
        }
        #endregion
        #endregion
    }
}
