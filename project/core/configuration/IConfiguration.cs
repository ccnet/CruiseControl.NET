using System;
using System.Collections;

namespace ThoughtWorks.CruiseControl.Core
{
	/// <summary>
	/// The configuration interface for CruiseControl.NET.  Extends <see cref="IEnumerable"/>
	/// and implementations should provide enumeration over projects.
	/// </summary>
	public interface IConfiguration : IEnumerable
	{
		void AddProject(IProject project);
		IProject GetProject(string name);
	}
}
