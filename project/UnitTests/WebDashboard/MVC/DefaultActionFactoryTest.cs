using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC
{
	[TestFixture]
	public class ConfiguredActionFactoryTest : Assertion
	{
		private DynamicMock mockRequest;
		private DynamicMock mockConfiguration;
		private DynamicMock mockTypeInstantiator;
		private DynamicMock mockAction;

		private IAction action;
		private IRequest request;
		private IConfiguredActionFactoryConfiguration configuration;
		private ITypeInstantiator typeInstantiator;

		private ConfiguredActionFactory actionFactory;

		[SetUp]
		public void Setup()
		{
			mockRequest = new DynamicMock(typeof(IRequest));
			mockConfiguration = new DynamicMock(typeof(IConfiguredActionFactoryConfiguration));
			mockTypeInstantiator = new DynamicMock(typeof(ITypeInstantiator));
			mockAction = new DynamicMock(typeof(IAction));

			configuration = (IConfiguredActionFactoryConfiguration) mockConfiguration.MockInstance;
			typeInstantiator = (ITypeInstantiator) mockTypeInstantiator.MockInstance;
			request = (IRequest) mockRequest.MockInstance;
			action = (IAction) mockAction.MockInstance;

			actionFactory = new ConfiguredActionFactory(configuration, typeInstantiator);
		}

		private void VerifyAll()
		{
			mockRequest.Verify();
			mockConfiguration.Verify();
			mockAction.Verify();
			mockTypeInstantiator.Verify();
		}

		[Test]
		public void ReturnsDefaultActionIfNoActionInRequest()
		{
			/// Setup
			ITypeSpecification typeSpecification = (ITypeSpecification) new DynamicMock(typeof(ITypeSpecification)).MockInstance;
			mockConfiguration.ExpectAndReturn("GetDefaultActionTypeSpecification", typeSpecification);
			mockTypeInstantiator.ExpectAndReturn("GetInstance", action, typeSpecification);

			/// Execute & Verify
			AssertEquals(action, actionFactory.Create(request));
			VerifyAll();
		}

		[Test]
		public void ReturnsCorrectActionForRequest()
		{
			/// Setup
			mockRequest.ExpectAndReturn("FindParameterStartingWith", "MyAction", "_action_");
			ITypeSpecification typeSpecification = (ITypeSpecification) new DynamicMock(typeof(ITypeSpecification)).MockInstance;
			mockConfiguration.ExpectAndReturn("GetTypeSpecification", typeSpecification, "MyAction");
			mockTypeInstantiator.ExpectAndReturn("GetInstance", action, typeSpecification);
			
			/// Execute & Verify
			AssertEquals(action, actionFactory.Create(request));
			VerifyAll();
		}

		[Test]
		public void FailsWithCorrectExceptionIfActionNotConfigured()
		{
			/// Setup
			mockRequest.ExpectAndReturn("FindParameterStartingWith", "MyAction", "_action_");
			ITypeSpecification nullTypeSpecification = new NullTypeSpecification();
			mockConfiguration.ExpectAndReturn("GetTypeSpecification", nullTypeSpecification, "MyAction");
			mockTypeInstantiator.ExpectNoCall("GetInstance", typeof(ITypeSpecification));
			
			/// Execute & Verify
			try
			{
				actionFactory.Create(request);
				Fail("Should throw an exception");
			}
			catch (ActionFactoryConfigurationException)
			{
				
			}
			VerifyAll();
		}

		[Test]
		public void FailsWithCorrectExceptionIfNonActionTypeConfigured()
		{
			/// Setup
			ITypeSpecification typeSpecification = (ITypeSpecification) new DynamicMock(typeof(ITypeSpecification)).MockInstance;
			mockConfiguration.ExpectAndReturn("GetDefaultActionTypeSpecification", typeSpecification);
			mockTypeInstantiator.ExpectAndReturn("GetInstance", "this is not an action", typeSpecification);

			/// Execute & Verify
			try
			{
				actionFactory.Create(request);
				Fail("Should throw an exception");
			}
			catch (ActionFactoryConfigurationException)
			{
				
			}
			VerifyAll();
		}
	}
}
