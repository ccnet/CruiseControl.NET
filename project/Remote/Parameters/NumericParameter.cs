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
    /// A numeric value parameter.
    /// </summary>
#if !NoReflector
    [ReflectorType("numericParameter")]
#endif
    [Serializable]
    public class NumericParameter
        : ParameterBase
    {
        private double myMinValue = double.MinValue;
        private double myMaxValue = double.MaxValue;
        private bool myIsRequired = false;
        
        #region Constructors
        /// <summary>
        /// Initialise a new instance of a <see cref="NumericParameter"/>.
        /// </summary>
        public NumericParameter()
            : base()
        {
        }

        /// <summary>
        /// Initialise an instance of a <see cref="NumericParameter"/> with a name.
        /// </summary>
        public NumericParameter(string name)
            : base(name)
        {
        }
        #endregion

        /// <summary>
        /// The mimimum allowed value of the parameter.
        /// </summary>
#if !NoReflector
        [ReflectorProperty("minimum", Required = false)]
#endif
        [XmlAttribute("minimum")]
        [DefaultValue(double.MinValue)]
        public virtual double MinimumValue
        {
            get { return myMinValue; }
            set { myMinValue = value; }
        }

        /// <summary>
        /// The maximum allowed value of the parameter.
        /// </summary>
#if !NoReflector
        [ReflectorProperty("maximum", Required = false)]
#endif
        [XmlAttribute("maximum")]
        [DefaultValue(double.MaxValue)]
        public virtual double MaximumValue
        {
            get { return myMaxValue; }
            set { myMaxValue = value; }
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
            get { return typeof(double); }
        }

        /// <summary>
        /// An array of allowed values.
        /// </summary>
        public override string[] AllowedValues
        {
            get { return null; }
        }

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
            double actualValue;

            if (string.IsNullOrEmpty(value))
            {
                if (IsRequired) exceptions.Add(GenerateException("Value of '{name}' is required"));
            }
            else
            {
                if (double.TryParse(value, out actualValue))
                {
                    if (actualValue < myMinValue)
                    {
                        exceptions.Add(
                            GenerateException("Value of '{name}' is less than the minimum allowed ({0})",
                                    myMinValue));
                    }
                    if (actualValue > myMaxValue)
                    {
                        exceptions.Add(
                            GenerateException("Value of '{name}' is more than the maximum allowed ({0})",
                                    myMaxValue));
                    }
                }
                else
                {
                    exceptions.Add(GenerateException("Value of '{name}' is not numeric"));
                }
            }

            return exceptions.ToArray();
        }
        #endregion
        #endregion
    }
}
