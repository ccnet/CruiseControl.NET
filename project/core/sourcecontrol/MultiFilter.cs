using System;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// The MultiFilter can be used with any other filter type.  All filters within a MultiFilter much accept the change in order to return true.
    /// </summary>
    /// <version>1.7</version>
    /// <title>MultiFilter</title>
    /// <example>
    /// <code>
    /// &lt;multiFilter&gt;
    /// &lt;filters&gt;
    /// &lt;pathFilter&gt;
    /// &lt;pattern&gt;$/Kunigunda/ServiceLocator/Sources/Kunigunda.ServiceLocator/AssemblyInfo.cs&lt;/pattern&gt;
    /// &lt;/pathFilter&gt;
    /// &lt;actionFilter&gt;
    /// &lt;actions&gt;
    /// &lt;action&gt;deleted&lt;/action&gt;
    /// &lt;/actions&gt;
    /// &lt;/actionFilter&gt;
    /// &lt;/filters&gt;
    /// &lt;/multiFilter&gt;
    /// </code>
    /// </example>
    [ReflectorType("multiFilter")]
    public class MultiFilter : IModificationFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionFilter"/> class.
        /// </summary>
        public MultiFilter()
        {
            this.Filters = new IModificationFilter[0];
        }

        /// <summary>
        /// The actions to filter.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("filters")]
        public IModificationFilter[] Filters { get; set; }

        /// <summary>
        /// Accepts the specified m.	
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool Accept(Modification m)
        {
            if (this.Filters.Length == 0)
            {
                return false;
            }

            foreach (IModificationFilter mf in Filters)
            {
                if (!mf.Accept(m))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Toes the string.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string ToString()
        {
            return "MultiFilter";
        }
    }
}
