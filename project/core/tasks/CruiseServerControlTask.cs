namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Core.Config;

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
        : TaskBase, IConfigurationValidation
    {
        #region Private fields
        private List<string> cachedProjects = new List<string>();
        #endregion

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

        #region Logger
        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Validate()
        /// <summary>
        /// Checks the internal validation of the item.
        /// </summary>
        /// <param name="configuration">The entire configuration.</param>
        /// <param name="parent">The parent item for the item being validated.</param>
        /// <param name="errorProcesser">The error processer to use.</param>
        public virtual void Validate(IConfiguration configuration, object parent, IConfigurationErrorProcesser errorProcesser)
        {
            if ((this.Actions == null) || (this.Actions.Length == 0))
            {
                errorProcesser.ProcessWarning("This task will not do anything - no actions specified");
            }
        }
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
            // Initialise the task
            var logger = this.Logger ?? new DefaultLogger();
            var factory = this.ClientFactory ?? new CruiseServerClientFactory();
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description)
                ? Description
                : "Performing server actions");
            logger.Info("Performing server actions");

            // Initialise the client and cache the project names
            logger.Debug("Initialising client");
            var client = factory.GenerateClient(Server ?? "tcp://localhost:21234");
            this.CacheProjectNames(logger, client);

            // Perform each action
            var count = 0;
            foreach (var action in Actions ?? new CruiseServerControlTaskAction[0])
            {
                // Get the projects
                var projects = this.ListProjects(action.Project);
                logger.Info("Found " + projects.Count + " project(s) for pattern '" + action.Project + "'");

                // Determine the action to perform
                Action<string> projectAction = RetrieveAction(logger, client, action);

                // Perform the action on each projec
                if (projectAction != null)
                {
                    foreach (var project in projects)
                    {
                        logger.Debug("Sending action to " + project);
                        count++;
                        projectAction(project);
                    }
                }
                else
                {
                    throw new CruiseControlException("Unknown action specified: " + action.Type.ToString());
                }
            }

            logger.Info("Server actions completed: " + count + " command(s) sent");
            return true;
        }
        #endregion
        #endregion

        #region Private methods
        #region CacheProjectNames()
        /// <summary>
        /// Caches the project names.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="client">The client.</param>
        private void CacheProjectNames(ILogger logger, CruiseServerClientBase client)
        {
            logger.Info("Retrieving projects from server");
            this.cachedProjects.Clear();
            var serverProjects = client.GetProjectStatus();
            foreach (var serverProject in serverProjects)
            {
                this.cachedProjects.Add(serverProject.Name);
            }

            logger.Debug(this.cachedProjects.Count + " project(s) retrieved");
        }
        #endregion

        #region RetrieveAction()
        /// <summary>
        /// Retrieves the action to perform.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="client">The client.</param>
        /// <param name="action">The action definition.</param>
        /// <returns>The action to perform.</returns>
        private static Action<string> RetrieveAction(ILogger logger, CruiseServerClientBase client, CruiseServerControlTaskAction action)
        {
            Action<string> projectAction = null;
            switch (action.Type)
            {
                case CruiseServerControlTaskActionType.StartProject:
                    logger.Info("Performing start project action");
                    projectAction = p =>
                    {
                        client.StartProject(p);
                    };
                    break;
                case CruiseServerControlTaskActionType.StopProject:
                    logger.Info("Performing stop project action");
                    projectAction = p =>
                    {
                        client.StopProject(p);
                    };
                    break;
            }
            return projectAction;
        }
        #endregion

        #region ListProjects()
        /// <summary>
        /// Lists the projects.
        /// </summary>
        /// <param name="projectPattern">The project pattern.</param>
        /// <returns>The project names that match the pattern.</returns>
        private IList<string> ListProjects(string projectPattern)
        {
            var list = new List<string>();

            if (projectPattern.Contains("*") || projectPattern.Contains("?"))
            {
                // Check all the projects to see if any match
                var pattern = projectPattern
                    .Replace("*", ".*")
                    .Replace("?", ".");
                var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                foreach (var project in this.cachedProjects)
                {
                    if (regex.IsMatch(project))
                    {
                        list.Add(project);
                    }
                }
            }
            else
            {
                // Just add the pattern
                list.Add(projectPattern);
            }

            return list;
        }
        #endregion
        #endregion
    }
}
