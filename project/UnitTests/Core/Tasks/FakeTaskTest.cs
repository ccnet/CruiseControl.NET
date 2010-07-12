using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exortech.NetReflector;
using NUnit.Framework;
using Rhino.Mocks;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    [TestFixture]
    public class FakeTaskTest
    {
        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void PopulateFromReflector()
        {
            var task = new FakeTask();
            const string xml = @"
    <fake>
    	<executable>C:\FAKE.exe</executable>
    	<baseDirectory>C:\</baseDirectory>
		<buildFile>mybuild.fx</buildFile>
		<description>Test description</description>
    </fake>";

            NetReflector.Read(xml, task);
            Assert.AreEqual(@"C:\FAKE.exe", task.Executable);
            Assert.AreEqual(@"C:\", task.ConfiguredBaseDirectory);
            Assert.AreEqual("mybuild.fx", task.BuildFile);
            Assert.AreEqual("Test description", task.Description);
        }

        [Test]
        public void PopulateFromConfigurationUsingOnlyRequiredElementsAndCheckDefaultValues()
        {
            
            var task = new FakeTask();
            const string xml = "<fake />";

            NetReflector.Read(xml, task);
            Assert.AreEqual(FakeTask.defaultExecutable, task.Executable);
            Assert.AreEqual(string.Empty, task.ConfiguredBaseDirectory);
            Assert.AreEqual(string.Empty, task.BuildFile);
            Assert.AreEqual(null, task.Description);
        }
    }
}
