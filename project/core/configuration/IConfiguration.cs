using System;
using System.Collections;

namespace ThoughtWorks.CruiseControl.Core
{
	public interface IConfiguration
	{
		IProjectList Projects { get; }
		IProjectIntegratorList Integrators { get; }
	}
}
