using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Remote;

namespace CCNet.CSharpDemos.Plugin.Tasks
{
    [ReflectorType("helloWorld")]
    public class HelloWorldTask
        : ITask, IConfigurationValidation
    {
        public HelloWorldTask()
        {
            this.RepeatCount = 1;
        }

        [ReflectorProperty("name")]
        public string PersonsName { get; private set; }

        [ReflectorProperty("count", Required = false)]
        public int RepeatCount { get; set; }

        public void Run(IIntegrationResult result)
        {
            result.BuildProgressInformation
                .SignalStartRunTask("Sending a hello world greeting");
            for (var loop = 0; loop < this.RepeatCount; loop++)
            {
                result.AddTaskResult("Hello " + this.PersonsName +
                    " from " + result.ProjectName +
                    "(build started " + result.StartTime + ")");
            }

            result.Status = IntegrationStatus.Success;
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
