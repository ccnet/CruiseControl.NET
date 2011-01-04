namespace CruiseControl.Common.Tests.Messages
{
    using CruiseControl.Common.Messages;
    using NUnit.Framework;

    [TestFixture]
    public class ProjectMessageTests
    {
        #region Tests
        [Test]
        public void BasePropertiesAreSerialised()
        {
            var original = new ProjectMessage
                               {
                                   ProjectName = "local"
                               };
            var result = original.DoRoundTripTest();
            Assert.AreEqual(original.ProjectName, result.ProjectName);
        }
        #endregion
    }
}
