using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Label
{
	/// <summary>
	/// Labeller for use with source code control systems that have numbered changes.
	/// This labeller uses the last change number (IIntegrationResult.LastChangeNumber) 
	/// as the build number, with an optional prefix.
	/// </summary>
	/// <remarks>
	/// This code is based on code\label\DefaultLabeller.cs.
	/// </remarks> 
	[ReflectorType("lastChangeLabeller")]
	public class LastChangeLabeller : ILabeller
	{
		/// <summary>
		/// The string to be prepended onto the last change number.
		/// </summary>
		[ReflectorProperty("prefix", Required=false)]
		public string LabelPrefix = string.Empty;

		/// <summary>
		/// Generate a label string from the last change number.
		/// </summary>
		/// <param name="resultFromThisBuild">IntegrationResult object for the current build</param>
		/// <returns>the new label</returns>
		public virtual string Generate(IIntegrationResult resultFromThisBuild)
		{
			int changeNumber = resultFromThisBuild.LastChangeNumber;
			Log.Debug(string.Format("Last change number is \"{0}\"", changeNumber));
			if (changeNumber != 0)
				return LabelPrefix + changeNumber;
			else
				return LabelPrefix + "unknown";
		}
		
		// Nothing seems to use this, but ILabellers are ITasks, and ITasks need a Run().  All the other
		// labellers use this implementation, so we will too.
		public void Run(IIntegrationResult result)
		{
			result.Label = Generate(result);
		}
	}
}