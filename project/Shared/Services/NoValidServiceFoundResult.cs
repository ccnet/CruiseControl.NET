using System;

namespace ThoughtWorks.CruiseControl.Shared.Services
{
	/// <summary>
	/// This Result returned by an aggregating service when there are no child services
	/// available to run the requested command type
	/// </summary>
	public class NoValidServiceFoundResult : ICruiseResult
	{
	}
}
