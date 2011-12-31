using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Config
{
	[TestFixture]
	public class ConfigurationTest
	{
		[Test]
		public void CreateIntegrators()
		{
			Project project1 = new Project();
			project1.Name = "project1";
			Project project2 = new Project();
			project2.Name = "project2";

			Configuration config = new Configuration();
			config.AddProject(project1);
			config.AddProject(project2);
		}

        [Test]
        public void FindQueueFound()
        {
            DefaultQueueConfiguration queueConfig = new DefaultQueueConfiguration("Test Queue");
            Configuration config = new Configuration();
            config.QueueConfigurations.Add(queueConfig);
            IQueueConfiguration foundConfig = config.FindQueueConfiguration("Test Queue");
            Assert.IsNotNull(foundConfig);
            Assert.AreSame(queueConfig, foundConfig);
        }

        [Test]
        public void FindQueueNotFound()
        {
            Configuration config = new Configuration();
            IQueueConfiguration foundConfig = config.FindQueueConfiguration("Test Queue");
            Assert.IsNotNull(foundConfig);
            Assert.That(foundConfig, Is.InstanceOf<DefaultQueueConfiguration>());
            Assert.AreEqual("Test Queue", foundConfig.Name);
            Assert.AreEqual(QueueDuplicateHandlingMode.UseFirst, foundConfig.HandlingMode);
        }
    }
}
