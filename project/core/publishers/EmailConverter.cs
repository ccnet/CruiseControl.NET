using System.Text.RegularExpressions;
using Exortech.NetReflector;


namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	[ReflectorType("converter")]
	public class EmailConverter
	{
        private string find;
        private string replace;

        /// <summary>
        /// A regular expression to match against the username and identify parts to be replaced.
        /// </summary>
        [ReflectorProperty("find")]
        public string Find
        {
            get { return find; }
            set { find = value; }
        }

        /// <summary>
        /// A string to replace the matched pattern in the username.
        /// </summary>
        [ReflectorProperty("replace")]
        public string Replace
        {
            get { return replace; }
            set { replace = value; }
        }


		public EmailConverter()
		{
		}


		public EmailConverter(string find, string replace)
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
