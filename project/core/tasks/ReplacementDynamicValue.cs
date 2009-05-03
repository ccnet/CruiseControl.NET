using Exortech.NetReflector;
using System;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// Sets a dynamic value using a format string.
    /// </summary>
    [ReflectorType("replacementValue")]
    public class ReplacementDynamicValue
        : IDynamicValue
    {
        private string propertyName;
        private NameValuePair[] parameterValues;
        private string formatValue;

        public ReplacementDynamicValue() { }
        public ReplacementDynamicValue(string format, string property, params NameValuePair[] parameters)
        {
            this.formatValue = format;
            this.propertyName = property;
            this.parameterValues = parameters;
        }

        /// <summary>
        /// The name of the property to set.
        /// </summary>
        [ReflectorProperty("property")]
        public string PropertyName
        {
            get { return propertyName; }
            set { propertyName = value; }
        }

        /// <summary>
        /// The parameters to use.
        /// </summary>
        [ReflectorArray("parameters")]
        public NameValuePair[] Parameters
        {
            get { return parameterValues; }
            set { parameterValues = value; }
        }

        /// <summary>
        /// The default value to use if nothing is set in the parameters.
        /// </summary>
        [ReflectorProperty("format")]
        public string FormatValue
        {
            get { return formatValue; }
            set { formatValue = value; }
        }

        /// <summary>
        /// Applies a dynamc value to an object.
        /// </summary>
        /// <param name="value">The object to apply the value to.</param>
        /// <param name="parameters">The parameters to apply.</param>
        public virtual void ApplyTo(object value, Dictionary<string, string> parameters)
        {
            DynamicValueUtility.PropertyValue property = DynamicValueUtility.FindProperty(value, propertyName);
            if (property != null)
            {
                List<string> actualParameters = new List<string>();
                foreach (NameValuePair parameterName in parameterValues)
                {
                    if (parameters.ContainsKey(parameterName.Name))
                    {
                        actualParameters.Add(parameters[parameterName.Name]);
                    }
                    else
                    {
                        actualParameters.Add(parameterName.Value);
                    }
                }
                string parameterValue = string.Format(this.formatValue, 
                    actualParameters.ToArray());
                property.ChangeProperty(parameterValue);
            }
        }
    }
}
