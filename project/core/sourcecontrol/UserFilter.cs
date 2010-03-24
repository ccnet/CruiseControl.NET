using System;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// The UserFilter can be used to filter modifications on the basis of the username that committed the changes.
    /// </summary>
    /// <title>UserFilter</title>
    /// <version>1.0</version>
	[ReflectorType("userFilter")]
	public class UserFilter : IModificationFilter
	{
        /// <summary>
        /// The user names to filter.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("names")]
		public string[] UserNames = new string[0];

		public bool Accept(Modification m)
		{
			return Array.IndexOf(UserNames, m.UserName) >= 0;
		}

        public override string ToString()
        {
            return "UserFilter";
        }
	}
}