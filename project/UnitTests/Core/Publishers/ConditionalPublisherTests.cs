using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
    [TestFixture]
    public class ConditionalPublisherTests
    {
        #region Private fields
        private MockRepository mocks = new MockRepository(MockBehavior.Default);
        #endregion

        #region Tests
        [Test]
        public void RunExecutesPublishersWhenConditionIsMet()
        {
            var result = GenerateResultMock();
            var logger = mocks.Create<ILogger>().Object;
            var childPublisher = mocks.Create<ITask>(MockBehavior.Strict).Object;
            Mock.Get(result).Setup(_result => _result.Clone()).Returns((IIntegrationResult)null).Verifiable();
            Mock.Get(result).Setup(_result => _result.Merge(It.IsAny<IIntegrationResult>())).Verifiable();
            Mock.Get(childPublisher).Setup(_childPublisher => _childPublisher.Run(null)).Verifiable();
            var publisher = new ConditionalPublisher
            {
                Logger = logger,
                Tasks = new ITask[] {
                    childPublisher
                },
                Conditions = new IntegrationStatus[] {
                    IntegrationStatus.Success
                }
            };

            result.Status = IntegrationStatus.Success;
            publisher.Run(result);

            mocks.Verify();
        }

        [Test]
        public void RunDoesNotExecutePublishersWhenConditionIsNotMet()
        {
            var result = GenerateResultMock();
            var logger = mocks.Create<ILogger>().Object;
            var childPublisher = mocks.Create<ITask>(MockBehavior.Strict).Object;
            var publisher = new ConditionalPublisher
            {
                Logger = logger,
                Tasks = new ITask[] {
                    childPublisher
                },
                Conditions = new IntegrationStatus[] {
                    IntegrationStatus.Success
                }
            };

            result.Status = IntegrationStatus.Failure;
            publisher.Run(result);

            mocks.Verify();
        }

        private IIntegrationResult GenerateResultMock()
        {
            var buildInfo = mocks.Create<BuildProgressInformation>(string.Empty, string.Empty).Object;
            var result = mocks.Create<IIntegrationResult>(MockBehavior.Strict).Object;
            Mock.Get(result).SetupGet(_result => _result.BuildProgressInformation).Returns(buildInfo);
            Mock.Get(result).SetupGet(_result => _result.ProjectName).Returns("Project name");
            Mock.Get(result).SetupProperty(_result => _result.Status);
            return result;
        }
        #endregion
    }
}
