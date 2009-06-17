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
    }
}
