using System;

namespace ThoughtWorks.CruiseControl.Shared.Services
{
	public interface ISpecializedCruiseService : ICruiseService
	{
		Type[] SupportedCommandTypes {get;}
	}
}
