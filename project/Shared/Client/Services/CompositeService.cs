using System;

using ThoughtWorks.CruiseControl.Shared.Services;

namespace ThoughtWorks.CruiseControl.Shared.Client.Services
{
	public class CompositeService : ICruiseService
	{
		ICruiseService[] _services;

		public CompositeService(ICruiseService[] services)
		{
			_services = services;
		}

		public ICruiseResult Run(ICruiseCommand command)
		{
			foreach (ICruiseService service in _services)
			{
				ICruiseResult result = service.Run(command);
				if ( !(result is NoValidServiceFoundResult))
				{
					return result;
				}
			}

			return new NoValidServiceFoundResult();
		}
	}
}
