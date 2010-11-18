namespace CruiseControl.Web.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class ActionHandlerAttributeTests
    {
        #region Tests
        [Test]
        public void ConstructorSetsProperties()
        {
            var name = "actionHandlerName";
            var targets = ActionHandlerTargets.Root | ActionHandlerTargets.Server;
            var attribute = new ActionHandlerAttribute(name, targets);
            Assert.That(attribute.Name, Is.EqualTo(name));
            Assert.That(attribute.Targets, Is.EqualTo(targets));
        }
        #endregion
    }
}
