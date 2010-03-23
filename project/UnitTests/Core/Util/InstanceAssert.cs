namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;

    public static class InstanceAssert
    {
        #region Public methods
        #region PropertiesAreEqual()
        /// <summary>
        /// Checks that the properties on two objects are the same.
        /// </summary>
        /// <param name="expected">The expected instance.</param>
        /// <param name="actual">The actual instance.</param>
        /// <param name="properties">The properties to check.</param>
        public static void PropertiesAreEqual(object expected, object actual, params string[] properties)
        {
            // Sanity check - make sure we don't get any garbage input
            Assert.IsNotNull(expected, "Unable to check properties - expected value is null");
            Assert.IsNotNull(actual, "Unable to check properties - actual value is null");
            var type = expected.GetType();
            Assert.IsInstanceOf(type, actual, "Unable to check properties - expected and actual types do not match");

            // Check each property to make sure they are the same
            var count = 0;
            var builder = new StringBuilder();
            foreach (var property in properties)
            {
                var propertyInfo = type.GetProperty(property);
                var expectedValue = propertyInfo.GetValue(expected, new object[0]);
                var actualValue = propertyInfo.GetValue(actual, new object[0]);
                if (!object.Equals(expectedValue, actualValue))
                {
                    count++;
                    builder.Append(
                        Environment.NewLine + 
                        "> " + propertyInfo.Name +
                        " does not match - expected " + FormatValue(expectedValue) +
                        ", found " + FormatValue(actualValue));
                }
            }

            // Fail the assertion if there are any mismatches
            if (count > 0)
            {
                var message = (count == 1 ? "1 property does" : count.ToString() + " properties do") + 
                    " not match";
                Assert.Fail(message + builder.ToString());
            }
        }
        #endregion
        #endregion

        #region Private methods
        #region FormatValue
        /// <summary>
        /// Formats the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A string containing the formatted value.</returns>
        private static string FormatValue(object value)
        {
            if (value == null)
            {
                return "<null>";
            }
            else if (value is string)
            {
                return "\"" + value.ToString() + "\"";
            }
            else
            {
                return value.ToString();
            }
        }
        #endregion
        #endregion
    }
}
