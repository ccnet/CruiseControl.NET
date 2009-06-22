using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// A task for controlling a CruiseControl.NET server.
    /// </summary>
    [ReflectorType("cruiseServerControl")]
    public class CruiseServerControlTask
        : TaskBase
    {
        #region Public properties
        #region Server
        /// <summary>
        /// The server to send the commands to.
        /// </summary>
        [ReflectorProperty("server", Required = false)]
        public string Server { get; set; }
        #endregion

        #region Actions
        /// <summary>
        /// The actions to perform.
        /// </summary>
        [ReflectorProperty("actions", Required = true)]
        public CruiseServerControlTaskAction[] Actions { get; set; }
        #endregion

        #region ClientFactory
        /// <summary>
        /// The client factory to use.
        /// </summary>
        public ICruiseServerClientFactory ClientFactory { get; set; }
        #endregion
        #endregion

        #region Protected methods
        #region Execute()
        /// <summary>
        /// Sends the specified control tasks to the server.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected override bool Execute(IIntegrationResult result)
        {
            if (ClientFactory == null) ClientFactory = new CruiseServerClientFactory();
            var client = ClientFactory.GenerateClient(Server ?? "tcp://localhost:21234");

            foreach (var action in Actions)
            {
                // Perform each action
                switch (action.Type)
                {
                    case CruiseServerControlTaskActionType.StartProject:
                        client.StartProject(action.Project);
                        break;
                    case CruiseServerControlTaskActionType.StopProject:
                        client.StopProject(action.Project);
                        break;
                }
            }

            return true;
        }
        #endregion
        #endregion
    }
}
