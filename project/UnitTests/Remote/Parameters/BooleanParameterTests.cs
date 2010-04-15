namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Parameters
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ThoughtWorks.CruiseControl.Remote.Parameters;
    using ThoughtWorks.CruiseControl.Remote;

    [TestFixture]
    public class BooleanParameterTests
    {
        [Test]
        public void SetGetProperties()
        {
            var parameter = new BooleanParameter();
            var trueValue = new NameValuePair("TrueName", "TrueValue");
            var falseValue = new NameValuePair("FalseName", "FalseValue");
            parameter.TrueValue = trueValue;
            parameter.FalseValue = falseValue;
            Assert.AreEqual(typeof(string), parameter.DataType, "DataType does not match");
            Assert.AreSame(trueValue, parameter.TrueValue);
            Assert.AreSame(falseValue, parameter.FalseValue);
            Assert.IsNotNull(parameter.AllowedValues);
            Assert.AreEqual(2, parameter.AllowedValues.Length);
            Assert.AreEqual(trueValue.Name, parameter.AllowedValues[0]);
            Assert.AreEqual(falseValue.Name, parameter.AllowedValues[1]);

            parameter.IsRequired = false;
            Assert.AreEqual(false, parameter.IsRequired, "IsRequired does not match");
            parameter.IsRequired = true;
            Assert.AreEqual(true, parameter.IsRequired, "IsRequired does not match");
            parameter.Description = "Some description goes here";
            Assert.AreEqual("Some description goes here", parameter.Description, "Description does not match");
            parameter.Name = "Some name";
            Assert.AreEqual("Some name", parameter.Name, "Name does not match");
            Assert.AreEqual("Some name", parameter.DisplayName, "DisplayName does not match");
            parameter.DisplayName = "Another name";
            Assert.AreEqual("Another name", parameter.DisplayName, "DisplayName does not match");
        }

        [Test]
        public void IsRequiredWithBlank()
        {
            var parameter = new BooleanParameter();
            parameter.Name = "Test";
            parameter.IsRequired = true;
            var results = parameter.Validate(string.Empty);
            Assert.AreEqual(1, results.Length, "Number of exceptions does not match");
            Assert.AreEqual("Value of 'Test' is required", results[0].Message, "Exception message does not match");
        }
    }
}
