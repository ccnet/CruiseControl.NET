using System;
using System.Collections;

namespace ThoughtWorks.CruiseControl.Core
{
	public interface IProjectList : IEnumerable
	{
		IProject this[string projectName] { get; }
	}
}
