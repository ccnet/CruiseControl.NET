using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Exortech.NetReflector;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    [TestFixture]
    class XslTransformationTaskTest
    {
        private MockRepository mocks = new MockRepository(MockBehavior.Default);

        [Test]
        public void ConfigurationIsLoadedCorrectly()
        {
            var reader = new NetReflectorConfigurationReader();
            var xml = new XmlDocument();
            xml.LoadXml(@"
<cruisecontrol>
    <project name=""WebTrunkTest"">
        <tasks>
            <xslt>
                <xmlfile>File 1</xmlfile>
                <xslfile>File 2</xslfile>
                <outputfile>File 3</outputfile>
                <xsltArgs>
                    <namedValue name=""ArgumentName"" value=""SomeValue"" />
                </xsltArgs>
            </xslt>
        </tasks>
    </project>
</cruisecontrol>
");
            var configuration = reader.Read(xml, null);
            Assert.IsNotNull(configuration, "Configuration not loaded");
            var project = configuration.Projects["WebTrunkTest"] as Project;
            Assert.IsNotNull(project, "Project not loaded");
            Assert.AreNotEqual(0, project.Tasks.Length, "Tasks not loaded");
            var task = project.Tasks[0] as XslTransformationTask;
            Assert.IsNotNull(task, "Task not correctly loaded");
            Assert.AreEqual("File 1", task.XMLFile, "XMLFile is incorrect");
            Assert.AreEqual("File 2", task.XSLFile, "XSLFile is incorrect");
            Assert.AreEqual("File 3", task.OutputFile, "OutputFile is incorrect");
            Assert.AreEqual(1, task.XsltArgs.Length, "Invalid number of xslt arguments");
            Assert.AreEqual("ArgumentName", task.XsltArgs[0].Name, "Argument name is incorrect");
            Assert.AreEqual("SomeValue", task.XsltArgs[0].Value, "Argument value is incorrect");
        }

        [Test]
        public void Run()
        {
            var working = Path.Combine(Path.GetTempPath(), "working");
            var artefact = Path.Combine(Path.GetTempPath(), "artefact");
            var label = "1.2.3.4";
            var source = Path.Combine(working, "source.xml");
            var stylesheet = Path.Combine(working, "stylesheet.xsl");
            var output = Path.Combine(working, "output.xml");

            // Mock the file system
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.OpenInputStream(source)).Returns(new MemoryStream()).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.OpenOutputStream(output)).Returns(new MemoryStream()).Verifiable();

            // Mock the transformer
            var transformer = mocks.Create<ITransformer>(MockBehavior.Strict).Object;
            Mock.Get(transformer).Setup(_transformer => _transformer.Transform(It.Is<string>(s => s == ""), It.Is<string>(s => s == stylesheet), It.Is<Hashtable>(t => t.Count == 0))).
                Returns("").Verifiable();

            // Initialise the task
            var task = new XslTransformationTask(transformer, fileSystem)
            {
                XMLFile = source,
                XSLFile = stylesheet,
                OutputFile = output
            };

            // Mock the result
            var result = mocks.Create<IIntegrationResult>(MockBehavior.Strict).Object;
            Mock.Get(result).SetupProperty(_result => _result.Status);
            Mock.Get(result).SetupSet(_result => _result.Status = It.IsAny<IntegrationStatus>()).Verifiable();

            var buildProgress = mocks.Create<BuildProgressInformation>(MockBehavior.Strict, artefact, "Project1").Object;
            Mock.Get(result).SetupGet(_result => _result.BuildProgressInformation)
                .Returns(buildProgress);
            Mock.Get(result).SetupGet(_result => _result.WorkingDirectory).Returns(working);
            Mock.Get(result).SetupGet(_result => _result.ArtifactDirectory).Returns(artefact);
            Mock.Get(result).SetupGet(_result => _result.Label).Returns(label);
            Mock.Get(buildProgress).Setup(_buildProgress => _buildProgress.SignalStartRunTask("Transforming")).Verifiable();

            // Run the test
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);

            // Check the results
            mocks.Verify();
        }

        [Test]
        public void RunWithXsltArguments()
        {
            var working = Path.Combine(Path.GetTempPath(), "working");
            var artefact = Path.Combine(Path.GetTempPath(), "artefact");
            var label = "1.2.3.4";
            var source = Path.Combine(working, "source.xml");
            var stylesheet = Path.Combine(working, "stylesheet.xsl");
            var output = Path.Combine(working, "output.xml");

            // Mock the file system
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.OpenInputStream(source)).Returns(new MemoryStream()).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.OpenOutputStream(output)).Returns(new MemoryStream()).Verifiable();

            // Mock the transformer
            var transformer = mocks.Create<ITransformer>(MockBehavior.Strict).Object;
            Mock.Get(transformer).Setup(_transformer => _transformer.Transform(It.Is<string>(s => s == ""), It.Is<string>(s => s == stylesheet), It.Is<Hashtable>(t => t.Count == 1 && (string)t["Test"] == "SomeValue"))).
                Returns("").Verifiable();

            // Initialise the task
            var task = new XslTransformationTask(transformer, fileSystem)
            {
                XMLFile = source,
                XSLFile = stylesheet,
                OutputFile = output,
                XsltArgs = new NameValuePair[1] { new NameValuePair() { Name = "Test", Value = "SomeValue" } }
            };

            // Mock the result
            var result = mocks.Create<IIntegrationResult>(MockBehavior.Strict).Object;
            Mock.Get(result).SetupProperty(_result => _result.Status);
            Mock.Get(result).SetupSet(_result => _result.Status = It.IsAny<IntegrationStatus>()).Verifiable();

            var buildProgress = mocks.Create<BuildProgressInformation>(MockBehavior.Strict, artefact, "Project1").Object;
            Mock.Get(result).SetupGet(_result => _result.BuildProgressInformation)
                .Returns(buildProgress);
            Mock.Get(result).SetupGet(_result => _result.WorkingDirectory).Returns(working);
            Mock.Get(result).SetupGet(_result => _result.ArtifactDirectory).Returns(artefact);
            Mock.Get(result).SetupGet(_result => _result.Label).Returns(label);
            Mock.Get(buildProgress).Setup(_buildProgress => _buildProgress.SignalStartRunTask("Transforming")).Verifiable();

            // Run the test
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);

            // Check the results
            mocks.Verify();
        }

    }
}
