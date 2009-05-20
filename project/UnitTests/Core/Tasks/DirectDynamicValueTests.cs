using Exortech.NetReflector;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    [TestFixture]
    public class DirectDynamicValueTests
    {
        [Test]
        public void SetGetProperties()
        {
            DirectDynamicValue value = new DirectDynamicValue();
            value.ParameterName = "test parameter";
            Assert.AreEqual("test parameter", value.ParameterName, "ParameterName not being get/set correctly");
            value.PropertyName = "test property";
            Assert.AreEqual("test property", value.PropertyName, "PropertyName not being get/set correctly");
        }

        [Test]
        public void ApplyTo()
        {
            MsBuildTask testTask = new MsBuildTask();
            DirectDynamicValue value = new DirectDynamicValue("newDir", "workingDirectory");
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("newDir", "a location");
            value.ApplyTo(testTask, parameters, null);
            Assert.AreEqual("a location", testTask.WorkingDirectory, "Value has not been correctly set");
        }
    }
}
