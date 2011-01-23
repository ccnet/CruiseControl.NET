namespace ThoughtWorks.CruiseControl.UnitTests.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Xml;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Remote.Events;
    using ThoughtWorks.CruiseControl.Remote.Messages;

    /// <summary>
    /// Test harness for running integration tests.
    /// </summary>
    /// <remarks>
    /// This harness provides the framework for running integration tests, including both initialisation and termination of all
    /// the objects, files, folders, etc.
    /// </remarks>
    public class CruiseServerHarness
        : IDisposable
    {
        #region Private fields
        private readonly string configFile;
        private readonly List<string> projects;
        private readonly string[] stateFiles;
        private readonly string[] buildFolders;
        private readonly CruiseServerFactory factory;
        private readonly ICruiseServer server;
        private readonly ManualResetEvent[] completionEvents;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CruiseServerHarness"/> class.
        /// </summary>
        /// <param name="projects">The projects.</param>
        public CruiseServerHarness(XmlDocument configuration, params string[] projects)
        {
            // Initialise the default values
            this.TimeoutLength = new TimeSpan(0, 5, 0);

            // Initialise the configuration file (this needs to be in the physical directory since CC.NET reads from the directory)
            var workingFolder = Environment.CurrentDirectory;
            this.configFile = Path.Combine(workingFolder, "ScenarioTests.xml");
            configuration.Save(configFile);

            // Initialise the projects (state files, build folders, etc.)
            this.projects = new List<string>(projects);
            this.stateFiles = projects.Select(p => Path.Combine(workingFolder, p + ".state")).ToArray();
            this.buildFolders = projects.Select(p => Path.Combine(workingFolder, Path.Combine("ScenarioTests", p))).ToArray();

            // Initialise the server instance
            this.factory = new CruiseServerFactory();
            this.server = this.factory.Create(true, this.configFile);

            // Initialise the events
            this.completionEvents = new ManualResetEvent[projects.Length];
            for (var loop = 0; loop < this.completionEvents.Length; loop++)
            {
                this.completionEvents[loop] = new ManualResetEvent(false);
            }

            // Handle integration completed
            this.server.IntegrationCompleted += (o, e) =>
            {
                if (this.IntegrationCompleted != null)
                {
                    this.IntegrationCompleted(o, e);
                }

                // Do this last - this ensures that any processing in the event handler has completed (otherwise strange errors can occur due to timing issues)
                this.FindCompletionEvent(e.ProjectName).Set();
            };
        }
        #endregion

        #region Public properties
        #region Server
        /// <summary>
        /// Gets the server.
        /// </summary>
        /// <value>The server.</value>
        public ICruiseServer Server
        {
            get { return this.server; }
        }
        #endregion

        #region CompletionEvents
        /// <summary>
        /// Gets the completion events.
        /// </summary>
        /// <value>The completion events.</value>
        public ManualResetEvent[] CompletionEvents
        {
            get { return this.completionEvents; }
        }
        #endregion

        #region TimeoutLength
        /// <summary>
        /// Gets or sets the length of the timeout.
        /// </summary>
        /// <value>The length of the timeout.</value>
        public TimeSpan TimeoutLength { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region TriggerBuildAndWait()
        /// <summary>
        /// Triggers a build on the server and waits for the build to complete.
        /// </summary>
        /// <param name="projectName">The name of the project to build.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The status of the project.</returns>
        public ProjectStatus TriggerBuildAndWait(string projectName, params NameValuePair[] parameters)
        {
            // Generate the request
            var completionEvent = this.FindCompletionEvent(projectName);
            var request = new BuildIntegrationRequest
            {
                ProjectName = projectName,
                BuildValues = new List<NameValuePair>(parameters)
            };

            // Attempt to force the build
            ValidateResponse(this.server.ForceBuild(request));
            
            // Wait for the build to complete (or time-out)
            if (!completionEvent.WaitOne(this.TimeoutLength, false))
            {
                // Attempt to cancel the build and tell the caller that we timed-out
                ValidateResponse(this.server.AbortBuild(request));
                throw new HarnessException("Build did not complete within the time-out period");
            }

            // Return the project status
            var status = ValidateResponse(this.server.GetProjectStatus(request));
            return status.Projects.Single(p => p.Name == projectName);
        }
        #endregion

        #region FindCompletionEvent()
        /// <summary>
        /// Finds the completion event for a project.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <returns>The completion event for the project.</returns>
        public ManualResetEvent FindCompletionEvent(string projectName)
        {
            var projectIndex = this.projects.IndexOf(projectName);
            return this.completionEvents[projectIndex];
        }
        #endregion

        #region Dispose()
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Delete the state files
            foreach (var state in this.stateFiles.Where(s => File.Exists(s)))
            {
                File.Delete(state);
            }

            // Delete the build folders
            foreach (var build in this.buildFolders.Where(f => Directory.Exists(f)))
            {
                Directory.Delete(build, true);
            }

            // Delete the configuration
            if (File.Exists(this.configFile))
            {
                File.Delete(this.configFile);
            }

            // Make sure the server is stopped
            this.server.Stop();
            this.server.WaitForExit();
            this.server.Dispose();
        }
        #endregion
        #endregion

        #region Public events
        #region IntegrationCompleted
        /// <summary>
        /// Occurs when an integration has completed.
        /// </summary>
        public event EventHandler<IntegrationCompletedEventArgs> IntegrationCompleted;
        #endregion
        #endregion

        #region Private methods
        #region ValidateResponse()
        /// <summary>
        /// Validates a response.
        /// </summary>
        /// <param name="response">The response.</param>
        private static TResponse ValidateResponse<TResponse>(TResponse response)
            where TResponse : Response
        {
            if (response.Result != ResponseResult.Success)
            {
                throw new HarnessException("Unable to trigger build");
            }

            return response;
        }
        #endregion
        #endregion
    }
}
