using System;

namespace ThoughtWorks.CruiseControl.Shared.Services
{
	public interface ICruiseService
	{
		ICruiseResult Run(ICruiseCommand command);
	}
}
