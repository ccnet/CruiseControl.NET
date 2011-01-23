namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks.Conditions
{
    using System;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Config;
    using ThoughtWorks.CruiseControl.Core.Tasks;

    public class MockCondition
        : ITaskCondition, IConfigurationValidation
    {
        public Func<IIntegrationResult, bool> EvalFunction { get; set; }
        public bool Eval(IIntegrationResult result)
        {
            if (this.EvalFunction != null)
            {
                return this.EvalFunction(result);
            }

            throw new NotImplementedException();
        }

        public Action<IConfiguration, ConfigurationTrace, IConfigurationErrorProcesser> ValidateAction { get; set; }
        public void Validate(IConfiguration configuration, ConfigurationTrace parent, IConfigurationErrorProcesser errorProcesser)
        {
            if (this.ValidateAction != null)
            {
                this.ValidateAction(configuration, parent, errorProcesser);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
