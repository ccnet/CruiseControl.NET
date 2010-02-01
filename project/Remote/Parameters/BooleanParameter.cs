#if !NoReflector
using Exortech.NetReflector;
#endif
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace ThoughtWorks.CruiseControl.Remote.Parameters
{
    /// <title>Boolean Parameter</title>
    /// <version>1.5</version>
    /// <summary>
    /// <para>
    /// This will prompt the user to enter a boolean value when a force build is requested.
    /// </para>
    /// <para>
    /// This parameter can then be used by a dynamic value in a task.
    /// </para>
    /// </summary>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;booleanParameter&gt;
    /// &lt;name&gt;Target&lt;/name&gt;
    /// &lt;true&gt;Yes&lt;/true&gt;
    /// &lt;false&gt;No&lt;/false&gt;
    /// &lt;/booleanParameter&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;booleanParameter&gt;
    /// &lt;name&gt;Target&lt;/name&gt;
    /// &lt;true name="PROD"&gt;Yes&lt;/true&gt;
    /// &lt;false name="DEV"&gt;No&lt;/false&gt;
    /// &lt;display&gt;Production Build&lt;/display&gt;
    /// &lt;description&gt;Do you want to generate a production build?&lt;/description&gt;
    /// &lt;default&gt;DEV&lt;/default&gt;
    /// &lt;required&gt;false&lt;/required&gt;
    /// &lt;/booleanParameter&gt;
    /// </code>
    /// <code title="Example in Context">
    /// &lt;project name="Test Project"&gt;
    /// &lt;sourcecontrol type="svn"&gt;
    /// &lt;!-- Omitted for brevity --&gt;
    /// &lt;/sourcecontrol&gt;
    /// &lt;triggers&gt;
    /// &lt;intervalTrigger /&gt;
    /// &lt;/triggers&gt;
    /// &lt;tasks&gt;
    /// &lt;!-- Omitted for brevity --&gt;
    /// &lt;/tasks&gt;
    /// &lt;publishers&gt;
    /// &lt;!-- Omitted for brevity --&gt;
    /// &lt;/publishers&gt;
    /// &lt;parameters&gt;
    /// &lt;booleanParameter&gt;
    /// &lt;name&gt;Target&lt;/name&gt;
    /// &lt;true name="PROD"&gt;Yes&lt;/true&gt;
    /// &lt;false name="DEV"&gt;No&lt;/false&gt;
    /// &lt;display&gt;Production Build&lt;/display&gt;
    /// &lt;description&gt;Do you want to generate a production build?&lt;/description&gt;
    /// &lt;default&gt;DEV&lt;/default&gt;
    /// &lt;required&gt;false&lt;/required&gt;
    /// &lt;/booleanParameter&gt;
    /// &lt;/parameters&gt;
    /// &lt;/project&gt;
    /// </code>
    /// </example>
#if !NoReflector
    [ReflectorType("booleanParameter")]
#endif
    [Serializable]
    public class BooleanParameter
        : ParameterBase
    {
        #region Private fields
        private bool myIsRequired = false;
        private NameValuePair trueValue;
        private NameValuePair falseValue;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new instance of a <see cref="BooleanParameter"/>.
        /// </summary>
        public BooleanParameter()
            : base()
        {
        }

        /// <summary>
        /// Initialise an instance of a <see cref="BooleanParameter"/> with a name.
        /// </summary>
        public BooleanParameter(string name)
            : base(name)
        {
        }
        #endregion

        #region Public properties
        #region IsRequired
        /// <summary>
        /// Is the parameter required?
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
#if !NoReflector
        [ReflectorProperty("required", Required = false)]
#endif
        [XmlAttribute("required")]
        [DefaultValue(false)]
        public virtual bool IsRequired
        {
            get { return myIsRequired; }
            set { myIsRequired = value; }
        }
        #endregion

        #region TrueValue
        /// <summary>
        /// The value to use when the parameter is true. If the name attribute is set, then the user will see that as
        /// the selection value. Otherwise the actual value will be displayed.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
#if !NoReflector
        [ReflectorProperty("true", typeof(NameValuePairSerialiserFactory))]
#endif
        [XmlElement("true")]
        public virtual NameValuePair TrueValue
        {
            get { return trueValue; }
            set { trueValue = value; }
        }
        #endregion

        #region FalseValue
        /// <summary>
        /// The value to use when the parameter is false. If the name attribute is set, then the user will see that
        /// as the selection value. Otherwise the actual value will be displayed.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
#if !NoReflector
        [ReflectorProperty("false", typeof(NameValuePairSerialiserFactory))]
#endif
        [XmlElement("false")]
        public virtual NameValuePair FalseValue
        {
            get { return falseValue; }
            set { falseValue = value; }
        }
        #endregion

        #region DataType
        /// <summary>
        /// The type of the parameter.
        /// </summary>
        public override Type DataType
        {
            get { return typeof(string); }
        }
        #endregion

        #region AllowedValues
        /// <summary>
        /// An array of allowed values.
        /// </summary>
        [XmlElement("allowedValue")]
        public override string[] AllowedValues
        {
            get
            {
                return new string[] {
                        string.IsNullOrEmpty(TrueValue.Name) ? TrueValue.Value : TrueValue.Name,
                        string.IsNullOrEmpty(FalseValue.Name) ? FalseValue.Value : FalseValue.Name
                    };
            }
        }
        #endregion
        #endregion

        #region Public methods
        #region Validate()
        /// <summary>
        /// Validates the parameter.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>Any validation exceptions.</returns>
        public override Exception[] Validate(string value)
        {
            List<Exception> exceptions = new List<Exception>();

            if (string.IsNullOrEmpty(value))
            {
                if (IsRequired) exceptions.Add(GenerateException("Value of '{name}' is required"));
            }

            return exceptions.ToArray();
        }
        #endregion

        #region Convert()
        /// <summary>
        /// Converts the parameter into the value to use.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The value to use.</returns>
        public override object Convert(string value)
        {
            var testValue = value;
            var actualValue = value;
            if ((testValue == TrueValue.Name) || (testValue == TrueValue.Value)) actualValue = TrueValue.Value;
            if ((testValue == FalseValue.Name) || (testValue == FalseValue.Value)) actualValue = FalseValue.Value;
            return actualValue;
        }
        #endregion
        #endregion
    }
}
