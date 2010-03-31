namespace CCNet.CSharpDemos.Plugin.Tasks
{
    using ThoughtWorks.CruiseControl.Core;

    public class HelloWorldTaskResult
        : ITaskResult
    {
        private readonly string personName;
        private readonly IIntegrationResult result;

        public HelloWorldTaskResult(string personName, IIntegrationResult result)
        {
            this.personName = personName;
            this.result = result;
        }

        public string Data
        {
            get
            {
                return "Hello " + this.personName +
                    " from " + this.result.ProjectName +
                    "(build started " + this.result.StartTime.ToString() + ")";
            }
        }

        public bool CheckIfSuccess()
        {
            return true;
        }
    }
}
