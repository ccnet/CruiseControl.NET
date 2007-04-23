using System;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
    public interface IFingerprintFactory
    {
        ConditionalGetFingerprint BuildFromRequest(IRequest request);
        ConditionalGetFingerprint BuildFromFileNames(params string[] filenames);
        ConditionalGetFingerprint BuildFromDate(DateTime date);
    }
}