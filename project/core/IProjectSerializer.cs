namespace ThoughtWorks.CruiseControl.Core
{
	public interface IProjectSerializer
	{
		string Serialize(IProject project);
		Project Deserialize(string serializedProject);
	}
}
