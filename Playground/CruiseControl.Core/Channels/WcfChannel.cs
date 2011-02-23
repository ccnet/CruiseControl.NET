namespace CruiseControl.Core.Channels
{
    using System;
    using CruiseControl.Common;
    using CruiseControl.Core.Interfaces;
    using NLog;

    /// <summary>
    /// The channel implementation for WCF.
    /// </summary>
    public class WcfChannel
        : ICommunicationsChannel
    {
        #region Private fields
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WcfChannel"/> class.
        /// </summary>
        /// <param name="invoker">The invoker.</param>
        public WcfChannel(IActionInvoker invoker)
        {
            this.Invoker = invoker;
        }
        #endregion

        #region Public properties
        #region Server
        /// <summary>
        /// Gets the invoker.
        /// </summary>
        public IActionInvoker Invoker { get; private set; }
        #endregion
        #endregion

        #region Public methods
        #region Ping()
        /// <summary>
        /// Checks if the service is available.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the service is available; <c>false</c> otherwise.
        /// </returns>
        public bool Ping()
        {
            logger.Debug("Responding to ping");
            return true;
        }
        #endregion

        #region Invoke()
        /// <summary>
        /// Invokes an action on the specified urn.
        /// </summary>
        /// <param name="urn">The URN to invoke the action.</param>
        /// <param name="arguments">The arguments for the action.</param>
        /// <returns>
        /// The results of the action.
        /// </returns>
        public InvokeResult Invoke(string urn, InvokeArguments arguments)
        {
            var logId = Guid.NewGuid();
            logger.Debug("Performing invoke on '{0}' - {1}", urn, logId);
            try
            {
                var result = this.Invoker.Invoke(urn, arguments);
                result.LogId = logId;
                logger.Debug("Invoke completed for '{0}' - {1}", urn, logId);
                return result;
            }
            catch (Exception error)
            {
                logger.ErrorException(
                    "Error happened on invoke for '" + urn +
                    "' - " + logId + ": " + error.Message,
                    error);
                return new InvokeResult
                           {
                               LogId = logId,
                               ResultCode = RemoteResultCode.FatalError
                           };
            }
        }
        #endregion

        #region Query()
        /// <summary>
        /// Queries the specified urn.
        /// </summary>
        /// <param name="urn">The URN to query for actions.</param>
        /// <param name="arguments">The arguments for the query.</param>
        /// <returns>
        /// The allowed actions on the URN.
        /// </returns>
        public QueryResult Query(string urn, QueryArguments arguments)
        {
            var logId = Guid.NewGuid();
            logger.Debug("Performing invoke on '{0}' - {1}", urn, logId);
            try
            {
                var result = this.Invoker.Query(urn, arguments);
                result.LogId = logId;
                logger.Debug("Invoke completed for '{0}' - {1}", urn, logId);
                return result;
            }
            catch (Exception error)
            {
                logger.ErrorException(
                    "Error happened on invoke for '" + urn +
                    "' - " + logId + ": " + error.Message,
                    error);
                return new QueryResult
                           {
                               LogId = logId,
                               ResultCode = RemoteResultCode.FatalError
                           };
            }
        }
        #endregion

        #region RetrieveServerName()
        /// <summary>
        /// Retrieves the name of the server.
        /// </summary>
        /// <returns>
        /// The URN of the server.
        /// </returns>
        public string RetrieveServerName()
        {
            logger.Debug("Retrieving server name");
            var name = this.Invoker.RetrieveServerName();
            return name;
        }
        #endregion
        #endregion
    }
}