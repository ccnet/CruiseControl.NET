namespace CruiseControl.Web.Tests
{
    using System;
    using System.Linq;
    using Moq;
    using NLog;
    using NLog.Config;
    using NLog.Targets;
    using NUnit.Framework;
    using Web.Configuration;

    [TestFixture]
    public class DynamicControllerTests
    {
        #region Tests
        [Test]
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
                controller.Index("blankServerName", "blankProjectName", "blankBuildName", "nowherenonsenseaction");

                var expectedMessages = new[] 
                    {
                        MakeMessage("Dynamically resolving request", LogLevel.Debug),
                        MakeMessage("Generating request context", LogLevel.Debug),
                        MakeMessage("Action is a build level report", LogLevel.Debug),
                        MakeMessage("Retrieving action handler for nowherenonsenseaction", LogLevel.Debug),
                        MakeMessage("Unable to find action handler for nowherenonsenseaction", LogLevel.Info),
                        MakeMessage("Generating error message", LogLevel.Debug)
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

        [Test]
        public void GenerateContextReturnsEverythingIfEverythingIsSet()
        {
            var controller = new DynamicController();
            var server = "serverName";
            var project = "projectName";
            var build = "buildName";
            var report = "reportName";
            var context = controller.GenerateContext(server, project, build, report);
            Assert.That(context.Server, Is.EqualTo(server));
            Assert.That(context.Project, Is.EqualTo(project));
            Assert.That(context.Build, Is.EqualTo(build));
            Assert.That(context.Report, Is.EqualTo(report));
            Assert.That(context.Level, Is.EqualTo(ActionHandlerTargets.Build));
        }

        [Test]
        public void GenerateContextShouldReturnDefaultRootActionIfNothingSet()
        {
            var controller = new DynamicController();
            try
            {
                Manager.Reset();
                Manager.Current = new Settings();
                var defaultAction = "defaultAction";
                AppendReportLevel(
                    ActionHandlerTargets.Root,
                    new Report
                        {
                            ActionName = defaultAction,
                            IsDefault = true
                        });
                var context = controller.GenerateContext(string.Empty, string.Empty, string.Empty, string.Empty);
                Assert.That(context.Report, Is.EqualTo(defaultAction));
                Assert.That(context.Level, Is.EqualTo(ActionHandlerTargets.Root));
            }
            finally
            {
                Manager.Reset();
            }
        }

        [Test]
        public void GenerateContextFailsIfNoDefaultRootLevelAction()
        {
            var controller = new DynamicController();
            try
            {
                Manager.Reset();
                Manager.Current = new Settings();
                var context = controller.GenerateContext(string.Empty, string.Empty, string.Empty, string.Empty);
                Assert.That(context.Report, Is.EqualTo("!!unknownAction!!"));
                Assert.That(context.Level, Is.EqualTo(ActionHandlerTargets.Root));
            }
            finally
            {
                Manager.Reset();
            }
        }

        [Test]
        public void GenerateContextShouldReturnRootLevelActionIfValidRootAction()
        {
            var controller = new DynamicController();
            try
            {
                Manager.Reset();
                Manager.Current = new Settings();
                var defaultAction = "defaultAction";
                AppendReportLevel(
                    ActionHandlerTargets.Root,
                    new Report
                    {
                        ActionName = defaultAction,
                        IsDefault = true
                    });
                var context = controller.GenerateContext(defaultAction, string.Empty, string.Empty, string.Empty);
                Assert.That(context.Report, Is.EqualTo(defaultAction));
                Assert.That(context.Level, Is.EqualTo(ActionHandlerTargets.Root));
            }
            finally
            {
                Manager.Reset();
            }
        }

        [Test]
        public void GenerateContextShouldReturnServerLevelActionIfRootIsNotAnAction()
        {
            var controller = new DynamicController();
            try
            {
                Manager.Reset();
                Manager.Current = new Settings();
                var defaultAction = "defaultAction";
                var server = "serverName";
                AppendReportLevel(
                    ActionHandlerTargets.Server,
                    new Report
                    {
                        ActionName = defaultAction,
                        IsDefault = true
                    });
                var context = controller.GenerateContext(server, string.Empty, string.Empty, string.Empty);
                Assert.That(context.Server, Is.EqualTo(server));
                Assert.That(context.Report, Is.EqualTo(defaultAction));
                Assert.That(context.Level, Is.EqualTo(ActionHandlerTargets.Server));
            }
            finally
            {
                Manager.Reset();
            }
        }

        [Test]
        public void GenerateContextShouldReturnServerLevelActionIfValidServerAction()
        {
            var controller = new DynamicController();
            try
            {
                Manager.Reset();
                Manager.Current = new Settings();
                var server = "theServer";
                var defaultAction = "defaultAction";
                AppendReportLevel(
                    ActionHandlerTargets.Server,
                    new Report
                    {
                        ActionName = defaultAction,
                        IsDefault = true
                    });
                var context = controller.GenerateContext(server, defaultAction, string.Empty, string.Empty);
                Assert.That(context.Server, Is.EqualTo(server));
                Assert.That(context.Report, Is.EqualTo(defaultAction));
                Assert.That(context.Level, Is.EqualTo(ActionHandlerTargets.Server));
            }
            finally
            {
                Manager.Reset();
            }
        }

        [Test]
        public void GenerateContextShouldReturnProjectLevelActionIfServerIsNotAnAction()
        {
            var controller = new DynamicController();
            try
            {
                Manager.Reset();
                Manager.Current = new Settings();
                var defaultAction = "defaultAction";
                var server = "theServer";
                var project = "theProject";
                AppendReportLevel(
                    ActionHandlerTargets.Project,
                    new Report
                    {
                        ActionName = defaultAction,
                        IsDefault = true
                    });
                var context = controller.GenerateContext(server, project, string.Empty, string.Empty);
                Assert.That(context.Server, Is.EqualTo(server));
                Assert.That(context.Project, Is.EqualTo(project));
                Assert.That(context.Report, Is.EqualTo(defaultAction));
                Assert.That(context.Level, Is.EqualTo(ActionHandlerTargets.Project));
            }
            finally
            {
                Manager.Reset();
            }
        }

        [Test]
        public void GenerateContextShouldReturnProjectLevelActionIfValidProjectAction()
        {
            var controller = new DynamicController();
            try
            {
                Manager.Reset();
                Manager.Current = new Settings();
                var defaultAction = "defaultAction";
                var server = "theServer";
                var project = "theProject";
                AppendReportLevel(
                    ActionHandlerTargets.Project,
                    new Report
                    {
                        ActionName = defaultAction,
                        IsDefault = true
                    });
                var context = controller.GenerateContext(server, project, defaultAction, string.Empty);
                Assert.That(context.Server, Is.EqualTo(server));
                Assert.That(context.Project, Is.EqualTo(project));
                Assert.That(context.Report, Is.EqualTo(defaultAction));
                Assert.That(context.Level, Is.EqualTo(ActionHandlerTargets.Project));
            }
            finally
            {
                Manager.Reset();
            }
        }

        [Test]
        public void GenerateContextShouldReturnBuildLevelActionIfProjectIsNotAnAction()
        {
            var controller = new DynamicController();
            try
            {
                Manager.Reset();
                Manager.Current = new Settings();
                var defaultAction = "defaultAction";
                var server = "theServer";
                var project = "theProject";
                var build = "aBuild";
                AppendReportLevel(
                    ActionHandlerTargets.Build,
                    new Report
                    {
                        ActionName = defaultAction,
                        IsDefault = true
                    });
                var context = controller.GenerateContext(server, project, build, string.Empty);
                Assert.That(context.Server, Is.EqualTo(server));
                Assert.That(context.Project, Is.EqualTo(project));
                Assert.That(context.Build, Is.EqualTo(build));
                Assert.That(context.Report, Is.EqualTo(defaultAction));
                Assert.That(context.Level, Is.EqualTo(ActionHandlerTargets.Build));
            }
            finally
            {
                Manager.Reset();
            }
        }
        #endregion

        #region Helpers
        private static Lazy<ActionHandler, IActionHandlerMetadata> InitialiseMockHandler(string actionName)
        {
            ActionHandlerFactory.Reset();
            ActionHandlerFactory.Default = new ActionHandlerFactory();
            var metadataMock = new Mock<IActionHandlerMetadata>(MockBehavior.Strict);
            metadataMock.Setup(metadata => metadata.Name).Returns(actionName);
            var expected = new Lazy<ActionHandler, IActionHandlerMetadata>(metadataMock.Object);
            return expected;
        }

        private static string MakeMessage(string message, LogLevel level)
        {
            var text = typeof(DynamicController).FullName + "|" +
                level + "|" +
                message;
            return text;
        }

        private void AppendReportLevel(ActionHandlerTargets level, params Report[] reports)
        {
            var reportLevel = new ReportLevel {Target = level};
            foreach (var report in reports)
            {
                reportLevel.Reports.Add(report);
            }

            Manager.Current.ReportLevels.Add(reportLevel);
        }
        #endregion
    }
}
