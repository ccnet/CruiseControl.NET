namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.ProjectReport
{
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
    using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;

    public class PackageListPluginTests
    {
        #region Private fields
        private MockRepository mocks;
        #endregion

        #region Setup
        [SetUp]
        public void Setup()
        {
            this.mocks = new MockRepository();
        }
        #endregion

        #region Tests
        [Test]
        public void DescriptionIsCorrect()
        {
            var plugin = new PackageListPlugin(null);
            Assert.AreEqual("Package List", plugin.LinkDescription);
        }

        [Test]
        public void NamedActionsReturnsSingleAction()
        {
            var actionInstantiator = this.mocks.StrictMock<IActionInstantiator>();
            var action = this.mocks.StrictMock<ICruiseAction>();
            SetupResult.For(actionInstantiator.InstantiateAction(typeof(PackageListAction)))
                .Return(action);

            this.mocks.ReplayAll();
            var plugin = new PackageListPlugin(actionInstantiator);
            var actions = plugin.NamedActions;

            this.mocks.VerifyAll();
            Assert.AreEqual(1, actions.Length);
            Assert.IsInstanceOf<ImmutableNamedAction>(actions[0]);
            Assert.AreEqual(PackageListAction.ActionName, actions[0].ActionName);
            Assert.AreSame(action, actions[0].Action);
        }
        #endregion
    }
}
