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
				if (service is ISpecializedCruiseService)
				{
					if (ServiceSupportsCommand((ISpecializedCruiseService) service, command))
					{
						return service.Run(command);
					}
				}
				else
				{
					ICruiseResult result = service.Run(command);
					if (result != null && !(result is NoValidServiceFoundResult))
					{
						return result;
					}
				}
			}

			return new NoValidServiceFoundResult();
		}

		private bool ServiceSupportsCommand(ISpecializedCruiseService service, ICruiseCommand command)
		{
			foreach (Type type in service.SupportedCommandTypes)
			{
				if (type.Equals(command.GetType()))
				{
					return true;
				}
			}
			return false;
		}
	}
}
