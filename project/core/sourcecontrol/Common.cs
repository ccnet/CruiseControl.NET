namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{

    /// <summary>
    /// Class holding common definitions used by all source controls
    /// </summary>
    public class Common
    {
        /// <summary>
        /// Ways of handling source control errors
        /// </summary>
        public enum SourceControlErrorHandlingPolicy
        {
            /// <summary>
            /// A build log is made on every failure
            /// </summary>
            ReportEveryFailure,
            /// <summary>
            /// A build log is only made once when the amount of consecutive build failures reaches the retry amount
            /// </summary>
            ReportOnRetryAmount,
            /// <summary>
            /// A build log is made every time when the amount of consecutive build failures reaches the retry amount
            /// Build failure counter is reset to 0 when retry amount has been reached.
            /// </summary>
            ReportOnEveryRetryAmount
        }
    }
}
