using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	/// <summary>
	/// A class to represent an assembly.
	/// </summary>
	[ReflectorType("assemblyMatch")]
	public class AssemblyMatch
	{
		/// <summary>
		/// The name expression of the assembly, e.g. "*.dll".
		/// Masks (? and *) are allowed.
		/// </summary>
		[ReflectorProperty("expr", Required = true)]
		public string Expression = string.Empty;
	}
}
