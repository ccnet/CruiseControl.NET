using System.Text.RegularExpressions;
using Exortech.NetReflector;


namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    /// <summary>
    /// <para>
    /// Matches the username against a regular expression pattern and modifies it according to a specified replacement.
    /// Uses the .NET regular expression language.
    /// </para>
    /// <para>
    /// The find attribute contains a regular expression that is matched against the source control userid. The replace
    /// attribute contains a replacement expression that is used to modify the address. Example : Appending
    /// "@TheCompany.com" to the username
    /// </para>
    /// </summary>
    /// <title>Regular Expression Email Converter</title>
    /// <version>1.0</version>
    /// <example>
    /// <code>
    /// &lt;converters&gt;
    /// &lt;regexConverter find="$" replace="@TheCompany.com" /&gt;
    /// &lt;/converters&gt;
    /// </code>
    /// </example>
    [ReflectorType("regexConverter")]
	public class EmailRegexConverter : IEmailConverter
	{
        private string find;
        private string replace;

        /// <summary>
        /// A regular expression to match against the username and identify parts to be replaced.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("find")]
        public string Find
        {
            get { return find; }
            set { find = value; }
        }

        /// <summary>
        /// A string to replace the matched pattern in the username.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("replace")]
        public string Replace
        {
            get { return replace; }
            set { replace = value; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
		public EmailRegexConverter()
		{
		}

        /// <summary>
        /// Extended constructor
        /// </summary>
        /// <param name="find"></param>
        /// <param name="replace"></param>
		public EmailRegexConverter(string find, string replace)
		{
			this.find = find;
			this.replace = replace;
		}

        /// <summary>
        /// Apply the conversion from username to email address.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>The email address.</returns>
		public string Convert(string username)
		{
		    return Regex.Replace(username, find, replace);
		}
       
	}
}
