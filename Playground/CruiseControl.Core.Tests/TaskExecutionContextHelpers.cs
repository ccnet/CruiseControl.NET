namespace CruiseControl.Core.Tests
{
    public static class TaskExecutionContextHelpers
    {
        #region Helper methods
        public static TaskExecutionContext Initialise(Project project)
        {
            var instance = new TaskExecutionContext(
                new TaskExecutionParameters
                    {
                        Project = project
                    });
            return instance;
        }
        #endregion
    }
}
