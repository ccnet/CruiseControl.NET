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
    [Serializable]
    [XmlInclude(typeof(TextParameter))]
    [XmlInclude(typeof(RangeParameter))]
    [XmlInclude(typeof(NumericParameter))]
    public abstract class ParameterBase
    {
        private string myName;
        private string myDisplayName = null;
        private string myDescription = null;
        private string myDefault = null;
        
        #region Constructors
        /// <summary>
        /// Initialise a new instance of a <see cref="ParameterBase"/>.
        /// </summary>
        public ParameterBase()
        {
        }

        /// <summary>
        /// Initialise an instance of a <see cref="ParameterBase"/> with a name.
        /// </summary>
        public ParameterBase(string name)
        {
            myName = name;
        }
        #endregion

        /// <summary>
        /// The name of the parameter.
        /// </summary>
#if !NoReflector
        [ReflectorProperty("name", Required = true)]
#endif
        [XmlAttribute("name")]
        public virtual string Name
        {
            get { return myName; }
            set { myName = value; }
        }

        /// <summary>
        /// The display name of the parameter.
        /// </summary>
#if !NoReflector
        [ReflectorProperty("display", Required = false)]
#endif
        [XmlAttribute("display")]
        public virtual string DisplayName
        {
            get { return myDisplayName ?? myName; }
            set { myDisplayName = value; }
        }

        /// <summary>
        /// The description of the parameter.
        /// </summary>
#if !NoReflector
        [ReflectorProperty("description", Required = false)]
#endif
        [XmlElement("description")]
        public virtual string Description
        {
            get { return myDescription; }
            set { myDescription = value; }
        }

        /// <summary>
        /// The default value to use.
        /// </summary>
#if !NoReflector
        [ReflectorProperty("default", Required = false)]
#endif
        [XmlElement("default")]
        public virtual string DefaultValue
        {
            get { return myDefault; }
            set { myDefault = value; }
        }

        /// <summary>
        /// The type of the parameter.
        /// </summary>
        public abstract Type DataType { get; }

        /// <summary>
        /// An array of allowed values.
        /// </summary>
        public abstract string[] AllowedValues { get; }

        /// <summary>
        /// Validates the parameter.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>Any validation exceptions.</returns>
        public abstract Exception[] Validate(string value);

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
    }
}
