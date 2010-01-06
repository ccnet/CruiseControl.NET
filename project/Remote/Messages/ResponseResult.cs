using System;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// The result code for a response.
    /// </summary>
    [Serializable]
    public enum ResponseResult
    {
        /// <summary>
        /// The server processed the request without any problems.
        /// </summary>
        Success,

        /// <summary>
        /// The server encountered an error while processing and was unable to complete the request.
        /// </summary>
        Failure,

        /// <summary>
        /// The server encountered a problem while processing but was still able to complete the request.
        /// </summary>
        Warning,

        /// <summary>
        /// The result code is unknown (has not been set yet.)
        /// </summary>
        Unknown,
    }
}
