namespace ThoughtWorks.CruiseControl.Core
{
	public interface IProjectSerializer
	{
		string Serialize(Project project);
		Project Deserialize(string serializedProject);
	}
}
