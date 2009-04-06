using System;
using System.Collections.Generic;
using System.Text;

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
