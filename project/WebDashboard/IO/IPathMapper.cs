namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	/// <summary>
	/// Used to (at least) wrap HttpContext.Server.MapPath
	/// </summary>
	public interface IPathMapper
	{
		string GetLocalPathFromURLPath(string originalPath);
		string GetAbsoluteURLForRelativePath(string relativePath);
	}
}
