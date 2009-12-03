using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// Sends a management task to a CruiseControl.NET server.
    /// </summary>
    /// <title>CruiseServer Control Task</title>
    /// <version>1.5</version>
    /// <example>
    /// <code>
    /// &lt;cruiseServerControl&gt;
    /// &lt;actions&gt;
    /// &lt;controlAction type="StartProject" project="CCNet" /&gt;
    /// &lt;/actions&gt;
    /// &lt;/cruiseServerControl&gt;
    /// </code>
    /// </example>
    [ReflectorType("cruiseServerControl")]
    public class CruiseServerControlTask
        : TaskBase
    {
        #region Public properties
        #region Server
        /// <summary>
        /// The server to send the commands to.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("server", Required = false)]
        public string Server { get; set; }
        #endregion

        #region Actions
        /// <summary>
        /// The actions to perform.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
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
