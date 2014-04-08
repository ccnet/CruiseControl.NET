#if !NoReflector
using Exortech.NetReflector;
#endif
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Parameters
{
    /// <title>Numeric Parameter</title>
    /// <version>1.5</version>
    /// <summary>
    /// <para>
    /// This will prompt the user to enter a numeric value when a force build is requested.
    /// </para>
    /// <para>
    /// This parameter can then be used by a dynamic value in a task.
    /// </para>
    /// </summary>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;numericParameter&gt;
    /// &lt;name&gt;MaxAllowedErrors&lt;/name&gt;
    /// &lt;/numericParameter&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;numericParameter&gt;
    /// &lt;name&gt;MaxAllowedErrors&lt;/name&gt;
    /// &lt;display&gt;Maximum Allowed Errors&lt;/display&gt;
    /// &lt;description&gt;What is the maximum allowed number of unit test errors?&lt;/description&gt;
    /// &lt;default&gt;5&lt;/default&gt;
    /// &lt;minimum&gt;0&lt;/minimum&gt;
    /// &lt;maximum&gt;10&lt;/maximum&gt;
    /// &lt;required&gt;false&lt;/required&gt;
    /// &lt;/numericParameter&gt;
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
    /// &lt;numericParameter&gt;
    /// &lt;name&gt;MaxAllowedErrors&lt;/name&gt;
    /// &lt;display&gt;Maximum Allowed Errors&lt;/display&gt;
    /// &lt;description&gt;What is the maximum allowed number of unit test errors?&lt;/description&gt;
    /// &lt;default&gt;5&lt;/default&gt;
    /// &lt;minimum&gt;0&lt;/minimum&gt;
    /// &lt;maximum&gt;10&lt;/maximum&gt;
    /// &lt;required&gt;false&lt;/required&gt;
    /// &lt;/numericParameter&gt;
    /// &lt;/parameters&gt;
    /// &lt;/project&gt;
    /// </code>
    /// </example>
#if !NoReflector
    [ReflectorType("numericParameter")]
#endif
    [Serializable]
    public class NumericParameter
        : ParameterBase
    {
        private double myMinValue = double.MinValue;
        private double myMaxValue = double.MaxValue;
        private bool myIsRequired/* = false*/;
        
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
        /// <version>1.5</version>
        /// <default>-1.79769e+308</default>
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
        /// <version>1.5</version>
        /// <default>1.79769e+308</default>
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

        /// <summary>
        /// The data type of the parameter.
        /// </summary>
        public override Type DataType
        {
            get { return typeof(double); }
        }

        #region ParameterType
        /// <summary>
        /// The type of the parameter.
        /// </summary>
        public override string ParameterType
        {
            get { return "Numeric"; }
        }
        #endregion

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
