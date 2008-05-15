using System;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("actionFilter")]
	public class ActionFilter : IModificationFilter
	{
		[ReflectorArray("actions")]
		public string[] Actions = new string[0];

		public bool Accept(Modification m)
		{
			return Array.IndexOf(Actions, m.Type) >= 0;
		}

        public override string ToString()
        {
            return "ActionFilter";
        }
	}
}