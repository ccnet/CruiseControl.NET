using System.Text;
using System.Text.RegularExpressions;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class ProcessArgumentBuilder
	{
        private static Regex hiddenTextRegex = new Regex("<hide>[^<]*</hide>");
        private readonly StringBuilder builder = new StringBuilder();

        /// <summary>
        /// Add the specified argument to the end of the argument list, separating it from the rest of the list
        /// with a space, formatting the value with the specified format string <i>a la</i>
        /// <see cref="StringBuilder.AppendFormat(string, object[])"/>.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="value">The argument value.</param>
        public void AppendArgument(string format, string value)
		{
			if (string.IsNullOrEmpty(value)) return;

			AppendSpaceIfNotEmpty();
			builder.AppendFormat(format, value);
		}

        /// <summary>
        /// Add the specified argument to the end of the argument list, separating it from the rest of the list
        /// with a space, formatting the value with the specified format string <i>a la</i>
        /// <see cref="StringBuilder.AppendFormat(string, object[])"/>.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="value">The argument value.</param>
        public void AppendHiddenArgument(string format, string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            AppendArgument(format, HideArgument(value));
        }

        /// <summary>
        /// Add the specified argument to the end of the argument list, separating it from the rest of the list
        /// with a space.
        /// </summary>
        /// <param name="value">The argument value.</param>
        public void AppendArgument(string value)
		{
            if (string.IsNullOrEmpty(value)) return;

			AppendSpaceIfNotEmpty();
			builder.Append(value);
		}

        /// <summary>
        /// Add the specified argument to the end of the argument list, separating it from the rest of the list
        /// with a space.
        /// </summary>
        /// <param name="value">The argument value.</param>
        public void AppendHiddenArgument(string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            AppendArgument(HideArgument(value));
        }

        /// <summary>
        /// Add a space to the end of the argument list if it is not empty.
        /// </summary>
		private void AppendSpaceIfNotEmpty()
		{
			if (builder.Length > 0) builder.Append(" ");
		}

        /// <summary>
        /// Add the specified argument to the end of the argument list if the specified condition is true, separating
        /// it from the rest of the list with a space.
        /// </summary>
        /// <param name="condition">True if the method should append, false otherwise.</param>
        /// <param name="value">The argument value.</param>
        public void AppendIf(bool condition, string value)
		{
			if (condition) AppendArgument(value);
		}

        /// <summary>
        /// Add the specified argument to the end of the argument list if the specified condition is true, separating 
        /// it from the rest of the list with a space, formatting the value with the specified format string <i>a la</i>
        /// <see cref="StringBuilder.AppendFormat(string, object[])"/>.
        /// </summary>
        /// <param name="condition">True if the method should append, false otherwise.</param>
        /// <param name="format">The format string.</param>
        /// <param name="argument">The argument value.</param>
        public void AppendIf(bool condition, string format, string argument)
		{
			if (condition) AppendArgument(format, argument);
		}

        /// <summary>
        /// Add the specified text to the end of the argument list exactly as it is.
        /// </summary>
        /// <param name="text">The text to add.</param>
		public void Append(string text)
		{
			builder.Append(text);
		}

        /// <summary>
        /// Add the specified argument to the end of the argument list, separating it from the rest of the list
        /// with a space, separating the name and value with a space, and enquoting the value if the value contains 
        /// any spaces.  If the value is an empty string or null, nothing is appended. 
        /// </summary>
        /// <param name="arg">The name of the argument to add.</param>
        /// <param name="value">The value of the argument to add.</param>
        public void AddArgument(string arg, string value)
		{
			AddArgument(arg, " ", value);
		}

        /// <summary>
        /// Add the specified argument to the end of the argument list, separating it from the rest of the list
        /// with a space, separating the name and value with the specified separator, and enquoting the value 
        /// if the value contains any spaces.  If the value is an empty string or null, nothing is appended. 
        /// </summary>
        /// <param name="arg">The name of the argument to add.</param>
        /// <param name="separator">The separator to place between the name and value.</param>
        /// <param name="value">The value of the argument to add.</param>
        public void AddArgument(string arg, string separator, string value)
		{
            if (string.IsNullOrEmpty(value)) return;
			AppendSpaceIfNotEmpty();

			builder.Append(string.Format("{0}{1}{2}", arg, separator, StringUtil.AutoDoubleQuoteString(value)));
		}

        /// <summary>
        /// Add the specified argument to the end of the argument list, separating it from the rest of the list
        /// with a space, and enquoting it if it contains any spaces.  If the argument is an empty string or null,
        /// nothing is appended. 
        /// </summary>
        /// <param name="value">The argument to add.</param>
		public void AddArgument(string value)
		{
            if (string.IsNullOrEmpty(value)) return;
			AppendSpaceIfNotEmpty();

			builder.Append(StringUtil.AutoDoubleQuoteString(value));			
		}

        /// <summary>
        /// Adds a hidden argument to the end of the argument list. 
        /// </summary>
        /// <param name="arg">The name of the argument to add.</param>
        /// <param name="value">The value of the argument to add.</param>
        public void AddHiddenArgument(string arg, string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            AddArgument(arg, " ", HideArgument(value));
        }

        /// <summary>
        /// Add the specified argument to the end of the argument list, separating it from the rest of the list
        /// with a space, separating the name and value with the specified separator, and enquoting the value 
        /// if the value contains any spaces.  If the value is an empty string or null, nothing is appended. 
        /// </summary>
        /// <param name="arg">The name of the argument to add.</param>
        /// <param name="separator">The separator to place between the name and value.</param>
        /// <param name="value">The value of the argument to add.</param>
        public void AddHiddenArgument(string arg, string separator, string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            AddArgument(arg, separator, HideArgument(value));
        }

        /// <summary>
        /// Add the specified argument to the end of the argument list, separating it from the rest of the list
        /// with a space, and enquoting it if it contains any spaces.  If the argument is an empty string or null,
        /// nothing is appended. 
        /// </summary>
        /// <param name="value">The argument to add.</param>
        public void AddHiddenArgument(string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            AddArgument(HideArgument(value));
        }

        /// <summary>
        /// Return the argument list, converted to a string.
        /// </summary>
        /// <returns>The argument list, converted to a string.</returns>
		public override string ToString()
		{
			return builder.ToString();
		}

        /// <summary>
        /// Marks an argument as hidden.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string HideArgument(string value)
        {
            return string.Format("<hide>{0}</hide>", value);
        }

        /// <summary>
        /// Generates a version of the arguments that can be used in logging.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static string GenerateSanitisedArguments(string arguments)
        {
            var value = hiddenTextRegex.Replace(arguments, (m => new string('#', 5)));
            return value;
        }

        /// <summary>
        /// Generates a version of the arguments that can be used in running an application.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static string GenerateApplicationArguments(string arguments)
        {
            var value = hiddenTextRegex.Replace(arguments, 
                (m => m.Value.Substring(6, m.Value.Length - 13)));
            return value;
        }
    }
}
