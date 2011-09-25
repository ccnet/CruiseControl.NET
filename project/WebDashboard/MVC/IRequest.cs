namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public interface IRequest
	{
		string FindParameterStartingWith(string prefix);
		string GetText(string id);
		bool GetChecked(string id);
		int GetInt(string id, int defaultValue);
	    string RawUrl { get; }
		string FileNameWithoutExtension { get; }
		string[] SubFolders { get; }
		string ApplicationPath { get; }
	    string IfModifiedSince { get; }
	    string IfNoneMatch { get; }

        int RefreshInterval { get; set; }
	}
}