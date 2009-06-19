using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Extensions
{
    /// <summary>
    /// This class extends CruiseServer and provides a WCF interface to it.
    /// </summary>
    public class WcfServerExtension
        : ICruiseServerExtension, IDisposable
    {
        #region Private fields
        private ServiceHost wcfServiceHost;
        private ICruiseServer cruiseServer;
        #endregion

        #region Public properties
        #region IsRunning
        /// <summary>
        /// Gets whether the service is currently listening for requests.
        /// </summary>
        public bool IsRunning
        {
            get { return (wcfServiceHost.State == CommunicationState.Opened); }
        }
        #endregion
        #endregion

        #region Public methods
        #region Initialise()
        /// <summary>
        /// Initialises the service host to use.
        /// </summary>
        /// <param name="server">The CruiseServer that is initialising this extension.</param>
        /// <param name="extensionConfig">The configuration for the extension.</param>
        public void Initialise(ICruiseServer server, ExtensionConfiguration extensionConfig)
        {
            // Validate the input parameters
            if (server == null) throw new ArgumentNullException("server");

            // Store the parameters that we need for later
            cruiseServer = server;

            // Create a new service host
            wcfServiceHost = new ServiceHost(new CruiseControlImplementation(cruiseServer));
        }
        #endregion

        #region Start()
        /// <summary>
        /// Starts listening for WCF requests.
        /// </summary>
        public void Start()
        {
            // Start the service host
            if ((wcfServiceHost.State != CommunicationState.Opened) &&
                (wcfServiceHost.State != CommunicationState.Opening))
            {
                Log.Info("Opening service host");
                wcfServiceHost.Open();
                Log.Debug("Service host opened");
            }
        }
        #endregion

        #region Stop()
        /// <summary>
        /// Stops listening for WCF requests.
        /// </summary>
        public void Stop()
        {
            // Stop the service host without waiting
            if ((wcfServiceHost.State != CommunicationState.Closed) &&
                (wcfServiceHost.State != CommunicationState.Closing) &&
                (wcfServiceHost.State != CommunicationState.Faulted))
            {
                Log.Info("Closing service host");
                wcfServiceHost.Close();
                Log.Debug("Service host closed");
            }
        }
        #endregion

        #region Abort()
        /// <summary>
        /// Stops listening for WCF requests.
        /// </summary>
        public void Abort()
        {
            // Stop the service host and wait for it to close
            if ((wcfServiceHost.State != CommunicationState.Closed) &&
                (wcfServiceHost.State != CommunicationState.Closing) &&
                (wcfServiceHost.State != CommunicationState.Faulted))
            {
                Log.Info("Aborting service host");
                wcfServiceHost.Abort();
                Log.Debug("Service host aborted");
            }
        }
        #endregion

        #region Dispose()
        /// <summary>
        /// Make sure everything is closed.
        /// </summary>
        public void Dispose()
        {
            Abort();
        }
        #endregion
        #endregion
    }
}
