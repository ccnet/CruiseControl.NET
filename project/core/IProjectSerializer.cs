namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// 	
    /// </summary>
	public interface IProjectSerializer
	{
        /// <summary>
        /// Serializes the specified project.	
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		string Serialize(IProject project);
        /// <summary>
        /// Deserializes the specified serialized project.	
        /// </summary>
        /// <param name="serializedProject">The serialized project.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		IProject Deserialize(string serializedProject);
	}
}
