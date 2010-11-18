namespace CruiseControl.Web.Tests
{
    using System;
    using System.Linq;
    using Moq;
    using NLog;
    using NLog.Config;
    using NLog.Targets;
    using NUnit.Framework;

    [TestFixture]
    public class DynamicControllerTests
    {
        #region Tests
        [Test]
        [Explicit("Method not implemented yet - therefore this will fail with an exception")]
        public void IndexLogsEvents()
        {
            // Initialise the configuration
            var config = new LoggingConfiguration();
            var target = new MemoryTarget
                {
                    Layout = "${Logger}|${Level}|${Message}"
                };
            config.AddTarget("unitTesting", target);
            var rule = new LoggingRule("*", LogLevel.Debug, target);
            config.LoggingRules.Add(rule);

            // Generate the logger
            var factory = new LogFactory
                {
                    Configuration = config
                };
            var logger = factory.GetLogger(typeof(DynamicController).FullName);
            try
            {
                DynamicController.OverrideLogger(logger);
                var controller = new DynamicController();

                // Resolve an action that doesn't exist - we are only interested that logging is actually workig
                controller.Index("nowherenonsenseaction", string.Empty, string.Empty, string.Empty);

                var expectedMessages = new[] 
                    {
                        MakeMessage("Dynamically resolving request", LogLevel.Debug),
                        MakeMessage("Generating request context", LogLevel.Debug)
                    };
                Assert.That(target.Logs.ToArray(), Is.EqualTo(expectedMessages));
            }
            finally
            {
                DynamicController.ResetLogger();
            }
        }

        [Test]
        public void RetrieveHandlerRetrievesAHandlerIfItExists()
        {
            var actionName = "testAction";
            try
            {
                var expected = InitialiseMockHandler(actionName);
                ActionHandlerFactory.Default.ActionHandlers.Add(expected);
                var controller = new DynamicController();
                var actual = controller.RetrieveHandler(actionName);
                Assert.That(actual, Is.SameAs(expected));
            }
            finally
            {
                ActionHandlerFactory.Reset();
            }
        }

        [Test]
        public void RetrieveHandlerIgnoresCase()
        {
            var actionName = "testAction";
            try
            {
                var expected = InitialiseMockHandler(actionName);
                ActionHandlerFactory.Default.ActionHandlers.Add(expected);
                var controller = new DynamicController();
                var actual = controller.RetrieveHandler(actionName.ToUpperInvariant());
                Assert.That(actual, Is.SameAs(expected));
            }
            finally
            {
                ActionHandlerFactory.Reset();
            }
        }

        [Test]
        public void RetrieveHandlerReturnsNullIfNotFound()
        {
            try
            {
                var expected = InitialiseMockHandler("testAction");
                ActionHandlerFactory.Default.ActionHandlers.Add(expected);
                var controller = new DynamicController();
                var actual = controller.RetrieveHandler("garbage");
                Assert.That(actual, Is.Null);
            }
            finally
            {
                ActionHandlerFactory.Reset();
            }
        }
        #endregion

        #region Helpers
        private static Lazy<ActionHandler, IActionHandlerMetadata> InitialiseMockHandler(string actionName)
        {
            ActionHandlerFactory.Reset();
            ActionHandlerFactory.Default = new ActionHandlerFactory();
            var metadataMock = new Mock<IActionHandlerMetadata>();
            metadataMock.Setup(metadata => metadata.Name).Returns(actionName);
            var expected = new Lazy<ActionHandler, IActionHandlerMetadata>(metadataMock.Object);
            return expected;
        }

        private static string MakeMessage(string message, LogLevel level)
        {
            var text = typeof(DynamicController).FullName + "|" +
                level.ToString() + "|" +
                message;
            return text;
        }
        #endregion
    }
}
