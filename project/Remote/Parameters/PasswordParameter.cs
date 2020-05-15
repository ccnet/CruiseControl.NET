#if !NoReflector
using Exortech.NetReflector;
#endif
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.Remote.Parameters
{
    /// <title>Password Parameter</title>
    /// <version>1.0</version>
    /// <summary>
    /// <para>
    /// This will prompt the user to enter a masked text value when a force build is requested.
    /// </para>
    /// <para>
    /// This parameter can then be used by a dynamic value in a task.
    /// </para>
    /// </summary>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;passwordParameter&gt;
    /// &lt;name&gt;Target&lt;/name&gt;
    /// &lt;/passwordParameter&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;passwordParameter&gt;
    /// &lt;name&gt;Password&lt;/name&gt;
    /// &lt;display&gt;User password&lt;/display&gt;
    /// &lt;description&gt;Enter your account password&lt;/description&gt;
    /// &lt;default&gt;Password123&lt;/default&gt;
    /// &lt;minimum&gt;3&lt;/minimum&gt;
    /// &lt;maximum&gt;18&lt;/maximum&gt;
    /// &lt;required&gt;false&lt;/required&gt;
    /// &lt;/passwordParameter&gt;
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
    /// &lt;passwordParameter&gt;
    /// &lt;name&gt;Password&lt;/name&gt;
    /// &lt;display&gt;User password&lt;/display&gt;
    /// &lt;description&gt;Enter your account password&lt;/description&gt;
    /// &lt;default&gt;Password123&lt;/default&gt;
    /// &lt;minimum&gt;3&lt;/minimum&gt;
    /// &lt;maximum&gt;18&lt;/maximum&gt;
    /// &lt;required&gt;false&lt;/required&gt;
    /// &lt;/passwordParameter&gt;
    /// &lt;/parameters&gt;
    /// &lt;/project&gt;
    /// </code>
    /// </example>
#if !NoReflector
    [ReflectorType("passwordParameter")]
#endif
    [XmlRoot("passwordParameter")]
    [Serializable]
    public class PasswordParameter
        : ParameterBase
    {
        #region Private fields
        private int myMinLength/* = 0*/;
        private int myMaxLength = int.MaxValue;
        private bool myIsRequired/* = false*/;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new instance of a <see cref="PasswordParameter"/>.
        /// </summary>
        public PasswordParameter()
            : base()
        {
        }

        /// <summary>
        /// Initialise an instance of a <see cref="PasswordParameter"/> with a name.
        /// </summary>
        public PasswordParameter(string name)
            : base(name)
        {
        }
        #endregion

        /// <summary>
        /// The minimum allowed length.
        /// </summary>
        /// <version>1.5</version>
        /// <default>0</default>
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
        /// <version>1.5</version>
        /// <default>2147483647</default>
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
        /// The type of the parameter.
        /// </summary>
        public override Type DataType
        {
            get { return typeof(string); }
        }

        #region ParameterType
        /// <summary>
        /// The type of the parameter.
        /// </summary>
        public override string ParameterType
        {
            get { return "Password"; }
        }
        #endregion

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
