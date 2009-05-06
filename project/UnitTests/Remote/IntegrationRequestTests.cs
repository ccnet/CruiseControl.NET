using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    [TestFixture]
    public class IntegrationRequestTests
    {
        #region Test methods
        #region GetHashCode()
        [Test]
        public void GetHashCodeReturnsAValidHasCode()
        {
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, 
                "Me");
            int expected = request.ToString().GetHashCode();
            int actual = request.GetHashCode();
            Assert.AreEqual(expected, actual);
        }
        #endregion
        #endregion
    }
}
