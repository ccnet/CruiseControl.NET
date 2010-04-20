namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Parameters
{
    using System;
    using System.IO;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Remote.Parameters;

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

            var defaultValue = "daDefault";
            var clientValue = "daDefaultForDaClient";
            parameter.DefaultValue = defaultValue;
            Assert.AreEqual(defaultValue, parameter.DefaultValue);
            Assert.AreEqual(defaultValue, parameter.ClientDefaultValue);
            parameter.ClientDefaultValue = clientValue;
            Assert.AreEqual(clientValue, parameter.ClientDefaultValue);
        }

        [Test]
        public void DefaultValueChecksAllowedValues()
        {
            var parameter = new SelectParameter();
            parameter.DataValues = new NameValuePair[] 
            {
                new NameValuePair("name1", "value1"),
                new NameValuePair("name2", "value2")
            };
            parameter.DefaultValue = "value2";
            Assert.AreEqual("name2", parameter.ClientDefaultValue);
        }

        [Test]
        public void ConvertReturnsValueForName()
        {
            var parameter = new SelectParameter();
            parameter.DataValues = new NameValuePair[] 
            {
                new NameValuePair("name1", "value1"),
                new NameValuePair("name2", "value2")
            };
            var value = parameter.Convert("name1");
            Assert.AreEqual("value1", value);
        }

        [Test]
        public void ConvertReturnsOriginalIfNameNotFound()
        {
            var parameter = new SelectParameter();
            parameter.DataValues = new NameValuePair[] 
            {
                new NameValuePair("name1", "value1"),
                new NameValuePair("name2", "value2")
            };
            var value = parameter.Convert("notFound");
            Assert.AreEqual("notFound", value);
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

        [Test]
        public void GenerateClientDefaultLoadsFromAFile()
        {
            var sourceFile = Path.GetTempFileName();
            try
            {
                var lines = new string[] 
                {
                    "Option #1",
                    "Option #2"
                };
                File.WriteAllLines(sourceFile, lines);
                var parameter = new SelectParameter();
                parameter.SourceFile = sourceFile;
                parameter.GenerateClientDefault();
                CollectionAssert.AreEqual(lines, parameter.AllowedValues);
            }
            finally
            {
                File.Delete(sourceFile);
            }
        }
    }
}
