namespace ThoughtWorks.CruiseControl.Core
{
	public interface ILabeller : ITask
	{
		/// <summary>
		/// Returns the label to apply.
		/// </summary>
		/// <remarks>
		/// There is a known issue with implementations for VSS that return labels that are the same
		/// as a previous label.  See http://confluence.public.thoughtworks.org/display/CCNET/VSS.
		/// </remarks>
		/// <param name="previousLabel">state information for determining the label</param>
		/// <returns>the label to apply</returns>
		string Generate(IIntegrationResult previousLabel);
	}
}
