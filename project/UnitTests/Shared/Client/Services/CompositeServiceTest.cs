using System;
using System.Collections;

using NUnit.Framework;
using NMock;
using NMock.Constraints;

using ThoughtWorks.CruiseControl.Shared.Client.Services;
using ThoughtWorks.CruiseControl.Shared.Services.Commands.Reporting;
using ThoughtWorks.CruiseControl.Shared.Services;

namespace ThoughtWorks.CruiseControl.UnitTests.Shared.Client.Services
{
	[TestFixture]
	public class CompositeServiceTest : Assertion
	{
		ICruiseCommand _command;
		ICruiseResult _result;

		[SetUp]
		public void SetUp()
		{
			_command = (ICruiseCommand) new DynamicMock(typeof(ICruiseCommand)).MockInstance;
			_result = (ICruiseResult) new DynamicMock(typeof(ICruiseResult)).MockInstance;
		}

		[Test]
		public void ReturnsResultFromFirstServiceIfItSupportsCommand()
		{
			DynamicMock service1Mock = new DynamicMock(typeof(ICruiseService));
			DynamicMock service2Mock = new DynamicMock(typeof(ICruiseService));
			ICruiseService service1 = (ICruiseService) service1Mock.MockInstance;
			ICruiseService service2 = (ICruiseService) service2Mock.MockInstance;

			service1Mock.ExpectAndReturn("Run", _result, _command);
			service2Mock.ExpectNoCall("Run");

			CompositeService service = new CompositeService(new ICruiseService[] {service1, service2} );
			AssertEquals(_result, service.Run(_command));

			service1Mock.Verify();
			service2Mock.Verify();
		}

		[Test]
		public void ReturnsResultFromSecondServiceIfFirstDoesntSupportCommand()
		{
			DynamicMock service1Mock = new DynamicMock(typeof(ICruiseService));
			DynamicMock service2Mock = new DynamicMock(typeof(ICruiseService));
			ICruiseService service1 = (ICruiseService) service1Mock.MockInstance;
			ICruiseService service2 = (ICruiseService) service2Mock.MockInstance;

			service1Mock.ExpectAndReturn("Run", new NoValidServiceFoundResult(), _command);
			service2Mock.ExpectAndReturn("Run", _result, _command);

			CompositeService service = new CompositeService(new ICruiseService[] {service1, service2} );
			AssertEquals(_result, service.Run(_command));

			service1Mock.Verify();
			service2Mock.Verify();
		}


		[Test]
		public void ReturnsNoValidServiceFoundResultIfNoServicesAvailable()
		{
			CompositeService service = new CompositeService(new ICruiseService[0] );
			AssertEquals(typeof(NoValidServiceFoundResult), service.Run(_command).GetType());
		}

		[Test]
		public void ReturnsNoValidServiceFoundResultIfNoValidServiceAvailable()
		{
			DynamicMock service1Mock = new DynamicMock(typeof(ICruiseService));
			ICruiseService service1 = (ICruiseService) service1Mock.MockInstance;
			service1Mock.ExpectAndReturn("Run", new NoValidServiceFoundResult(), _command);

			CompositeService service = new CompositeService(new ICruiseService[] {service1} );
			AssertEquals(typeof(NoValidServiceFoundResult), service.Run(_command).GetType());

			service1Mock.Verify();
		}
	}
}