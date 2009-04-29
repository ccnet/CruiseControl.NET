using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Parameters
{
    [TestFixture]
    public class NumericParameterTests
    {
        [Test]
        public void SetGetProperties()
        {
            NumericParameter parameter = new NumericParameter();
            Assert.IsNull(parameter.AllowedValues, "AllowedValues is not null");
            Assert.AreEqual(typeof(double), parameter.DataType, "DataType does not match");

            parameter.IsRequired = false;
            Assert.AreEqual(false, parameter.IsRequired, "IsRequired does not match");
            parameter.IsRequired = true;
            Assert.AreEqual(true, parameter.IsRequired, "IsRequired does not match");
            parameter.MaximumValue = 100;
            Assert.AreEqual(100, parameter.MaximumValue, "MaximumValue does not match");
            parameter.MaximumValue = 0;
            Assert.AreEqual(0, parameter.MaximumValue, "MaximumValue does not match");
            parameter.MinimumValue = 100;
            Assert.AreEqual(100, parameter.MinimumValue, "MinimumValue does not match");
            parameter.MinimumValue = 0;
            Assert.AreEqual(0, parameter.MinimumValue, "MinimumValue does not match");
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
            NumericParameter parameter = new NumericParameter();
            parameter.Name = "Test";
            parameter.IsRequired = true;
            Exception[] results = parameter.Validate(string.Empty);
            Assert.AreEqual(1, results.Length, "Number of exceptions does not match");
            Assert.AreEqual("Value of 'Test' is required", results[0].Message, "Exception message does not match");
        }

        [Test]
        public void ValueIsNumeric()
        {
            NumericParameter parameter = new NumericParameter();
            parameter.Name = "Test";
            Exception[] results = parameter.Validate("Test");
            Assert.AreEqual(1, results.Length, "Number of exceptions does not match");
            Assert.AreEqual("Value of 'Test' is not numeric", results[0].Message, "Exception message does not match");
        }

        [Test]
        public void ValueWithinValueRange()
        {
            NumericParameter parameter = new NumericParameter();
            parameter.Name = "Test";
            parameter.MinimumValue = 0;
            parameter.MaximumValue = 100;
            Exception[] results = parameter.Validate("50");
            Assert.AreEqual(0, results.Length, "Number of exceptions does not match");
        }

        [Test]
        public void ValueBelowValueRange()
        {
            NumericParameter parameter = new NumericParameter();
            parameter.Name = "Test";
            parameter.MinimumValue = 75;
            parameter.MaximumValue = 100;
            Exception[] results = parameter.Validate("50");
            Assert.AreEqual(1, results.Length, "Number of exceptions does not match");
            Assert.AreEqual("Value of 'Test' is less than the minimum allowed (75)", results[0].Message, "Exception message does not match");
        }

        [Test]
        public void ValueAboveValueRange()
        {
            NumericParameter parameter = new NumericParameter();
            parameter.Name = "Test";
            parameter.MinimumValue = 0;
            parameter.MaximumValue = 25;
            Exception[] results = parameter.Validate("50");
            Assert.AreEqual(1, results.Length, "Number of exceptions does not match");
            Assert.AreEqual("Value of 'Test' is more than the maximum allowed (25)", results[0].Message, "Exception message does not match");
        }
    }
}
