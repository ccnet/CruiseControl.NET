#if !NoReflector
using Exortech.NetReflector;
#endif
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;
using System.Text.RegularExpressions;

namespace ThoughtWorks.CruiseControl.Remote.Parameters
{
    /// <summary>
    /// A parameter to select from a range of values.
    /// </summary>
#if !NoReflector
    [ReflectorType("selectParameter")]
#endif
    [Serializable]
    public class SelectParameter
        : ParameterBase
    {
        #region Private fields
        private bool myIsRequired = false;
        private NameValuePair[] myAllowedValues = { };
        private string myClientDefault;
        private bool isLoaded;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new instance of a <see cref="SelectParameter"/>.
        /// </summary>
        public SelectParameter()
            : base()
        {
        }

        /// <summary>
        /// Initialise an instance of a <see cref="SelectParameter"/> with a name.
        /// </summary>
        public SelectParameter(string name)
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

        #region DataType
        /// <summary>
        /// The type of the parameter.
        /// </summary>
        public override Type DataType
        {
            get { return typeof(string); }
        }
        #endregion

        #region SourceFile
        /// <summary>
        /// Load the values from a file.
        /// </summary>
#if !NoReflector
        [ReflectorProperty("sourceFile", Required = false)]
#endif
        [XmlIgnore]
        public virtual string SourceFile { get; set; }
        #endregion

        #region DataValues
        /// <summary>
        /// An array of allowed values.
        /// </summary>
#if !NoReflector
        [ReflectorProperty("allowedValues", typeof(NameValuePairListSerialiserFactory), Required = false)]
#endif
        [XmlIgnore]
        public virtual NameValuePair[] DataValues
        {
            get { return myAllowedValues; }
            set
            {
                myAllowedValues = value;
                SetClientDefault();
            }
        }
        #endregion

        #region DefaultValue()
        /// <summary>
        /// The default value to use.
        /// </summary>
#if !NoReflector
        [ReflectorProperty("default", Required = false)]
#endif
        [XmlIgnore]
        public override string DefaultValue
        {
            get { return base.DefaultValue; }
            set
            {
                base.DefaultValue = value;
                SetClientDefault();
            }
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
                var values = new List<string>();
                foreach (var value in myAllowedValues)
                {
                    if (string.IsNullOrEmpty(value.Name))
                    {
                        values.Add(value.Value);
                    }
                    else
                    {
                        values.Add(value.Name);
                    }
                }
                return values.ToArray();
            }
        }
        #endregion

        #region ClientDefaultValue
        /// <summary>
        /// The default value for the clients to use.
        /// </summary>
        [XmlElement("default")]
        public override string ClientDefaultValue
        {
            get { return myClientDefault; }
            set { myClientDefault = value; }
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
            else
            {
                bool isAllowed = false;
                foreach (var allowedValue in myAllowedValues)
                {
                    if ((string.IsNullOrEmpty(allowedValue.Name) && (allowedValue.Value == value)) ||
                        (!string.IsNullOrEmpty(allowedValue.Name) && (allowedValue.Name == value)))
                    {
                        isAllowed = true;
                        break;
                    }
                }
                if (!isAllowed) exceptions.Add(GenerateException("Value of '{name}' is not an allowed value"));
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
            foreach (var valueToCheck in myAllowedValues)
            {
                if ((testValue == valueToCheck.Name) || 
                    (string.IsNullOrEmpty(valueToCheck.Name) && (testValue == valueToCheck.Value)))
                {
                    actualValue = valueToCheck.Value;
                    break;
                }
            }
            return actualValue;
        }
        #endregion

        #region GenerateClientDefault()
        /// <summary>
        /// Updates the client default value.
        /// </summary>
        public override void GenerateClientDefault()
        {
            if (!isLoaded & !string.IsNullOrEmpty(SourceFile))
            {
                isLoaded = true;
                using (var reader = File.OpenText(SourceFile))
                {
                    var currentLine = reader.ReadLine();
                    var values = new List<NameValuePair>();
                    while (currentLine != null)
                    {
                        currentLine = currentLine.Trim();
                        if (currentLine.Length > 0) values.Add(new NameValuePair(null, currentLine));
                        currentLine = reader.ReadLine();
                    }
                    myAllowedValues = values.ToArray();
                }
            }
            else
            {
                isLoaded = true;
            }
        }
        #endregion
        #endregion

        #region Private methods
        #region SetClientDefault()
        /// <summary>
        /// Sets the client default value.
        /// </summary>
        private void SetClientDefault()
        {
            myClientDefault = DefaultValue;
            foreach (var value in myAllowedValues)
            {
                if (!string.IsNullOrEmpty(value.Name) && (DefaultValue == value.Value))
                {
                    myClientDefault = value.Name;
                    break;
                }
            }
        }
        #endregion
        #endregion
    }
}
