using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Parameters
{
    [TestFixture]
    public class TextParameterTests
    {
        [Test]
        public void SetGetProperties()
        {
            TextParameter parameter = new TextParameter();
            Assert.IsNull(parameter.AllowedValues, "AllowedValues is not null");
            Assert.AreEqual(typeof(string), parameter.DataType, "DataType does not match");

            parameter.IsRequired = false;
            Assert.AreEqual(false, parameter.IsRequired, "IsRequired does not match");
            parameter.IsRequired = true;
            Assert.AreEqual(true, parameter.IsRequired, "IsRequired does not match");
            parameter.MaximumLength = 100;
            Assert.AreEqual(100, parameter.MaximumLength, "MaximumLength does not match");
            parameter.MaximumLength = 0;
            Assert.AreEqual(0, parameter.MaximumLength, "MaximumLength does not match");
            parameter.MinimumLength = 100;
            Assert.AreEqual(100, parameter.MinimumLength, "MinimumLength does not match");
            parameter.MinimumLength = 0;
            Assert.AreEqual(0, parameter.MinimumLength, "MinimumLength does not match");
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
            TextParameter parameter = new TextParameter();
            parameter.Name = "Test";
            parameter.IsRequired = true;
            Exception[] results = parameter.Validate(string.Empty);
            Assert.AreEqual(1, results.Length, "Number of exceptions does not match");
            Assert.AreEqual("Value of 'Test' is required", results[0].Message, "Exception message does not match");
        }

        [Test]
        public void ValueWithinValueRange()
        {
            TextParameter parameter = new TextParameter();
            parameter.Name = "Test";
            parameter.MinimumLength = 0;
            parameter.MaximumLength = 20;
            Exception[] results = parameter.Validate("50");
            Assert.AreEqual(0, results.Length, "Number of exceptions does not match");
        }

        [Test]
        public void ValueBelowLengthRange()
        {
            TextParameter parameter = new TextParameter();
            parameter.Name = "Test";
            parameter.MinimumLength = 15;
            parameter.MaximumLength = 20;
            Exception[] results = parameter.Validate("50");
            Assert.AreEqual(1, results.Length, "Number of exceptions does not match");
            Assert.AreEqual("Value of 'Test' is less than the minimum length (15)", results[0].Message, "Exception message does not match");
        }

        [Test]
        public void ValueAboveLengthRange()
        {
            TextParameter parameter = new TextParameter();
            parameter.Name = "Test";
            parameter.MinimumLength = 0;
            parameter.MaximumLength = 20;
            Exception[] results = parameter.Validate("123456789012345678901234567890");
            Assert.AreEqual(1, results.Length, "Number of exceptions does not match");
            Assert.AreEqual("Value of 'Test' is more than the maximum length (20)", results[0].Message, "Exception message does not match");
        }

        [Test]
        public void ConvertReturnsOriginalString()
        {
            var parameter = new TextParameter();
            var value = parameter.Convert("testValue");
            Assert.AreEqual("testValue", value);
        }
    }
}
