using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
    public interface IConditionalGetFingerprintProvider
    {
        ConditionalGetFingerprint GetFingerprint(IRequest request);
    }
}