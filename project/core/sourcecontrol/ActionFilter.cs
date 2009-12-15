using System;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// The ActionFilter can be used to filter modifications on the basis of the type of modification that was committed. Modification types
    /// are specific to each source control provider. Consult each source control provider for the list of actions to filter.
    /// </summary>
    /// <version>1.0</version>
    /// <title>ActionFilter</title>
    /// <example>
    /// <code>
    /// &lt;actionFilter&gt;
    /// &lt;actions&gt;&lt;action&gt;deleted&lt;/action&gt;&lt;/actions&gt;
    /// &lt;/actionFilter&gt;
    /// </code>
    /// </example>
	[ReflectorType("actionFilter")]
	public class ActionFilter : IModificationFilter
	{
        /// <summary>
        /// The actions to filter.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
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