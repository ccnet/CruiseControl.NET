namespace CruiseControl.Core.Channels
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Windows.Markup;
    using CruiseControl.Common;
    using CruiseControl.Core.Interfaces;
    using NLog;

    /// <summary>
    /// A client channel using WCF.
    /// </summary>
    [ContentProperty("Endpoints")]
    public class Wcf
        : ClientChannel
    {
        #region Private fields
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private ServiceHost host;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Wcf"/> class.
        /// </summary>
        public Wcf()
        {
            this.Endpoints = new List<WcfEndpoint>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Wcf"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public Wcf(string name)
            : this()
        {
            this.Name = name;
        }
        #endregion

        #region Public properties
        #region Name
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The channel name.
        /// </value>
        public string Name { get; set; }
        #endregion

        #region Endpoints
        /// <summary>
        /// Gets the endpoints.
        /// </summary>
        public IList<WcfEndpoint> Endpoints { get; private set; }
        #endregion
        #endregion

        #region Public methods
        #region Validate()
        /// <summary>
        /// Validates this channel.
        /// </summary>
        /// <param name="validationLog">The validation log.</param>
        public override void Validate(IValidationLog validationLog)
        {
            base.Validate(validationLog);
            if (string.IsNullOrEmpty(this.Name))
            {
                validationLog.AddWarning("Channel does not have a name");
            }
        }
        #endregion
        #endregion

        #region Protected methods
        #region OnInitialise()
        /// <summary>
        /// Called when this channel is initialised.
        /// </summary>
        protected override bool OnInitialise()
        {
            if (this.host == null)
            {
                var channelName = this.Name ?? string.Empty;
                logger.Debug("Initialising WCF channel '{0}'", channelName);
                this.host = new ServiceHost(typeof(WcfChannel));
                foreach (var endpoint in this.Endpoints)
                {
                    logger.Info("Adding endpoint {0} using {1} to '{2}'", endpoint.Address, endpoint.GetType().Name, channelName);
                    this.host.AddServiceEndpoint(
                        typeof(ICommunicationsChannel),
                        endpoint.Binding,
                        endpoint.Address);
                }

                logger.Debug("Adding instance provider to '{0}'", channelName);
                this.host.Description.Behaviors.Add(new WcfChannelInstanceProvider(this.Invoker));
                try
                {
                    logger.Info("Opening WCF channel '{0}'", channelName);
                    this.host.Open();
                    return true;
                }
                catch (Exception error)
                {
                    logger.ErrorException(
                        "Unable to open channel '" + channelName + "'",
                        error);
                    this.host = null;
                }
            }

            return false;
        }
        #endregion

        #region OnCleanUp()
        /// <summary>
        /// Called when this channel is cleaned up.
        /// </summary>
        protected override void OnCleanUp()
        {
            if (this.host != null)
            {
                var channelName = this.Name ?? string.Empty;
                try
                {
                    logger.Info("Closing WCF channel '{0}'", channelName);
                    this.host.Close();
                }
                catch (Exception error)
                {
                    logger.WarnException(
                        "An error occurring while closing channel '" + channelName + "'",
                        error);
                }
                finally
                {
                    this.host = null;
                }
            }
        }
        #endregion
        #endregion
    }
}
