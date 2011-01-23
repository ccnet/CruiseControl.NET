namespace ThoughtWorks.CruiseControl.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Xml;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Remote.Events;

    /// <summary>
    /// A server extension to throttle the number of concurrent integrations.
    /// </summary>
    public class IntegrationRequestThrottleExtension
        : ICruiseServerExtension
    {
        #region Private fields
        private List<string> requests = new List<string>();
        private object updateLock = new object();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationRequestThrottleExtension"/> class.
        /// </summary>
        public IntegrationRequestThrottleExtension()
        {
            this.NumberOfRequestsAllowed = 5;
        }
        #endregion

        #region Public properties
        #region NumberOfRequestsAllowed
        /// <summary>
        /// Gets or sets the number of requests allowed.
        /// </summary>
        /// <value>The number of requests allowed.</value>
        public int NumberOfRequestsAllowed { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Initialise()
        /// <summary>
        /// Initialises the extension.
        /// </summary>
        /// <param name="extensionConfig"></param>
        /// <param name="server">The server that this extension is for.</param>
        public void Initialise(ICruiseServer server, ExtensionConfiguration extensionConfig)
        {
            foreach (var itemEl in extensionConfig.Items ?? new XmlElement[0])
            {
                if (itemEl.Name == "limit")
                {
                    this.NumberOfRequestsAllowed = Convert.ToInt32(
                        itemEl.InnerText, 
                        CultureInfo.CurrentCulture);
                }
            }

            server.IntegrationStarted += server_IntegrationStarted;
            server.IntegrationCompleted += server_IntegrationCompleted;
        }
        #endregion

        #region Start()
        /// <summary>
        /// Starts the extension.
        /// </summary>
        public void Start()
        {
        }
        #endregion

        #region Stop()
        /// <summary>
        /// Stops the extension.
        /// </summary>
        public void Stop()
        {
        }
        #endregion

        #region Abort()
        /// <summary>
        /// Terminates the extension immediately.
        /// </summary>
        public void Abort()
        {
        }
        #endregion
        #endregion

        #region Private methods
        #region server_IntegrationCompleted()
        /// <summary>
        /// Handles the IntegrationCompleted event of the server control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ThoughtWorks.CruiseControl.Remote.Events.IntegrationCompletedEventArgs"/> instance containing the event data.</param>
        private void server_IntegrationCompleted(object sender, IntegrationCompletedEventArgs e)
        {
            lock (updateLock)
            {
                if (requests.Contains(e.ProjectName))
                {
                    requests.Remove(e.ProjectName);
                }
            }
        }
        #endregion

        #region server_IntegrationStarted()
        /// <summary>
        /// Handles the IntegrationStarted event of the server control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ThoughtWorks.CruiseControl.Remote.Events.IntegrationStartedEventArgs"/> instance containing the event data.</param>
        private void server_IntegrationStarted(object sender, IntegrationStartedEventArgs e)
        {
            Log.Debug(string.Format(
                CultureInfo.CurrentCulture,
                "Checking if '{0}' can integrate", 
                e.ProjectName));
            int numberOfRequests = 0;
            string[] currentRequests = new string[0];
            lock (updateLock)
            {
                if (!requests.Contains(e.ProjectName))
                {
                    requests.Add(e.ProjectName);
                }

                numberOfRequests = requests.Count;
                currentRequests = requests.ToArray();
            }

            if (numberOfRequests <= this.NumberOfRequestsAllowed)
            {
                Log.Debug(string.Format(CultureInfo.CurrentCulture,"'{0}' can integrate", e.ProjectName));
                e.Result = IntegrationStartedEventArgs.EventResult.Continue;
            }
            else
            {
                Log.Debug(string.Format(
                    CultureInfo.CurrentCulture,
                    "'{0}' is delayed - number of requests ({1}) has been exceeded ({2})", 
                    e.ProjectName,
                    this.NumberOfRequestsAllowed, 
                    numberOfRequests));
                bool isAllowed = false;
                for (int loop = 0; loop < this.NumberOfRequestsAllowed; loop++)
                {
                    if (currentRequests[loop] == e.ProjectName)
                    {
                        isAllowed = true;
                        break;
                    }
                }

                if (isAllowed)
                {
                    e.Result = IntegrationStartedEventArgs.EventResult.Continue;
                }
                else
                {
                    e.Result = IntegrationStartedEventArgs.EventResult.Delay;
                }
            }
        }
        #endregion
        #endregion
    }
}
