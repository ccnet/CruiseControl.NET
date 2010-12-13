namespace CruiseControl.Core.Tests
{
    using System;
    using NUnit.Framework;

    public class IntegrationRequestTests
    {
        #region Tests
        [Test]
        public void ConstructorSetsItemAndTime()
        {
            var item = new Project();
            var startTime = DateTime.Now;
            var request = new IntegrationRequest(
                new IntegrationContext(item));
            var endTime = DateTime.Now;
            Assert.IsTrue(request.RequestTime >= startTime && request.RequestTime <= endTime);
            Assert.AreSame(item, request.Context.Item);
        }
        #endregion
    }
}
