namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public interface IRequest
	{
		string FindParameterStartingWith(string prefix);
		string GetText(string id);
		bool GetChecked(string id);
		int GetInt(string id);
	}
}
