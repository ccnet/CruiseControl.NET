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
		public void ReturnsResultFromFirstSpecializedServiceThatSupportsCommand()
		{
			DynamicMock service1Mock = new DynamicMock(typeof(ISpecializedCruiseService));
			DynamicMock service2Mock = new DynamicMock(typeof(ISpecializedCruiseService));
			ISpecializedCruiseService service1 = (ISpecializedCruiseService) service1Mock.MockInstance;
			ISpecializedCruiseService service2 = (ISpecializedCruiseService) service2Mock.MockInstance;

			service1Mock.SetupResult("SupportedCommandTypes", new Type[] { _command.GetType() });
			service1Mock.ExpectAndReturn("Run", _result, _command);
			service2Mock.ExpectNoCall("Run");

			CompositeService service = new CompositeService(new ICruiseService[] {service1, service2} );
			AssertEquals(_result, service.Run(_command));

			// Bug in our old version of NMock? this fails on setup result
			//service1Mock.Verify();
			service2Mock.Verify();
		}

		[Test]
		public void ReturnsNoValidServiceFoundResultIfNoServicesAvailable()
		{
			CompositeService service = new CompositeService(new ICruiseService[0] );
			AssertEquals(typeof(NoValidServiceFoundResult), service.Run(_command).GetType());
		}

		[Test]
		public void ReturnsNoValidServiceFoundResultIfNoValidSpecializedServiceAvailable()
		{
			DynamicMock service1Mock = new DynamicMock(typeof(ISpecializedCruiseService));
			ISpecializedCruiseService service1 = (ISpecializedCruiseService) service1Mock.MockInstance;
			service1Mock.SetupResult("SupportedCommandTypes", new Type[0]);
			service1Mock.ExpectNoCall("Run");

			CompositeService service = new CompositeService(new ICruiseService[] {service1} );
			AssertEquals(typeof(NoValidServiceFoundResult), service.Run(_command).GetType());

			// Bug in our old version of NMock? this fails on setup result
			//service1Mock.Verify();
		}

		[Test]
		public void DelegatesThroughToNonSpecializedServicesAndReturnsResultIfProcessed()
		{
			DynamicMock service1Mock = new DynamicMock(typeof(ICruiseService));
			DynamicMock service2Mock = new DynamicMock(typeof(ISpecializedCruiseService));
			ICruiseService service1 = (ICruiseService) service1Mock.MockInstance;
			ISpecializedCruiseService service2 = (ISpecializedCruiseService) service2Mock.MockInstance;

			service1Mock.ExpectAndReturn("Run", _result, _command);
			service2Mock.ExpectNoCall("Run");

			CompositeService service = new CompositeService(new ICruiseService[] {service1, service2} );
			AssertEquals(_result, service.Run(_command));

			// Bug in our old version of NMock? this fails on setup result
			//service1Mock.Verify();
			service2Mock.Verify();
		}

		[Test]
		public void DelegatesThroughToNonSpecializedServicesAndTriesAnotherIfNoServiceFound()
		{
			DynamicMock service1Mock = new DynamicMock(typeof(ICruiseService));
			DynamicMock service2Mock = new DynamicMock(typeof(ISpecializedCruiseService));
			ICruiseService service1 = (ICruiseService) service1Mock.MockInstance;
			ISpecializedCruiseService service2 = (ISpecializedCruiseService) service2Mock.MockInstance;

			service2Mock.SetupResult("SupportedCommandTypes", new Type[] { _command.GetType() });
			service1Mock.ExpectAndReturn("Run", new NoValidServiceFoundResult(), _command);
			service2Mock.ExpectAndReturn("Run", _result, _command);

			CompositeService service = new CompositeService(new ICruiseService[] {service1, service2} );
			AssertEquals(_result, service.Run(_command));

			// Bug in our old version of NMock? this fails on setup result
			//service1Mock.Verify();
			//service2Mock.Verify();
		}
	}
}