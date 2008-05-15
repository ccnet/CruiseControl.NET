using System;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("userFilter")]
	public class UserFilter : IModificationFilter
	{
		[ReflectorArray("names")]
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