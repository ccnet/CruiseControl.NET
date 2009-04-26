using System;
using System.Collections.Generic;
using System.Xml;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Events;

namespace ThoughtWorks.CruiseControl.Core.Extensions
{
    /// <summary>
    /// A server extension to throttle the number of concurrent integrations.
    /// </summary>
    public class IntegrationRequestThrottleExtension
        : ICruiseServerExtension
    {
        #region Private fields
        private int numberOfRequestsAllowed = 5;
        private List<string> requests = new List<string>();
        private object updateLock = new object();
        #endregion

        #region Public methods
        #region Initialise()
        /// <summary>
        /// Initialises the extension.
        /// </summary>
        /// <param name="server">The server that this extension is for.</param>
        public void Initialise(ICruiseServer server, ExtensionConfiguration extensionConfig)
        {
            foreach (XmlElement itemEl in extensionConfig.Items)
            {
                if (itemEl.Name == "limit") numberOfRequestsAllowed = Convert.ToInt32(itemEl.InnerText);
            }

            server.IntegrationStarted += new EventHandler<IntegrationStartedEventArgs>(server_IntegrationStarted);
            server.IntegrationCompleted += new EventHandler<IntegrationCompletedEventArgs>(server_IntegrationCompleted);
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
        private void server_IntegrationCompleted(object sender, IntegrationCompletedEventArgs e)
        {
            lock (updateLock)
            {
                if (requests.Contains(e.ProjectName)) requests.Remove(e.ProjectName);
            }
        }
        #endregion

        #region server_IntegrationStarted()
        private void server_IntegrationStarted(object sender, IntegrationStartedEventArgs e)
        {
            Log.Debug(string.Format("Checking if '{0}' can integrate", e.ProjectName));
            int numberOfRequests = 0;
            string[] currentRequests = new string[0];
            lock (updateLock)
            {
                if (!requests.Contains(e.ProjectName)) requests.Add(e.ProjectName);
                numberOfRequests = requests.Count;
                currentRequests = requests.ToArray();
            }
            if (numberOfRequests <= numberOfRequestsAllowed)
            {
                Log.Debug(string.Format("'{0}' can integrate", e.ProjectName));
                e.Result = IntegrationStartedEventArgs.EventResult.Continue;
            }
            else
            {
                Log.Debug(string.Format("'{0}' is delayed - number of requests ({1}) has been exceeded ({2})", 
                    e.ProjectName, 
                    numberOfRequestsAllowed, 
                    numberOfRequests));
                bool isAllowed = false;
                for (int loop = 0; loop < numberOfRequestsAllowed; loop++)
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
