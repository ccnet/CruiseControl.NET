using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Exortech.NetReflector;
using NUnit.Framework;
using Rhino.Mocks;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{

    [TestFixture]
    class DumpValueTaskTest
    {
        #region Private fields
        private string dumpFilePath;
        #endregion

        #region Setup
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            // Delete any outputs from the last build
            string buildFolder = Path.Combine(Path.GetTempPath(), "A Label");
            if (Directory.Exists(buildFolder)) Directory.Delete(buildFolder, true);
            dumpFilePath = Path.Combine(Path.GetTempPath(), "dumpfile.xml");
        }

        [SetUp]
        public void Setup()
        {
            if (File.Exists(dumpFilePath)) File.Delete(dumpFilePath);
        }
        #endregion

        #region CleanUp
        [TearDown]
        public void TearDown()
        {
            if (File.Exists(dumpFilePath)) File.Delete(dumpFilePath);
        }
        #endregion

        #region Test methods
        [Test]
        public void PopulateFromReflector()
        {
            var task = new DumpValueTask();
            const string xml = @"
    <DumpValue>
    	<xmlFileName>C:\some\path\to\file.xml</xmlFileName>
    	<values>
            <namedValue name='The Name' value='something' />
            <namedValue name='The Name 2' value='some other thing' />
            <namedValue name='The Name 3' value='stuff' />
            <namedValue name='The Name 4' value='last but not least' />
        </values>
    </DumpValue>";

            NetReflector.Read(xml, task);
            Assert.AreEqual(@"C:\some\path\to\file.xml", task.XmlFileName);
            Assert.AreEqual(4, task.Values.Length);
            Assert.AreEqual("The Name", task.Values[0].Name);
            Assert.AreEqual("something", task.Values[0].Value);
            Assert.AreEqual("The Name 2", task.Values[1].Name);
            Assert.AreEqual("some other thing", task.Values[1].Value);
            Assert.AreEqual("The Name 3", task.Values[2].Name);
            Assert.AreEqual("stuff", task.Values[2].Value);
            Assert.AreEqual("The Name 4", task.Values[3].Name);
            Assert.AreEqual("last but not least", task.Values[3].Value);
        }

        /// <summary>
        /// Run the task with the minimum options.
        /// </summary>
        [Test]
        public void MinimalRun()
        {

            BaseTest(new NameValuePair[] {new NameValuePair("TestName", "TestValue")});
        }

        [Test]
        public void MultiplePairsRun()
        {

            BaseTest(new NameValuePair[] { new NameValuePair("TestName", "TestValue"), 
                                           new NameValuePair("SecondName", "SecondValue")});
        }

        [Test]
        public void WithCarriageReturnRun()
        {

            BaseTest(new NameValuePair[] { new NameValuePair("TestName", "TestValue\r\nWith carriage returns") });
        }

        [Test]
        public void WithXMLCharsRun()
        {

            BaseTest(new NameValuePair[] { new NameValuePair("TestName", "TestValue With > nice & xml < \" ' characters") });
        }
        #endregion

        #region Private methods
        private void BaseTest(NameValuePair[] NameValues)
        {
            DumpValueTask task = new DumpValueTask();
            task.XmlFileName = dumpFilePath;
            task.Values = NameValues;
            task.Run(GetResult());
            Assert.IsTrue(File.Exists(dumpFilePath), "Dump file not generated");

            StreamReader reader = File.OpenText(dumpFilePath);
            string dumpContent = reader.ReadToEnd();
            reader.Close();

            Assert.AreEqual(GetExpectedXMLContent(task.Values), dumpContent);
        }

        private IntegrationResult GetResult()
        {
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, "Somewhere", null);
            IntegrationSummary summary = new IntegrationSummary(IntegrationStatus.Success, "A Label", "Another Label", new DateTime(2009, 1, 1));
            IntegrationResult result = new IntegrationResult("Test project", "Working directory", "Artifact directory", request, summary);
            Modification modification1 = GenerateModification("first file", "Add");
            Modification modification2 = GenerateModification("second file", "Modify");
            result.Modifications = new Modification[] { modification1, modification2 };
            result.Status = IntegrationStatus.Success;
            result.ArtifactDirectory = Path.GetTempPath();

            return result;
        }

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

        private string GetExpectedXMLContent(NameValuePair[] NameValues)
        {
            StringBuilder builder = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                                                      "<ValueDumper>\r\n");
            foreach (NameValuePair pair in NameValues)
            {
                builder.Append("  <ValueDumperItem>\r\n");
                builder.Append("    <Name>" + pair.Name + "</Name>\r\n");
                builder.Append("    <Value><![CDATA[" + pair.Value + "]]></Value>\r\n");
                builder.Append("  </ValueDumperItem>\r\n");
            }
            builder.Append("</ValueDumper>");

            return builder.ToString();
        }
        #endregion
    }
}
