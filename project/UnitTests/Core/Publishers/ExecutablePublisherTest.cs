using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Test;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
    [TestFixture]
    public class ExecutablePublisherTest
    {
        private ExecutablePublisher _publisher;
        private IntegrationResult _result;
		private IMock _mockExecutor;

        private static readonly string _EXECUTABLE = "someexe.exe";
        private static readonly string _ARGUMENTS = "arg1 arg2";
        private static readonly int _TIMEOUT = 777;
        private static readonly string _WORKING_DIRECTORY = "D:\\somedir";
        private static readonly bool _NONZERO_EXIT_FATAL = true;
        private static readonly string _LABEL = "foobar123";

        private static readonly string _LABEL_ENVIRONMENT_KEY = "ccnet.label";

        private static readonly string _XML_STUB =
            @"<publisher type=""executable""
             executable=""{0}""
             arguments=""{1}""
             timeout=""{2}""
             workingDirectory=""{3}""
             nonzeroExitFatal=""{4}"" />";

    	[SetUp]
        public void SetUp()
        {
			_mockExecutor = new DynamicMock(typeof(ProcessExecutor));
            _publisher = new ExecutablePublisher((ProcessExecutor) _mockExecutor.MockInstance);
            _publisher.Executable = "cmd.exe";
            _publisher.NonzeroExitFatal = true;
            _result = IntegrationResultMother.CreateSuccessful();
            _result.Label = _LABEL;
        }

        [TearDown]
        public void TearDown()
        {
			_mockExecutor.Verify();
        }

        [Test]
        public void CanInitFromXml()
        {
            _publisher = new ExecutablePublisher();
            NetReflector.Read( CreateConfigurationXml( _EXECUTABLE,
                _ARGUMENTS,
                _TIMEOUT.ToString(),
                _WORKING_DIRECTORY,
                _NONZERO_EXIT_FATAL.ToString() ), _publisher);

            Assert.AreEqual( _EXECUTABLE, _publisher.Executable, "could not init the executable" );
            Assert.AreEqual( _ARGUMENTS, _publisher.Arguments, "could not init the arguments" );
            Assert.AreEqual( _NONZERO_EXIT_FATAL, _publisher.NonzeroExitFatal, "could not init NonzeroExitFatal" );
            Assert.AreEqual( _TIMEOUT, _publisher.Timeout, "could not init TimeOut" );
            Assert.AreEqual( _WORKING_DIRECTORY, _publisher.WorkingDirectory, "could not init WorkingDirectory" );
        }

        [Test]
        [ExpectedException( typeof( NetReflectorException ) )]
        public void InvalidFatalExitConfigElementCausesException()
        {
            _publisher = new ExecutablePublisher();
            NetReflector.Read( CreateConfigurationXml( _EXECUTABLE,
                _ARGUMENTS,
                _TIMEOUT.ToString(),
                _WORKING_DIRECTORY,
                "NOT_A_BOOL" ), _publisher);
        }

        [Test]
        [ExpectedException( typeof( NetReflectorException ) )]
        public void InvalidTimeoutConfigElementCausesException()
        {
            _publisher = new ExecutablePublisher();
            NetReflector.Read( CreateConfigurationXml( _EXECUTABLE,
                _ARGUMENTS,
                "NOT_AN_INT",
                _WORKING_DIRECTORY,
                _NONZERO_EXIT_FATAL.ToString() ), _publisher);
        }

        [Test]
		public void CanGetLabel()
        {
        	CollectingConstraint args = new CollectingConstraint();
        	_mockExecutor.ExpectAndReturn("Execute", ProcessResultFixture.CreateSuccessfulResult(), args);
			
			_publisher.Arguments = "/C \"set\"";
            _publisher.PublishIntegrationResults(_result );

            Assert.IsNotNull( _publisher.StandardOutput, "StandardOutput was null" );

			ProcessInfo actualInfo = (ProcessInfo) args.Parameter;
            Assert.IsTrue( actualInfo.EnvironmentVariables.ContainsKey(_LABEL_ENVIRONMENT_KEY),
                "the label key wasn't in the environment" );
            Assert.AreEqual( _LABEL, actualInfo.EnvironmentVariables[_LABEL_ENVIRONMENT_KEY],
                "the label wasn't in the environment" );
        }

        [Test, ExpectedException(typeof( CruiseControlException ) )]
        public void CanHandleTimeout()
        {
			_mockExecutor.ExpectAndReturn("Execute", ProcessResultFixture.CreateTimedOutResult(), new IsAnything());
			
			_publisher.Arguments = "/C \"sleep 200\"";
            _publisher.Timeout = 1;
            _publisher.PublishIntegrationResults(_result );
        }

        [Test, ExpectedException( typeof( CruiseControlException ) )]
        public void NonzeroExitFatal()
        {
			_mockExecutor.ExpectAndReturn("Execute", ProcessResultFixture.CreateNonZeroExitCodeResult(), new IsAnything());

            _publisher.Arguments = "/C \"exit 1\"";
            _publisher.PublishIntegrationResults(_result );
        }

        [Test]
        public void CanExecuteSimpleProcess()
        {
			_mockExecutor.ExpectAndReturn("Execute", ProcessResultFixture.CreateSuccessfulResult(), new IsAnything());
			
			_publisher.Arguments = "/C \"echo foo\"";
            _publisher.PublishIntegrationResults(_result );
        }

        private string CreateConfigurationXml( string executable, string arguments, string timeout, string workingDirectory, string nonzeroExitFatal )
        {
            return string.Format( _XML_STUB, executable, arguments, timeout, workingDirectory, nonzeroExitFatal );
        }
    }
}