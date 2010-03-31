namespace CCNet.CSharpDemos.Plugin.SourceControl
{
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
    using ThoughtWorks.CruiseControl.Core.Tasks;
    using ThoughtWorks.CruiseControl.Core.Util;

    [ReflectorType("getMyCode")]
    public class GetMyCodeSourceControl
        : ProcessSourceControl
    {
        public GetMyCodeSourceControl()
            : this(new ProcessExecutor(), new IndexFileHistoryParser())
        {
        }

        public GetMyCodeSourceControl(ProcessExecutor executor, IHistoryParser parser)
            : base(parser, executor)
        {
        }

        [ReflectorProperty("executable", Required = false)]
        public string Executable { get; set; }

        [ReflectorProperty("source")]
        public string Source { get; set; }

        public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
        {
            var processResult = this.ExecuteCommand(to, "list");
            var modifications = this.ParseModifications(processResult, 
                from.StartTime, to.StartTime);
            return modifications;
        }

        public override void LabelSourceControl(IIntegrationResult result)
        {
            if (result.Succeeded)
            {
                var processResult = this.ExecuteCommand(result, 
                    "label", result.Label);
                result.AddTaskResult(
                    new ProcessTaskResult(processResult));
            }
        }

        public override void GetSource(IIntegrationResult result)
        {
            var processResult = this.ExecuteCommand(result, "get");
            result.AddTaskResult(
                new ProcessTaskResult(processResult));
        }

        private ProcessResult ExecuteCommand(IIntegrationResult result, 
            string command, params string[] args)
        {
            var buffer = new PrivateArguments(command);
            buffer.Add(this.Source);
            foreach (var arg in args)
            {
                buffer.Add(string.Empty,
                    arg,
                    true);
            }

            var executable = string.IsNullOrEmpty(this.Executable) ?
                "GetMyCode" : this.Executable;
            var processInfo = new ProcessInfo(
                result.BaseFromWorkingDirectory(executable),
                buffer,
                result.WorkingDirectory);
            var processResult = this.Execute(processInfo);
            return processResult;
        }
    }
}
