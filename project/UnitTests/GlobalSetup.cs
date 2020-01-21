using System;
using System.IO;
using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.UnitTests
{
    [SetUpFixture]
    public class GlobalSetup
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            // or identically under the hoods
            //Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        }
    }
}
