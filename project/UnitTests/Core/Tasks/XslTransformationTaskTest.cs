using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Text;
using Exortech.NetReflector;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    [TestFixture]
    class XslTransformationTaskTest
    {
        private MockRepository mocks = new MockRepository();

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
            var fileSystem = mocks.StrictMock<IFileSystem>();
            Expect.Call(fileSystem.OpenInputStream(source)).Return(new MemoryStream());
            Expect.Call(fileSystem.OpenOutputStream(output)).Return(new MemoryStream());

            // Mock the transformer
            var transformer = mocks.StrictMock<ITransformer>();
            Expect.Call(transformer.Transform(null, null, null)).
                IgnoreArguments().Constraints(
                Rhino.Mocks.Constraints.Is.Equal(""), 
                Rhino.Mocks.Constraints.Is.Equal(stylesheet),
                Rhino.Mocks.Constraints.Is.TypeOf(typeof(Hashtable)) &&
                    Rhino.Mocks.Constraints.List.Count(Rhino.Mocks.Constraints.Is.Equal(0))).
                Return("");

            // Initialise the task
            var task = new XslTransformationTask(transformer, fileSystem)
            {
                XMLFile = source,
                XSLFile = stylesheet,
                OutputFile = output
            };

            // Mock the result
            var result = mocks.StrictMock<IIntegrationResult>();
            Expect.Call(result.Status).PropertyBehavior();

            var buildProgress = mocks.StrictMock<BuildProgressInformation>(artefact, "Project1");
            SetupResult.For(result.BuildProgressInformation)
                .Return(buildProgress);
            SetupResult.For(result.WorkingDirectory).Return(working);
            SetupResult.For(result.ArtifactDirectory).Return(artefact);
            SetupResult.For(result.Label).Return(label);
            Expect.Call(() => { buildProgress.SignalStartRunTask("Transforming"); });

            // Run the test
            mocks.ReplayAll();
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);

            // Check the results
            mocks.VerifyAll();
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
            var fileSystem = mocks.StrictMock<IFileSystem>();
            Expect.Call(fileSystem.OpenInputStream(source)).Return(new MemoryStream());
            Expect.Call(fileSystem.OpenOutputStream(output)).Return(new MemoryStream());

            // Mock the transformer
            var transformer = mocks.StrictMock<ITransformer>();
            Expect.Call(transformer.Transform(null, null, null)).
                IgnoreArguments().Constraints(
                Rhino.Mocks.Constraints.Is.Equal(""),
                Rhino.Mocks.Constraints.Is.Equal(stylesheet),
                Rhino.Mocks.Constraints.Is.TypeOf(typeof(Hashtable)) && 
                    Rhino.Mocks.Constraints.List.Count(Rhino.Mocks.Constraints.Is.Equal(1)) &&
                    Rhino.Mocks.Constraints.List.Element<string>("Test", Rhino.Mocks.Constraints.Is.Equal("SomeValue"))
                ).
                Return("");

            // Initialise the task
            var task = new XslTransformationTask(transformer, fileSystem)
            {
                XMLFile = source,
                XSLFile = stylesheet,
                OutputFile = output,
                XsltArgs = new NameValuePair[1] { new NameValuePair() { Name = "Test", Value = "SomeValue" } }
            };

            // Mock the result
            var result = mocks.StrictMock<IIntegrationResult>();
            Expect.Call(result.Status).PropertyBehavior();

            var buildProgress = mocks.StrictMock<BuildProgressInformation>(artefact, "Project1");
            SetupResult.For(result.BuildProgressInformation)
                .Return(buildProgress);
            SetupResult.For(result.WorkingDirectory).Return(working);
            SetupResult.For(result.ArtifactDirectory).Return(artefact);
            SetupResult.For(result.Label).Return(label);
            Expect.Call(() => { buildProgress.SignalStartRunTask("Transforming"); });

            // Run the test
            mocks.ReplayAll();
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);

            // Check the results
            mocks.VerifyAll();
        }

    }
}
