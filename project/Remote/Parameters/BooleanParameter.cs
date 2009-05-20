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
    /// <summary>
    /// A boolean value parameter.
    /// </summary>
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
        /// The view to use when true is selected.
        /// </summary>
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
        /// The view to use when false is selected.
        /// </summary>
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
