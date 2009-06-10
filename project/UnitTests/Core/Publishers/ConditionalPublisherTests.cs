using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
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
        private MockRepository mocks = new MockRepository();
        #endregion

        #region Tests
        [Test]
        public void RunExecutesPublishersWhenConditionIsMet()
        {
            var result = GenerateResultMock();
            var logger = mocks.DynamicMock<ILogger>();
            var childPublisher = mocks.StrictMock<ITask>();
            Expect.Call(() => { childPublisher.Run(result); });
            var publisher = new ConditionalPublisher
            {
                Logger = logger,
                Publishers = new ITask[] {
                    childPublisher
                },
                Conditions = new IntegrationStatus[] {
                    IntegrationStatus.Success
                }
            };
            mocks.ReplayAll();

            result.Status = IntegrationStatus.Success;
            publisher.Run(result);

            mocks.VerifyAll();
        }

        [Test]
        public void RunDoesNotExecutePublishersWhenConditionIsNotMet()
        {
            var result = GenerateResultMock();
            var logger = mocks.DynamicMock<ILogger>();
            var childPublisher = mocks.StrictMock<ITask>();
            var publisher = new ConditionalPublisher
            {
                Logger = logger,
                Publishers = new ITask[] {
                    childPublisher
                },
                Conditions = new IntegrationStatus[] {
                    IntegrationStatus.Success
                }
            };
            mocks.ReplayAll();

            result.Status = IntegrationStatus.Failure;
            publisher.Run(result);

            mocks.VerifyAll();
        }

        private IIntegrationResult GenerateResultMock()
        {
            var buildInfo = mocks.DynamicMock<BuildProgressInformation>(string.Empty, string.Empty);
            var result = mocks.StrictMock<IIntegrationResult>();
            SetupResult.For(result.BuildProgressInformation).Return(buildInfo);
            SetupResult.For(result.ProjectName).Return("Project name");
            Expect.Call(result.Status).PropertyBehavior();
            return result;
        }
        #endregion
    }
}
