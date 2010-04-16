using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Remote.Parameters;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Parameters
{
    [TestFixture]
    public class SelectParameterTests
    {
        [Test]
        public void SetGetProperties()
        {
            SelectParameter parameter = new SelectParameter();
            Assert.AreEqual(typeof(string), parameter.DataType, "DataType does not match");

            parameter.IsRequired = false;
            Assert.AreEqual(false, parameter.IsRequired, "IsRequired does not match");
            parameter.IsRequired = true;
            Assert.AreEqual(true, parameter.IsRequired, "IsRequired does not match");
            parameter.DataValues = new NameValuePair[] { 
                new NameValuePair(string.Empty, "Dev"), 
                new NameValuePair("Test", "Test"), 
                new NameValuePair(null, "Prod") 
            };
            Assert.AreEqual(3, parameter.AllowedValues.Length, "AllowedValues does not match");
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
            SelectParameter parameter = new SelectParameter();
            parameter.Name = "Test";
            parameter.IsRequired = true;
            Exception[] results = parameter.Validate(string.Empty);
            Assert.AreEqual(1, results.Length, "Number of exceptions does not match");
            Assert.AreEqual("Value of 'Test' is required", results[0].Message, "Exception message does not match");
        }

        [Test]
        public void IsAllowedValue()
        {
            SelectParameter parameter = new SelectParameter();
            parameter.Name = "Test";
            parameter.DataValues = new NameValuePair[] { 
                new NameValuePair(string.Empty, "Dev"), 
                new NameValuePair("Test", "Test"), 
                new NameValuePair(null, "Prod") 
            };
            Exception[] results = parameter.Validate("Dev");
            Assert.AreEqual(0, results.Length, "Number of exceptions does not match");
        }

        [Test]
        public void IsNotAllowedValue()
        {
            SelectParameter parameter = new SelectParameter();
            parameter.Name = "Test";
            parameter.DataValues = new NameValuePair[] { 
                new NameValuePair(string.Empty, "Dev"), 
                new NameValuePair("Test", "Test"), 
                new NameValuePair(null, "Prod") 
            };
            Exception[] results = parameter.Validate("QA");
            Assert.AreEqual(1, results.Length, "Number of exceptions does not match");
            Assert.AreEqual("Value of 'Test' is not an allowed value", results[0].Message, "Exception message does not match");
        }

        [Test]
        public void CanGetSetDataValues()
        {
            var parameter = new SelectParameter();
            var dataValues = new NameValuePair[] 
            {
                new NameValuePair("one", "1"),
                new NameValuePair("two", "2"),
                new NameValuePair("three", "3")
            };
            parameter.DataValues = dataValues;
            Assert.AreSame(dataValues, parameter.DataValues);
        }
    }
}
