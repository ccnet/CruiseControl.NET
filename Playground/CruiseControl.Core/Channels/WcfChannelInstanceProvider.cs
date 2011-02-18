namespace CruiseControl.Core.Channels
{
    using System.Collections.ObjectModel;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using CruiseControl.Core.Interfaces;

    /// <summary>
    /// generates new service instances against the current server.
    /// </summary>
    public class WcfChannelInstanceProvider
        : IInstanceProvider, IServiceBehavior
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WcfChannel"/> class.
        /// </summary>
        /// <param name="invoker">The invoker.</param>
        public WcfChannelInstanceProvider(IActionInvoker invoker)
        {
            this.Invoker = invoker;
        }
        #endregion

        #region Public properties
        #region Server
        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>
        /// The action invoker.
        /// </value>
        public IActionInvoker Invoker { get; private set; }
        #endregion
        #endregion

        #region Public methods
        #region Validate()
        /// <summary>
        /// Provides the ability to inspect the service host and the service description to confirm that the service can run successfully.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The service host that is currently being constructed.</param>
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            // Do nothing
        }
        #endregion

        #region AddBindingParameters()
        /// <summary>
        /// Provides the ability to pass custom data to binding elements to support the contract implementation.
        /// </summary>
        /// <param name="serviceDescription">The service description of the service.</param>
        /// <param name="serviceHostBase">The host of the service.</param>
        /// <param name="endpoints">The service endpoints.</param>
        /// <param name="bindingParameters">Custom objects to which binding elements have access.</param>
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
            // Do nothing
        }
        #endregion

        #region ApplyDispatchBehavior()
        /// <summary>
        /// Provides the ability to change run-time property values or insert custom extension objects such as error handlers, message or parameter interceptors, security extensions, and other custom extension objects.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The host that is currently being built.</param>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                var channelDispatcher = channelDispatcherBase as ChannelDispatcher;
                if (channelDispatcher != null)
                {
                    foreach (var endpoint in channelDispatcher.Endpoints)
                    {
                        endpoint.DispatchRuntime.InstanceProvider = this;
                    }
                }
            }
        }
        #endregion

        #region GetInstance()
        /// <summary>
        /// Returns a service object given the specified <see cref="T:System.ServiceModel.InstanceContext"/> object.
        /// </summary>
        /// <param name="instanceContext">The current <see cref="T:System.ServiceModel.InstanceContext"/> object.</param>
        /// <returns>
        /// A user-defined service object.
        /// </returns>
        public object GetInstance(InstanceContext instanceContext)
        {
            return new WcfChannel(this.Invoker);
        }

        /// <summary>
        /// Returns a service object given the specified <see cref="T:System.ServiceModel.InstanceContext"/> object.
        /// </summary>
        /// <param name="instanceContext">The current <see cref="T:System.ServiceModel.InstanceContext"/> object.</param>
        /// <param name="message">The message that triggered the creation of a service object.</param>
        /// <returns>
        /// The service object.
        /// </returns>
        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return this.GetInstance(instanceContext);
        }
        #endregion

        #region ReleaseInstance()
        /// <summary>
        /// Called when an <see cref="T:System.ServiceModel.InstanceContext"/> object recycles a service object.
        /// </summary>
        /// <param name="instanceContext">The service's instance context.</param>
        /// <param name="instance">The service object to be recycled.</param>
        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            // Do nothing
        }
        #endregion
        #endregion
    }
}
