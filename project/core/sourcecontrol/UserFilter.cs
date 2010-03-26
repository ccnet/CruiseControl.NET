namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    using System;
    using Exortech.NetReflector;

    /// <summary>
    /// The UserFilter can be used to filter modifications on the basis of the username that committed the changes.
    /// </summary>
    /// <title>UserFilter</title>
    /// <version>1.0</version>
	[ReflectorType("userFilter")]
	public class UserFilter : IModificationFilter
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="UserFilter"/> class.
        /// </summary>
        public UserFilter()
        {
            this.UserNames = new string[0];
        }

        /// <summary>
        /// The user names to filter.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("names")]
        public string[] UserNames { get; set; }

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