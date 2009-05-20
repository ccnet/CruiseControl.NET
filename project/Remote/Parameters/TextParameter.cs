#if !NoReflector
using Exortech.NetReflector;
#endif
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.Remote.Parameters
{
    /// <summary>
    /// A text length parameter.
    /// </summary>
#if !NoReflector
    [ReflectorType("textParameter")]
#endif
    [XmlRoot("textParameter")]
    [Serializable]
    public class TextParameter
        : ParameterBase
    {
        #region Private fields
        private int myMinLength = 0;
        private int myMaxLength = int.MaxValue;
        private bool myIsRequired = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new instance of a <see cref="TextParameter"/>.
        /// </summary>
        public TextParameter()
            : base()
        {
        }

        /// <summary>
        /// Initialise an instance of a <see cref="TextParameter"/> with a name.
        /// </summary>
        public TextParameter(string name)
            : base(name)
        {
        }
        #endregion

        /// <summary>
        /// The mimimum allowed length of the parameter.
        /// </summary>
#if !NoReflector
        [ReflectorProperty("minimum", Required = false)]
#endif
        [XmlAttribute("minimum")]
        [DefaultValue(0)]
        public virtual int MinimumLength
        {
            get { return myMinLength; }
            set { myMinLength = value; }
        }

        /// <summary>
        /// The maximum allowed length of the parameter.
        /// </summary>
#if !NoReflector
        [ReflectorProperty("maximum", Required = false)]
#endif
        [XmlAttribute("maximum")]
        [DefaultValue(int.MaxValue)]
        public virtual int MaximumLength
        {
            get { return myMaxLength; }
            set { myMaxLength = value; }
        }

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
        public override string[] AllowedValues
        {
            get { return null; }
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
                if (value.Length < myMinLength)
                {
                    exceptions.Add(
                        GenerateException("Value of '{name}' is less than the minimum length ({0})",
                                myMinLength));
                }
                if (value.Length > myMaxLength)
                {
                    exceptions.Add(
                        GenerateException("Value of '{name}' is more than the maximum length ({0})",
                                myMaxLength));
                }
            }

            return exceptions.ToArray();
        }
    }
}
