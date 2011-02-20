namespace CruiseControl.Common
{
    using System.ServiceModel;

    /// <summary>
    /// Defines the contract for a communications channel.
    /// </summary>
    [ServiceContract]
    public interface ICommunicationsChannel
    {
        #region Public methods
        #region Ping()
        /// <summary>
        /// Checks if the service is available.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the service is available; <c>false</c> otherwise.
        /// </returns>
        [OperationContract]
        bool Ping();
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
        [OperationContract]
        InvokeResult Invoke(string urn, InvokeArguments arguments);
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
        [OperationContract]
        QueryResult Query(string urn, QueryArguments arguments);
        #endregion
        #endregion
    }
}
