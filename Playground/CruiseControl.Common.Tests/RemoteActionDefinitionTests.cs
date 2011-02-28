namespace CruiseControl.Common.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class RemoteActionDefinitionTests
    {
        #region Tests
        [Test]
        public void ToStringHandlesMissingInputAndOuput()
        {
            var definition = new RemoteActionDefinition
                                 {
                                     Name = "TestAction"
                                 };
            var expected = "[TestAction(?)=>(?)]";
            var actual = definition.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToStringHandlesInputAndOuput()
        {
            var definition = new RemoteActionDefinition
                                 {
                                     Name = "TestAction",
                                     InputData = "<definition name=\"Blank\" namespace=\"urn:cruisecontrol:common\" />",
                                     OutputData = "<definition name=\"BuildRequest\" namespace=\"urn:cruisecontrol:common\"></definition>"
                                 };
            var expected = "[TestAction(Blank)=>(BuildRequest)]";
            var actual = definition.ToString();
            Assert.AreEqual(expected, actual);
        }
        #endregion
    }
}
