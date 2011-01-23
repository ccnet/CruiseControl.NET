
using System;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Events;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// Provides a base implementation of all the events required for <see cref="ICruiseServer"/>.
    /// </summary>
    public abstract class CruiseServerEventsBase
    {
        #region Events
        /// <summary>
        /// A project is starting.
        /// </summary>
        public event EventHandler<CancelProjectEventArgs> ProjectStarting;

        /// <summary>
        /// A project has started.
        /// </summary>
        public event EventHandler<ProjectEventArgs> ProjectStarted;

        /// <summary>
        /// A project is stopping.
        /// </summary>
        public event EventHandler<CancelProjectEventArgs> ProjectStopping;

        /// <summary>
        /// A project has stopped.
        /// </summary>
        public event EventHandler<ProjectEventArgs> ProjectStopped;

        /// <summary>
        /// A force build has been received.
        /// </summary>
        public event EventHandler<CancelProjectEventArgs<string>> ForceBuildReceived;

        /// <summary>
        /// A force build has been processed.
        /// </summary>
        public event EventHandler<ProjectEventArgs<string>> ForceBuildProcessed;

        /// <summary>
        /// An abort build has been received.
        /// </summary>
        public event EventHandler<CancelProjectEventArgs<string>> AbortBuildReceived;

        /// <summary>
        /// An abort build has been processed.
        /// </summary>
        public event EventHandler<ProjectEventArgs<string>> AbortBuildProcessed;

        /// <summary>
        /// A send message has been received.
        /// </summary>
        public event EventHandler<CancelProjectEventArgs<Message>> SendMessageReceived;

        /// <summary>
        /// A send message has been processed.
        /// </summary>
        public event EventHandler<ProjectEventArgs<Message>> SendMessageProcessed;

        /// <summary>
        /// A project integrator is starting an integration.
        /// </summary>
        public event EventHandler<IntegrationStartedEventArgs> IntegrationStarted;

        /// <summary>
        /// A project integrator has completed an integration.
        /// </summary>
        public event EventHandler<IntegrationCompletedEventArgs> IntegrationCompleted;
        #endregion

        #region Event triggers
        #region StartProject
        /// <summary>
        /// Fires the ProjectStarting event.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <returns>Whether this event was canceled or not.</returns>
        protected virtual bool FireProjectStarting(string projectName)
        {
            bool isCanceled = false;
            if (ProjectStarting != null)
            {
                CancelProjectEventArgs args = new CancelProjectEventArgs(projectName);
                ProjectStarting(this, args);
                isCanceled = args.Cancel;
            }
            return isCanceled;
        }

        /// <summary>
        /// Fires the ProjectStarted event.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        protected virtual void FireProjectStarted(string projectName)
        {
            if (ProjectStarted != null)
            {
                ProjectEventArgs args = new ProjectEventArgs(projectName);
                ProjectStarted(this, args);
            }
        }
        #endregion

        #region StopProject
        /// <summary>
        /// Fires the ProjectStopping event.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <returns>Whether this event was canceled or not.</returns>
        protected virtual bool FireProjectStopping(string projectName)
        {
            bool isCanceled = false;
            if (ProjectStopping != null)
            {
                CancelProjectEventArgs args = new CancelProjectEventArgs(projectName);
                ProjectStopping(this, args);
                isCanceled = args.Cancel;
            }
            return isCanceled;
        }

        /// <summary>
        /// Fires the ProjectStopped event.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        protected virtual void FireProjectStopped(string projectName)
        {
            if (ProjectStopped != null)
            {
                ProjectEventArgs args = new ProjectEventArgs(projectName);
                ProjectStopped(this, args);
            }
        }
        #endregion

        #region ForceBuild
        /// <summary>
        /// Fires the ForceBuildReceived event.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <param name="enforcerName">The name of the person forcing the build.</param>
        /// <returns>Whether this event was canceled or not.</returns>
        protected virtual bool FireForceBuildReceived(string projectName, string enforcerName)
        {
            bool isCanceled = false;
            if (ForceBuildReceived != null)
            {
                CancelProjectEventArgs<string> args = new CancelProjectEventArgs<string>(projectName, enforcerName);
                ForceBuildReceived(this, args);
                isCanceled = args.Cancel;
            }
            return isCanceled;
        }

        /// <summary>
        /// Fires the ForceBuildProcessed event.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <param name="enforcerName">The name of the person forcing the build.</param>
        protected virtual void FireForceBuildProcessed(string projectName, string enforcerName)
        {
            if (ForceBuildProcessed != null)
            {
                ProjectEventArgs<string> args = new ProjectEventArgs<string>(projectName, enforcerName);
                ForceBuildProcessed(this, args);
            }
        }
        #endregion

        #region AbortBuild
        /// <summary>
        /// Fires the AbortBuildReceived event.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <param name="enforcerName">The name of the person aborting the build.</param>
        /// <returns>Whether this event was canceled or not.</returns>
        protected virtual bool FireAbortBuildReceived(string projectName, string enforcerName)
        {
            bool isCanceled = false;
            if (AbortBuildReceived != null)
            {
                CancelProjectEventArgs<string> args = new CancelProjectEventArgs<string>(projectName, enforcerName);
                AbortBuildReceived(this, args);
                isCanceled = args.Cancel;
            }
            return isCanceled;
        }

        /// <summary>
        /// Fires the AbortBuildProcessed event.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <param name="enforcerName">The name of the person aborting the build.</param>
        protected virtual void FireAbortBuildProcessed(string projectName, string enforcerName)
        {
            if (AbortBuildProcessed != null)
            {
                ProjectEventArgs<string> args = new ProjectEventArgs<string>(projectName, enforcerName);
                AbortBuildProcessed(this, args);
            }
        }
        #endregion

        #region SendMessage
        /// <summary>
        /// Fires the SendMessageReceived event.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <param name="message">The message to be sent.</param>
        /// <returns>Whether this event was canceled or not.</returns>
        protected virtual bool FireSendMessageReceived(string projectName, Message message)
        {
            bool isCanceled = false;
            if (SendMessageReceived != null)
            {
                CancelProjectEventArgs<Message> args = new CancelProjectEventArgs<Message>(projectName, message);
                SendMessageReceived(this, args);
                isCanceled = args.Cancel;
            }
            return isCanceled;
        }

        /// <summary>
        /// Fires the SendMessageProcessed event.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <param name="message">The message that was sent.</param>
        protected virtual void FireSendMessageProcessed(string projectName, Message message)
        {
            if (SendMessageProcessed != null)
            {
                ProjectEventArgs<Message> args = new ProjectEventArgs<Message>(projectName, message);
                SendMessageProcessed(this, args);
            }
        }
        #endregion

        #region Integration
        /// <summary>
        /// Fires the IntegrationStarted event.
        /// </summary>
        /// <param name="request">The integration request.</param>
        /// <param name="projectName"></param>
        protected virtual IntegrationStartedEventArgs.EventResult FireIntegrationStarted(IntegrationRequest request, string projectName)
        {
            IntegrationStartedEventArgs.EventResult result = IntegrationStartedEventArgs.EventResult.Continue;
            if (IntegrationStarted != null)
            {
                IntegrationStartedEventArgs args = new IntegrationStartedEventArgs(request, projectName);
                IntegrationStarted(this, args);
                result = args.Result;
            }
            return result;
        }

        /// <summary>
        /// Fires the IntegrationCompleted event.
        /// </summary>
        /// <param name="request">The integration request.</param>
        /// <param name="projectName">The name of the project.</param>
        /// <param name="status">The outcome of the integration.</param>
        protected virtual void FireIntegrationCompleted(IntegrationRequest request, string projectName, IntegrationStatus status)
        {
            if (IntegrationCompleted != null)
            {
                IntegrationCompletedEventArgs args = new IntegrationCompletedEventArgs(request, projectName, status);
                IntegrationCompleted(this, args);
            }
        }
        #endregion
        #endregion
    }
}
