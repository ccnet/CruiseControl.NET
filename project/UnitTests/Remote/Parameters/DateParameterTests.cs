namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Parameters
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ThoughtWorks.CruiseControl.Remote.Parameters;
    using ThoughtWorks.CruiseControl.Remote;

    [TestFixture]
    public class DateParameterTests
    {
        [Test]
        public void ConstructorSetsName()
        {
            var name = "newParam";
            var parameter = new DateParameter(name);
            Assert.AreEqual(name, parameter.Name);
        }

        [Test]
        public void SetGetProperties()
        {
            var parameter = new DateParameter();
            var trueValue = new NameValuePair("TrueName", "TrueValue");
            var falseValue = new NameValuePair("FalseName", "FalseValue");
            Assert.IsNull(parameter.AllowedValues);
            Assert.AreEqual(typeof(DateTime), parameter.DataType, "DataType does not match");

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

            var minValue = new DateTime(2010, 1, 1);
            parameter.MinimumValue = minValue;
            Assert.AreEqual(minValue, parameter.MinimumValue);

            var maxValue = new DateTime(2010, 1, 1);
            parameter.MaximumValue = maxValue;
            Assert.AreEqual(maxValue, parameter.MaximumValue);

            var defaultValue = "today";
            parameter.ClientDefaultValue = defaultValue;
            Assert.AreEqual(defaultValue, parameter.ClientDefaultValue);
        }

        [Test]
        public void IsRequiredWithBlank()
        {
            var parameter = new DateParameter();
            parameter.Name = "Test";
            parameter.IsRequired = true;
            var results = parameter.Validate(string.Empty);
            Assert.AreEqual(1, results.Length, "Number of exceptions does not match");
            Assert.AreEqual("Value of 'Test' is required", results[0].Message, "Exception message does not match");
        }

        [Test]
        public void CanGenerateDefault()
        {
            var parameter = new DateParameter();
            parameter.ClientDefaultValue = "today";
            parameter.GenerateClientDefault();
            Assert.AreEqual(DateTime.Today.ToShortDateString(), parameter.ClientDefaultValue);
        }

        [Test]
        public void ConvertHandlesEmptyString()
        {
            var parameter = new DateParameter();
            var actualValue = parameter.Convert(string.Empty);
            Assert.AreEqual(DateTime.Today, actualValue);
        }

        [Test]
        public void ConvertHandlesToday()
        {
            var parameter = new DateParameter();
            var actualValue = parameter.Convert("Today");
            Assert.AreEqual(DateTime.Today, actualValue);
        }

        [Test]
        public void ConvertHandlesAddition()
        {
            var parameter = new DateParameter();
            var actualValue = parameter.Convert("Today+2");
            Assert.AreEqual(DateTime.Today.AddDays(2), actualValue);
        }

        [Test]
        public void ConvertHandlesSubtraction()
        {
            var parameter = new DateParameter();
            var actualValue = parameter.Convert("Today-2");
            Assert.AreEqual(DateTime.Today.AddDays(-2), actualValue);
        }

        [Test]
        public void ConvertHandlesDayOfWeek()
        {
            var parameter = new DateParameter();
            var actualValue = parameter.Convert("dayofweek(3)");
            Assert.AreEqual(DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 3), actualValue);
        }

        [Test]
        public void ConvertHandlesDayOfMonth()
        {
            var parameter = new DateParameter();
            var actualValue = parameter.Convert("dayofmonth(10)");
            Assert.AreEqual(DateTime.Today.AddDays(-DateTime.Today.Day+10), actualValue);
        }

        [Test]
        public void ConvertHandlesDate()
        {
            var parameter = new DateParameter();
            var actualValue = parameter.Convert("2010-01-01");
            Assert.AreEqual(new DateTime(2010, 1, 1), actualValue);
        }
    }
}
