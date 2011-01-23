namespace ThoughtWorks.CruiseControl.Core.Util
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Handles the generation of arguments for an external process, including handling private data.
    /// </summary>
    public class PrivateArguments
        : IPrivateData
    {
        #region Private fields
        private List<PrivateArgument> arguments = new List<PrivateArgument>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateArguments"/> class with some arguments.
        /// </summary>
        /// <param name="args">The args.</param>
        public PrivateArguments(params object[] args)
        {
            foreach (var arg in args)
            {
                this.Add(arg);
            }
        }
        #endregion

        #region Public properties
        #region Count
        /// <summary>
        /// Gets the number of arguments.
        /// </summary>
        /// <value>The count of arguments.</value>
        public int Count
        {
            get { return arguments.Count; }
        }
        #endregion
        #endregion

        #region Public methods
        #region ToString()
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// <remarks>
        /// This will return the data in public (hidden) mode.
        /// </remarks>
        public override string ToString()
        {
            return this.ToString(SecureDataMode.Public);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance in the specified data mode.
        /// </summary>
        /// <param name="dataMode">The data mode to use.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(SecureDataMode dataMode)
        {
            var builder = new StringBuilder();

            // Add each argument
            foreach (var argument in this.arguments)
            {
                var arg = argument.ToString(dataMode);
                if (arg.Length > 0)
                {
                    builder.Append(arg + " ");
                }
            }

            // Remove the trailing space
            if (builder.Length > 0)
            {
                builder.Remove(builder.Length - 1, 1);
            }

            return builder.ToString();
        }
        #endregion

        #region Add()
        /// <summary>
        /// Adds a new argument.
        /// </summary>
        /// <param name="value">The argument.</param>
        public void Add(object value)
        {
            this.Add(null, value, false);
        }

        /// <summary>
        /// Adds a new argument with a prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="value">The argument.</param>
        public void Add(string prefix, object value)
        {
            this.Add(prefix, value, false);
        }

        /// <summary>
        /// Adds a new double-quoted argument with a prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="value">The argument.</param>
        /// <param name="doubleQuote">If set to <c>true</c> the argument will be double quoted (if necessary).</param>
        public void Add(string prefix, object value, bool doubleQuote)
        {
            this.arguments.Add(
                new PrivateArgument(prefix, value, doubleQuote));
        }
        #endregion

        #region AddIf()
        /// <summary>
        /// Adds a new conditional argument.
        /// </summary>
        /// <param name="condition">If set to <c>true</c> the argument will be added.</param>
        /// <param name="value">The argument.</param>
        public void AddIf(bool condition, object value)
        {
            if (condition)
            {
                this.Add(null, value, false);
            }
        }

        /// <summary>
        /// Adds a new conditional argument with a prefix.
        /// </summary>
        /// <param name="condition">If set to <c>true</c> the argument will be added.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="value">The argument.</param>
        public void AddIf(bool condition, string prefix, object value)
        {
            if (condition)
            {
                this.Add(prefix, value, false);
            }
        }

        /// <summary>
        /// Adds a new conditional double-quoted argument with a prefix.
        /// </summary>
        /// <param name="condition">If set to <c>true</c> the argument will be added.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="value">The argument.</param>
        /// <param name="doubleQuote">If set to <c>true</c> the argument will be double quoted (if necessary).</param>
        public void AddIf(bool condition, string prefix, object value, bool doubleQuote)
        {
            if (condition)
            {
                this.Add(prefix, value, doubleQuote);
            }
        }
        #endregion

        #region AddQuote()
        /// <summary>
        /// Adds a quoted value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void AddQuote(object value)
        {
            this.Add(null, "\"" + (value == null ? string.Empty : value.ToString()) + "\"", false);
        }

        /// <summary>
        /// Adds a quoted value with a prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="value">The value.</param>
        public void AddQuote(string prefix, object value)
        {
            this.Add(prefix, "\"" + (value == null ? string.Empty : value.ToString()) + "\"", false);
        }
        #endregion
        #endregion

        #region Operators
        #region implicit
        /// <summary>
        /// Performs an implicit conversion from <see cref="String"/> to <see cref="PrivateArguments"/>.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator PrivateArguments(string args)
        {
            return new PrivateArguments(args);
        }
        #endregion

        #region +()
        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="args">The args to append to.</param>
        /// <param name="value">The value.</param>
        /// <returns>The result of the operator.</returns>
        public static PrivateArguments operator +(PrivateArguments args, object value)
        {
            args.Add(value);
            return args;
        }
        #endregion
        #endregion

        #region Private classes
        #region PrivateArgument
        private class PrivateArgument
        {
            #region Private fields
            private readonly object prefix;
            private readonly object value;
            private readonly bool doubleQuote;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="PrivateArgument"/> class.
            /// </summary>
            /// <param name="prefix">The prefix.</param>
            /// <param name="value">The value.</param>
            /// <param name="doubleQuote">if set to <c>true</c> [double quote].</param>
            public PrivateArgument(string prefix, object value, bool doubleQuote)
            {
                this.prefix = prefix;
                this.value = value;
                this.doubleQuote = doubleQuote;
            }
            #endregion

            #region Public methods
            #region ToString()
            /// <summary>
            /// Returns a <see cref="System.String"/> that represents this instance in the specified data mode.
            /// </summary>
            /// <param name="dataMode">The data mode to use.</param>
            /// <returns>
            /// A <see cref="System.String"/> that represents this instance.
            /// </returns>
            public string ToString(SecureDataMode dataMode)
            {
                var privateValue = this.value as IPrivateData;
                var actualValue = privateValue == null ? (this.value ?? string.Empty).ToString() : privateValue.ToString(dataMode);
                if (this.doubleQuote)
                {
                    actualValue = StringUtil.AutoDoubleQuoteString(actualValue);
                }

                return (this.prefix ?? null) + actualValue;
            }
            #endregion
            #endregion
        }
        #endregion
        #endregion
    }
}
