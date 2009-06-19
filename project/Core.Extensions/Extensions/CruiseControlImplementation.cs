using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ThoughtWorks.CruiseControl.Remote.Messages;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Extensions
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CruiseControlImplementation
        : ICruiseControlContract
    {
        #region Private fields
        private ICruiseServer cruiseServer;
        #endregion 

        #region Constructors
        /// <summary>
        /// Initialises a new instance of this implementation.
        /// </summary>
        /// <param name="server">The associated server.</param>
        public CruiseControlImplementation(ICruiseServer server)
        {
            // Validate the input parameters
            if (server == null) throw new ArgumentNullException("server");

            // Store the parameters that we need for later
            cruiseServer = server;
        }
        #endregion

        #region Methods
        #region ProcessMessage()
        /// <summary>
        /// Processes a message.
        /// </summary>
        /// <param name="action">The action to use.</param>
        /// <param name="message">The request message.</param>
        /// <returns>The response message.</returns>
        public Response ProcessMessage(string action, ServerRequest message)
        {
            var response = cruiseServer.CruiseServerClient.ProcessMessage(action, message);
            return response;
        }
        #endregion
        #endregion
    }
}
