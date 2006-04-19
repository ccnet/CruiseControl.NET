namespace ThoughtWorks.CruiseControl.Core
{
	public interface IProjectSerializer
	{
		string Serialize(IProject project);
		IProject Deserialize(string serializedProject);
	}
}
