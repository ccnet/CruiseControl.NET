using System;

namespace ThoughtWorks.CruiseControl.Shared.Services
{
	public interface ICruiseServiceConfig
	{
		Type ServiceType { get; }
	}
}
