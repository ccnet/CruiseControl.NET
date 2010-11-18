namespace CruiseControl.Web.Tests
{
    using NUnit.Framework;
    using System.IO;

    [TestFixture]
    public class ActionHandlerFactoryTests
    {
        #region Tests
        [Test]
        public void DefaultInitialisesActionsHandlers()
        {
            ActionHandlerFactory.Reset();
            ActionHandlerFactory.PluginFolder = null;
            Assert.That(
                ActionHandlerFactory.Default.ActionHandlers,
                Is.Not.Empty);
        }

        [Test]
        public void DefaultScansPluginDirectory()
        {
            ActionHandlerFactory.Reset();
            ActionHandlerFactory.PluginFolder = Path.Combine(
                Path.GetTempPath(),
                "CCNetTesting");
            if (Directory.Exists(ActionHandlerFactory.PluginFolder))
            {
                Directory.Delete(ActionHandlerFactory.PluginFolder, true);
            }

            Directory.CreateDirectory(ActionHandlerFactory.PluginFolder);
            Assert.That(
                ActionHandlerFactory.Default.ActionHandlers,
                Is.Not.Empty);
        }

        [Test]
        public void DefaultIgnoresNonExistantDirectory()
        {
            ActionHandlerFactory.Reset();
            ActionHandlerFactory.PluginFolder = Path.Combine(
                Path.GetTempPath(),
                "CCNetTesting");
            if (Directory.Exists(ActionHandlerFactory.PluginFolder))
            {
                Directory.Delete(ActionHandlerFactory.PluginFolder, true);
            }

            Assert.That(
                ActionHandlerFactory.Default.ActionHandlers,
                Is.Not.Empty);
        }
        #endregion
    }
}
