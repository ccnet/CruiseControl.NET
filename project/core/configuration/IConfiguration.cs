using System;
using System.Collections;

namespace ThoughtWorks.CruiseControl.Core
{
	public interface IConfiguration : IEnumerable
	{
		void AddProject(IProject project);
		IProject GetProject(string name);
	}
}
