#if !NoReflector
using Exortech.NetReflector;
#endif
using System;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Parameters
{
    /// <summary>
    /// Abstract base class for parameters.
    /// </summary>
    /// <title>Parameters</title>
    [Serializable]
    [XmlInclude(typeof(TextParameter))]
    [XmlInclude(typeof(SelectParameter))]
    [XmlInclude(typeof(NumericParameter))]
    [XmlInclude(typeof(DateParameter))]
    [XmlInclude(typeof(BooleanParameter))]
    [XmlInclude(typeof(PasswordParameter))]
    public abstract class ParameterBase
    {
        #region Private fields
        private string myName;
        private string myParameterType = null;
        private string myDisplayName = null;
        private string myDescription = null;
        private string myDefault = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new instance of a <see cref="ParameterBase"/>.
        /// </summary>
        protected ParameterBase()
        {
        }

        /// <summary>
        /// Initialise an instance of a <see cref="ParameterBase"/> with a name.
        /// </summary>
        protected ParameterBase(string name)
        {
            myName = name;
        }
        #endregion

        #region Public properties
        #region Name
        /// <summary>
        /// The name of the parameter.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
#if !NoReflector
        [ReflectorProperty("name", Required = true)]
#endif
        [XmlAttribute("name")]
        public virtual string Name
        {
            get { return myName; }
            set { myName = value; }
        }
        #endregion

        #region DisplayName
        /// <summary>
        /// The display name of the parameter.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
#if !NoReflector
        [ReflectorProperty("display", Required = false)]
#endif
        [XmlAttribute("display")]
        public virtual string DisplayName
        {
            get { return myDisplayName ?? myName; }
            set { myDisplayName = value; }
        }
        #endregion

        #region Description
        /// <summary>
        /// The description of the parameter.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
#if !NoReflector
        [ReflectorProperty("description", Required = false)]
#endif
        [XmlElement("description")]
        public virtual string Description
        {
            get { return myDescription; }
            set { myDescription = value; }
        }
        #endregion

        #region DefaultValue
        /// <summary>
        /// The default value to use.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
#if !NoReflector
        [ReflectorProperty("default", Required = false)]
#endif
        [XmlIgnore]
        public virtual string DefaultValue
        {
            get { return myDefault; }
            set { myDefault = value; }
        }
        #endregion

        #region ClientDefaultValue
        /// <summary>
        /// The default value for the clients to use.
        /// </summary>
        [XmlElement("default")]
        public virtual string ClientDefaultValue
        {
            get { return myDefault; }
            set { myDefault = value; }
        }
        #endregion

        #region DataType
        /// <summary>
        /// The data type of the parameter.
        /// </summary>
        public abstract Type DataType { get; }
        #endregion

        #region ParameterType
        /// <summary>
        /// The type of the parameter.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
#if !NoReflector
        [ReflectorProperty("type", Required = false)]
#endif
        [XmlAttribute("type")]
        public virtual string ParameterType
        {
            get { return myParameterType; }
            set { myParameterType = value; }
        }
        #endregion

        #region AllowedValues
        /// <summary>
        /// An array of allowed values.
        /// </summary>
        [XmlElement("allowedValue")]
        public abstract string[] AllowedValues { get; }
        #endregion
        #endregion

        #region Public methods
        #region Validate()
        /// <summary>
        /// Validates the parameter.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>Any validation exceptions.</returns>
        public abstract Exception[] Validate(string value);
        #endregion

        #region Convert()
        /// <summary>
        /// Converts the parameter into the value to use.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The value to use.</returns>
        public virtual object Convert(string value)
        {
            object actualValue = value;
            if (DataType != typeof(string))
            {
                actualValue = System.Convert.ChangeType(value, DataType);
            }
            return actualValue;
        }
        #endregion

        #region GenerateClientDefault()
        /// <summary>
        /// Updates the client default value.
        /// </summary>
        public virtual void GenerateClientDefault()
        {
        }
        #endregion
        #endregion

        #region Protected methods
        #region GenerateException()
        /// <summary>
        /// Generates a validation exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected virtual Exception GenerateException(string message, params object[] values)
        {
            string actualMessage = message.Replace("{name}", DisplayName);
            Exception exception = new Exception(
                string.Format(actualMessage,
                values));
            return exception;
        }
        #endregion
        #endregion
    }
}
