using Exortech.NetReflector;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    [TestFixture]
    public class ReplacementDynamicValueTests
    {
        [Test]
        public void SetGetProperties()
        {
            ReplacementDynamicValue value = new ReplacementDynamicValue();
            value.FormatValue = "test parameter";
            Assert.AreEqual("test parameter", value.FormatValue, "FormatValue not being get/set correctly");
            value.PropertyName = "test property";
            Assert.AreEqual("test property", value.PropertyName, "PropertyName not being get/set correctly");
        }

        [Test]
        public void ApplyTo()
        {
            MsBuildTask testTask = new MsBuildTask();
            ReplacementDynamicValue value = new ReplacementDynamicValue("{0}\\Happy", "workingDirectory", 
                new NameValuePair("newDir", null));
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("newDir", "a location");
            value.ApplyTo(testTask, parameters, null);
            Assert.AreEqual("a location\\Happy", testTask.WorkingDirectory, "Value has not been correctly set");
        }
    }
}
