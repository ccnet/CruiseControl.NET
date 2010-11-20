namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    public interface IQuietPeriod 
    {
        Modification[] GetModifications(ISourceControl sourceControl, IIntegrationResult from, IIntegrationResult to);
    }
}
