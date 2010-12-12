namespace CruiseControl.Core.Tests.Configuration
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xaml;
    using CruiseControl.Core;
    using CruiseControl.Core.Structure;
    using NUnit.Framework;

    [TestFixture]
    public class ExampleTests
    {
        [Test]
        public void SingleProject()
        {
            Server configuration;
            using (var stream = this.RetrieveExampleFile("SingleProject"))
            {
                configuration = XamlServices.Load(stream) as Server;
            }

            Assert.IsNotNull(configuration);
            Assert.AreEqual("2.0", configuration.Version.ToString(2));
            this.VerifyChildren(
                configuration.Children, 
                new Project("TestProject"));
        }

        [Test]
        public void SimpleQueue()
        {
            Server configuration;
            using (var stream = this.RetrieveExampleFile("SimpleQueue"))
            {
                configuration = XamlServices.Load(stream) as Server;
            }

            Assert.IsNotNull(configuration);
            Assert.AreEqual("2.0", configuration.Version.ToString(2));
            this.VerifyChildren(
                configuration.Children, 
                new Queue("Queue1", new Project("Project1"), new Project("Project2")));
        }

        [Test]
        public void SimpleGate()
        {
            Server configuration;
            using (var stream = this.RetrieveExampleFile("SimpleGate"))
            {
                configuration = XamlServices.Load(stream) as Server;
            }

            Assert.IsNotNull(configuration);
            Assert.AreEqual("2.0", configuration.Version.ToString(2));
            this.VerifyChildren(
                configuration.Children,
                new Gate("Gate1", new Project("Project1"), new Project("Project2")));
        }

        [Test]
        public void SimplePipeline()
        {
            Server configuration;
            using (var stream = this.RetrieveExampleFile("SimplePipeline"))
            {
                configuration = XamlServices.Load(stream) as Server;
            }

            Assert.IsNotNull(configuration);
            Assert.AreEqual("2.0", configuration.Version.ToString(2));
            this.VerifyChildren(
                configuration.Children,
                new Pipeline("Pipeline1", new Project("Project1"), new Project("Project2")));
        }

        [Test]
        public void PipelineWithGates()
        {
            Server configuration;
            using (var stream = this.RetrieveExampleFile("PipelineWithGates"))
            {
                configuration = XamlServices.Load(stream) as Server;
            }

            Assert.IsNotNull(configuration);
            Assert.AreEqual("2.0", configuration.Version.ToString(2));
            var gate = new Gate("Gate1", new Project("Project2"), new Project("Project3"));
            this.VerifyChildren(
                configuration.Children,
                new Pipeline("Pipeline1", new Project("Project1"), gate, new Project("Project4")));
        }

        [Test]
        public void QueueOfQueues()
        {
            Server configuration;
            using (var stream = this.RetrieveExampleFile("QueueOfQueues"))
            {
                configuration = XamlServices.Load(stream) as Server;
            }

            Assert.IsNotNull(configuration);
            Assert.AreEqual("2.0", configuration.Version.ToString(2));
            var queue1 = new Queue("Queue1", new Project("Project1"), new Project("Project2"));
            var queue2 = new Queue("Queue2", new Project("Project3"), new Project("Project4"));
            this.VerifyChildren(
                configuration.Children,
                new Queue("Queue3", queue1, queue2));
        }

        [Test]
        public void RoundRobinOfQueues()
        {
            Server configuration;
            using (var stream = this.RetrieveExampleFile("RoundRobinOfQueues"))
            {
                configuration = XamlServices.Load(stream) as Server;
            }

            Assert.IsNotNull(configuration);
            Assert.AreEqual("2.0", configuration.Version.ToString(2));
            var queue1 = new Queue("Queue1", new Project("Project1"), new Project("Project2"));
            var queue2 = new Queue("Queue2", new Project("Project3"), new Project("Project4"));
            this.VerifyChildren(
                configuration.Children,
                new RoundRobin("RoundRobin", queue1, queue2));
        }

        private Stream RetrieveExampleFile(string exampleName)
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
    }
}
