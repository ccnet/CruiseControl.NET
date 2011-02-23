namespace CruiseControl.Common
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    /// <summary>
    /// A connection to a server.
    /// </summary>
    public class ServerConnection
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerConnection"/> class.
        /// </summary>
        public ServerConnection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerConnection"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        public ServerConnection(string address)
            : this(address, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerConnection"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="binding">The binding.</param>
        public ServerConnection(string address, Binding binding)
        {
            this.Address = address;
            this.Binding = binding;
            if (binding == null)
            {
                this.GenerateBindingFromProtocol();
            }
        }
        #endregion

        #region Public properties
        #region Address
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; set; }
        #endregion

        #region Binding
        /// <summary>
        /// Gets or sets the binding.
        /// </summary>
        /// <value>
        /// The binding.
        /// </value>
        public Binding Binding { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Ping()
        /// <summary>
        /// Attempt to ping the server.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the ping was successful; <c>false</c> otherwise.
        /// </returns>
        public bool Ping()
        {
            try
            {
                var ping = this.PerformChannelOperation(c => c.Ping());
                return ping;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region GenerateBindingFromProtocol()
        /// <summary>
        /// Generates the binding from protocol.
        /// </summary>
        public void GenerateBindingFromProtocol()
        {
            if (string.IsNullOrEmpty(this.Address))
            {
                throw new InvalidOperationException("Cannot generate binding when address is not set");
            }

            var protocolLength = this.Address.IndexOf("://");
            if (protocolLength < 0)
            {
                throw new InvalidOperationException("Unable to find protocol within address");
            }

            var protocol = this.Address.Substring(0, protocolLength).ToLowerInvariant();
            switch (protocol)
            {
                case "http":
                    this.Binding = new BasicHttpBinding();
                    break;

                case "net.tcp":
                    this.Binding = new NetTcpBinding();
                    break;

                default:
                    throw new InvalidOperationException("Unknown protocol: " + protocol);
            }
        }
        #endregion

        #region RetrieveServerName()
        /// <summary>
        /// Retrieves the URN of the server.
        /// </summary>
        /// <returns>
        /// The server URN.
        /// </returns>
        public virtual string RetrieveServerName()
        {
            var serverName = this.PerformChannelOperation(c => c.RetrieveServerName());
            return serverName;
        }
        #endregion

        #region Invoke()
        /// <summary>
        /// Invokes an action.
        /// </summary>
        /// <param name="urn">The URN to invoke the action on.</param>
        /// <param name="action">The action.</param>
        /// <param name="args">The arguments for the action.</param>
        /// <returns>
        /// The result of the action.
        /// </returns>
        public virtual object Invoke(string urn, string action, object args)
        {
            var request = new InvokeArguments
                              {
                                  Action = action,
                                  Data = MessageSerialiser.Serialise(args)
                              };
            var result = this.PerformChannelOperation(c => c.Invoke(urn, request));
            if (result.ResultCode == RemoteResultCode.Success)
            {
                return MessageSerialiser.Deserialise(result.Data);
            }

            throw new RemoteServerException(result.ResultCode, result.LogId);
        }
        #endregion
        #endregion

        #region Private methods
        #region PerformChannelOperation()
        /// <summary>
        /// Generates the channel.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="operation">The operation.</param>
        /// <returns>
        /// The result from the operation.
        /// </returns>
        private TResult PerformChannelOperation<TResult>(Func<ICommunicationsChannel, TResult> operation)
        {
            var address = new EndpointAddress(this.Address);
            var channel = ChannelFactory<ICommunicationsChannel>.CreateChannel(this.Binding, address);
            try
            {
                return operation(channel);
            }
            finally
            {
                var disposable = channel as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }
        #endregion
        #endregion
    }
}
