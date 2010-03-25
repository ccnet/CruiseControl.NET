using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	/// <summary>
	/// A class to represent an assembly.
	/// </summary>
    /// <title>Assembly Match</title>
    /// <version>1.4.3</version>
    /// <example>
    /// <code>
    /// &lt;assemblyMatch expr='*.dll' /&gt;
    /// </code>
    /// </example>
	[ReflectorType("assemblyMatch")]
	public class AssemblyMatch
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyMatch"/> class.
        /// </summary>
        public AssemblyMatch()
        {
            this.Expression = string.Empty;
        }

		/// <summary>
		/// The name expression of the assembly, e.g. "*.dll". Masks (? and *) are allowed.
		/// </summary>
        /// <version>1.4.3</version>
        /// <default>n/a</default>
        [ReflectorProperty("expr", Required = true)]
        public string Expression { get; set; }
	}
}
