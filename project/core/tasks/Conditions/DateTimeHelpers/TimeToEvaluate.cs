namespace ThoughtWorks.CruiseControl.Core.tasks.Conditions.DateTimeHelpers
{
    public enum TimeToEvaluate
    {
        now,
        buildStart,
        buildEnd,
        firstModification,
        lastModification
    }
}