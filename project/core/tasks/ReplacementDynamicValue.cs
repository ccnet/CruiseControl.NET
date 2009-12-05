using Exortech.NetReflector;
using System;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <title>Replacement Dynamic Value</title>
    /// <version>1.5</version>
    /// <summary>
    /// <para>
    /// This will replace any number of parameters into a format string. The format string can also include formats for each parameter.
    /// </para>
    /// </summary>
    /// <example>
    /// <code title="Basic example">
    /// &lt;nant&gt;
    /// &lt;!-- Omitted for brevity --&gt;
    /// &lt;buildArgs&gt;-t:Help&lt;/buildArgs&gt;
    /// &lt;dynamicValues&gt;
    /// &lt;replacementValue property="buildArgs"&gt;
    /// &lt;format&gt;-t:{0}&lt;/format&gt;
    /// &lt;parameters&gt;
    /// &lt;namedValue name="CommandToRun" value="Help"/&gt;
    /// &lt;/parameters&gt;
    /// &lt;/replacementValue&gt;
    /// &lt;/dynamicValues&gt;
    /// &lt;/nant&gt;
    /// </code>
    /// <code title="Shorthand example">
    /// &lt;nant&gt;
    /// &lt;!-- Omitted for brevity --&gt;
    /// &lt;buildArgs&gt;-t:$[CommandToRun|Help]&lt;/buildArgs&gt;
    /// &lt;/nant&gt;
    /// </code>
    /// <code title="Example in context">
    /// &lt;project name="Test Project"&gt;
    /// &lt;sourcecontrol type="svn"&gt;
    /// &lt;!-- Omitted for brevity --&gt;
    /// &lt;/sourcecontrol&gt;
    /// &lt;triggers&gt;
    /// &lt;intervalTrigger /&gt;
    /// &lt;/triggers&gt;
    /// &lt;tasks&gt;
    /// &lt;nant&gt;
    /// &lt;!-- Omitted for brevity --&gt;
    /// &lt;buildArgs&gt;-t:Help&lt;/buildArgs&gt;
    /// &lt;dynamicValues&gt;
    /// &lt;replacementValue property="buildArgs"&gt;
    /// &lt;format&gt;-t:{0}&lt;/format&gt;
    /// &lt;parameters&gt;
    /// &lt;namedValue name="CommandToRun" value="Help"/&gt;
    /// &lt;/parameters&gt;
    /// &lt;/replacementValue&gt;
    /// &lt;/dynamicValues&gt;
    /// &lt;/nant&gt;
    /// &lt;!-- Omitted for brevity --&gt;
    /// &lt;/tasks&gt;
    /// &lt;publishers&gt;
    /// &lt;!-- Omitted for brevity --&gt;
    /// &lt;/publishers&gt;
    /// &lt;parameters&gt;
    /// &lt;!-- Omitted for brevity --&gt;
    /// &lt;/parameters&gt;
    /// &lt;/project&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>
    /// The format string is any standard .NET format string that can be used with string.Format(). The parameters will be replaced in
    /// the order that they are defined in the parameters property.
    /// </para>
    /// </remarks>
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
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("property")]
        public string PropertyName
        {
            get { return propertyName; }
            set { propertyName = value; }
        }

        /// <summary>
        /// The parameters to use.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorArray("parameters")]
        public NameValuePair[] Parameters
        {
            get { return parameterValues; }
            set { parameterValues = value; }
        }

        /// <summary>
        /// The default value to use if nothing is set in the parameters.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
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
        /// <param name="parameterDefinitions">The original parameter definitions.</param>
        public virtual void ApplyTo(object value, Dictionary<string, string> parameters, IEnumerable<ParameterBase> parameterDefinitions)
        {
            DynamicValueUtility.PropertyValue property = DynamicValueUtility.FindProperty(value, propertyName);
            if (property != null)
            {
                var actualParameters = new List<object>();
                foreach (NameValuePair parameterName in parameterValues)
                {
                    object actualValue;
                    if (parameters.ContainsKey(parameterName.Name))
                    {
                        var inputValue = parameters[parameterName.Name];
                        actualValue = DynamicValueUtility.ConvertValue(parameterName.Name, inputValue, parameterDefinitions);
                    }
                    else
                    {
                        actualValue = DynamicValueUtility.ConvertValue(parameterName.Name, parameterName.Value, parameterDefinitions);
                    }
                    actualParameters.Add(actualValue);
                }
                string parameterValue = string.Format(this.formatValue, 
                    actualParameters.ToArray());
                property.ChangeProperty(parameterValue);
            }
        }
    }
}
