using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Queues;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Queues
{
    [TestFixture]
    public class IntegrationQueueManagerFactoryTests
    {
        private MockRepository mocks = new MockRepository();

        [Test]
        public void CreateManagerGeneratesDefault()
        {
            IConfiguration configuration = mocks.DynamicMock<IConfiguration>();
            IProjectList projectList = mocks.DynamicMock<IProjectList>();
            IEnumerator enumerator = mocks.DynamicMock<IEnumerator>();
            SetupResult.For(enumerator.MoveNext()).Return(false);
            SetupResult.For(projectList.GetEnumerator()).Return(enumerator);
            SetupResult.For(configuration.Projects).Return(projectList);
            IProjectIntegratorListFactory listFactory = mocks.DynamicMock<IProjectIntegratorListFactory>();
            IProjectIntegratorList list = mocks.DynamicMock<IProjectIntegratorList>();
            SetupResult.For(list.Count).Return(0);
            SetupResult.For(listFactory.CreateProjectIntegrators(null, null)).IgnoreArguments().Return(list);
            mocks.ReplayAll();

            object instance = IntegrationQueueManagerFactory.CreateManager(listFactory, configuration, null);
            Assert.That(instance, Is.InstanceOf<IntegrationQueueManager>());
        }

        [Test]
        public void OverrideChangesFactory()
        {
            IQueueManagerFactory newFactory = mocks.CreateMock<IQueueManagerFactory>();
            IQueueManager newManager = mocks.CreateMock<IQueueManager>();
            Expect.Call(newFactory.Create(null, null, null)).Return(newManager);
            mocks.ReplayAll();

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
