using System;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	/// <summary>
	/// Used to (at least) wrap HttpContext.Server.MapPath
	/// </summary>
	public interface IPathMapper
	{
		string MapPath(string originalPath);
	}
}
