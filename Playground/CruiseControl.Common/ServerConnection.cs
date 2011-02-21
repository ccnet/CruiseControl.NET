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
            var address = new EndpointAddress(this.Address);
            try
            {
                var channel = ChannelFactory<ICommunicationsChannel>
                    .CreateChannel(this.Binding, address);
                try
                {
                    return channel.Ping();
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
        #endregion
    }
}
