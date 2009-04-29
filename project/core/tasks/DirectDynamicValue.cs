using Exortech.NetReflector;
using System;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// Directly sets a dynamic value.
    /// </summary>
    [ReflectorType("directValue")]
    public class DirectDynamicValue
        : IDynamicValue
    {
        private string propertyName;
        private string parameterName;
        private string defaultValue;

        public DirectDynamicValue() { }
        public DirectDynamicValue(string parameter, string property)
        {
            propertyName = property;
            parameterName = parameter;
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
        /// The name of the parameter to use.
        /// </summary>
        [ReflectorProperty("parameter")]
        public string ParameterName
        {
            get { return parameterName; }
            set { parameterName = value; }
        }

        /// <summary>
        /// The default value to use if nothing is set in the parameters.
        /// </summary>
        [ReflectorProperty("default")]
        public string DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
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
                string parameterValue = defaultValue;
                if (parameters.ContainsKey(parameterName)) parameterValue = parameters[parameterName];
                property.ChangeProperty(parameterValue);
            }
        }
    }
}
