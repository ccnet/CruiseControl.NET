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
        /// Initializes a new instance of the <see cref="ActionFilter"/> class.
        /// </summary>
        public ActionFilter()
        {
            this.Actions = new string[0];
        }

        /// <summary>
        /// The actions to filter.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("actions")]
        public string[] Actions { get; set; }

        /// <summary>
        /// Accepts the specified m.	
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public bool Accept(Modification m)
		{
			return Array.IndexOf(Actions, m.Type) >= 0;
		}

        /// <summary>
        /// Toes the string.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string ToString()
        {
            return "ActionFilter";
        }
	}
}