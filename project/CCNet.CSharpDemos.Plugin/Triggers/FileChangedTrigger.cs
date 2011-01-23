using System;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Triggers;
using ThoughtWorks.CruiseControl.Remote;

namespace CCNet.CSharpDemos.Plugin.Triggers
{
    [ReflectorType("fileChangedTrigger")]
    public class FileChangedTrigger
        : ITrigger, IConfigurationValidation
    {
        private DateTime? lastChanged;

        public FileChangedTrigger()
        {
            this.BuildCondition = BuildCondition.IfModificationExists;
            this.InnerTrigger = new IntervalTrigger { IntervalSeconds = 5 };
        }

        [ReflectorProperty("file")]
        public string MonitorFile { get; set; }

        [ReflectorProperty("buildCondition", Required = false)]
        public BuildCondition BuildCondition { get; set; }

        [ReflectorProperty("trigger", InstanceTypeKey = "type", Required = false)]
        public ITrigger InnerTrigger { get; set; }

        public DateTime NextBuild
        {
            get { return DateTime.MaxValue; }
        }

        public void IntegrationCompleted()
        {
            this.InnerTrigger.IntegrationCompleted();
            this.lastChanged = File.GetLastWriteTime(this.MonitorFile);
        }

        public IntegrationRequest Fire()
        {
            IntegrationRequest request = null;
            if (this.lastChanged.HasValue)
            {
                if (this.InnerTrigger.Fire() != null)
                {
                    var changeTime = File.GetLastWriteTime(this.MonitorFile);
                    if (changeTime > this.lastChanged.Value)
                    {
                        request = new IntegrationRequest(
                            this.BuildCondition,
                            this.GetType().Name,
                            null);
                        this.lastChanged = changeTime;
                    }
                }
            }
            else
            {
                this.lastChanged = File.GetLastWriteTime(this.MonitorFile);
            }

            return request;
        }

        public void Validate(IConfiguration configuration,
            ConfigurationTrace parent,
            IConfigurationErrorProcesser errorProcesser)
        {
            if (string.IsNullOrEmpty(this.MonitorFile))
            {
                errorProcesser.ProcessError("File cannot be empty");
            }
            else if (!File.Exists(this.MonitorFile))
            {
                errorProcesser.ProcessWarning(
                    "File '" + this.MonitorFile + "' does not exist");
            }
        }
    }
}
