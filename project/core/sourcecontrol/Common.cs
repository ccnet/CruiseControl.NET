namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    public class Common
    {
        public enum SourceControlErrorHandlingPolicy
        {
            ReportEveryFailure,
            ReportOnRetryAmount,
            ReportOnEveryRetryAmount
        }

        /// <summary>
        /// The action to perform after a source control exception has been resolved.
        /// </summary>
        public enum SourceExceptionResolutionAction
        {
            /// <summary>
            /// Do not perform any action.
            /// </summary>
            None,

            /// <summary>
            /// Mark the build as successful.
            /// </summary>
            MarkSuccess,

            /// <summary>
            /// Force the build.
            /// </summary>
            ForceBuild,
        }
    }
}
