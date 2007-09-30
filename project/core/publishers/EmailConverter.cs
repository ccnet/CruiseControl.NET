using System;
using System.Text.RegularExpressions;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;


namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	[ReflectorType("converter")]
	public class EmailConverter
	{
        private string find;
        private string replace;

        [ReflectorProperty("find")]
        public string Find
        {
            get { return find; }
            set { find = value; }
        }

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

		public string Convert(string username)
		{
			return Regex.Replace(username, find, replace);
		}
       
	}
}
