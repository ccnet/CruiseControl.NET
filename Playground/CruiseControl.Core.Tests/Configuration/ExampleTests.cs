namespace CruiseControl.Core.Tests.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xaml;
    using CruiseControl.Core;
    using CruiseControl.Core.Channels;
    using CruiseControl.Core.Structure;
    using CruiseControl.Core.Tasks;
    using CruiseControl.Core.Xaml;
    using NUnit.Framework;

    [TestFixture]
    public class ExampleTests
    {
        [Test]
        public void ReadSingleProject()
        {
            var configuration = LoadConfiguration(
                RetrieveExampleFile("SingleProject"));

            Assert.IsNotNull(configuration);
            Assert.AreEqual("2.0", configuration.Version.ToString(2));
            this.VerifyChildren(
                configuration.Children,
                new Project("TestProject"));
        }

        [Test]
        public void WriteSingleProject()
        {
            var configuration = new Project(
                "TestProject",
                new Comment("TestComment", "A Test Comment"));
            PerformSerialisationTest(configuration, "SingleProject");
        }

        [Test]
        public void ReadSimpleSourceControlExample()
        {
            var configuration = LoadConfiguration(
                RetrieveExampleFile("SimpleSourceControlExample"));

            Assert.IsNotNull(configuration);
            Assert.AreEqual("2.0", configuration.Version.ToString(2));
            this.VerifyChildren(
                configuration.Children,
                new Project("CCNetvNext"));
        }

        [Test]
        public void ReadScmProject()
        {
            var configuration = LoadConfiguration(
                            RetrieveExampleFile("ScmProject"));

            Assert.IsNotNull(configuration);
            Assert.AreEqual("2.0", configuration.Version.ToString(2));
            this.VerifyChildren(
                configuration.Children,
                new ScmProject("OldStyle"));
        }

        [Test]
        public void ReadComplexSourceControlExample()
        {
            var configuration = LoadConfiguration(
                            RetrieveExampleFile("ComplexSourceControlExample"));

            Assert.IsNotNull(configuration);
            Assert.AreEqual("2.0", configuration.Version.ToString(2));
            this.VerifyChildren(
                configuration.Children,
                new Project("CCNetvNext"));
        }

        [Test]
        public void ReadCommonTaskProperties()
        {
            var configuration = LoadConfiguration(
                            RetrieveExampleFile("CommonTaskProperties"));

            Assert.IsNotNull(configuration);
            Assert.AreEqual("2.0", configuration.Version.ToString(2));
            this.VerifyChildren(
                configuration.Children,
                new Project("SampleProject"));
        }

        [Test]
        public void ReadSimpleQueue()
        {
            var configuration = LoadConfiguration(
                            RetrieveExampleFile("SimpleQueue"));

            Assert.IsNotNull(configuration);
            Assert.AreEqual("2.0", configuration.Version.ToString(2));
            this.VerifyChildren(
                configuration.Children,
                new Queue("Queue1", new Project("Project1"), new Project("Project2")));
            var queue = configuration.Children[0] as Queue;
            Assert.AreEqual(3, Queue.GetPriority(queue.Children[0]));
        }

        [Test]
        public void WriteSimpleQueue()
        {
            var project = new Project("Project1");
            Queue.SetPriority(project, 3);
            var configuration = new Queue(
                "Queue1",
                project,
                new Project("Project2"));
            PerformSerialisationTest(configuration, "SimpleQueue");
        }

        [Test]
        public void ReadSimpleGate()
        {
            var configuration = LoadConfiguration(
                            RetrieveExampleFile("SimpleGate"));

            Assert.IsNotNull(configuration);
            Assert.AreEqual("2.0", configuration.Version.ToString(2));
            this.VerifyChildren(
                configuration.Children,
                new Gate("Gate1", new Project("Project1"), new Project("Project2")));
        }

        [Test]
        public void WriteSimpleGate()
        {
            var configuration = new Gate(
                "Gate1",
                new Project("Project1"),
                new Project("Project2"));
            PerformSerialisationTest(configuration, "SimpleGate");
        }

        [Test]
        public void ReadSimplePipeline()
        {
            var configuration = LoadConfiguration(
                            RetrieveExampleFile("SimplePipeline"));

            Assert.IsNotNull(configuration);
            Assert.AreEqual("2.0", configuration.Version.ToString(2));
            this.VerifyChildren(
                configuration.Children,
                new Pipeline("Pipeline1", new Project("Project1"), new Project("Project2")));
        }

        [Test]
        public void WriteSimplePipeline()
        {
            var configuration = new Pipeline(
                "Pipeline1",
                new Project("Project1"),
                new Project("Project2"));
            PerformSerialisationTest(configuration, "SimplePipeline");
        }

        [Test]
        public void ReadPipelineWithGates()
        {
            var configuration = LoadConfiguration(
                            RetrieveExampleFile("PipelineWithGates"));

            Assert.IsNotNull(configuration);
            Assert.AreEqual("2.0", configuration.Version.ToString(2));
            var gate = new Gate("Gate1", new Project("Project2"), new Project("Project3"));
            this.VerifyChildren(
                configuration.Children,
                new Pipeline("Pipeline1", new Project("Project1"), gate, new Project("Project4")));
        }

        [Test]
        public void WritePipelineWithGates()
        {
            var gate = new Gate("Gate1", new Project("Project2"), new Project("Project3"));
            var configuration = new Pipeline(
                "Pipeline1",
                new Project("Project1"),
                gate,
                new Project("Project4"));
            PerformSerialisationTest(configuration, "PipelineWithGates");
        }

        [Test]
        public void ReadQueueOfQueues()
        {
            var configuration = LoadConfiguration(
                            RetrieveExampleFile("QueueOfQueues"));

            Assert.IsNotNull(configuration);
            Assert.AreEqual("2.0", configuration.Version.ToString(2));
            var queue1 = new Queue("Queue1", new Project("Project1"), new Project("Project2"));
            var queue2 = new Queue("Queue2", new Project("Project3"), new Project("Project4"));
            this.VerifyChildren(
                configuration.Children,
                new Queue("Queue3", queue1, queue2));
        }

        [Test]
        public void WriteQueueOfQueues()
        {
            var queue1 = new Queue("Queue1", new Project("Project1"), new Project("Project2"));
            var queue2 = new Queue("Queue2", new Project("Project3"), new Project("Project4"));
            var configuration = new Queue("Queue3", queue1, queue2)
                                    {
                                        AllowedActive = 2
                                    };
            PerformSerialisationTest(configuration, "QueueOfQueues");
        }

        [Test]
        public void ReadRoundRobinOfQueues()
        {
            var configuration = LoadConfiguration(
                            RetrieveExampleFile("RoundRobinOfQueues"));

            Assert.IsNotNull(configuration);
            Assert.AreEqual("2.0", configuration.Version.ToString(2));
            var queue1 = new Queue("Queue1", new Project("Project1"), new Project("Project2"));
            var queue2 = new Queue("Queue2", new Project("Project3"), new Project("Project4"));
            this.VerifyChildren(
                configuration.Children,
                new RoundRobin("RoundRobin", queue1, queue2));
        }

        [Test]
        public void WriteRoundRobinOfQueues()
        {
            var queue1 = new Queue("Queue1", new Project("Project1"), new Project("Project2"));
            var queue2 = new Queue("Queue2", new Project("Project3"), new Project("Project4"));
            var configuration = new RoundRobin("RoundRobin", queue1, queue2);
            PerformSerialisationTest(configuration, "RoundRobinOfQueues");
        }

        [Test]
        public void ReadWcfChannel()
        {
            var configuration = LoadConfiguration(
                RetrieveExampleFile("WcfChannel"));
            Assert.IsNotNull(configuration);
            Assert.AreEqual(1, configuration.ClientChannels.Count);
            Assert.IsInstanceOf<Wcf>(configuration.ClientChannels[0]);
            var channel = configuration.ClientChannels[0] as Wcf;
            Assert.AreEqual("TestChannel", channel.Name);
        }

        [Test]
        public void WriteWcfChannel()
        {
            var channel = new Wcf("TestChannel");
            PerformSerialisationTest(channel, "WcfChannel");
        }

        private static Stream RetrieveExampleFile(string exampleName)
        {
            var assembly = typeof(ExampleTests).Assembly;
            var streamName = "CruiseControl.Core.Tests.Configuration.Examples." +
                exampleName +
                ".xaml";
            var stream = assembly.GetManifestResourceStream(streamName);
            return stream;
        }

        private void VerifyChildren(IList<ServerItem> actual, params ServerItem[] expected)
        {
            Assert.AreEqual(expected.Length, actual.Count);
            for (var loop = 0; loop < actual.Count && loop < expected.Length; loop++)
            {
                Assert.AreEqual(expected[loop].GetType().FullName, actual[loop].GetType().FullName, "Type do not match for item #" + loop);
                Assert.AreEqual(expected[loop].Name, actual[loop].Name, "Names do not match for item #" + loop);
                var container = expected[loop] as IServerItemContainer;
                if (container != null)
                {
                    this.VerifyChildren(
                        (actual[loop] as IServerItemContainer).Children,
                        container.Children.ToArray());
                }
            }
        }

        private static void PerformSerialisationTest(ServerItem configuration, string example)
        {
            var server = new Server
                             {
                                 Version = new Version(2, 0)
                             };
            server.Children.Add(configuration);
            var xaml = XamlServices.Save(server);
            using (var stream = RetrieveExampleFile(example))
            {
                using (var reader = new StreamReader(stream))
                {
                    var expected = reader.ReadToEnd();
                    Assert.AreEqual(expected, xaml);
                }
            }
        }

        private static void PerformSerialisationTest(ClientChannel configuration, string example)
        {
            var server = new Server
                             {
                                 Version = new Version(2, 0)
                             };
            server.ClientChannels.Add(configuration);
            var xaml = XamlServices.Save(server);
            using (var stream = RetrieveExampleFile(example))
            {
                using (var reader = new StreamReader(stream))
                {
                    var expected = reader.ReadToEnd();
                    Assert.AreEqual(expected, xaml);
                }
            }
        }

        private static Server LoadConfiguration(Stream stream)
        {
            var service = new ConfigurationService();
            var config = service.Load(stream);
            return config;
        }
    }
}
