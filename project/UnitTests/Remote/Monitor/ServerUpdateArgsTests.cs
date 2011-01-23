namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Monitor
{
    using System;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote.Monitor;

    public class ServerUpdateArgsTests
    {
        #region Tests
        [Test]
        public void ConstructorSetsException()
        {
            var exception = new Exception();
            var args = new ServerUpdateArgs(exception);
            Assert.AreSame(exception, args.Exception);
        }
        #endregion
    }
}
