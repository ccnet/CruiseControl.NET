namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	// NOT CURRENTLY USED - Just a possible implementation of genericising the MVC - likely to be deleted
	public class ConfiguredActionFactory : IActionFactory
	{
		public static readonly string ACTION_PARAMETER_PREFIX = "_action_";

		private readonly ITypeInstantiator typeInstantiator;
		private readonly IConfiguredActionFactoryConfiguration configuration;

		public ConfiguredActionFactory(IConfiguredActionFactoryConfiguration configuration, ITypeInstantiator typeInstantiator)
		{
			this.configuration = configuration;
			this.typeInstantiator = typeInstantiator;
		}

		public IAction Create(IRequest request)
		{
			return GetInstance(GetTypeSpecification(request.FindParameterStartingWith(ACTION_PARAMETER_PREFIX)));
		}

		private ITypeSpecification GetTypeSpecification(string actionName)
		{
			ITypeSpecification typeSpecification = GetTypeSpecificationAccordingToAction(actionName);
			if (typeSpecification is NullTypeSpecification)
			{
				throw new ActionFactoryConfigurationException(string.Format("Action Name {0} does not map to a configured action type", actionName));
			}
			return typeSpecification;
		}

		private ITypeSpecification GetTypeSpecificationAccordingToAction(string actionName)
		{
			if (actionName == null || actionName == "")
			{
				return configuration.GetDefaultActionTypeSpecification();
			}
			else
			{
				return configuration.GetTypeSpecification(actionName);
			}
		}

		private IAction GetInstance(ITypeSpecification typeSpecification)
		{
			object actionInstance = typeInstantiator.GetInstance(typeSpecification);
			if (! (actionInstance is IAction))
			{
				throw new ActionFactoryConfigurationException(string.Format("Action Type {0} is not an implementation of IAction", actionInstance.GetType()));
			}
			return (IAction) actionInstance;
		}
	}
}
