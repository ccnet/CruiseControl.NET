namespace CCNet.CSharpDemos.Plugin.Tasks
{
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Config;
    using ThoughtWorks.CruiseControl.Core.Tasks;
    using ThoughtWorks.CruiseControl.Remote;

    [ReflectorType("helloWorld2")]
    public class HelloWorldTask2
        : TaskBase, IConfigurationValidation
    {
        public HelloWorldTask2()
        {
            this.RepeatCount = 1;
        }

        [ReflectorProperty("name")]
        public string PersonsName { get; private set; }

        [ReflectorProperty("count", Required = false)]
        public int RepeatCount { get; set; }

        protected override bool Execute(IIntegrationResult result)
        {
            result.BuildProgressInformation
                .SignalStartRunTask("Sending a hello world greeting");
            for (var loop = 0; loop < this.RepeatCount; loop++)
            {
                result.AddTaskResult(
                    new HelloWorldTaskResult(this.PersonsName, result));
            }

            return true;
        }

        public void Validate(IConfiguration configuration, ConfigurationTrace parent, IConfigurationErrorProcesser errorProcesser)
        {
            if (this.RepeatCount <= 0)
            {
                errorProcesser.ProcessWarning("count is less than 1!");
            }
        }
    }
}
