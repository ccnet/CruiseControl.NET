#if !NoReflector
using Exortech.NetReflector;
#endif
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

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
        private bool myIsRequired = false;
        private string[] myAllowedValues = new string[0];
        
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

        /// <summary>
        /// The type of the parameter.
        /// </summary>
        public override Type DataType
        {
            get { return typeof(string); }
        }

        /// <summary>
        /// An array of allowed values.
        /// </summary>
#if !NoReflector
        [ReflectorArray("allowedValues")]
#endif
        [XmlElement("value")]
        public virtual string[] DataValues
        {
            get { return myAllowedValues; }
            set { myAllowedValues = value; }
        }

        /// <summary>
        /// An array of allowed values.
        /// </summary>
        [XmlIgnore]
        public override string[] AllowedValues
        {
            get { return myAllowedValues; }
        }

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
                foreach (string allowedValue in myAllowedValues)
                {
                    if (allowedValue == value)
                    {
                        isAllowed = true;
                        break;
                    }
                }
                if (!isAllowed) exceptions.Add(GenerateException("Value of '{name}' is not an allowed value"));
            }

            return exceptions.ToArray();
        }
    }
}
