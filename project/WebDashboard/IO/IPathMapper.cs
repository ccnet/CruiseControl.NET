namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	/// <summary>
	/// Used to (at least) wrap HttpContext.Server.MapPath
	/// </summary>
	public interface IPathMapper
	{
		string MapPath(string originalPath);
	}
}
