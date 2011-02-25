namespace CruiseControl.Core.Utilities
{
    using System.Collections.Generic;
    using System.Linq;
    using CruiseControl.Core.Interfaces;

    /// <summary>
    /// Handles the generation of arguments for an external process, including handling private data.
    /// </summary>
    public class SecureArguments
        : ISecureData
    {
        #region Private fields
        private readonly List<PrivateArgument> arguments = new List<PrivateArgument>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SecureArguments"/> class with some arguments.
        /// </summary>
        /// <param name="args">The args.</param>
        public SecureArguments(params object[] args)
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
            // Add each argument
            var values = this.arguments
                .Select(argument => argument.ToString(dataMode))
                .Where(arg => arg.Length > 0);
            var stringValue = string.Join(" ", values);
            return stringValue;
        }
        #endregion

        #region Add()
        /// <summary>
        /// Adds a new argument.
        /// </summary>
        /// <param name="value">The argument.</param>
        public void Add(object value)
        {
            this.Add(value, null, false);
        }

        /// <summary>
        /// Adds a new argument with a prefix.
        /// </summary>
        /// <param name="value">The argument.</param>
        /// <param name="prefix">The prefix.</param>
        public void Add(object value, string prefix)
        {
            this.Add(value, prefix, false);
        }

        /// <summary>
        /// Adds a new double-quoted argument with a prefix.
        /// </summary>
        /// <param name="value">The argument.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="doubleQuote">If set to <c>true</c> the argument will be double quoted (if necessary).</param>
        public void Add(object value, string prefix, bool doubleQuote)
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
            this.AddIf(condition, value, null, false);
        }

        /// <summary>
        /// Adds a new conditional argument with a prefix.
        /// </summary>
        /// <param name="condition">If set to <c>true</c> the argument will be added.</param>
        /// <param name="value">The argument.</param>
        /// <param name="prefix">The prefix.</param>
        public void AddIf(bool condition, object value, string prefix)
        {
            this.AddIf(condition, value, prefix, false);
        }

        /// <summary>
        /// Adds a new conditional double-quoted argument with a prefix.
        /// </summary>
        /// <param name="condition">If set to <c>true</c> the argument will be added.</param>
        /// <param name="value">The argument.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="doubleQuote">If set to <c>true</c> the argument will be double quoted (if necessary).</param>
        public void AddIf(bool condition, object value, string prefix, bool doubleQuote)
        {
            if (condition)
            {
                this.Add(value, prefix, doubleQuote);
            }
        }
        #endregion
        #endregion

        #region Operators
        #region implicit
        /// <summary>
        /// Performs an implicit conversion from <see cref="string"/> to <see cref="SecureArguments"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator SecureArguments(string args)
        {
            return new SecureArguments(args);
        }
        #endregion

        #region +()
        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="args">The arguments to append to.</param>
        /// <param name="value">The value.</param>
        /// <returns>The result of the operator.</returns>
        public static SecureArguments operator +(SecureArguments args, object value)
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
                this.prefix = prefix ?? string.Empty;
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
                var privateValue = this.value as ISecureData;
                var actualValue = privateValue == null ?
                    (this.value ?? string.Empty).ToString() :
                    privateValue.ToString(dataMode);
                if (this.doubleQuote)
                {
                    actualValue = (actualValue.StartsWith("\"") ? string.Empty : "\"") +
                                  actualValue +
                                  (actualValue.EndsWith("\"") ? string.Empty : "\"");
                }

                return this.prefix + actualValue;
            }
            #endregion
            #endregion
        }
        #endregion
        #endregion
    }
}
