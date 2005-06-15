namespace ThoughtWorks.CruiseControl.Core
{
	public interface ILabeller : ITask
	{
		/// <summary>
		/// Returns the label to use for the current build.
		/// </summary>
		/// <param name="resultFromLastBuild">IntegrationResult from last build used to determine the next label</param>
		/// <returns>the label for the new build</returns>
		string Generate(IIntegrationResult resultFromLastBuild);
	}
}