using System;
using System.Collections;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Queues;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Queues
{
    [TestFixture]
    public class IntegrationQueueManagerFactoryTests
    {
        private MockRepository mocks = new MockRepository(MockBehavior.Default);

        [Test]
        public void CreateManagerGeneratesDefault()
        {
            IConfiguration configuration = mocks.Create<IConfiguration>().Object;
            IProjectList projectList = mocks.Create<IProjectList>().Object;
            IEnumerator enumerator = mocks.Create<IEnumerator>().Object;
            Mock.Get(enumerator).Setup(_enumerator => _enumerator.MoveNext()).Returns(false);
            Mock.Get(projectList).Setup(_projectList => _projectList.GetEnumerator()).Returns(enumerator);
            Mock.Get(configuration).Setup(_configuration => _configuration.Projects).Returns(projectList);
            IProjectIntegratorListFactory listFactory = mocks.Create<IProjectIntegratorListFactory>().Object;
            IProjectIntegratorList list = mocks.Create<IProjectIntegratorList>().Object;
            Mock.Get(list).SetupGet(_list => _list.Count).Returns(0);
            Mock.Get(listFactory).Setup(_listFactory => _listFactory.CreateProjectIntegrators(It.IsAny<IProjectList>(), It.IsAny<IntegrationQueueSet>())).Returns(list);

            object instance = IntegrationQueueManagerFactory.CreateManager(listFactory, configuration, null);
            Assert.That(instance, Is.InstanceOf<IntegrationQueueManager>());
        }

        [Test]
        public void OverrideChangesFactory()
        {
            IQueueManagerFactory newFactory = mocks.Create<IQueueManagerFactory>(MockBehavior.Strict).Object;
            IQueueManager newManager = mocks.Create<IQueueManager>(MockBehavior.Strict).Object;
            Mock.Get(newFactory).Setup(_newFactory => _newFactory.Create(null, null, null)).Returns(newManager).Verifiable();

            IntegrationQueueManagerFactory.OverrideFactory(newFactory);
            try
            {
                object instance = IntegrationQueueManagerFactory.CreateManager(null, null, null);
                Assert.AreSame(newManager, instance);
            }
            finally
            {
                // Clean up - otherwise the static instance will be corrected
                IntegrationQueueManagerFactory.ResetFactory();
            }
            mocks.VerifyAll();
        }
    }
}
