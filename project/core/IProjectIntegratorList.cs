using System;
using System.Collections;

namespace ThoughtWorks.CruiseControl.Core
{
	public interface IProjectIntegratorList : IEnumerable
	{
		IProjectIntegrator this[string projectName] { get; }
	}
}
