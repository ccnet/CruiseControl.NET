namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    using System;
    using CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Config;

    public class MockTask
        : ITask, IConfigurationValidation
    {
        public Action<IIntegrationResult> RunAction { get; set; }
        public void Run(IIntegrationResult result)
        {
            if (this.RunAction != null)
            {
                this.RunAction(result);
                return;
            }

            throw new NotImplementedException();
        }

        public Action<IConfiguration, ConfigurationTrace, IConfigurationErrorProcesser> ValidateAction { get; set; }
        public void Validate(IConfiguration configuration, ConfigurationTrace parent, IConfigurationErrorProcesser errorProcesser)
        {
            if (this.ValidateAction != null)
            {
                this.ValidateAction(configuration, parent, errorProcesser);
                return;
            }

            throw new NotImplementedException();
        }
    }
}
