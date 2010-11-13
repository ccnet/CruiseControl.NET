using System.Diagnostics;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace CCNet.CSharpDemos.Plugin.Tasks
{
    [ReflectorType("callHelloWorld")]
    public class CallHelloWorldTask
        : BaseExecutableTask
    {
        [ReflectorProperty("name")]
        public string PersonsName { get; private set; }

        [ReflectorProperty("executable")]
        public string Executable { get; private set; }

        protected override string GetProcessFilename()
        {
            return string.IsNullOrEmpty(this.Executable) ?
                "HelloWorldApp" :
                this.Executable;
        }

        protected override string GetProcessArguments(IIntegrationResult result)
        {
            return "\"" + this.PersonsName + "\"";
        }

        protected override string GetProcessBaseDirectory(IIntegrationResult result)
        {
            return result.WorkingDirectory;
        }

        protected override ProcessPriorityClass GetProcessPriorityClass()
        {
            return ProcessPriorityClass.Normal;
        }

        protected override int GetProcessTimeout()
        {
            return 300;
        }

        protected override bool Execute(IIntegrationResult result)
        {
            var processResult = this.TryToRun(
                this.CreateProcessInfo(result),
                result);
            result.AddTaskResult(new ProcessTaskResult(processResult));
            return !processResult.Failed;
        }
    }
}
