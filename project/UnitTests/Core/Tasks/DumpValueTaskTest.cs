using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;
using NUnit.Framework;
using Rhino.Mocks;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{

    [TestFixture]
    class DumpValueTaskTest
    {
        private MockRepository mocks;
        private ProcessExecutor executor;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
            executor = mocks.StrictMock<ProcessExecutor>();
        }

        [Test]
        public void PopulateFromReflector()
        {
            var task = new DumpValueTask();
            const string xml = @"
    <DumpValue>
    	<xmlFileName>C:\some\path\to\file.xml</xmlFileName>
    	<values>
            <namedValue name='The Name' value='something' />
            <namedValue name='The Name 2' value='some other thing' />
            <namedValue name='The Name 3' value='stuff' />
            <namedValue name='The Name 4' value='last but not least' />
        </values>
    </DumpValue>";

            NetReflector.Read(xml, task);
            Assert.AreEqual(@"C:\some\path\to\file.xml", task.XmlFileName);
            Assert.AreEqual(4, task.Values.Length);
            Assert.AreEqual("The Name", task.Values[0].Name);
            Assert.AreEqual("something", task.Values[0].Value);
            Assert.AreEqual("The Name 2", task.Values[1].Name);
            Assert.AreEqual("some other thing", task.Values[1].Value);
            Assert.AreEqual("The Name 3", task.Values[2].Name);
            Assert.AreEqual("stuff", task.Values[2].Value);
            Assert.AreEqual("The Name 4", task.Values[3].Name);
            Assert.AreEqual("last but not least", task.Values[3].Value);
        }

    }
}
