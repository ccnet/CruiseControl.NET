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
    /// <title>Date Parameter</title>
    /// <version>1.5</version>
    /// <summary>
    /// <para>
    /// This will prompt the user to enter a date value when a force build is requested.
    /// </para>
    /// <para>
    /// This parameter can then be used by a dynamic value in a task.
    /// </para>
    /// </summary>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;dateParameter&gt;
    /// &lt;name&gt;CutOffDate&lt;/name&gt;
    /// &lt;/dateParameter&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;dateParameter&gt;
    /// &lt;name&gt;CutOffDate&lt;/name&gt;
    /// &lt;display&gt;Cut Off Date&lt;/display&gt;
    /// &lt;description&gt;What is the cut-off date for changes?&lt;/description&gt;
    /// &lt;default&gt;today&lt;/default&gt;
    /// &lt;minimum&gt;1-Jan-2000&lt;/minimum&gt;
    /// &lt;maximum&gt;31-Dec-2100&lt;/maximum&gt;
    /// &lt;required&gt;false&lt;/required&gt;
    /// &lt;/dateParameter&gt;
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
    /// &lt;dateParameter&gt;
    /// &lt;name&gt;CutOffDate&lt;/name&gt;
    /// &lt;display&gt;Cut Off Date&lt;/display&gt;
    /// &lt;description&gt;What is the cut-off date for changes?&lt;/description&gt;
    /// &lt;default&gt;today&lt;/default&gt;
    /// &lt;minimum&gt;1-Jan-2000&lt;/minimum&gt;
    /// &lt;maximum&gt;31-Dec-2100&lt;/maximum&gt;
    /// &lt;required&gt;false&lt;/required&gt;
    /// &lt;/dateParameter&gt;
    /// &lt;/parameters&gt;
    /// &lt;/project&gt;
    /// </code>
    /// </example>
#if !NoReflector
    [ReflectorType("dateParameter")]
#endif
    [Serializable]
    public class DateParameter
        : ParameterBase
    {
        #region Private fields
        private static Regex dayOfWeekRegex = new Regex("(?<=dayofweek\\()[0-6](?=\\))", RegexOptions.IgnoreCase);
        private static Regex dayOfMonthRegex = new Regex("(?<=dayofmonth\\()[1-9][0-9]?(?=\\))", RegexOptions.IgnoreCase);
        private DateTime myMinValue = DateTime.MinValue;
        private DateTime myMaxValue = DateTime.MaxValue;
        private bool myIsRequired = false;
        private string myClientDefault;
        #endregion
        
        #region Constructors
        /// <summary>
        /// Initialise a new instance of a <see cref="DateParameter"/>.
        /// </summary>
        public DateParameter()
            : base()
        {
        }

        /// <summary>
        /// Initialise an instance of a <see cref="DateParameter"/> with a name.
        /// </summary>
        public DateParameter(string name)
            : base(name)
        {
        }
        #endregion

        #region Public properties
        #region MinimumValue
        /// <summary>
        /// The mimimum allowed value of the parameter.
        /// </summary>
        /// <version>1.5</version>
        /// <default>1-Jan-0000</default>
#if !NoReflector
        [ReflectorProperty("minimum", Required = false)]
#endif
        [XmlAttribute("minimum")]
        public virtual DateTime MinimumValue
        {
            get { return myMinValue; }
            set { myMinValue = value; }
        }
        #endregion

        #region MaximumValue
        /// <summary>
        /// The maximum allowed value of the parameter.
        /// </summary>
        /// <version>1.5</version>
        /// <default>31-Dev-9999</default>
#if !NoReflector
        [ReflectorProperty("maximum", Required = false)]
#endif
        [XmlAttribute("maximum")]
        public virtual DateTime MaximumValue
        {
            get { return myMaxValue; }
            set { myMaxValue = value; }
        }
        #endregion

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

        #region DataType
        /// <summary>
        /// The type of the parameter.
        /// </summary>
        public override Type DataType
        {
            get { return typeof(DateTime); }
        }
        #endregion

        #region AllowedValues
        /// <summary>
        /// An array of allowed values.
        /// </summary>
        public override string[] AllowedValues
        {
            get { return null; }
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
            DateTime actualValue;

            if (string.IsNullOrEmpty(value))
            {
                if (IsRequired) exceptions.Add(GenerateException("Value of '{name}' is required"));
            }
            else
            {
                if (DateTime.TryParse(value, out actualValue))
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
                    exceptions.Add(GenerateException("Value of '{name}' is not date"));
                }
            }

            return exceptions.ToArray();
        }
        #endregion

        #region GenerateClientDefault()
        /// <summary>
        /// Updates the client default value.
        /// </summary>
        public override void GenerateClientDefault()
        {
            SetClientDefault();
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
            var actualValue = CalculateDate(value);
            return actualValue;
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
            myClientDefault = CalculateDate(DefaultValue).ToShortDateString();
        }
        #endregion

        #region CalculateDate()
        private DateTime CalculateDate(string value)
        {
            DateTime date;
            if (DefaultValue.StartsWith("today", StringComparison.CurrentCultureIgnoreCase))
            {
                date = CalculateOperation(value.Substring(5), DateTime.Today);
            }
            else if (dayOfWeekRegex.IsMatch(value))
            {
                var day = dayOfWeekRegex.Match(value).Value;
                var diff = (int)DateTime.Today.DayOfWeek;
                date = DateTime.Today.AddDays(System.Convert.ToInt32(day) - diff);
                date = CalculateOperation(value.Substring(12), date);
            }
            else if (dayOfMonthRegex.IsMatch(value))
            {
                var day = dayOfMonthRegex.Match(value);
                date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, System.Convert.ToInt32(day.Value));
                date = CalculateOperation(value.Substring(day.Value.Length + 12), date);
            }
            else
            {
                date = DateTime.Parse(value);
            }
            return date;
        }
        #endregion

        #region CalculateToday()
        private DateTime CalculateOperation(string operation, DateTime baseDate)
        {
            DateTime date = baseDate;
            if (!string.IsNullOrEmpty(operation))
            {
                var number = System.Convert.ToInt32(operation.Substring(1));
                var op = operation.Substring(0, 1);
                switch (op)
                {
                    case "+":
                        date = date.AddDays(number);
                        break;
                    case "-":
                        date = date.AddDays(-number);
                        break;
                    default:
                        throw new InvalidOperationException("Unknown operation: '" + op + "'");
                }
            }
            return date;
        }
        #endregion
        #endregion
    }
}
