namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	public interface IStarTeamRegExProvider
	{
		string FolderRegEx { get; }
		string FileRegEx { get; }
		string FileHistoryRegEx { get; }
	}
}
